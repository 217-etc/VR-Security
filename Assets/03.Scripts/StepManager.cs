using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    public StepUIManager stepUI; // UI 매니저 연결
    public List<Step> steps = new List<Step>();  // 모든 단계 정보 저장
    private int currentStepIndex = -1;  // 현재 단계 인덱스 (-1부터 시작)
    private bool isPlayerActionCompleted = false;  // 플레이어 행동 완료 여부
    private bool isStepInProgress = false;  // 중복 실행 방지용 플래그


    void Start()
    {
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

        // 1. 오브젝트 아웃라인 활성화
        Outline outline = step.target?.GetComponentInChildren<Outline>();
        if (outline != null)
        {
            outline.enabled = true;  // 아웃라인 켜기
        }

        // 2. UI & 음성 활성화 + 오브젝트 할당UI 활성화
        DialogueManager.Instance.StartDialogue(step.dialogueKey);

        // 3. 해당 타겟 오브젝트의 자식에서 HandGrabInteractable 찾아 활성화하기
        Transform[] children = step.target.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.name.Contains("HandGrabInteractable") || child.name.Contains("HandGrabInteractable_Mirror"))
            {
                child.gameObject.SetActive(true);
            }
        }

        // 4. 게이지 UI 활성화
        if (step.gaugeUI != null)
        {
            Debug.Log("고리게이지 활성화~");
            step.gaugeUI.SetActive(true);
        }

        // 5. 가이드 손 활성화 (GuideHand)
        foreach (Transform child in children)
        {
            if (child.name.Contains("GuideHand"))
            {
                child.gameObject.SetActive(true);
            }
        }

    }

    void EndStep(Step step)
    {
        Debug.Log($"단계 종료: {step.stepName}");

        // 6. 오브젝트 아웃라인 비활성화
        Outline outline = step.target?.GetComponentInChildren<Outline>();
        if (outline != null)
        {
            outline.enabled = false; // 아웃라인 끄기
        }

        // 7. UI & 음성 활성화
        DialogueManager.Instance.ShowNext?.Invoke();

        // 8. 게이지 UI 비활성화
        if (step.gaugeUI != null)
        {
            step.gaugeUI.SetActive(false);
        }

        // 9. HandGrab 오브젝트 비활성화
        Transform[] children = step.target.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.name.Contains("HandGrabInteractable") || child.name.Contains("HandGrabInteractable_Mirror"))
            {
                child.gameObject.SetActive(false); // 비활성화만
            }
        }
    }

    public void CompleteCurrentStep()
    {
        if (isPlayerActionCompleted) return;  // 🔹 중복 실행 방지
        isPlayerActionCompleted = true;

        Debug.Log("현재 단계 완료. 다음 단계로 이동합니다.");

        Invoke(nameof(NextStep), 0.1f);  // 약간의 딜레이 후 실행 (혹시 모를 중복 호출 방지)
    }

    public void OnPlayerActionCompleted() // 해당 함수는 다른 스크립트에서 플레이어 행동이 완료될시 호출
    {
        // 10. 플레이어 행동 완료
        if (isPlayerActionCompleted) return;  // 이미 완료된 경우 실행 방지

        Debug.Log("플레이어가 행동을 완료했습니다.");
        CompleteCurrentStep();
    }

    // 물체를 Grab했을 때
    public void WhenGrabbedObject()
    {
        if (currentStepIndex >= 0 && currentStepIndex < steps.Count)
        {
            Transform[] children = steps[currentStepIndex].target.GetComponentsInChildren<Transform>(true);
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