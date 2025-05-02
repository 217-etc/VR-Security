using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StepManager : MonoBehaviour
{
    public StepUIManager stepUI;                    // UI 매니저 연결
    public List<Step> steps = new List<Step>();     // 모든 단계 정보 저장
    private int currentStepIndex = -1;              // 현재 단계 인덱스 (-1부터 시작)
    private bool isPlayerActionCompleted = false;   // 플레이어 행동 완료 여부
    private bool isStepInProgress = false;          // 중복 실행 방지용 플래그

    [SerializeField] GameObject _noticeUI;

    void Start()
    {
        DialogueManager.Instance.noticeUI = _noticeUI;
        DialogueManager.Instance._animator = _noticeUI.GetComponent<Animator>();

        NextStep(); // 첫 번째 단계 시작
    }

    void NextStep()
    {
        if (isStepInProgress) return;           // 중복 실행 방지
        isStepInProgress = true;

        currentStepIndex++;

        if (currentStepIndex >= steps.Count)
        {
            Debug.Log("모든 단계를 완료했습니다.");
            return;
        }

        isPlayerActionCompleted = false;

        StartStep(steps[currentStepIndex]);     // 새로운 단계 시작
        isStepInProgress = false;
    }

    void StartStep(Step step)
    {
        Debug.Log($"현재 단계: {step.stepName}");

        isPlayerActionCompleted = false;        // 새로운 단계에서 플레이어 행동 초기화

        if (stepUI == null)
        {
            stepUI = FindObjectOfType<StepUIManager>();
        }
        if (stepUI != null)
        {
            stepUI.UpdateStepText(step.stepName);
        }

        foreach (GameObject obj in step.target)
        {
            // 1. 아웃라인 켜기
            Outline outline = obj?.GetComponentInChildren<Outline>();
            if (outline != null) outline.enabled = true;

            // 2. HandGrabInteractable / GuideHand 활성화
            Transform[] children = obj.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.name.Contains("HandGrabInteractable") || child.name.Contains("HandGrabInteractable_Mirror"))
                {
                    child.gameObject.SetActive(true);
                }

                if (child.name.Contains("GuideHand"))
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
        // 3. UI & 음성 활성화
        DialogueManager.Instance.StartDialogue(step.dialogueKey);

        // 4. 게이지 UI 활성화
        if (step.gaugeUI != null)
        {
            step.gaugeUI.SetActive(true);
        }
    }

    void EndStep(Step step)
    {
        Debug.Log($"단계 종료: {step.stepName}");

        foreach (GameObject obj in step.target)
        {
            // 6. 아웃라인 끄기
            Outline outline = obj?.GetComponentInChildren<Outline>();
            if (outline != null) outline.enabled = false;

            // 7. HandGrabInteractable 비활성화
            Transform[] children = obj.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.name.Contains("HandGrabInteractable") || child.name.Contains("HandGrabInteractable_Mirror"))
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        DialogueManager.Instance.ShowNext?.Invoke();

        // 게이지 UI 비활성화는 WaitForDialogueThenProceed에서
    }

    public void CompleteCurrentStep()
    {
        if (isPlayerActionCompleted) return;    // 중복 실행 방지
        isPlayerActionCompleted = true;

        StartCoroutine(WaitForDialogueThenProceed());
    }

    private IEnumerator WaitForDialogueThenProceed()
    {
        // 대사 진행 중이면 대기
        while (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
        {
            yield return null;
        }

        // 8. 게이지 UI 비활성화
        Step step = steps[currentStepIndex];
        if (step.gaugeUI != null)
        {
            step.gaugeUI.SetActive(false);
        }

        // 다음 단계로 이동
        Invoke(nameof(NextStep), 0.1f);
    }

    // 5. 플레이어 행동 완료
    public void OnPlayerActionCompleted()      // 해당 함수는 다른 스크립트에서 플레이어 행동이 완료될시 호출
    {
        if (isPlayerActionCompleted) return;

        SoundManager.Instance.PlaySFX("0.Suc_bell");
        Debug.LogWarning(" OnPlayerActionCompleted() 호출됨!");
        Debug.LogWarning(Environment.StackTrace);

        EndStep(steps[currentStepIndex]);
        CompleteCurrentStep();
    }

    // 물체를 Grab했을 때
    // 7. GuideHand 비활성화
    public void WhenGrabbedObject()
    {
        SoundManager.Instance.PlaySFX("0.Grabbing");
        if (currentStepIndex >= 0 && currentStepIndex < steps.Count)
        {
            foreach (GameObject obj in steps[currentStepIndex].target)
            {
                Transform[] children = obj.GetComponentsInChildren<Transform>(true);
                foreach (Transform child in children)
                {
                    if (child.name.Contains("GuideHand"))
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
