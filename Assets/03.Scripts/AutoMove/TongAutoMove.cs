using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class TongAutoMove : MonoBehaviour
{
    private bool hasRotated = false; // �̹� �̵��ߴ��� üũ
    private bool isLocked = false; // ��ǥ ������ �����ߴ��� üũ
    private float targetRotation = -90f; // ��ǥ ȸ������
    private float moveDuration = 1.0f; // �̵��ϴ� �� �ɸ��� �ð�
    private float rotationThreshold = -60f; // �ڵ� ȸ�� Ʈ���� ����

    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;

    public StepManager stepManager;

    void Start()
    {
        if (stepManager == null)
        {
            Debug.LogError("StepManager�� �Ҵ���� �ʾҽ��ϴ�! Unity �ν����Ϳ��� �Ҵ��ϼ���.");
        }
    }

    void Update()
    {
        if (isLocked) return; // �̹� ��ǥ�� �����ߴٸ� ���� X

        float angleX = transform.localEulerAngles.x;
        if (angleX > 180f) angleX -= 360f; // 360���� �Ѿ�� -������ ��ȯ

        // ����ڰ� ���� ���ٰ� ���� ������ ��, Ư�� ���� �������� Ȯ�� �� �ڵ� ����
        if (angleX <= rotationThreshold && !hasRotated)
        {
            StartCoroutine(RotateBackSmoothly());
            hasRotated = true; // �� ���� ����ǵ��� ����
        }
    }

    IEnumerator RotateBackSmoothly()
    {
        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotationQ = Quaternion.Euler(targetRotation, transform.eulerAngles.y, transform.eulerAngles.z);

        while (elapsedTime < moveDuration)
        {
            if (isLocked) yield break; // �̹� �����Ǿ��ٸ� ��� ����
            transform.rotation = Quaternion.Lerp(startRotation, targetRotationQ, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotationQ; // ���� ����
        LockTong(); // �� ����

        // �̵��� ���� �� ��� ��� ����
        Destroy(HandMoveObject);
        Destroy(HandMoveObject_mirror);

        // �÷��̾� �ൿ �Ϸ�
        if (stepManager != null)
        {
            stepManager.OnPlayerActionCompleted();
        }
        else
        {
            Debug.LogError("StepManager�� ã�� �� �����ϴ�.");
        }
    }

    void LockTong()
    {
        isLocked = true;
    }
}
