using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 worldPos = transform.position;
        Vector3 localPos = transform.localPosition;

        Debug.Log("월드 위치 (World): " +
            $"X: {worldPos.x.ToString("F5")}, Y: {worldPos.y.ToString("F5")}, Z: {worldPos.z.ToString("F5")}");

        Debug.Log("로컬 위치 (Local): " +
            $"X: {localPos.x.ToString("F5")}, Y: {localPos.y.ToString("F5")}, Z: {localPos.z.ToString("F5")}");
    }
}
