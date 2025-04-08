using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S2Gauge : MonoBehaviour
{
    [Header("Gauge Settings")]
    public Image gaugeImage;
    public Image gaugeBG;

    private float gaugeValue = 0f;          // 현재 게이지 값 (0~1)
    private float previousGaugeValue = 0f;   // 이전 프레임의 게이지 값
    private bool isGaugeFull = false;        // 게이지가 가득 찼는지 확인

    void Start()
    {
        if (gaugeImage != null)
            gaugeImage.fillAmount = 0f;     // 시작 시 게이지를 0으로 설정

        if (gaugeBG != null)
            gaugeBG.gameObject.SetActive(true);  // 배경은 항상 활성화
    }

    void Update()
    {
        FillGauge();
    }

    void FillGauge()
    {
        // S2의 글로벌 y축 회전값 받아오기
        float rotationY = transform.eulerAngles.y;

        // 270~360도를 -90~0도로 변환
        if (rotationY >= 270f && rotationY <= 360f)
        {
            rotationY -= 360f;
        }

        // 270도 → 0, 90도 → 1로 정규화
        float newGaugeValue = Mathf.InverseLerp(270f, 90f, rotationY);

        // 게이지 값 업데이트 (감소 방지)
        newGaugeValue = Mathf.Clamp(newGaugeValue, previousGaugeValue, 1f);
        gaugeValue = newGaugeValue;

        // 게이지 UI 업데이트
        if (gaugeImage != null)
        {
            gaugeImage.fillAmount = gaugeValue;

            // 게이지가 사라지는 현상 방지
            if (!gaugeImage.gameObject.activeSelf)
                gaugeImage.gameObject.SetActive(true);
        }

        // 게이지가 가득 찬 경우
        if (gaugeValue >= 1f && !isGaugeFull)
        {
            isGaugeFull = true;  // 중복 실행 방지
        }
        else if (gaugeValue < 1f)
        {
            isGaugeFull = false; // 게이지가 다시 내려가면 초기화
        }

        // 이전 프레임의 게이지 값 저장
        previousGaugeValue = gaugeValue;
    }
}
