using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class S2AutoMove : MonoBehaviour
{
    // 기존에 사용하던 요소들
    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;
    public StepManager stepManager;

    // 이동 관련 플래그
    private bool hasMovedToYNeg7 = false;  // Phase 1 (y = -7) 수행 여부
    private bool hasMovedToYNeg5 = false;  // Phase 2 (y = -5) 수행 여부
    private bool isAutoMoving = false;     // 자동 이동 중 여부

    // 이동 시간 및 목표 y 위치
    public float moveDuration = 1.0f;
    private float targetYPhase1 = -7f;  // Phase 1 목표 Y
    private float targetYPhase2 = -5f;  // Phase 2 목표 Y

    /// <summary>
    /// 외부(다른 스크립트)에서 S2가 잡혔을 때 호출하여 Phase 1 이동(y = -7)을 시작합니다.
    /// </summary>
    public void TriggerPhase1Move()
    {
        if (!hasMovedToYNeg7 && !isAutoMoving)
        {
            StartCoroutine(MoveYPosition(targetYPhase1, false));
            hasMovedToYNeg7 = true;
        }
    }

    void Update()
    {
        // 자동 이동 중에는 다른 처리를 하지 않음
        if (isAutoMoving)
            return;

        // [Phase 2] Phase 1 이후, 오브젝트가 회전되어(및 놓인 상태라고 가정) 로컬 x축 회전값이 -179 이하이면 실행
        if (hasMovedToYNeg7 && !hasMovedToYNeg5)
        {
            float localX = transform.localEulerAngles.x;
            if (localX > 180f)
                localX -= 360f;

            if (localX <= -179f)
            {
                StartCoroutine(MoveYPosition(targetYPhase2, true));
                hasMovedToYNeg5 = true;
            }
        }
    }

    /// <summary>
    /// 오브젝트의 y축 위치를 부드럽게 targetY까지 이동시키는 코루틴.
    /// isFinal이 true이면 Phase 2 완료 후 HandMoveObject들을 삭제하고 StepManager의 OnPlayerActionCompleted()를 호출합니다.
    /// </summary>
    /// <param name="targetY">목표 y 위치</param>
    /// <param name="isFinal">Phase 2 완료 여부</param>
    IEnumerator MoveYPosition(float targetY, bool isFinal)
    {
        isAutoMoving = true;

        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(startPosition.x, targetY, startPosition.z);

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;

        if (isFinal)
        {
            if (HandMoveObject != null)
                Destroy(HandMoveObject);
            if (HandMoveObject_mirror != null)
                Destroy(HandMoveObject_mirror);

            if (stepManager != null)
                stepManager.OnPlayerActionCompleted();
            else
                Debug.LogError("StepManager가 할당되지 않았습니다.");
        }

        isAutoMoving = false;
    }
}
