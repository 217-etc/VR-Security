using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StepManager : MonoBehaviour
{
    public StepUIManager stepUI;
    public List<Step> steps = new List<Step>();
    private int currentStepIndex = -1;
    private bool isPlayerActionCompleted = false;
    private bool isStepInProgress = false;

    [SerializeField] GameObject _noticeUI;

    void Start()
    {
        DialogueManager.Instance.noticeUI = _noticeUI;
        DialogueManager.Instance._animator = _noticeUI.GetComponent<Animator>();

        NextStep();
    }

    void NextStep()
    {
        if (isStepInProgress) return;
        isStepInProgress = true;

        /*
        if (currentStepIndex >= 0 && currentStepIndex < steps.Count)
        {
            EndStep(steps[currentStepIndex]);  // 이전 단계 정리
        }*/

        currentStepIndex++;

        if (currentStepIndex >= steps.Count)
        {
            Debug.Log("모든 단계를 완료했습니다.");
            return;
        }

        isPlayerActionCompleted = false;
        StartStep(steps[currentStepIndex]);
        isStepInProgress = false;
    }

    void StartStep(Step step)
    {
        Debug.Log($"현재 단계: {step.stepName}");

        isPlayerActionCompleted = false;

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
            Outline outline = obj?.GetComponentInChildren<Outline>();
            if (outline != null) outline.enabled = true;

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

        DialogueManager.Instance.StartDialogue(step.dialogueKey);

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
            Outline outline = obj?.GetComponentInChildren<Outline>();
            if (outline != null) outline.enabled = false;

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
        if (isPlayerActionCompleted) return;
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

        // 게이지 UI 비활성화
        Step step = steps[currentStepIndex];
        if (step.gaugeUI != null)
        {
            step.gaugeUI.SetActive(false);
        }

        // 다음 단계로 이동
        Invoke(nameof(NextStep), 0.1f);
    }

    public void OnPlayerActionCompleted()
    {
        if (isPlayerActionCompleted) return;

        SoundManager.Instance.PlaySFX("0.Suc_bell");
        Debug.LogWarning(" OnPlayerActionCompleted() 호출됨!");
        Debug.LogWarning(Environment.StackTrace);

        // 아웃라인/HandGrab 비활성화 처리 & 대사 실행
        /*Step step = steps[currentStepIndex];
        foreach (GameObject obj in step.target)
        {
            Outline outline = obj?.GetComponentInChildren<Outline>();
            if (outline != null) outline.enabled = false;

            Transform[] children = obj.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.name.Contains("HandGrabInteractable") || child.name.Contains("HandGrabInteractable_Mirror"))
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        DialogueManager.Instance.ShowNext?.Invoke();*/
        EndStep(steps[currentStepIndex]);
        CompleteCurrentStep();
    }

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
