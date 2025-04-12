using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Step
{
    public string stepName;      // 단계 이름
    public string dialogueKey;   // 엑셀 데이터에서 불러올 키값 (TTS & UI 연동)
    public List<GameObject> target;
    public GameObject gaugeUI;   // 단계별 게이지 UI 추가   
}


