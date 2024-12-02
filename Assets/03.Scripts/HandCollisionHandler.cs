using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollisionHandler : MonoBehaviour
{
    public OVRSkeleton ovrSkeleton; // OVRSkeleton 참조
    public string wallTag = "Wall"; // 벽의 태그
    public float detectionRadius = 0.1f; // 손바닥 감지 반경

    private Transform wristTransform; // 손목 Transform
    private Transform middleFingerTransform; // 중지 관절 Transform
    private bool isTouchingWall = false; // 손바닥 충돌 상태
    public bool IsTouchingWall => isTouchingWall;
    public bool canDetect = true;

    public LineRenderer lineRenderer; // LineRenderer 참조
    public int circleSegments = 36; // 원을 구성하는 세그먼트 수
    public Color gizmoColor = Color.red; // 디버그 색상

    void Start()
    {
        StartCoroutine(InitializeSkeleton());

        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned!");
        }
        else
        {
            lineRenderer.positionCount = circleSegments + 1; // 원을 그리기 위해 필요한 점 개수
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.loop = true; // 원을 닫음
        }
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

        // 충돌 감지
        Collider[] hitColliders = Physics.OverlapSphere(palmPosition, detectionRadius);

        bool wallDetected = false;

        foreach (var hitCollider in hitColliders)
        {
            Debug.Log($"Detected Collider: {hitCollider.name}, Tag: {hitCollider.tag}");
            if (hitCollider.CompareTag(wallTag))
            {
                wallDetected = true;
                if (!isTouchingWall)
                {
                    isTouchingWall = true;
                    Debug.Log("Palm touched the wall!");
                    OnPalmTouchWall(); // 충돌 이벤트 실행
                }
                break;
            }
        }

        // 손바닥이 벽에서 떨어졌을 때
        if (!wallDetected && isTouchingWall)
        {
            isTouchingWall = false;
            Debug.Log("Palm left the wall!");
            OnPalmLeaveWall(); // 벽에서 벗어나는 이벤트 실행
        }

        // 원을 구성하는 점 계산
        Vector3[] circlePoints = new Vector3[circleSegments + 1];
        for (int i = 0; i <= circleSegments; i++)
        {
            float angle = i * 360f / circleSegments * Mathf.Deg2Rad; // 각도 계산
            circlePoints[i] = palmPosition + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * detectionRadius;
        }

        // LineRenderer로 점 설정
        lineRenderer.SetPositions(circlePoints);
    }

    private void OnPalmTouchWall()
    {
        // 손바닥이 벽에 닿았을 때 실행할 로직
        Debug.Log("Palm collision started.");
    }

    private void OnPalmLeaveWall()
    {
        // 손바닥이 벽에서 벗어났을 때 실행할 로직
        Debug.Log("Palm collision ended.");
    }

    void OnDrawGizmos()
    {
        // 손바닥 감지 반경 시각화
        if (wristTransform != null && middleFingerTransform != null)
        {
            Vector3 palmPosition = (wristTransform.position + middleFingerTransform.position) / 2;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(palmPosition, detectionRadius);
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
