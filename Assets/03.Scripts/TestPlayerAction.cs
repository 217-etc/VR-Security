using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerAction : MonoBehaviour
{
    public StepManager stepManager;  // StepManager ����

    void Start()
    {
       
    }

    void Update()
    {
        // E Ű�� ������ �÷��̾� �ൿ �Ϸ�
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("�ܰ� : EŰ�� ���ǽ��ϴ�");
            stepManager?.OnPlayerActionCompleted();
        }
    }
}