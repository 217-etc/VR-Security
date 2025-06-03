using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class ScrewAutoMove_APT : MonoBehaviour
{
    public StepManager stepManager;
    private bool isLocked = false; // 나사가 고정되었는지 여부
    private bool isRotating = false; // 회전 중인지 여부
    private int rotateCount = 0; // 회전 횟수
    private float rotationStep = 120f; // 자동 회전 각도 (반시계 방향)
    private float positionStep = 0.0078547f; // Y축 이동 거리
    private float requiredRotation = 10f; // 사용자가 최소한으로 회전해야 하는 각도
    private Vector3 initialPosition; // 초기 위치 저장
    private float lastRotationY;

    public Outline outline;
    public GameObject gauge;
    public GameObject guideHand;

    public ScrewGauge screwGauge;  // 새로운 스크류 게이지 시스템 추가
    public GameObject supporterHandGrab1;
    public GameObject supporterHandGrab2;

    // 반대회전막기
    public OneGrabRotateTransformer_APT transformerScript; // 인스펙터에서 연결
    private bool hasSetMax = false;

    void Start()
    {
        initialPosition = transform.position;
        lastRotationY = transform.localEulerAngles.y;  // 초기값 설정
    }

    void Update()
    {
        if (isLocked || isRotating) return; // 회전 중이거나 고정된 경우 실행 안 함

        float currentRotationY = transform.localEulerAngles.y;
        float rotationDiff = Mathf.DeltaAngle(lastRotationY, currentRotationY);  // 회전 변화량 계산

        // 처음에 최소각도 설정
        if (!hasSetMax)
        {
            Debug.Log($"[각도 회전 시작]");
            transformerScript.SetInitialAngleOnce(currentRotationY);
            hasSetMax = true;
            
        }
        // 회전 중에는 MinAngle 계속 갱신
        if (hasSetMax)
        {
            transformerScript.UpdateMinAngle(currentRotationY);
        }

        // 반시계 방향 & 최소 일정 이상 회전 시 감지
        if (currentRotationY >= transformerScript.Constraints.MaxAngle.Value - 0.5f)
        {
            Debug.Log("각도 회전 감지: MaxAngle 도달 → 자동 회전 시작");
            StartCoroutine(AutoRotateAndMove());
            hasSetMax = false;
        }
        //lastRotationY = currentRotationY;
        //Debug.Log($"Local Y: {transform.localEulerAngles.y}, World Y: {transform.eulerAngles.y}");
    }

    IEnumerator AutoRotateAndMove()
    {
        isRotating = true;  // 회전 시작
        rotateCount++;

        float elapsedTime = 0f;
        float moveDuration = 1.0f; // 자동 이동 시간

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y - rotationStep, transform.eulerAngles.z); // 시계 방향 회전
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + new Vector3(0, positionStep, 0); // Y축 이동

        // HandGrabInteractable 비활성화
        ToggleGrabInteractable(false);

        while (elapsedTime < moveDuration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / moveDuration);
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        transform.position = targetPosition;

        // **마지막 회전값 업데이트 (다음 회전 감지를 위해)**
        lastRotationY = transform.localEulerAngles.y;

        // HandGrabInteractable 다시 활성화
        ToggleGrabInteractable(true);

        isRotating = false; // 회전 완료

        // **스크류 게이지 업데이트 (3번 회전 중 현재 몇 번째인지 비율로 전달)**
        if (screwGauge != null)
        {
            screwGauge.UpdateScrewGauge(rotateCount / 3f); // 0~1 값으로 변환
        }

        // 3번 회전하면 고정
        if (rotateCount >= 3)
        {
            SoundManager.Instance.PlaySFX("Link");
            isLocked = true;
            stepManager?.OnPlayerActionCompleted();

            // 6. 아웃라인 끄기
            outline.enabled = false;
            guideHand.SetActive(false);
            gauge.SetActive(false);
            supporterHandGrab1.SetActive(true);
            supporterHandGrab2.SetActive(true);
        }
    }

    // HandGrabInteractable 활성화/비활성화
    private void ToggleGrabInteractable(bool state)
    {
        Transform[] children = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.name.Contains("HandGrabInteractable") || child.name.Contains("HandGrabInteractable_Mirror"))
            {
                child.gameObject.SetActive(state);
                //Debug.Log("HandGrabInteractable 찾아서 활성화&비활성화!!");
            }
        }
    }
}
