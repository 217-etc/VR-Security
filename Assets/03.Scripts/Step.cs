using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Step
{
    public string stepName;      // 단계 이름
    public GameObject target;    // 상호작용할 오브젝트
    public string dialogueKey;   // 엑셀 데이터에서 불러올 키값 (TTS & UI 연동)
    public int materialIndex;    // 몇 번째 머티리얼을 변경할 것인지 저장

    // 생성자
    public Step(string name, GameObject targetObj, string key, int matIndex)
    {
        stepName = name;
        target = targetObj;
        dialogueKey = key;   // 엑셀 파일에서 불러올 키값
        materialIndex = matIndex;  // 변경할 머티리얼의 인덱스 저장
    }

    // UI & TTS 연동 함수 (한 번에 처리)
    public void StartDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogueKey);  // UI + TTS 자동 실행
    }
}

/* 
 예제
 
Step step1 = new Step(
    "창문 열기",
    windowObject,
    "Dialogue_A001",  // 엑셀 키값 (TTS & UI 연동)
    1
);
*/
