using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public LineRenderer palmDirectionLineRenderer; // �չٴ��� �ݴ� ���� ���� LineRenderer
    public LineRenderer normalDirectionLineRenderer; // ���� ���� ���� LineRenderer
    public Color palmDirectionColor = Color.green; // �չٴ� �ݴ� ���� ���� ����
    public Color normalDirectionColor = Color.blue; // ���� ���� ���� ����

    void Start()
    {
        StartCoroutine(InitializeSkeleton());

        if (palmDirectionLineRenderer == null || normalDirectionLineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned!");
        }
        // LineRenderer ����
        palmDirectionLineRenderer = new GameObject("PalmDirectionLine").AddComponent<LineRenderer>();
        palmDirectionLineRenderer.startWidth = 0.01f;
        palmDirectionLineRenderer.endWidth = 0.01f;
        palmDirectionLineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        palmDirectionLineRenderer.material.color = palmDirectionColor;

        normalDirectionLineRenderer = new GameObject("NormalDirectionLine").AddComponent<LineRenderer>();
        normalDirectionLineRenderer.startWidth = 0.01f;
        normalDirectionLineRenderer.endWidth = 0.01f;
        normalDirectionLineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        normalDirectionLineRenderer.material.color = normalDirectionColor;

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

        // �չٴ��� �ݴ� ����
        Vector3 oppositePalmDirection = -palmDirection;

        // LineRenderer�� �չٴ��� �ݴ� ���� ���� �ð�ȭ
        palmDirectionLineRenderer.SetPosition(0, palmPosition);
        palmDirectionLineRenderer.SetPosition(1, palmPosition + oppositePalmDirection * 0.2f);

        // �浹 ����
        Collider[] hitColliders = Physics.OverlapSphere(palmPosition, detectionRadius);

        bool wallDetected = false;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(wallTag))
            {
                // �浹 ���� ���� ���� ��������
                Vector3 collisionPoint = hitCollider.ClosestPoint(palmPosition);
                Vector3 collisionNormal = (collisionPoint - palmPosition).normalized;

                float angle = Vector3.Angle(oppositePalmDirection, collisionNormal);
                Debug.Log("�չٴڰ� �� ������ ���� : " + angle);

                // LineRenderer�� ���� ���� ���� �ð�ȭ
                normalDirectionLineRenderer.SetPosition(0, collisionPoint);
                normalDirectionLineRenderer.SetPosition(1, collisionPoint + collisionNormal * 0.2f);

                if (angle < 100 && angle > 80)
                {
                    wallDetected = true;
                    if (!isTouchingWall)
                    {
                        isTouchingWall = true;
                        Debug.Log("Palm touched the wall!");
                        OnPalmTouchWall();
                    }
                }
                break;
            }
        }

        if (!wallDetected && isTouchingWall)
        {
            isTouchingWall = false;
            Debug.Log("Palm left the wall!");
            OnPalmLeaveWall();
        }
    }

    private void OnPalmTouchWall()
    {
        Debug.Log("Palm collision started.");
        SoundManager.Instance.PlaySFX("WallHit");
    }

    private void OnPalmLeaveWall()
    {
        Debug.Log("Palm collision ended.");
    }

    void OnDrawGizmos()
    {
        if (wristTransform != null && middleFingerTransform != null)
        {
            Vector3 palmPosition = (wristTransform.position + middleFingerTransform.position) / 2;
            Vector3 palmDirection = (middleFingerTransform.position - wristTransform.position).normalized;
            Vector3 oppositePalmDirection = -palmDirection;

            // �չٴ��� �ݴ� ���� ���� (���)
            Gizmos.color = palmDirectionColor;
            Gizmos.DrawLine(palmPosition, palmPosition + oppositePalmDirection * 0.2f);

            // ���� ���� ���� (�Ķ���)
            Collider[] hitColliders = Physics.OverlapSphere(palmPosition, detectionRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag(wallTag))
                {
                    Vector3 collisionPoint = hitCollider.ClosestPoint(palmPosition);
                    Vector3 collisionNormal = (collisionPoint - palmPosition).normalized;

                    Gizmos.color = normalDirectionColor;
                    Gizmos.DrawLine(collisionPoint, collisionPoint + collisionNormal * 0.2f);
                }
            }
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
