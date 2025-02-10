using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelAutoMove : MonoBehaviour
{
    public StepManager stepManager;
    private bool actionCompleted = false; // �� ���� ����ǵ��� üũ
    private float thresholdY = -0.1f; // Ʈ���ŵǴ� Y ��ǥ ��

    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;

    void Start()
    {
        if (stepManager == null)
        {
            Debug.LogError("StepManager�� �Ҵ���� �ʾҽ��ϴ�! Unity �ν����Ϳ��� �Ҵ��ϼ���.");
        }
    }

    void Update()
    {
        if (!actionCompleted && transform.position.y <= thresholdY)
        {
            actionCompleted = true; // �ߺ� ���� ����
            Debug.Log("���� ��ǥ ��ġ�� �����߽��ϴ�! �÷��̾� �ൿ �Ϸ� ó��.");
            stepManager?.OnPlayerActionCompleted();

            // �̵��� ���� �� ��� ��� ����
            Destroy(HandMoveObject);
            Destroy(HandMoveObject_mirror);
        }
    }
}
