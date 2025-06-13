using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoriChangeController : MonoBehaviour
{
    public GameObject prevGori1;
    public GameObject prevGori2;
    public GameObject prevGori3;
    public GameObject prevGori4;
    public GameObject newGori;
    public GameObject stepManager;
    public StepManager stepManager_before;
    public GameObject hand1;
    public GameObject hand2;
    public GameObject parent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SupporterHole"))
        {
            prevGori1.SetActive(false);
            prevGori2.SetActive(false);
            prevGori3.SetActive(false);
            prevGori4.SetActive(false);
            hand1.SetActive(false);
            hand2.SetActive(false);
            newGori.SetActive(true);
            //parent.transform.position = new Vector3(0.952000022f, -0.885999978f, -0.291999996f);
            parent.transform.parent = newGori.transform;
            stepManager_before.OnPlayerActionCompleted();
        }
    }
}
