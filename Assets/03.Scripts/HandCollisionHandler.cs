using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HandCollisionHandler : MonoBehaviour
{
    public OVRSkeleton ovrSkeleton; // OVRSkeleton 참조
    public string wallTag = "Wall"; // 벽의 태그
    public float detectionRadius = 0.1f; // 손바닥 감지 반경
    public float angleThreshold = 10f; // 손바닥 방향과 표면 법선 벡터의 허용 각도

    private Transform wristTransform; // 손목 Transform
    private Transform middleFingerTransform; // 중지 관절 Transform
    private bool isTouchingWall = false; // 손바닥 충돌 상태
    public bool IsTouchingWall => isTouchingWall;
    public bool canDetect = true;

    public LineRenderer palmDirectionLineRenderer; // 손바닥의 반대 방향 벡터 LineRenderer
    public LineRenderer normalDirectionLineRenderer; // 벽의 법선 벡터 LineRenderer
    public Color palmDirectionColor = Color.green; // 손바닥 반대 방향 벡터 색상
    public Color normalDirectionColor = Color.blue; // 벽의 법선 벡터 색상

    void Start()
    {
        StartCoroutine(InitializeSkeleton());

        if (palmDirectionLineRenderer == null || normalDirectionLineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned!");
        }
        // LineRenderer 생성
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
        // OVRSkeleton이 초기화될 때까지 대기
        while (ovrSkeleton.Bones == null || ovrSkeleton.Bones.Count == 0)
        {
            yield return null;
        }

        // 손목과 중지 관절의 Transform 찾기
        foreach (var bone in ovrSkeleton.Bones)
        {
            if (bone.Id == OVRSkeleton.BoneId.Hand_WristRoot) // 손목
            {
                wristTransform = bone.Transform;
            }
            else if (bone.Id == OVRSkeleton.BoneId.Hand_Middle1) // 중지 첫 번째 관절
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

        // 손바닥 중심 위치 계산
        Vector3 palmPosition = (wristTransform.position + middleFingerTransform.position) / 2;

        // 손바닥 방향 계산
        Vector3 palmDirection = (middleFingerTransform.position - wristTransform.position).normalized;

        // 손바닥의 반대 방향
        Vector3 oppositePalmDirection = -palmDirection;

        // LineRenderer로 손바닥의 반대 방향 벡터 시각화
        palmDirectionLineRenderer.SetPosition(0, palmPosition);
        palmDirectionLineRenderer.SetPosition(1, palmPosition + oppositePalmDirection * 0.2f);

        // 충돌 감지
        Collider[] hitColliders = Physics.OverlapSphere(palmPosition, detectionRadius);

        bool wallDetected = false;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(wallTag))
            {
                // 충돌 면의 법선 벡터 가져오기
                Vector3 collisionPoint = hitCollider.ClosestPoint(palmPosition);
                Vector3 collisionNormal = (collisionPoint - palmPosition).normalized;

                float angle = Vector3.Angle(oppositePalmDirection, collisionNormal);
                Debug.Log("손바닥과 벽 사이의 각도 : " + angle);

                // LineRenderer로 벽의 법선 벡터 시각화
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

            // 손바닥의 반대 방향 벡터 (녹색)
            Gizmos.color = palmDirectionColor;
            Gizmos.DrawLine(palmPosition, palmPosition + oppositePalmDirection * 0.2f);

            // 벽의 법선 벡터 (파란색)
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
