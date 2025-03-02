using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrewGauge : MonoBehaviour
{
    public Image gaugeImage;
    public Image gaugeBG;
    public Transform gaugeParent;

    private float gaugeValue = 0f;

    public void UpdateScrewGauge(float value)
    {
        gaugeValue = Mathf.Clamp01(value); // 0~1 사이 값으로 제한
        gaugeImage.fillAmount = gaugeValue;

        if (gaugeParent != null)
        {
            gaugeImage.transform.position = gaugeParent.position + new Vector3(0, 30, 0);
            gaugeImage.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
            gaugeBG.transform.position = gaugeParent.position + new Vector3(0, 30, 0);
            gaugeBG.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }

        if (gaugeValue == 1f)
        {
            Debug.Log("스크류 게이지가 가득 찼습니다!");
        }
    }
}
