using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupCheck : MonoBehaviour
{
    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;
    public GameObject S2CKUI; // 섭첵 확인 UI
    public GameObject StepUI; // 스텝 UI
    public GameObject GuideHand21; // 섭첵 가이드손
    public StepManager stepManager;
    public ToolGrabManager toolGrab;
    public Outline outline2;

    private bool supChecked = false;
    private bool SCKSetting = false;
    public bool SupChecked => supChecked; // 외부에서 읽기 전용 접근 허용 (S21 차용)

    void Start()
    {
        if (stepManager == null)
        { Debug.LogError("StepManager가 할당되지 않았습니다! Unity 인스펙터에서 할당하세요."); }
    }

    void Update()
    {
        if (toolGrab.ToolDone && !SCKSetting) 
        {
            DialogueManager.Instance.StartDialogue("Dialogue_A004-01"); // 대사 출력
            HandMoveObject.SetActive(true);
            HandMoveObject_mirror.SetActive(true);
            stepManager.gameObject.SetActive(false);
            StepUI.SetActive(false); // 체크 UI 켜기
            S2CKUI.SetActive(true); // 체크 UI 켜기
            outline2.enabled = true; // 아웃라인 켜기
            GuideHand21.SetActive(true); // 가이드손 켜기
            SCKSetting = true;
            StartCoroutine(WaitForInitialDialogue());
        }
    }

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
        SoundManager.Instance.PlaySFX("0.Suc_bell");
        DialogueManager.Instance.StartDialogue("Dialogue_A004-01_act"); // 대사 출력
        HandMoveObject.SetActive(true);
        HandMoveObject_mirror.SetActive(true);
        supChecked = true;
        Destroy(GuideHand21); // 가이드손 끄기
        S2CKUI.SetActive(false); // 체크 UI 끄기
        //stepManager?.OnPlayerActionCompleted();
    }

    //대사 대기 함수
    private IEnumerator WaitForInitialDialogue()
    {
        HandMoveObject.SetActive(false);
        HandMoveObject_mirror.SetActive(false);
        while (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
        {
            yield return null;
        }
        HandMoveObject.SetActive(true);
        HandMoveObject_mirror.SetActive(true);
    }
}
