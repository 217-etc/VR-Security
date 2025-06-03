using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class S3AutoMoveEdited : MonoBehaviour
{
    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;
    public StepManager stepManager;
    public S4AutoMoveEdited s4AutoMoveEdited;
    public GameObject GaugeImage;
    public GameObject GuideHand;
    public Outline outline3;

    private bool isLocked = false;
    private bool S3Moved = false;
    private bool S3setting = false;

    void Start()
    {
        DeactivateHandMoveObjects(); // 처음엔 그랩 비활성화

        if (stepManager == null)
        {
            Debug.LogError("StepManager가 할당되지 않았습니다! Unity 인스펙터에서 할당하세요.");
        }
    }

    void Update()
    {
        if (isLocked) return;

        bool S4Done = s4AutoMoveEdited != null && s4AutoMoveEdited.GetS4Done();

        // S4Done이 참이면 게이지, 그랩 활성화
        if (S4Done && !S3setting) { 
            ActivateHandMoveObjects();
            outline3.enabled = true;
            GaugeImage.SetActive(true);
            GuideHand.SetActive(true);
            S3setting = true;
        }

        // S41Moved가 참이고 x축 회전값이 0도이면 → 그랩 비활성화 + S3Moved = true
        // (인스펙터) 90 -> 180으로 이동 / 콘솔: 90 -> 0
        // S4의 x축 회전값이 20도보다 작아지면 자동 이동
        if (S4Done && transform.localEulerAngles.x <= 20.0f)
        {
            DeactivateHandMoveObjects();
            Destroy(GuideHand);
            outline3.enabled = false;
            StartCoroutine(RotateS3(0.1f));
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
        isLocked = true;
        SoundManager.Instance.PlaySFX("Supporter");
        GaugeImage.SetActive(false);
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
}
