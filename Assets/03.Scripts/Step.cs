using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Step
{
    public string stepName;      // �ܰ� �̸�
    public GameObject target;    // ��ȣ�ۿ��� ������Ʈ
    public string dialogueKey;   // ���� �����Ϳ��� �ҷ��� Ű�� (TTS & UI ����)
    

    // ������
    public Step(string name, GameObject targetObj, string key)
    {
        stepName = name;
        target = targetObj;
        dialogueKey = key;   // ���� ���Ͽ��� �ҷ��� Ű��
        
    }

    // UI & TTS ���� �Լ� (�� ���� ó��)
    /*
    public void StartDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogueKey);  // UI + TTS �ڵ� ����
    }
    */
}


