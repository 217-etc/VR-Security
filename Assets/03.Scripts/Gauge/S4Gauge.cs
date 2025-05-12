using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S4Gauge : MonoBehaviour
{
    [Header("Gauge Settings")]
    public Image gaugeImage;
    public Image gaugeBG;

    private float gaugeValue = 0f;
    private float previousGaugeValue = 0f;
    private bool isGaugeFull = false;

    void Start()
    {
        if (gaugeImage != null)
            gaugeImage.fillAmount = 0f;

        if (gaugeBG != null)
            gaugeBG.gameObject.SetActive(true);
    }

    void Update()
    {
        FillGauge();
    }

    void FillGauge()
    {
        // S4 객체 자신의 로컬 y축 회전값 가져오기
        float rotationY = transform.localEulerAngles.y;

        // 360도를 넘는 경우 보정 (예: 370도 → 10도)
        // rotationY = NormalizeAngle(rotationY);

        // 0도 → 0%, 90도 → 100% 정규화
        float newGaugeValue = Mathf.InverseLerp(0f, 90f, rotationY);

        // 게이지 값 업데이트 (감소 방지)
        newGaugeValue = Mathf.Clamp(newGaugeValue, previousGaugeValue, 1f);
        gaugeValue = newGaugeValue;

        // 게이지 UI 업데이트
        if (gaugeImage != null)
        {
            gaugeImage.fillAmount = gaugeValue;

            if (!gaugeImage.gameObject.activeSelf)
                gaugeImage.gameObject.SetActive(true);
        }

        if (gaugeValue >= 1f && !isGaugeFull)
        {
            isGaugeFull = true;
        }
        else if (gaugeValue < 1f)
        {
            isGaugeFull = false;
        }

        previousGaugeValue = gaugeValue;
    }

    // 0~360도 범위를 -180~180도로 보정
    float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;
        return angle;
    }
}
