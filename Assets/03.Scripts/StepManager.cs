using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    public List<Step> steps = new List<Step>();  // 모든 단계 정보 저장
    private int currentStepIndex = -1;  // 현재 단계 인덱스 (-1부터 시작)

    private bool isPlayerActionCompleted = false;  // 플레이어 행동 완료 여부

    void Start()
    {
        NextStep();  // 첫 번째 단계 시작
    }

    void NextStep()
    {
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
    }

    void StartStep(Step step)
    {
        Debug.Log($"현재 단계: {step.stepName}");
        isPlayerActionCompleted = false;  // 새로운 단계에서 플레이어 행동 초기화

        // 1. 오브젝트 빛나게 하기 + (아웃라인 추가)
        ChangeMaterial changeMaterial = step.target?.GetComponent<ChangeMaterial>();
        if (changeMaterial != null)
        {
            changeMaterial.ApplyHighlight(step.materialIndex);  // materialIndex 전달
        }

        // 2. UI & 음성 활성화
        step.StartDialogue();
    }

    void EndStep(Step step)
    {
        Debug.Log($"단계 종료: {step.stepName}");

        // 4. 하이라이트 제거
        ChangeMaterial changeMaterial = step.target?.GetComponent<ChangeMaterial>();
        if (changeMaterial != null)
        {
            changeMaterial.RemoveHighlight(step.materialIndex);  // materialIndex 전달
        }

        // 5. UI & 음성 활성화
        DialogueManager.Instance.ShowNext?.Invoke();
    }

    public void CompleteCurrentStep()
    {
        if (isPlayerActionCompleted)
        {
            Debug.Log("현재 단계 완료. 다음 단계로 이동합니다.");
            NextStep();
        }
    }

    public void OnPlayerActionCompleted() // 해당 함수는 다른 스크립트에서 플레이어 행동이 완료될시 호출
    {
        // 3. 플레이어 행동 완료
        isPlayerActionCompleted = true;
        CompleteCurrentStep();  // 다음 단계로 이동
    }
}