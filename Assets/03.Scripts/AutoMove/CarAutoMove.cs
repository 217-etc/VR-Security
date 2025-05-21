using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAutoMove : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        Debug.Log("Car의 y축 회전값: " + transform.eulerAngles.y);
    }
}
