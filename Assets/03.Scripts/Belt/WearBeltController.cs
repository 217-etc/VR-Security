using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WearBeltController : MonoBehaviour
{
    public GameObject openBelt;
    public Transform bottomPoint;
    public GameObject rope;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Head")){
            rope.GetComponent<GogoGaga.OptimizedRopesAndCables.Rope>().endPoint = bottomPoint;
            openBelt.SetActive(true);
            Destroy(this.gameObject);
        } 
    }
}
