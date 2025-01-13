using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandEx : MonoBehaviour
{
    public Transform handTransform; 

    void Update()
    {
        transform.position = handTransform.position;
        transform.rotation = handTransform.rotation;
    }
}
