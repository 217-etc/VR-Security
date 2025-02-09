using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Step
{
    public string stepName;      // �ܰ� �̸�
    public GameObject target;    // ��ȣ�ۿ��� ������Ʈ
    public string dialogueKey;   // ���� �����Ϳ��� �ҷ��� Ű�� (TTS & UI ����)
    public int materialIndex;    // �� ��° ��Ƽ������ ������ ������ ����

    // ������
    public Step(string name, GameObject targetObj, string key, int matIndex)
    {
        stepName = name;
        target = targetObj;
        dialogueKey = key;   // ���� ���Ͽ��� �ҷ��� Ű��
        materialIndex = matIndex;  // ������ ��Ƽ������ �ε��� ����
    }

    // UI & TTS ���� �Լ� (�� ���� ó��)
    public void StartDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogueKey);  // UI + TTS �ڵ� ����
    }
}

/* 
 ����
 
Step step1 = new Step(
    "â�� ����",
    windowObject,
    "Dialogue_A001",  // ���� Ű�� (TTS & UI ����)
    1
);
*/
