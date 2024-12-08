using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollisionManager : MonoBehaviour
{
    public HandCollisionHandler leftHand;
    public HandCollisionHandler rightHand;
    public GameObject eye;
    public float smoothTime = 0.3f; // ���� �ð�
    private Vector3 targetPosition; // ��ǥ ��ġ
    private Vector3 velocity = Vector3.zero; // �̵� �ӵ�(������)
    private bool isMoving = false; // �̵� ���� Ȯ��

    void Update()
    {
 
        if (leftHand.IsTouchingWall && rightHand.IsTouchingWall && eye.transform.position.y > 1f)
        {
            leftHand.StartMove();
            rightHand.StartMove();
            StartMoving();
        }

        // �̵� ó��
        if (isMoving)
        {
            eye.transform.position = Vector3.SmoothDamp(
                eye.transform.position, // ���� ��ġ
                targetPosition,     // ��ǥ ��ġ
                ref velocity,       // �ӵ� (������ ����)
                smoothTime          // ���� �ð�
            );

            // ��ǥ ��ġ�� ���� �����ϸ� �̵� ����
            if (Vector3.Distance(eye.transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
                leftHand.EndMove();
                rightHand.EndMove();
                velocity = Vector3.zero; // �ӵ� �ʱ�ȭ
            }
        }
        else
        {
            Debug.Log("�̹� �ٴڿ� ������");
        }
    }

    void StartMoving()
    {
        Debug.Log("�� �� �������� ����");
        targetPosition = eye.transform.position + new Vector3(0, -30, 0); // ��ǥ ��ġ ����
        isMoving = true; // �̵� Ȱ��ȭ
    }


}
