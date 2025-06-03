using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupCheck : MonoBehaviour
{
    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;
    public GameObject GuideHand21;
    public GameObject GuideHand211;
    public StepManager stepManager;

    private bool supChecked = false;
    public bool SupChecked => supChecked; // 외부에서 읽기 전용 접근 허용 (S21 차용)

    void Start()
    {
        if (stepManager == null)
        { Debug.LogError("StepManager가 할당되지 않았습니다! Unity 인스펙터에서 할당하세요."); }
        GuideHand21.SetActive(true);
    }

    /*void Update()
    {
        //SupChecked = supChecked;
        Debug.Log("섭첵 값: " + supChecked);
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
        yield return new WaitForSeconds(2f);

        SoundManager.Instance.PlaySFX("0.Suc_bell");
        HandMoveObject.SetActive(true);
        HandMoveObject_mirror.SetActive(true);
        supChecked = true;
        Debug.Log("섭첵 값: " + supChecked);
        Destroy(GuideHand21);
        GuideHand211.SetActive(true);
        stepManager?.OnPlayerActionCompleted();
    }
}
