using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GhostHandController : MonoBehaviour
{
    [SerializeField] private Transform _startTransform;
    [SerializeField] private Transform _endTransform;
    public void ShowGhostHand()
    {
        gameObject.SetActive(true);
    }
    public void EndGhostHand()
    {
        gameObject.SetActive(false);
    }
}
