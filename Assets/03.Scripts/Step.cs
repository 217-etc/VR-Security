using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Step
{
    public string stepName;      // 단계 이름
    public GameObject target;    // 상호작용할 오브젝트
    public string dialogueKey;   // 엑셀 데이터에서 불러올 키값 (TTS & UI 연동)
    

    // 생성자
    public Step(string name, GameObject targetObj, string key)
    {
        stepName = name;
        target = targetObj;
        dialogueKey = key;   // 엑셀 파일에서 불러올 키값
        
    }

    // UI & TTS 연동 함수 (한 번에 처리)
    /*
    public void StartDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogueKey);  // UI + TTS 자동 실행
    }
    */
}


