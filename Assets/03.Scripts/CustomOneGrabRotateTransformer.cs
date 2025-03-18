using System;
using UnityEngine;
using Oculus.Interaction;
using UnityEditor;
using UnityEngine.Animations;

public class CustomOneGrabRotateTransformer : MonoBehaviour, ITransformer
{
    private TransformerUtils.PositionConstraints _positionConstraints =
     new TransformerUtils.PositionConstraints()
     {
         XAxis = new TransformerUtils.ConstrainedAxis(),
         YAxis = new TransformerUtils.ConstrainedAxis(),
         ZAxis = new TransformerUtils.ConstrainedAxis()
     };

    [SerializeField]
    private TransformerUtils.RotationConstraints _rotationConstraints =
        new TransformerUtils.RotationConstraints()
        {
            XAxis = new TransformerUtils.ConstrainedAxis(),
            YAxis = new TransformerUtils.ConstrainedAxis(),
            ZAxis = new TransformerUtils.ConstrainedAxis()
        };


    public enum Axis
    {
        Right = 0,
        Up = 1,
        Forward = 2
    }

    [SerializeField, Optional]
    private Transform _pivotTransform = null;

    public Transform Pivot => _pivotTransform != null ? _pivotTransform : transform;

    [SerializeField]
    private Axis _rotationAxis = Axis.Up;

    public Axis RotationAxis => _rotationAxis;

    [Serializable]
    public class OneGrabRotateConstraints
    {
        public FloatConstraint MinAngle;
        public FloatConstraint MaxAngle;
    }

    [SerializeField]
    private OneGrabRotateConstraints _constraints =
        new OneGrabRotateConstraints()
        {
            MinAngle = new FloatConstraint(),
            MaxAngle = new FloatConstraint()
        };

    public OneGrabRotateConstraints Constraints
    {
        get
        {
            return _constraints;
        }

        set
        {
            _constraints = value;
        }
    }

    private float _relativeAngle = 0.0f;
    private float _constrainedRelativeAngle = 0.0f;

    private IGrabbable _grabbable;
    private Vector3 _grabPositionInPivotSpace;
    private Pose _transformPoseInPivotSpace;

    private Pose _worldPivotPose;
    private Vector3 _previousVectorInPivotSpace;

    private Quaternion _localRotation;
    private float _startAngle = 0;

    // New code
    private Pose _grabDeltaInLocalSpace;
    private TransformerUtils.PositionConstraints _parentConstraints;

    public void Initialize(IGrabbable grabbable)
    {
        _grabbable = grabbable;
        // New code
        Vector3 initialPosition = _grabbable.Transform.localPosition;
        _parentConstraints = TransformerUtils.GenerateParentConstraints(_positionConstraints, initialPosition);
    }

    public Pose ComputeWorldPivotPose()
    {
        if (_pivotTransform != null)
        {
            return _pivotTransform.GetPose();
        }

        var targetTransform = _grabbable.Transform;

        Vector3 worldPosition = targetTransform.position;
        Quaternion worldRotation = targetTransform.parent != null
            ? targetTransform.parent.rotation * _localRotation
            : _localRotation;

        return new Pose(worldPosition, worldRotation);
    }

    public void BeginTransform()
    {
        var grabPoint = _grabbable.GrabPoints[0];
        var targetTransform = _grabbable.Transform;


        if (_pivotTransform == null)
        {
            _localRotation = targetTransform.localRotation;
        }

        Vector3 localAxis = Vector3.zero;
        localAxis[(int)_rotationAxis] = 1f;

        _worldPivotPose = ComputeWorldPivotPose();
        Vector3 rotationAxis = _worldPivotPose.rotation * localAxis;

        Quaternion inverseRotation = Quaternion.Inverse(_worldPivotPose.rotation);

        Vector3 grabDelta = grabPoint.position - _worldPivotPose.position;
        // The initial delta must be non-zero between the pivot and grab location for rotation
        if (Mathf.Abs(grabDelta.magnitude) < 0.001f)
        {
            Vector3 localAxisNext = Vector3.zero;
            localAxisNext[((int)_rotationAxis + 1) % 3] = 0.001f;
            grabDelta = _worldPivotPose.rotation * localAxisNext;
        }

        _grabPositionInPivotSpace =
            inverseRotation * grabDelta;

        Vector3 worldPositionDelta =
            inverseRotation * (targetTransform.position - _worldPivotPose.position);

        Quaternion worldRotationDelta = inverseRotation * targetTransform.rotation;
        _transformPoseInPivotSpace = new Pose(worldPositionDelta, worldRotationDelta);

        Vector3 initialOffset = _worldPivotPose.rotation * _grabPositionInPivotSpace;
        Vector3 initialVector = Vector3.ProjectOnPlane(initialOffset, rotationAxis);
        _previousVectorInPivotSpace = Quaternion.Inverse(_worldPivotPose.rotation) * initialVector;

        _startAngle = _constrainedRelativeAngle;
        _relativeAngle = _startAngle;

        float parentScale = targetTransform.parent != null ? targetTransform.parent.lossyScale.x : 1f;
        _transformPoseInPivotSpace.position /= parentScale;

        // New code
        _grabDeltaInLocalSpace = new Pose(targetTransform.InverseTransformVector(grabPoint.position - targetTransform.position),
                                            Quaternion.Inverse(grabPoint.rotation) * targetTransform.rotation);
    }

    public void UpdateTransform()
    {
        var grabPoint = _grabbable.GrabPoints[0];
        var targetTransform = _grabbable.Transform;

        Vector3 localAxis = Vector3.zero;
        localAxis[(int)_rotationAxis] = 1f;
        _worldPivotPose = ComputeWorldPivotPose();
        Vector3 rotationAxis = _worldPivotPose.rotation * localAxis;

        // Project our positional offsets onto a plane with normal equal to the rotation axis
        Vector3 targetOffset = grabPoint.position - _worldPivotPose.position;
        Vector3 targetVector = Vector3.ProjectOnPlane(targetOffset, rotationAxis);

        Vector3 previousVectorInWorldSpace =
            _worldPivotPose.rotation * _previousVectorInPivotSpace;

        // update previous
        _previousVectorInPivotSpace = Quaternion.Inverse(_worldPivotPose.rotation) * targetVector;

        float signedAngle =
            Vector3.SignedAngle(previousVectorInWorldSpace, targetVector, rotationAxis);

        _relativeAngle += signedAngle;

        _constrainedRelativeAngle = _relativeAngle;
        if (Constraints.MinAngle.Constrain)
        {
            _constrainedRelativeAngle = Mathf.Max(_constrainedRelativeAngle, Constraints.MinAngle.Value);
        }
        if (Constraints.MaxAngle.Constrain)
        {
            _constrainedRelativeAngle = Mathf.Min(_constrainedRelativeAngle, Constraints.MaxAngle.Value);
        }

        Quaternion deltaRotation = Quaternion.AngleAxis(_constrainedRelativeAngle - _startAngle, rotationAxis);

        float parentScale = targetTransform.parent != null ? targetTransform.parent.lossyScale.x : 1f;
        Pose transformDeltaInWorldSpace =
            new Pose(
                _worldPivotPose.rotation * (parentScale * _transformPoseInPivotSpace.position),
                _worldPivotPose.rotation * _transformPoseInPivotSpace.rotation);

        Pose transformDeltaRotated = new Pose(
            deltaRotation * transformDeltaInWorldSpace.position,
            deltaRotation * transformDeltaInWorldSpace.rotation);

        // New code
        //targetTransform.position = _worldPivotPose.position + transformDeltaRotated.position;
        Vector3 updatedPosition = grabPoint.position - targetTransform.TransformVector(_grabDeltaInLocalSpace.position);
        targetTransform.position = TransformerUtils.GetConstrainedTransformPosition(updatedPosition, _parentConstraints, targetTransform.parent);
        targetTransform.rotation = transformDeltaRotated.rotation;
    }

    public void EndTransform() { }

    #region Inject

    public void InjectOptionalPivotTransform(Transform pivotTransform)
    {
        _pivotTransform = pivotTransform;
    }

    public void InjectOptionalRotationAxis(Axis rotationAxis)
    {
        _rotationAxis = rotationAxis;
    }

    public void InjectOptionalConstraints(OneGrabRotateConstraints constraints)
    {
        _constraints = constraints;
    }

    #endregion

    #region Subclass
    public class TransformerUtils
    {
        [Serializable]
        public struct FloatRange
        {
            public float Min;
            public float Max;
        }

        [Serializable]
        public struct ConstrainedAxis
        {
            public bool ConstrainAxis;
            public FloatRange AxisRange;
        }

        [Serializable]
        public class PositionConstraints
        {
            public bool ConstraintsAreRelative;
            public ConstrainedAxis XAxis;
            public ConstrainedAxis YAxis;
            public ConstrainedAxis ZAxis;
        }

        [Serializable]
        public class RotationConstraints
        {
            public ConstrainedAxis XAxis;
            public ConstrainedAxis YAxis;
            public ConstrainedAxis ZAxis;
        }

        [Serializable]
        public class ScaleConstraints
        {
            public bool ConstraintsAreRelative;
            public ConstrainedAxis XAxis;
            public ConstrainedAxis YAxis;
            public ConstrainedAxis ZAxis;
        }

        public static PositionConstraints GenerateParentConstraints(PositionConstraints constraints, Vector3 initialPosition)
        {
            PositionConstraints parentConstraints;

            if (!constraints.ConstraintsAreRelative)
            {
                parentConstraints = constraints;
            }
            else
            {
                parentConstraints = new PositionConstraints();

                parentConstraints.XAxis = new ConstrainedAxis();
                parentConstraints.YAxis = new ConstrainedAxis();
                parentConstraints.ZAxis = new ConstrainedAxis();

                if (constraints.XAxis.ConstrainAxis)
                {
                    parentConstraints.XAxis.ConstrainAxis = true;
                    parentConstraints.XAxis.AxisRange.Min = constraints.XAxis.AxisRange.Min + initialPosition.x;
                    parentConstraints.XAxis.AxisRange.Max = constraints.XAxis.AxisRange.Max + initialPosition.x;
                }
                if (constraints.YAxis.ConstrainAxis)
                {
                    parentConstraints.YAxis.ConstrainAxis = true;
                    parentConstraints.YAxis.AxisRange.Min = constraints.YAxis.AxisRange.Min + initialPosition.y;
                    parentConstraints.YAxis.AxisRange.Max = constraints.YAxis.AxisRange.Max + initialPosition.y;
                }
                if (constraints.ZAxis.ConstrainAxis)
                {
                    parentConstraints.ZAxis.ConstrainAxis = true;
                    parentConstraints.ZAxis.AxisRange.Min = constraints.ZAxis.AxisRange.Min + initialPosition.z;
                    parentConstraints.ZAxis.AxisRange.Max = constraints.ZAxis.AxisRange.Max + initialPosition.z;
                }
            }

            return parentConstraints;
        }

        public static ScaleConstraints GenerateParentConstraints(ScaleConstraints constraints, Vector3 initialScale)
        {
            ScaleConstraints parentConstraints;

            if (!constraints.ConstraintsAreRelative)
            {
                parentConstraints = constraints;
            }
            else
            {
                parentConstraints = new ScaleConstraints();

                parentConstraints.XAxis = new ConstrainedAxis();
                parentConstraints.YAxis = new ConstrainedAxis();
                parentConstraints.ZAxis = new ConstrainedAxis();

                if (constraints.XAxis.ConstrainAxis)
                {
                    parentConstraints.XAxis.ConstrainAxis = true;
                    parentConstraints.XAxis.AxisRange.Min = constraints.XAxis.AxisRange.Min * initialScale.x;
                    parentConstraints.XAxis.AxisRange.Max = constraints.XAxis.AxisRange.Max * initialScale.x;
                }
                if (constraints.YAxis.ConstrainAxis)
                {
                    parentConstraints.YAxis.ConstrainAxis = true;
                    parentConstraints.YAxis.AxisRange.Min = constraints.YAxis.AxisRange.Min * initialScale.y;
                    parentConstraints.YAxis.AxisRange.Max = constraints.YAxis.AxisRange.Max * initialScale.y;
                }
                if (constraints.ZAxis.ConstrainAxis)
                {
                    parentConstraints.ZAxis.ConstrainAxis = true;
                    parentConstraints.ZAxis.AxisRange.Min = constraints.ZAxis.AxisRange.Min * initialScale.z;
                    parentConstraints.ZAxis.AxisRange.Max = constraints.ZAxis.AxisRange.Max * initialScale.z;
                }
            }

            return parentConstraints;
        }

        public static Vector3 GetConstrainedTransformPosition(Vector3 unconstrainedPosition, PositionConstraints positionConstraints, Transform relativeTransform = null)
        {
            Vector3 constrainedPosition = unconstrainedPosition;

            // the translation constraints occur in parent space
            if (relativeTransform != null)
            {
                constrainedPosition = relativeTransform.InverseTransformPoint(constrainedPosition);
            }

            if (positionConstraints.XAxis.ConstrainAxis)
            {
                constrainedPosition.x = Mathf.Clamp(constrainedPosition.x, positionConstraints.XAxis.AxisRange.Min, positionConstraints.XAxis.AxisRange.Max);
            }
            if (positionConstraints.YAxis.ConstrainAxis)
            {
                constrainedPosition.y = Mathf.Clamp(constrainedPosition.y, positionConstraints.YAxis.AxisRange.Min, positionConstraints.YAxis.AxisRange.Max);
            }
            if (positionConstraints.ZAxis.ConstrainAxis)
            {
                constrainedPosition.z = Mathf.Clamp(constrainedPosition.z, positionConstraints.ZAxis.AxisRange.Min, positionConstraints.ZAxis.AxisRange.Max);
            }

            // Convert the constrained position back to world space
            if (relativeTransform != null)
            {
                constrainedPosition = relativeTransform.TransformPoint(constrainedPosition);
            }

            return constrainedPosition;
        }

        public static Quaternion GetConstrainedTransformRotation(Quaternion unconstrainedRotation, RotationConstraints rotationConstraints)
        {
            var newX = unconstrainedRotation.eulerAngles.x;
            var newY = unconstrainedRotation.eulerAngles.y;
            var newZ = unconstrainedRotation.eulerAngles.z;

            if (rotationConstraints.XAxis.ConstrainAxis)
            {
                newX = Mathf.Clamp(unconstrainedRotation.eulerAngles.x, rotationConstraints.XAxis.AxisRange.Min, rotationConstraints.XAxis.AxisRange.Max);
            }
            if (rotationConstraints.YAxis.ConstrainAxis)
            {
                newY = Mathf.Clamp(unconstrainedRotation.eulerAngles.y, rotationConstraints.YAxis.AxisRange.Min, rotationConstraints.YAxis.AxisRange.Max);
            }
            if (rotationConstraints.ZAxis.ConstrainAxis)
            {
                newZ = Mathf.Clamp(unconstrainedRotation.eulerAngles.z, rotationConstraints.ZAxis.AxisRange.Min, rotationConstraints.ZAxis.AxisRange.Max);
            }

            return Quaternion.Euler(newX, newY, newZ);
        }

        public static Vector3 GetConstrainedTransformScale(Vector3 unconstrainedScale, ScaleConstraints scaleConstraints)
        {
            Vector3 constrainedScale = unconstrainedScale;

            if (scaleConstraints.XAxis.ConstrainAxis)
            {
                constrainedScale.x = Mathf.Clamp(constrainedScale.x, scaleConstraints.XAxis.AxisRange.Min, scaleConstraints.XAxis.AxisRange.Max);
            }
            if (scaleConstraints.YAxis.ConstrainAxis)
            {
                constrainedScale.y = Mathf.Clamp(constrainedScale.y, scaleConstraints.YAxis.AxisRange.Min, scaleConstraints.YAxis.AxisRange.Max);
            }
            if (scaleConstraints.ZAxis.ConstrainAxis)
            {
                constrainedScale.z = Mathf.Clamp(constrainedScale.z, scaleConstraints.ZAxis.AxisRange.Min, scaleConstraints.ZAxis.AxisRange.Max);
            }

            return constrainedScale;
        }
    }
    #endregion
}
