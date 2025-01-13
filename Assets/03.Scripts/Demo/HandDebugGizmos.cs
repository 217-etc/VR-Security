using UnityEngine;
using Oculus.Interaction.Input;
using UnityEngine.Assertions;
using System.Collections.Generic;


namespace Oculus.Interaction.Hands
{
    /// <summary>
    /// Debugs an <cref"IHand" /> by drawing its joints and connections.
    /// </summary>
    public class HandDebugGizmos : MonoBehaviour
    {
        public enum CoordSpace
        {
            World,
            Local,
        }

        [SerializeField, Interface(typeof(IHand))]
        private UnityEngine.Object _hand;
        private IHand Hand;

        [Tooltip("The coordinate space in which to draw the hand skeleton.")]
        [SerializeField]
        private CoordSpace _space = CoordSpace.World;

        [SerializeField]
        private Color lineColor = Color.green;

        [SerializeField]
        private float sphereRadius = 0.01f;

        [SerializeField]
        private Color sphereColor = Color.blue;

        private LineRenderer lineRenderer;
        private Dictionary<HandJointId, GameObject> jointSpheres = new Dictionary<HandJointId, GameObject>();

        protected bool _started = false;

        protected virtual void Awake()
        {
            Hand = _hand as IHand;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(Hand);
            this.EndStart(ref _started);

            InitializeLineRenderer();
            InitializeJointSpheres();
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                Hand.WhenHandUpdated += HandleHandUpdated;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                Hand.WhenHandUpdated -= HandleHandUpdated;
            }
        }

        private void InitializeLineRenderer()
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.005f;
            lineRenderer.endWidth = 0.005f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
        }

        private void InitializeJointSpheres()
        {
            foreach (HandJointId jointId in System.Enum.GetValues(typeof(HandJointId)))
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.localScale = Vector3.one * sphereRadius;
                sphere.GetComponent<Renderer>().material.color = sphereColor;
                sphere.name = jointId.ToString();
                jointSpheres[jointId] = sphere;
            }
        }

        protected virtual void HandleHandUpdated()
        {
            List<Vector3> jointPositions = new List<Vector3>();

            foreach (HandJointId jointId in System.Enum.GetValues(typeof(HandJointId)))
            {
                if (Hand.GetJointPose(jointId, out Pose pose))
                {
                    // Update joint sphere position
                    jointSpheres[jointId].transform.position = pose.position;

                    // Add line segments between parent and child joints
                    if (TryGetParentJointId(jointId, out HandJointId parentId) &&
                        Hand.GetJointPose(parentId, out Pose parentPose))
                    {
                        jointPositions.Add(pose.position);
                        jointPositions.Add(parentPose.position);
                    }
                }
            }

            // Update LineRenderer
            lineRenderer.positionCount = jointPositions.Count;
            lineRenderer.SetPositions(jointPositions.ToArray());
        }

        private bool TryGetParentJointId(HandJointId jointId, out HandJointId parentId)
        {
            // Define parent-child relationships manually
            parentId = HandJointId.Invalid;

            // Example: Thumb0 -> Wrist
            if (jointId == HandJointId.HandWristRoot) return false; // No parent for wrist

            // Logic to map parent joints (adjust as per your skeleton definition)
            parentId = jointId - 1;
            return true;
        }
    }
}
