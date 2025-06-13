using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelController : MonoBehaviour
{
    public void ReleaseReel()
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Finish"))
        {
            FindAnyObjectByType<StepManager>().OnPlayerActionCompleted();
        }
    }
}
