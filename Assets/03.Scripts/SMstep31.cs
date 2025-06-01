using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMstep31 : MonoBehaviour
{
    public StepManager stepManager;

    void Start()
    {
        if (stepManager == null)
        { Debug.LogError("StepManager가 할당되지 않았습니다! Unity 인스펙터에서 할당하세요."); }
        StartCoroutine(DelayFunction31());
    }

    IEnumerator DelayFunction31()
    {
        // 10초 대기
        yield return new WaitForSeconds(10f);

        SoundManager.Instance.PlaySFX("Supporter");
        stepManager?.OnPlayerActionCompleted();
    }
}
