using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
 
        if (leftHand.IsTouchingWall && rightHand.IsTouchingWall && eye.transform.position.y > 15f)
        {
            leftHand.StartMove();
            rightHand.StartMove();
            StartMoving();
        }
        if (leftHand.IsTouchingWall && rightHand.IsTouchingWall && eye.transform.position.y < 15f && eye.transform.position.y > 5f)
        {
            leftHand.StartMove();
            rightHand.StartMove();
            StartLanding();
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
    }

    void StartMoving()
    {
        Debug.Log("�� �� �������� ����");
        SoundManager.Instance.PlaySFX("Rope");
        targetPosition = eye.transform.position + new Vector3(0, -20, 0); // ��ǥ ��ġ ����
        isMoving = true; // �̵� Ȱ��ȭ
    }
    void StartLanding()
    {
        SoundManager.Instance.PlaySFX("Rope");
        targetPosition = eye.transform.position + new Vector3(0, -10, 0); // ��ǥ ��ġ ����
        isMoving = true; // �̵� Ȱ��ȭ
    }


}
