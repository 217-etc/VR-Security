using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class S4AutoMove : MonoBehaviour
{
    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;

    public StepManager stepManager;
    public S3AutoMove s3AutoMove; // S3 스크립트 참조

    private float moveDuration = 1.0f; // 이동하는 데 걸리는 시간
    private bool S41Moved = false;

    void Start()
    {
        ActivateHandMoveObjects(); // 처음엔 그랩 활성화

        if (stepManager == null)
        {
            Debug.LogError("StepManager가 할당되지 않았습니다! Unity 인스펙터에서 할당하세요.");
        }

        if (s3AutoMove == null)
        {
            Debug.LogError("S3Controller가 할당되지 않았습니다! Unity 인스펙터에서 할당하세요.");
        }
    }

    void Update()
    {
        bool S3Moved = s3AutoMove != null && s3AutoMove.GetS3Moved(); // 할당 잘 됨
        // Debug.Log("S4의 y축 회전값: " + transform.localEulerAngles.y);

        // S41Move: 물체를 잡으면 (y축 회전값이 0.5도보다 커지면) → x축 0으로 자동 이동 후 (올리기) 그랩 비활성화 + S41Moved = true
        if (transform.localEulerAngles.y >= 0.5f)
        {
            StartCoroutine(MoveToPosition(new Vector3(0f, transform.localPosition.y, transform.localPosition.z), false));
            S41Moved = true; // 잘 움직임
            Debug.Log("S41Moved");
        }

        // S3Moved와 S41Moved가 모두 참이면 → 자동으로 x축 위치 1.5까지 이동 후 (내리기) 그랩 활성화
        if (S3Moved && S41Moved)
        {
            StartCoroutine(MoveToPosition(new Vector3(1.5f, transform.localPosition.y, transform.localPosition.z), true));
            Debug.Log("S42Moved");
        }

        // S3Moved, S41Moved가 모두 참이고 S4의 y축 회전값이 85도보다 크면 → 그랩 비활성화 후 스텝매니저 호출
        if (S3Moved && S41Moved && transform.localEulerAngles.y >= 85.0f)
        {
            DeactivateHandMoveObjects();

            Debug.Log("S4 모든 행동 완료");
            stepManager.OnPlayerActionCompleted();
        }
    }

    // private void OnTriggerEnter(Collider other) { float yRotation = transform.eulerAngles.y; 원래 여기에 S41Move 들어감 }

    IEnumerator MoveToPosition(Vector3 targetPosition, bool enableGrabAfterMove)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.localPosition;

        while (elapsedTime < moveDuration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition; // 최종 위치 보정

        if (enableGrabAfterMove) { ActivateHandMoveObjects(); }
    }

    public void ActivateHandMoveObjects()
    {
        HandMoveObject.SetActive(true);
        HandMoveObject_mirror.SetActive(true);
    }

    public void DeactivateHandMoveObjects()
    {
        HandMoveObject.SetActive(false);
        HandMoveObject_mirror.SetActive(false);
    }

    public bool GetS41Moved() { return S41Moved; }
}

