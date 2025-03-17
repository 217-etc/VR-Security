using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TieBeltController : MonoBehaviour
{
    [SerializeField] private bool IsGrabBelt = false;
    [SerializeField] private bool IsGrabGori = false;

    private void Awake()
    {
        // grab 여부 초기화
        IsGrabBelt = false;
        IsGrabGori = false;
    }

    private void Update()
    {
        if (IsGrabBelt)
        {
            Debug.Log("[Belt] : Connect Belt를 잡고 있음");
        }
    }

    // Connect Belt를 잡았을 때
    public void GrabConnectBelt()
    {
        IsGrabBelt = true;
        Debug.Log("[Belt] : Connect Belt를 잡음");
    }
    // Connect Belt를 놓았을 때
    public void ReleaseConnectBelt()
    {
        IsGrabBelt = false;
        Debug.Log("[Belt] : Connect Belt를 놓음");
    }
    // Gori를 잡았을 때
    public void GrabGori()
    {
        IsGrabGori = true;
        Debug.Log("[Belt] : Gori를 잡음");
    }
    // Gori를 놓았을 때
    public void ReleaseGori()
    {
        IsGrabGori = false;
        Debug.Log("[Belt] : Gori를 놓음");
    }
}
