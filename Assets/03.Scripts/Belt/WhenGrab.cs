using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhenGrab : MonoBehaviour
{
    public void GrabObject()
    {
        SoundManager.Instance.PlaySFX("0.Grabbing");
    }
}
