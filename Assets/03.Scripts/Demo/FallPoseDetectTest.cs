using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallPoseDetectTest : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject eye;
    public GameObject ui;
   
    void Update()
    {
        if(IsUpperHead(leftHand.transform) && IsUpperHead(rightHand.transform))
        {
            Debug.LogWarning($"경고 UI 띄우기");
            ui.SetActive(true);
        }
        else
        {
            ui.SetActive(false);
        }
    }

    public bool IsUpperHead(Transform hand)
    {
        if(hand.localPosition.y > eye.transform.localPosition.y)
        {
            Debug.LogWarning($"{hand.name} 손이 머리 위로 올라감");
            return true;
        }
        return false;
    }
}
