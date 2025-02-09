using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;


public class WindowAutoMove : MonoBehaviour
{
    private bool hasMoved = false; // â���� �̹� �̵��ߴ��� üũ
    private bool isLocked = false; // â���� 4.423�� �����ߴ��� üũ
    private float targetZ = 4.423f; // ��ǥ Z ��ġ
    private float moveDuration = 1.0f; // �̵��ϴ� �� �ɸ��� �ð�

    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;

    public StepManager stepManager;

    void Start()
    {
        //stepManager = FindObjectOfType<StepManager>();
        if (stepManager == null)
        {
            Debug.LogError("StepManager�� �Ҵ���� �ʾҽ��ϴ�! Unity �ν����Ϳ��� �Ҵ��ϼ���.");
        }
    }

    void Update()
    {
        if (isLocked) return; // â���� ������ ���¸� �� �̻� Update ���� �� ��

        // ����ڰ� â���� ���ٰ� ���� ������ Z �� Ȯ�� �� �̵� ����
        if (transform.position.z >= 4.19f && !hasMoved)
        {
            StartCoroutine(MoveWindowSmoothly());
            hasMoved = true; // �� ���� ����ǵ��� ����
        }
    }

    IEnumerator MoveWindowSmoothly()
    {

        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(startPosition.x, startPosition.y, targetZ);

        while (elapsedTime < moveDuration)
        {
            if (isLocked) yield break; // â���� �����Ǿ����� ��� �ڷ�ƾ ����
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // �� ������ ��ٸ�
        }

        transform.position = targetPosition; // �̵��� ������ ���� ��ġ ����
        LockWindow(); // â�� ����

        //�̵��� ������ â���� ����� �ƿ� ����
        Destroy(HandMoveObject);
        Destroy(HandMoveObject_mirror);
        transform.position = targetPosition;

        //�÷��̾� �ൿ �Ϸ�
        if (stepManager != null)
        {
            stepManager.OnPlayerActionCompleted();
        }
        else
        {
            Debug.LogError("StepManager�� ã�� �� �����ϴ�.");
        }
    }
    void LockWindow()
    {
        isLocked = true;
    }
}
