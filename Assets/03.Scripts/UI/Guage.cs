using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guage : MonoBehaviour
{
    [Header("Lever Settings")]
    public Transform lever; // ���� ������Ʈ
    public float minRotation = -70f; // ������ �ּ� Z�� ����
    public float maxRotation = 70f;  // ������ �ִ� Z�� ����

    [Header("Gauge Settings")]
    public Image gaugeImage; // UI ������ �̹���
    public Image gaugeBG;
    public Transform gaugeParent; // �������� ����ٴ� 3D ������Ʈ (��: Cube)

    private float previousAngle; // ���� �������� ���� ȸ����
    private bool isGaugeFull = false; // �������� 100% á���� Ȯ���ϴ� ����


    // ������ �� ��ȯ (0~1 ����)
    public float GaugeValue { get; private set; }

    void Start()
    {
        // �ʱ� ���� ȸ���� ����
        previousAngle = GetLeverRotation();
    }

    void Update()
    {
        // ���� ������ Z�� ȸ���� ��������
        float currentAngle = GetLeverRotation();

        // ���� �����Ӻ��� ���� ������ �̵��Ϸ��� �ϸ� ����
        if (currentAngle < previousAngle)
        {
            currentAngle = previousAngle; // ���� �� ���� (��, ���Ҹ� ����)
            lever.localEulerAngles = new Vector3(lever.localEulerAngles.x, lever.localEulerAngles.y, previousAngle);
        }

        // ȸ������ ���� ���� �ȿ� ����
        currentAngle = Mathf.Clamp(currentAngle, minRotation, maxRotation);

        // -70 ~ +70 ���� 0 ~ 1�� ��ȯ
        GaugeValue = Mathf.InverseLerp(minRotation, maxRotation, currentAngle);

        // Ư�� ������ ��Ȯ�� 0�� 1�� ��ȯ�ϵ��� ����
        if (currentAngle <= minRotation) GaugeValue = 0f;
        else if (currentAngle >= maxRotation) GaugeValue = 1f;

        // ������ UI ������Ʈ
        gaugeImage.fillAmount = GaugeValue;

        // ������ ��ġ�� Ư�� 3D ������Ʈ(gaugeParent) ���� ����
        if (gaugeParent != null)
        {
            gaugeImage.transform.position = gaugeParent.position + new Vector3(0, 30, 0); // Cube ���� ����
            gaugeImage.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward); // ī�޶� ������ �ٶ󺸰� ����
            gaugeBG.transform.position = gaugeParent.position + new Vector3(0, 30, 0);
            gaugeBG.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }

        if (GaugeValue == 1f && !isGaugeFull)
        {
            Debug.Log("Gauge is fully filled!");
            isGaugeFull = true; // �ߺ� ��� ����
        }
        else if (GaugeValue < 1f)
        {
            isGaugeFull = false; // �������� �ٽ� �������� �ʱ�ȭ
        }

        // ���� ȸ������ ���� ������ ����
        previousAngle = currentAngle;
    }

    // ������ ���� Z�� ȸ������ �������� �Լ�
    private float GetLeverRotation()
    {
        float angle = lever.localEulerAngles.z;
        if (angle > 180) angle -= 360; // -180 ~ 180 ������ ��ȯ
        return angle;
    }
}

