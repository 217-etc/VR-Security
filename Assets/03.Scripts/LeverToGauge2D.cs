using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeverToGauge2D : MonoBehaviour
{

    [Header("Lever Settings")]
    public Transform lever; 
    public float minRotation = 70f; 
    public float maxRotation = -70f; 

    [Header("Gauge Settings")]
    public Image gaugeImage;

    void Update()
    {
        float leverZRotation = lever.localEulerAngles.z;
        if (leverZRotation > 180) leverZRotation -= 360;
        float normalizedValue = Mathf.InverseLerp(minRotation, maxRotation, leverZRotation);
        gaugeImage.fillAmount = normalizedValue;
    }
}
