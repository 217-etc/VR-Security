using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollisionManager : MonoBehaviour
{
    public HandCollisionHandler leftHand;
    public HandCollisionHandler rightHand;
    public GameObject eye;
    public float smoothTime = 0.3f; // 감속 시간
    private Vector3 targetPosition; // 목표 위치
    private Vector3 velocity = Vector3.zero; // 이동 속도(참조용)
    private bool isMoving = false; // 이동 상태 확인

    void Update()
    {
 
        if (leftHand.IsTouchingWall && rightHand.IsTouchingWall && eye.transform.position.y > 1f)
        {
            leftHand.StartMove();
            rightHand.StartMove();
            StartMoving();
        }

        // 이동 처리
        if (isMoving)
        {
            eye.transform.position = Vector3.SmoothDamp(
                eye.transform.position, // 현재 위치
                targetPosition,     // 목표 위치
                ref velocity,       // 속도 (참조로 전달)
                smoothTime          // 감속 시간
            );

            // 목표 위치에 거의 도달하면 이동 종료
            if (Vector3.Distance(eye.transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
                leftHand.EndMove();
                rightHand.EndMove();
                velocity = Vector3.zero; // 속도 초기화
            }
        }
        else
        {
            Debug.Log("이미 바닥에 도착함");
        }
    }

    void StartMoving()
    {
        Debug.Log("한 층 내려가기 시작");
        targetPosition = eye.transform.position + new Vector3(0, -30, 0); // 목표 위치 설정
        isMoving = true; // 이동 활성화
    }


}
