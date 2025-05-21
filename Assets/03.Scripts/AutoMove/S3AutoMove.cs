using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class S3AutoMove : MonoBehaviour
{
    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;
    public StepManager stepManager;
    public S4AutoMove1 s4AutoMove1;

    private bool isLocked = false;
    private bool S3Moved = false;

    void Start()
    {
        DeactivateHandMoveObjects(); // 처음엔 그랩 비활성화

        if (stepManager == null)
        {
            Debug.LogError("StepManager가 할당되지 않았습니다! Unity 인스펙터에서 할당하세요.");
        }

        if (s4AutoMove1 == null)
        {
            Debug.LogError("S4Controller가 할당되지 않았습니다! Unity 인스펙터에서 할당하세요.");
        }
    }

    void Update()
    {
        if (isLocked) return;

        bool S41Moved = s4AutoMove1 != null && s4AutoMove1.GetS41Moved();

        // S41Moved가 참이면 그랩 활성화
        if (S41Moved) { ActivateHandMoveObjects(); }

        // S41Moved가 참이고 x축 회전값이 0도이면 → 그랩 비활성화 + S3Moved = true
        // (인스펙터) 90 -> 180으로 이동 / 콘솔: 90 -> 0
        // S4의 x축 회전값이 20도보다 작아지면 자동 이동
        if (S41Moved && transform.localEulerAngles.x <= 20.0f)
        {
            DeactivateHandMoveObjects();
            StartCoroutine(RotateS3(0.1f));
            S3Moved = true;
        }
    }

    IEnumerator RotateS3(float targetXRotation)
    {
        float elapsedTime = 0f;
        float moveDuration = 1.0f;
        float startXRotation = transform.localEulerAngles.x;

        while (elapsedTime < moveDuration)
        {
            if (isLocked) yield break;
            float newXRotation = Mathf.Lerp(startXRotation, targetXRotation, elapsedTime / moveDuration);
            transform.localEulerAngles = new Vector3(newXRotation, transform.localEulerAngles.y, transform.localEulerAngles.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localEulerAngles = new Vector3(targetXRotation, transform.localEulerAngles.y, transform.localEulerAngles.z);
        LockWindow();
        stepManager?.OnPlayerActionCompleted();
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

    void LockWindow() { 
        isLocked = true;
        SoundManager.Instance.PlaySFX("Supporter");
    }

    public bool GetS3Moved() { return S3Moved; }
}
