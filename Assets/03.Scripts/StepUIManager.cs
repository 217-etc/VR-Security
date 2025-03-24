using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StepUIManager : MonoBehaviour
{
    public TextMeshProUGUI stepText; // UI Text

    public void UpdateStepText(string stepName)
    {
        Debug.Log("UpdateStepText Called: " + stepName); // 호출 확인용 로그
        if (stepText != null)
        {
            stepText.text = stepName;
        }
    }
}
