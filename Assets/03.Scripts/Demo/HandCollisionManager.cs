using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    }

    void StartMoving()
    {
        Debug.Log("한 층 내려가기 시작");
        SoundManager.Instance.PlaySFX("Rope");
        targetPosition = eye.transform.position + new Vector3(0, -20, 0); // 목표 위치 설정
        isMoving = true; // 이동 활성화
    }
    void StartLanding()
    {
        SoundManager.Instance.PlaySFX("Rope");
        targetPosition = eye.transform.position + new Vector3(0, -10, 0); // 목표 위치 설정
        isMoving = true; // 이동 활성화
    }


}
