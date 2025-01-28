using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public OVRHand ovrHand; // OVRHand 컴포넌트 참조

    private bool detectStart = false; // 플레이어의 손 감지 시작
    private bool isFist = false; // 현재 주먹 상태
    private bool isFirstPalm = false; // 처음 손바닥을 펼쳤는지
    private const float pinchThreshold = 0.4f; // 핀칭 강도 임계값
    private const float palmThreshold = 0.05f;
    private int fistCnt = 0;

    void Start()
    {
        //3초 뒤부터 손 감지 시작
        Invoke("DetectStart", 3f);
    }

    void DetectStart()
    {
        detectStart = true;
        Debug.LogWarning("감지 시작");
    }

    void Update()
    {
        // 손가락 상태를 감지
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
                    Debug.LogWarning("처음 손바닥을 펼쳤습니다.");
                    OnHandOpen();
                    return;
                }
                else
                {
                    if (isFist)
                    {
                        Debug.LogWarning("다시 손바닥을 펼쳤습니다.");
                        isFist = false;
                        OnHandOpen();
                        return;
                    }
                }
            }

            // 모든 손가락이 일정 강도 이상 핀칭 상태라면 주먹으로 간주
            if (isFirstPalm && isIndexPinching && isMiddlePinching && isRingPinching && isPinkyPinching)
            {
                if (!isFist) // 처음 주먹 상태로 변경될 때
                {
                    isFist = true;
                    fistCnt++;
                    Debug.LogWarning($"손이 주먹을 쥐었습니다! 주먹 쥔 횟수 : {fistCnt}");
                    OnFist(); // 주먹 동작 실행
                    return;
                }
            }
        }


        
        //else
        //{
        //    if (isFist) // 처음 손이 펴질 때
        //    {
        //        isFist = false;
        //        Debug.Log("손이 펴졌습니다!");
        //        OnHandOpen(); // 손 펴기 동작 실행
        //    }
        //}
    }

    private void OnFist()
    {
        // 주먹 쥐었을 때 실행할 동작
        Debug.LogWarning("주먹 동작 실행 중...");
    }

    private void OnHandOpen()
    {
        // 손을 폈을 때 실행할 동작
        Debug.LogWarning("손 펼침 동작 실행 중...");
    }
}
