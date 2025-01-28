using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public OVRHand ovrHand; // OVRHand ������Ʈ ����

    private bool detectStart = false; // �÷��̾��� �� ���� ����
    private bool isFist = false; // ���� �ָ� ����
    private bool isFirstPalm = false; // ó�� �չٴ��� ���ƴ���
    private const float pinchThreshold = 0.4f; // ��Ī ���� �Ӱ谪
    private const float palmThreshold = 0.05f;
    private int fistCnt = 0;

    void Start()
    {
        //3�� �ں��� �� ���� ����
        Invoke("DetectStart", 3f);
    }

    void DetectStart()
    {
        detectStart = true;
        Debug.LogWarning("���� ����");
    }

    void Update()
    {
        // �հ��� ���¸� ����
        bool isIndexPinching = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > pinchThreshold;
        bool isMiddlePinching = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle) > pinchThreshold;
        bool isRingPinching = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Ring) > pinchThreshold;
        bool isPinkyPinching = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky) > pinchThreshold;

        bool isThumbPalm = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb) < palmThreshold;
        bool isIndexPalm = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) < palmThreshold;
        bool isMiddlePalm = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle) < palmThreshold;
        bool isRingPalm = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Ring) < palmThreshold;
        bool isPinkyPalm = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky) < palmThreshold;

        if (detectStart)
        {
            if (isThumbPalm && isIndexPalm && isMiddlePalm && isRingPalm && isPinkyPalm)
            {
                if (!isFirstPalm)
                {
                    isFirstPalm = true;
                    Debug.LogWarning("ó�� �չٴ��� ���ƽ��ϴ�.");
                    OnHandOpen();
                    return;
                }
                else
                {
                    if (isFist)
                    {
                        Debug.LogWarning("�ٽ� �չٴ��� ���ƽ��ϴ�.");
                        isFist = false;
                        OnHandOpen();
                        return;
                    }
                }
            }

            // ��� �հ����� ���� ���� �̻� ��Ī ���¶�� �ָ����� ����
            if (isFirstPalm && isIndexPinching && isMiddlePinching && isRingPinching && isPinkyPinching)
            {
                if (!isFist) // ó�� �ָ� ���·� ����� ��
                {
                    isFist = true;
                    fistCnt++;
                    Debug.LogWarning($"���� �ָ��� ������ϴ�! �ָ� �� Ƚ�� : {fistCnt}");
                    OnFist(); // �ָ� ���� ����
                    return;
                }
            }
        }


        
        //else
        //{
        //    if (isFist) // ó�� ���� ���� ��
        //    {
        //        isFist = false;
        //        Debug.Log("���� �������ϴ�!");
        //        OnHandOpen(); // �� ��� ���� ����
        //    }
        //}
    }

    private void OnFist()
    {
        // �ָ� ����� �� ������ ����
        Debug.LogWarning("�ָ� ���� ���� ��...");
    }

    private void OnHandOpen()
    {
        // ���� ���� �� ������ ����
        Debug.LogWarning("�� ��ħ ���� ���� ��...");
    }
}
