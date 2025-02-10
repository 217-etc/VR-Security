using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelAutoMove : MonoBehaviour
{
    public StepManager stepManager;
    private bool actionCompleted = false; // 한 번만 실행되도록 체크
    private float thresholdY = -0.1f; // 트리거되는 Y 좌표 값

    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;

    void Start()
    {
        if (stepManager == null)
        {
            Debug.LogError("StepManager가 할당되지 않았습니다! Unity 인스펙터에서 할당하세요.");
        }
    }

    void Update()
    {
        if (!actionCompleted && transform.position.y <= thresholdY)
        {
            actionCompleted = true; // 중복 실행 방지
            Debug.Log("릴이 목표 위치에 도달했습니다! 플레이어 행동 완료 처리.");
            stepManager?.OnPlayerActionCompleted();

            // 이동이 끝난 뒤 잡기 기능 제거
            Destroy(HandMoveObject);
            Destroy(HandMoveObject_mirror);
        }
    }
}
