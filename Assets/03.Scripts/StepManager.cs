using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StepManager : MonoBehaviour
{
    public StepUIManager stepUI; // UI 매니저 연결
    public List<Step> steps = new List<Step>();  // 모든 단계 정보 저장
    private int currentStepIndex = -1;  // 현재 단계 인덱스 (-1부터 시작)
    private bool isPlayerActionCompleted = false;  // 플레이어 행동 완료 여부
    private bool isStepInProgress = false;  // 중복 실행 방지용 플래그

    [SerializeField] GameObject _noticeUI;


    void Start()
    {
        DialogueManager.Instance.noticeUI = _noticeUI;
        DialogueManager.Instance._animator = _noticeUI.GetComponent<Animator>();

        NextStep();  // 첫 번째 단계 시작
    }

    void NextStep()
    {
        if (isStepInProgress) return;  // 중복 실행 방지
        isStepInProgress = true;

        if (currentStepIndex >= 0 && currentStepIndex < steps.Count)
        {
            EndStep(steps[currentStepIndex]);  // 이전 단계 정리
        }

        currentStepIndex++;

        if (currentStepIndex >= steps.Count)
        {
            Debug.Log("모든 단계를 완료했습니다.");
            return;
        }
        isPlayerActionCompleted = false;    //반드시 단계 시작 시 초기화

        StartStep(steps[currentStepIndex]);  // 새로운 단계 시작
        isStepInProgress = false;
    }

    void StartStep(Step step)
    {
        Debug.Log($"현재 단계: {step.stepName}");

        isPlayerActionCompleted = false;  // 새로운 단계에서 플레이어 행동 초기화

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
            if (outline != null)
            {
                outline.enabled = true;
            }

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
            //Debug.Log("고리게이지 활성화~");
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
            if (outline != null)
            {
                outline.enabled = false;
            }

            // 7. HandGrabInteractable / GuideHand 비활성화
            Transform[] children = obj.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.name.Contains("HandGrabInteractable") || child.name.Contains("HandGrabInteractable_Mirror"))
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        // 8. UI & 음성 활성화
        DialogueManager.Instance.ShowNext?.Invoke();

        // 9. 게이지 UI 비활성화
        if (step.gaugeUI != null)
        {
            step.gaugeUI.SetActive(false);
        }
    }

    public void CompleteCurrentStep()
    {
        if (isPlayerActionCompleted) return;  // 중복 실행 방지
        isPlayerActionCompleted = true;

        Debug.Log("현재 단계 완료. 다음 단계로 이동합니다.");

        Invoke(nameof(NextStep), 0.1f);  // 약간의 딜레이 후 실행 (혹시 모를 중복 호출 방지)
    }

    public void OnPlayerActionCompleted() // 해당 함수는 다른 스크립트에서 플레이어 행동이 완료될시 호출
    {
        // 5. 플레이어 행동 완료
        if (isPlayerActionCompleted) return;  // 이미 완료된 경우 실행 방지
        SoundManager.Instance.PlaySFX("0.Suc_bell");
        Debug.LogWarning(" OnPlayerActionCompleted() 호출됨!");
        Debug.LogWarning(Environment.StackTrace); // 누가 호출했는지 스택 출력
        //Debug.Log("플레이어가 행동을 완료했습니다.");
        CompleteCurrentStep();
    }

    // 물체를 Grab했을 때
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