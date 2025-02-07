using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerAction : MonoBehaviour
{
    public StepManager stepManager;  // StepManager 참조

    void Start()
    {
       
    }

    void Update()
    {
        // E 키를 누르면 플레이어 행동 완료
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("단계 : E키가 눌렷습니다");
            stepManager?.OnPlayerActionCompleted();
        }
    }
}