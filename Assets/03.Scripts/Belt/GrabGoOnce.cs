using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabGoOnce : MonoBehaviour
{
    public bool isFirstGrab = false;
    public StepManager stepManager;

    public void WhenGrab()
    {
        if (!isFirstGrab)
        {
            isFirstGrab = true;
            stepManager.OnPlayerActionCompleted();
        }
    }
}
