using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S3Gauge : MonoBehaviour
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
        float rotationX = transform.localEulerAngles.x;

        // 90도 → 0%, 0도 → 100% 정규화
        float newGaugeValue = Mathf.InverseLerp(90f, 0f, rotationX);

        // 게이지 값 업데이트 (감소 방지)
        newGaugeValue = Mathf.Clamp(newGaugeValue, previousGaugeValue, 1f);
        gaugeValue = newGaugeValue;

        // UI에 반영
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
}
