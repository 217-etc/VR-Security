using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guage : MonoBehaviour
{
    [Header("Lever Settings")]
    public Transform lever; // 레버 오브젝트
    public float minRotation = -70f; // 레버의 최소 Z축 각도
    public float maxRotation = 70f;  // 레버의 최대 Z축 각도

    [Header("Gauge Settings")]
    public Image gaugeImage; // UI 게이지 이미지
    public Image gaugeBG;
    public Transform gaugeParent; // 게이지가 따라다닐 3D 오브젝트 (예: Cube)

    private float previousAngle; // 이전 프레임의 레버 회전값
    private bool isGaugeFull = false; // 게이지가 100% 찼는지 확인하는 변수


    // 게이지 값 반환 (0~1 범위)
    public float GaugeValue { get; private set; }

    void Start()
    {
        // 초기 레버 회전값 저장
        previousAngle = GetLeverRotation();
    }

    void Update()
    {
        // 현재 레버의 Z축 회전값 가져오기
        float currentAngle = GetLeverRotation();

        // 이전 프레임보다 작은 각도로 이동하려고 하면 제한
        if (currentAngle < previousAngle)
        {
            currentAngle = previousAngle; // 이전 값 유지 (즉, 감소를 방지)
            lever.localEulerAngles = new Vector3(lever.localEulerAngles.x, lever.localEulerAngles.y, previousAngle);
        }

        // 회전값을 제한 범위 안에 유지
        currentAngle = Mathf.Clamp(currentAngle, minRotation, maxRotation);

        // -70 ~ +70 값을 0 ~ 1로 변환
        GaugeValue = Mathf.InverseLerp(minRotation, maxRotation, currentAngle);

        // 특정 값에서 정확히 0과 1을 반환하도록 보정
        if (currentAngle <= minRotation) GaugeValue = 0f;
        else if (currentAngle >= maxRotation) GaugeValue = 1f;

        // 게이지 UI 업데이트
        gaugeImage.fillAmount = GaugeValue;

        // 게이지 위치를 특정 3D 오브젝트(gaugeParent) 위에 고정
        if (gaugeParent != null)
        {
            gaugeImage.transform.position = gaugeParent.position + new Vector3(0, 30, 0); // Cube 위로 띄우기
            gaugeImage.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward); // 카메라 방향을 바라보게 설정
            gaugeBG.transform.position = gaugeParent.position + new Vector3(0, 30, 0);
            gaugeBG.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }

        if (GaugeValue == 1f && !isGaugeFull)
        {
            Debug.Log("Gauge is fully filled!");
            isGaugeFull = true; // 중복 출력 방지
        }
        else if (GaugeValue < 1f)
        {
            isGaugeFull = false; // 게이지가 다시 내려가면 초기화
        }

        // 현재 회전값을 이전 값으로 저장
        previousAngle = currentAngle;
    }

    // 레버의 현재 Z축 회전값을 가져오는 함수
    private float GetLeverRotation()
    {
        float angle = lever.localEulerAngles.z;
        if (angle > 180) angle -= 360; // -180 ~ 180 범위로 변환
        return angle;
    }
}

