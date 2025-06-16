using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class GoriPartAutoMove : MonoBehaviour
{
    private bool hasRotated = false;
    private bool isLocked = false;
    private bool isOpen = false;

    private float targetYRotation = 270f;       // 최종 목표 Y각도
    private float moveDuration = 1.0f;          // 자동 회전 시간
    private float rotationThreshold = 265f;     // 이 이상일 때 자동 회전 트리거

    public StepManager stepManager;

    [SerializeField] private List<Rigidbody> _toolRigidbody = new List<Rigidbody>();

    void Start()
    {
        if (stepManager == null)
        {
            Debug.LogError("StepManager가 할당되지 않았습니다! Unity 인스펙터에서 할당하세요.");
        }
    }

    void Update()
    {
        if (isLocked) return;

        float angleY = transform.localEulerAngles.y;
        Debug.Log($"[GoriPartAutoMove] 현재 Y회전각도: {angleY}");

        // 사운드는 180 이상 돌았을 때 1번만 재생
        if (angleY > 180f && !isOpen)
        {
            SoundManager.Instance.PlaySFX("4-1.Tong");
            isOpen = true;
        }

        // 특정 각도 이상으로 회전 시 자동으로 마지막 위치까지 회전
        if (angleY >= rotationThreshold && !hasRotated)
        {
            StartCoroutine(RotateBackSmoothly());
            hasRotated = true;
        }
    }

    IEnumerator RotateBackSmoothly()
    {
        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotationQ = Quaternion.Euler(transform.eulerAngles.x, targetYRotation, transform.eulerAngles.z);

        while (elapsedTime < moveDuration)
        {
            if (isLocked) yield break;

            transform.rotation = Quaternion.Lerp(startRotation, targetRotationQ, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotationQ;
        LockPart();

        if (stepManager != null)
        {
            stepManager.OnPlayerActionCompleted();
        }
        else
        {
            Debug.LogError("StepManager를 찾을 수 없습니다.");
        }
    }

    void LockPart()
    {
        isLocked = true;
    }
}
