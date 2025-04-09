using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabSound : MonoBehaviour
{
    void Start()
    {
        Debug.Log("오브젝트 잡힘");
        SoundManager.Instance.PlaySFX("0.Hand_Object");
    }
}
