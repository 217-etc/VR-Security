using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    public List<Step> steps = new List<Step>();  // ��� �ܰ� ���� ����
    private int currentStepIndex = -1;  // ���� �ܰ� �ε��� (-1���� ����)

    private bool isPlayerActionCompleted = false;  // �÷��̾� �ൿ �Ϸ� ����

   
    void Start()
    {
        NextStep();  // ù ��° �ܰ� ����
    }

    void NextStep()
    {
        if (currentStepIndex >= 0 && currentStepIndex < steps.Count)
        {
            EndStep(steps[currentStepIndex]);  // ���� �ܰ� ����
        }

        currentStepIndex++;

        if (currentStepIndex >= steps.Count)
        {
            Debug.Log("��� �ܰ踦 �Ϸ��߽��ϴ�.");
            return;
        }

        StartStep(steps[currentStepIndex]);  // ���ο� �ܰ� ����
    }

    void StartStep(Step step)
    {
        Debug.Log($"���� �ܰ�: {step.stepName}");
        isPlayerActionCompleted = false;  // ���ο� �ܰ迡�� �÷��̾� �ൿ �ʱ�ȭ

        // 1. ������Ʈ ������ �ϱ� + (�ƿ����� �߰�)

        //���� ����κ�
        /* ChangeMaterial changeMaterial = step.target?.GetComponent<ChangeMaterial>();
        if (changeMaterial != null)
        {
            changeMaterial.ApplyHighlight(step.materialIndex);  // materialIndex ����
        }
        */
        //�ƿ����� �κ�
        Outline outline = step.target?.GetComponentInChildren<Outline>();
        if (outline != null)
        {
            outline.enabled = true;  // �ƿ����� �ѱ�
        }

        // 2. UI & ���� Ȱ��ȭ
        DialogueManager.Instance.StartDialogue(step.dialogueKey);
    }

    void EndStep(Step step)
    {
        Debug.Log($"�ܰ� ����: {step.stepName}");

        // 4. ���̶���Ʈ ����

        //���� ����κ�
        /*
        ChangeMaterial changeMaterial = step.target?.GetComponent<ChangeMaterial>();
        if (changeMaterial != null)
        {
            changeMaterial.RemoveHighlight(step.materialIndex);  // materialIndex ����
        }
        */
        //�ƿ����� �κ�
        Outline outline = step.target?.GetComponentInChildren<Outline>();
        if (outline != null)
        {
            outline.enabled = false; // �ƿ����� ����
        }

        // 5. UI & ���� Ȱ��ȭ
        DialogueManager.Instance.ShowNext?.Invoke();
    }

    public void CompleteCurrentStep()
    {
        if (isPlayerActionCompleted)
        {
            Debug.Log("���� �ܰ� �Ϸ�. ���� �ܰ�� �̵��մϴ�.");
            NextStep();
        }
    }

    public void OnPlayerActionCompleted() // �ش� �Լ��� �ٸ� ��ũ��Ʈ���� �÷��̾� �ൿ�� �Ϸ�ɽ� ȣ��
    {
        // 3. �÷��̾� �ൿ �Ϸ�
        Debug.Log("�÷��̾ �ൿ�� �Ϸ��߽��ϴ�.");
        isPlayerActionCompleted = true;
        CompleteCurrentStep();  // ���� �ܰ�� �̵�
    }
}