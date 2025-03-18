using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class S2AutoMove3 : MonoBehaviour
{
    private bool hasMoved = false; // S2가 한 번 이동했는지 체크
    private bool halfMoved = false;
    private bool hasDone = false; // S2가 두 번째 이동했는지 체크
    private bool isLocked = false;
    private float targetY1 = -2.0f; // 첫 목표 Y
    private float targetY2 = -5.0f; // 두 번째 목표 Y
    private float moveDuration = 1.0f;

    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;
    public StepManager stepManager;

    public AudioClip sound1; // 재생할 AudioClip
    private AudioSource audioSource; // AudioSource 변수 추가

    void Start()
    {
        //stepManager = FindObjectOfType<StepManager>();
        audioSource = gameObject.AddComponent<AudioSource>();
        if (stepManager == null)
        { Debug.LogError("StepManager가 할당되지 않았습니다! Unity 인스펙터에서 할당하세요."); }
    }

    void Update()
    {
        if (isLocked) return;
        Debug.Log("S2의 X축 회전값: " + transform.localEulerAngles.x + " / S2 y축 위치: " + transform.localPosition.y);

        // 일단 y축 위치가 -3보다 작으면 -3으로 이동
        // 잡는 모션 -> 각도가 359보다 작아지면 = 잡고 좀이라도 움직이면
        if (transform.localEulerAngles.x <=359.0f && transform.localPosition.y <= -3f && !hasMoved)
        {
            HandMoveObject.SetActive(false);
            HandMoveObject_mirror.SetActive(false);

            StartCoroutine(MoveS2Smoothly1());
            hasMoved = true;

            HandMoveObject.SetActive(true);
            HandMoveObject_mirror.SetActive(true);
        }

        if (transform.localEulerAngles.x >= 269.0f && transform.localEulerAngles.x <= 271.0f)
        {
            halfMoved = true;
            // Debug.Log("S2 절반 넘어감");
        }

        // y 위치가 -5보다 큼, x 회전값이 -15보다 큼, 한 번 움직인 적이 있으면
        // 각도 콘솔 출력값: 359 -> 270 -> 359 -> 0+. x축 회전값 -179, -1로 시작했을 때 똑같음.
        // 근데 1로 시작하면 넘어가는 순간 360도 돼버려서 끝나고, 반대쪽으로 돌리면 1 -> 90 -> 1- 돼서 또 똑같음;;;;
        if (transform.localEulerAngles.x >= 340.0f && hasMoved && halfMoved)
        {
            StartCoroutine(MoveS2RotateToTarget(359.5f));
        }

        if (hasDone)
        {
            StartCoroutine(MoveS2Smoothly2());
        }
    }

    IEnumerator MoveS2Smoothly1()
    {
        float elapsedTime = 0f;
        Vector3 S2startPosition = transform.localPosition;
        Vector3 S2targetPosition = new Vector3(S2startPosition.x, targetY1, S2startPosition.z);

        while (elapsedTime < moveDuration)
        {
            if (isLocked) yield break;
            transform.localPosition = Vector3.Lerp(S2startPosition, S2targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = S2targetPosition;
    }

    IEnumerator MoveS2Smoothly2()
    {
        float elapsedTime = 0f;
        Vector3 S2startPosition2 = transform.localPosition;
        Vector3 S2targetPosition2 = new Vector3(S2startPosition2.x, targetY2, S2startPosition2.z);

        while (elapsedTime < moveDuration)
        {
            if (isLocked) yield break;
            transform.localPosition = Vector3.Lerp(S2startPosition2, S2targetPosition2, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = S2targetPosition2; 
        Destroy(HandMoveObject);
        Destroy(HandMoveObject_mirror);
        LockWindow();
    }

    IEnumerator MoveS2RotateToTarget(float targetXRotation)
    {
        float elapsedTime = 0f;
        float moveDuration = 1.0f; // 회전하는 데 걸리는 시간
        float startXRotation = transform.localEulerAngles.x;

        while (elapsedTime < moveDuration)
        {
            if (isLocked) yield break; // 고정 상태면 종료

            // 현재 회전값과 목표 회전값 사이를 보간
            float newXRotation = Mathf.Lerp(startXRotation, targetXRotation, elapsedTime / moveDuration);
            transform.localEulerAngles = new Vector3(newXRotation, transform.localEulerAngles.y, transform.localEulerAngles.z);

            elapsedTime += Time.deltaTime;
            yield return null; // 한 프레임 기다림
        }

        // 최종 회전값 보정
        transform.localEulerAngles = new Vector3(targetXRotation, transform.localEulerAngles.y, transform.localEulerAngles.z);
        hasDone = true;
    }

    void LockWindow()
    {
        isLocked = true;
        if (audioSource != null && sound1 != null)
        {
            audioSource.clip = sound1;
            audioSource.Play();
        }
    }
}
