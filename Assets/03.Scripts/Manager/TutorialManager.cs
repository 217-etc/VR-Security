using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public OVRHand ovrHand; // OVRHand ������Ʈ ����

    private bool isFist = false; // ���� �ָ� ����
    private const float pinchThreshold = 0.5f; // ��Ī ���� �Ӱ谪

    void Update()
    {
        // �հ��� ���¸� ����
        bool isIndexPinching = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > pinchThreshold;
        bool isMiddlePinching = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle) > pinchThreshold;
        bool isRingPinching = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Ring) > pinchThreshold;
        bool isPinkyPinching = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky) > pinchThreshold;

        // ��� �հ����� ���� ���� �̻� ��Ī ���¶�� �ָ����� ����
        if (isIndexPinching && isMiddlePinching && isRingPinching && isPinkyPinching)
        {
            if (!isFist) // ó�� �ָ� ���·� ����� ��
            {
                isFist = true;
                Debug.Log("���� �ָ��� ������ϴ�!");
                OnFist(); // �ָ� ���� ����
            }
        }
        else
        {
            if (isFist) // ó�� ���� ���� ��
            {
                isFist = false;
                Debug.Log("���� �������ϴ�!");
                OnHandOpen(); // �� ��� ���� ����
            }
        }
    }

    private void OnFist()
    {
        // �ָ� ����� �� ������ ����
        Debug.Log("�ָ� ���� ���� ��...");
    }

    private void OnHandOpen()
    {
        // ���� ���� �� ������ ����
        Debug.Log("�� ��ħ ���� ���� ��...");
    }
}
