using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupCheck : MonoBehaviour
{
    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;
    public StepManager stepManager;

    private bool supChecked = false;
    public bool SupChecked => supChecked; // 외부에서 읽기 전용 접근 허용 (S21 차용)

    void Start()
    {
        if (stepManager == null)
        { Debug.LogError("StepManager가 할당되지 않았습니다! Unity 인스펙터에서 할당하세요."); }
    }

    /*void Update()
    {
        SupChecked = supChecked;
    }*/

    public void setChecked()
    {
        if (supChecked) { return; }
        StartCoroutine(DelayFunction());
    }

    IEnumerator DelayFunction()
    {
        // 1초 대기
        HandMoveObject.SetActive(false);
        HandMoveObject_mirror.SetActive(false);
        yield return new WaitForSeconds(1f);

        // 1초 후에 완료 처리
        SoundManager.Instance.PlaySFX("Supporter");
        supChecked = true;
        HandMoveObject.SetActive(true);
        HandMoveObject_mirror.SetActive(true);

        stepManager?.OnPlayerActionCompleted();
    }
}
