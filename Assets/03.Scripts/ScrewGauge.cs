using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrewGauge : MonoBehaviour
{
    [Header("Gauge Settings")]
    public Image gaugeImage;
    public Image gaugeBG;
    public Transform gaugeParent;

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

    public void UpdateScrewGauge(float value)
    {
        // 게이지 값 업데이트 (0 ~ 1 범위로 제한)
        float newGaugeValue = Mathf.Clamp01(value);

        // 이전 값보다 작아지지 않도록 설정 (감소 방지)
        if (newGaugeValue < previousGaugeValue)
        {
            newGaugeValue = previousGaugeValue;  // 이전 값 유지
        }

        gaugeValue = newGaugeValue;  // 최종 게이지 값 갱신

        // 게이지 UI 업데이트
        if (gaugeImage != null)
        {
            gaugeImage.fillAmount = gaugeValue;

            // 게이지가 사라지는 현상 방지
            if (!gaugeImage.gameObject.activeSelf)
                gaugeImage.gameObject.SetActive(true);
        }
        /*
        // 게이지 배경 위치 및 방향 설정
        if (gaugeParent != null)
        {
            Vector3 gaugePosition = gaugeParent.position + new Vector3(0, 0.3f, 0);  // 위치 조정
            gaugeImage.transform.position = gaugePosition;
            gaugeImage.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

            gaugeBG.transform.position = gaugePosition;
            gaugeBG.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }*/

        // 게이지가 가득 찬 경우
        if (gaugeValue >= 1f && !isGaugeFull)
        {
            Debug.Log("스크류 게이지가 가득 찼습니다!");
            isGaugeFull = true;  // 중복 출력 방지
        }
        else if (gaugeValue < 1f)
        {
            isGaugeFull = false; // 게이지가 다시 내려가면 초기화
        }

        // 이전 프레임의 게이지 값 저장
        previousGaugeValue = gaugeValue;
    }
}
