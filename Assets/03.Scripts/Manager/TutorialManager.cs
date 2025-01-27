using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public OVRHand ovrHand; // OVRHand 컴포넌트 참조

    private bool isFist = false; // 현재 주먹 상태
    private const float pinchThreshold = 0.5f; // 핀칭 강도 임계값

    void Update()
    {
        // 손가락 상태를 감지
        bool isIndexPinching = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > pinchThreshold;
        bool isMiddlePinching = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle) > pinchThreshold;
        bool isRingPinching = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Ring) > pinchThreshold;
        bool isPinkyPinching = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky) > pinchThreshold;

        // 모든 손가락이 일정 강도 이상 핀칭 상태라면 주먹으로 간주
        if (isIndexPinching && isMiddlePinching && isRingPinching && isPinkyPinching)
        {
            if (!isFist) // 처음 주먹 상태로 변경될 때
            {
                isFist = true;
                Debug.Log("손이 주먹을 쥐었습니다!");
                OnFist(); // 주먹 동작 실행
            }
        }
        else
        {
            if (isFist) // 처음 손이 펴질 때
            {
                isFist = false;
                Debug.Log("손이 펴졌습니다!");
                OnHandOpen(); // 손 펴기 동작 실행
            }
        }
    }

    private void OnFist()
    {
        // 주먹 쥐었을 때 실행할 동작
        Debug.Log("주먹 동작 실행 중...");
    }

    private void OnHandOpen()
    {
        // 손을 폈을 때 실행할 동작
        Debug.Log("손 펼침 동작 실행 중...");
    }
}
