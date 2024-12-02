using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollisionHandler : MonoBehaviour
{
    public OVRSkeleton ovrSkeleton; // OVRSkeleton ����
    public string wallTag = "Wall"; // ���� �±�
    public float detectionRadius = 0.1f; // �չٴ� ���� �ݰ�
    public float angleThreshold = 10f; // �չٴ� ����� ǥ�� ���� ������ ��� ����

    private Transform wristTransform; // �ո� Transform
    private Transform middleFingerTransform; // ���� ���� Transform
    private bool isTouchingWall = false; // �չٴ� �浹 ����
    public bool IsTouchingWall => isTouchingWall;
    public bool canDetect = true;

    public LineRenderer lineRenderer; // LineRenderer ����
    public int circleSegments = 36; // ���� �����ϴ� ���׸�Ʈ ��
    public Color gizmoColor = Color.red; // ����� ����

    void Start()
    {
        StartCoroutine(InitializeSkeleton());

        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned!");
        }
        else
        {
            lineRenderer.positionCount = circleSegments + 1; // ���� �׸��� ���� �ʿ��� �� ����
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.loop = true; // ���� ����
        }
    }

    private IEnumerator InitializeSkeleton()
    {
        // OVRSkeleton�� �ʱ�ȭ�� ������ ���
        while (ovrSkeleton.Bones == null || ovrSkeleton.Bones.Count == 0)
        {
            yield return null;
        }

        // �ո�� ���� ������ Transform ã��
        foreach (var bone in ovrSkeleton.Bones)
        {
            if (bone.Id == OVRSkeleton.BoneId.Hand_WristRoot) // �ո�
            {
                wristTransform = bone.Transform;
            }
            else if (bone.Id == OVRSkeleton.BoneId.Hand_Middle1) // ���� ù ��° ����
            {
                middleFingerTransform = bone.Transform;
            }
        }

        if (wristTransform == null || middleFingerTransform == null)
        {
            Debug.LogError("Wrist or Middle Finger bones not found in OVRSkeleton!");
        }
        else
        {
            Debug.Log("Skeleton initialized successfully.");
        }
    }

    void Update()
    {
        if (wristTransform == null || middleFingerTransform == null) return;

        if (!canDetect) return;

        // �չٴ� �߽� ��ġ ���
        Vector3 palmPosition = (wristTransform.position + middleFingerTransform.position) / 2;

        // �չٴ� ���� ���
        Vector3 palmDirection = (middleFingerTransform.position - wristTransform.position).normalized;

        // �浹 ����
        Collider[] hitColliders = Physics.OverlapSphere(palmPosition, detectionRadius);

        bool wallDetected = false;

        foreach (var hitCollider in hitColliders)
        {
            Debug.Log($"Detected Collider: {hitCollider.name}, Tag: {hitCollider.tag}");
            if (hitCollider.CompareTag(wallTag))
            {
                // �浹 ���� ���� ���� ��������
                Vector3 collisionNormal = hitCollider.ClosestPoint(palmPosition) - palmPosition;
                collisionNormal.Normalize();

                // �չٴ� ����� �浹 �� ������ ���� ��
                float angle = Vector3.Angle(-palmDirection, collisionNormal);
                Debug.Log("�չٴڰ� �� ������ ���� : " + angle);
        
                if (angle < 100 && angle > 80)
                {
                    wallDetected = true;
                    if (!isTouchingWall)
                    {
                        isTouchingWall = true;
                        Debug.Log("Palm touched the wall!");
                        OnPalmTouchWall(); // �浹 �̺�Ʈ ����
                    }
                }
                break;
            }
        }

        // �չٴ��� ������ �������� ��
        if (!wallDetected && isTouchingWall)
        {
            isTouchingWall = false;
            Debug.Log("Palm left the wall!");
            OnPalmLeaveWall(); // ������ ����� �̺�Ʈ ����
        }

        // ���� �����ϴ� �� ���
        Vector3[] circlePoints = new Vector3[circleSegments + 1];
        for (int i = 0; i <= circleSegments; i++)
        {
            float angle = i * 360f / circleSegments * Mathf.Deg2Rad; // ���� ���
            circlePoints[i] = palmPosition + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * detectionRadius;
        }

        // LineRenderer�� �� ����
        lineRenderer.SetPositions(circlePoints);
    }

    private void OnPalmTouchWall()
    {
        // �չٴ��� ���� ����� �� ������ ����
        Debug.Log("Palm collision started.");
    }

    private void OnPalmLeaveWall()
    {
        // �չٴ��� ������ ����� �� ������ ����
        Debug.Log("Palm collision ended.");
    }

    void OnDrawGizmos()
    {
        // �չٴ� ���� �ݰ� �ð�ȭ
        if (wristTransform != null && middleFingerTransform != null)
        {
            Vector3 palmPosition = (wristTransform.position + middleFingerTransform.position) / 2;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(palmPosition, detectionRadius);

            // �չٴ� ���� ���� �׸���
            Vector3 palmDirection = (middleFingerTransform.position - wristTransform.position).normalized;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(palmPosition, palmPosition + palmDirection * 0.1f); // ���� ����
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(palmPosition, palmPosition - palmDirection * 0.1f); // ���� ����
        }
    }
    

    public void StartMove()
    {
        isTouchingWall = false;
        canDetect = false;
    }

    public void EndMove()
    {
        canDetect = true;
    }

}
