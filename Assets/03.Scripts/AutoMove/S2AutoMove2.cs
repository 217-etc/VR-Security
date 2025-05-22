using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class S2AutoMove2 : MonoBehaviour
{
    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;
    public GameObject GaugeImage;
    public StepManager stepManager;

    public S2AutoMove1 s2AutoMove1; // S2AutoMove1을 참조

    private bool hasDone = false;
    private bool isLocked = false;
    private float targetY2 = -5.0f;
    private float moveDuration = 1.0f;

    void Start()
    {
        if (stepManager == null)
        { Debug.LogError("StepManager가 할당되지 않았습니다! Unity 인스펙터에서 할당하세요."); }
    }

    void Update()
    {
        if (isLocked || s2AutoMove1 == null) return;

        if (transform.localEulerAngles.x >= 340.0f && s2AutoMove1.HasMoved && s2AutoMove1.HalfMoved)
        {
            StartCoroutine(MoveS2RotateToTarget(359.5f));
        }

        if (hasDone)
        {
            StartCoroutine(MoveS2Smoothly2());
        }
    }

    IEnumerator MoveS2Smoothly2()
    {
        Destroy(HandMoveObject);
        Destroy(HandMoveObject_mirror);

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
        LockWindow();
        stepManager?.OnPlayerActionCompleted();
    }

    IEnumerator MoveS2RotateToTarget(float targetXRotation)
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
        hasDone = true;
    }

    void LockWindow()
    {
        isLocked = true;
        SoundManager.Instance.PlaySFX("Supporter");
        GaugeImage.SetActive(false); // 게이지 비활성화 삭제
    }
}
