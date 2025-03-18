using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class TieBeltController : MonoBehaviour
{
    [SerializeField] private bool IsGrabBelt = false;
    [SerializeField] private bool IsGrabGori = false;
    [SerializeField] private bool IsComplete = false;
    [SerializeField] private OneGrabTranslateTransformer goriTransformer;

    public Transform goriTransform;
    public Vector3 startPos;
    public Vector3 endPos;

    public Animator animator;
    private float distance;

    private void Awake()
    {
        // grab 여부 초기화
        IsGrabBelt = false;
        IsGrabGori = false;

        distance = Vector3.Distance(startPos, endPos);
        distance *= 0.1f;
        Debug.Log($"distance : {distance}");
    }

    private void Update()
    {
        if (IsComplete) return;

        if (IsGrabBelt)
        {
            //Debug.Log("[Belt] : Connect Belt를 잡고 있음");

            if (IsGrabGori)
            {
                //Debug.Log("[Belt] : Connect Belt와 Gori를 둘다 잡고 있음");
                SetConstraintValue(0f, -0.2f);
                TieGori();
            }
        }
        else
        {
            if (IsGrabGori)
            {
                //Debug.Log("[Belt] : Connect Belt를 잡고 있지 않음");
                SetConstraintValue(0f, 0f);
            }
        }
    }

    // Connect Belt를 잡았을 때
    public void GrabConnectBelt()
    {
        IsGrabBelt = true;
        Debug.Log("[Belt] : Connect Belt를 잡음");
    }
    // Connect Belt를 놓았을 때
    public void ReleaseConnectBelt()
    {
        IsGrabBelt = false;
        Debug.Log("[Belt] : Connect Belt를 놓음");
    }
    // Gori를 잡았을 때
    public void GrabGori()
    {
        IsGrabGori = true;
        Debug.Log("[Belt] : Gori를 잡음");
    }
    // Gori를 놓았을 때
    public void ReleaseGori()
    {
        IsGrabGori = false;
        Debug.Log("[Belt] : Gori를 놓음");
    }

    public void TieGori()
    {
        float distanceToStart = Vector3.Distance(goriTransform.localPosition, startPos);

        // 이동 진행도를 0~1 범위로 정규화
        float progress = distanceToStart / distance;
        Debug.Log($"[Progress] DistanceToStart: {distanceToStart}, Normalized Progress: {progress}");

        // 벨트 애니메이션 4개 동시 실행 (각 레이어에 적용)
        int layerCount = animator.layerCount;
        for (int i = 0; i < layerCount; i++)
        {
            animator.Play("Tie", i, progress);
        }

        // 이동 완료 체크
        if (progress >= 1f)
        {
            Debug.Log("[Belt] : 벨트 조이기를 완수함.");
            IsComplete = true;
        }
    }

    // 고리 이동 제한 스크립트의 minY, minZ 코드로 설정할 수 있는 함수
    // 두 손이 모두 잡고 있지 않으면 고리가 움직이지 않게 하기 위함임
    public void SetConstraintValue(float minY, float minZ)
    {
        goriTransformer.Constraints.MinY.Constrain = true;
        goriTransformer.Constraints.MinY.Value = minY;
        goriTransformer.Constraints.MinZ.Constrain = true;
        goriTransformer.Constraints.MinZ.Value = minZ;
    }
}
