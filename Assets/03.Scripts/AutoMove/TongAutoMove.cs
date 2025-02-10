using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class TongAutoMove : MonoBehaviour
{
    private bool hasRotated = false; // 이미 이동했는지 체크
    private bool isLocked = false; // 목표 각도에 도달했는지 체크
    private float targetRotation = -90f; // 목표 회전각도
    private float moveDuration = 1.0f; // 이동하는 데 걸리는 시간
    private float rotationThreshold = -60f; // 자동 회전 트리거 각도

    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;

    public StepManager stepManager;

    void Start()
    {
        if (stepManager == null)
        {
            Debug.LogError("StepManager가 할당되지 않았습니다! Unity 인스펙터에서 할당하세요.");
        }
    }

    void Update()
    {
        if (isLocked) return; // 이미 목표에 도달했다면 실행 X

        float angleX = transform.localEulerAngles.x;
        if (angleX > 180f) angleX -= 360f; // 360도를 넘어가면 -값으로 변환

        // 사용자가 통을 열다가 손을 놓았을 때, 특정 각도 이하인지 확인 후 자동 복귀
        if (angleX <= rotationThreshold && !hasRotated)
        {
            StartCoroutine(RotateBackSmoothly());
            hasRotated = true; // 한 번만 실행되도록 설정
        }
    }

    IEnumerator RotateBackSmoothly()
    {
        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotationQ = Quaternion.Euler(targetRotation, transform.eulerAngles.y, transform.eulerAngles.z);

        while (elapsedTime < moveDuration)
        {
            if (isLocked) yield break; // 이미 고정되었다면 즉시 종료
            transform.rotation = Quaternion.Lerp(startRotation, targetRotationQ, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotationQ; // 최종 보정
        LockTong(); // 통 고정

        // 이동이 끝난 뒤 잡기 기능 제거
        Destroy(HandMoveObject);
        Destroy(HandMoveObject_mirror);

        // 플레이어 행동 완료
        if (stepManager != null)
        {
            stepManager.OnPlayerActionCompleted();
        }
        else
        {
            Debug.LogError("StepManager를 찾을 수 없습니다.");
        }
    }

    void LockTong()
    {
        isLocked = true;
    }
}
