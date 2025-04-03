using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class S2AutoMove2 : MonoBehaviour
{
    private bool hasMoved = false; // S2가 한 번 이동했는지 체크
    private bool hasDone = false; // S2가 두 번째 이동했는지 체크
    private bool isLocked = false; // S2의 움직임이 다 끝났는지 체크
    private float targetY1 = -3.0f; // 첫 목표 Y
    private float moveDuration = 1.0f; // 이동하는 데 걸리는 시간

    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;
    public StepManager stepManager;

    void Start()
    {
        if (stepManager == null)
        { Debug.LogError("StepManager가 할당되지 않았습니다! Unity 인스펙터에서 할당하세요."); }
    }

    void Update()
    {
        if (isLocked) return; // 창문이 고정된 상태면 더 이상 Update 실행 안 함

        // 일단 y축 위치가 -3보다 작으면 -3으로 이동
        // 잡는 모션 추가 필요 -> 각도가 179.9보다 커지면?
        if (transform.localPosition.y <= -3f && !hasMoved)
        {
            StartCoroutine(MoveS2Smoothly1());
            hasMoved = true; // 한 번만 실행되도록 설정
        }

    }

    IEnumerator MoveS2Smoothly1()
    {
        float elapsedTime = 0f;
        Vector3 S2startPosition = transform.localPosition;
        Vector3 S2targetPosition = new Vector3(S2startPosition.x, targetY1, S2startPosition.z);

        while (elapsedTime < moveDuration)
        {
            if (isLocked) yield break; // S2가 고정되었으면 즉시 코루틴 종료
            transform.localPosition = Vector3.Lerp(S2startPosition, S2targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // 한 프레임 기다림
        }

        transform.localPosition = S2targetPosition; // 이동이 끝나면 최종 위치 보정
        // transform.localPosition = S2targetPosition;
    }


    void LockWindow()
    {
        isLocked = true;
    }
}