using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;


public class WindowAutoMove : MonoBehaviour
{
    private bool hasMoved = false; // 창문이 이미 이동했는지 체크
    private bool isLocked = false; // 창문이 4.423에 도달했는지 체크
    private float targetZ = 4.423f; // 목표 Z 위치
    private float moveDuration = 1.0f; // 이동하는 데 걸리는 시간

    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;

    public StepManager stepManager;

    void Start()
    {
        //stepManager = FindObjectOfType<StepManager>();
        if (stepManager == null)
        {
            Debug.LogError("StepManager가 할당되지 않았습니다! Unity 인스펙터에서 할당하세요.");
        }
    }

    void Update()
    {
        if (isLocked) return; // 창문이 고정된 상태면 더 이상 Update 실행 안 함

        // 사용자가 창문을 열다가 손을 놓으면 Z 값 확인 후 이동 실행
        if (transform.position.z >= 4.19f && !hasMoved)
        {
            StartCoroutine(MoveWindowSmoothly());
            hasMoved = true; // 한 번만 실행되도록 설정
        }
    }

    IEnumerator MoveWindowSmoothly()
    {

        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(startPosition.x, startPosition.y, targetZ);

        while (elapsedTime < moveDuration)
        {
            if (isLocked) yield break; // 창문이 고정되었으면 즉시 코루틴 종료
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // 한 프레임 기다림
        }

        transform.position = targetPosition; // 이동이 끝나면 최종 위치 보정
        LockWindow(); // 창문 고정

        //이동이 끝난뒤 창문의 잡기기능 아예 삭제
        Destroy(HandMoveObject);
        Destroy(HandMoveObject_mirror);
        transform.position = targetPosition;

        //플레이어 행동 완료
        if (stepManager != null)
        {
            stepManager.OnPlayerActionCompleted();
        }
        else
        {
            Debug.LogError("StepManager를 찾을 수 없습니다.");
        }
    }
    void LockWindow()
    {
        isLocked = true;
    }
}
