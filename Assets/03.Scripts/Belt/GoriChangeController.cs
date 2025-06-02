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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SupporterHole"))
        {
            prevGori1.SetActive(false);
            prevGori2.SetActive(false);
            prevGori3.SetActive(false);
            prevGori4.SetActive(false);
            newGori.SetActive(true);
        }
    }
}
