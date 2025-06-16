using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhenGrabTongGori : MonoBehaviour
{
    public GameObject newGori;
    public void WhenGrab()
    {
        // 지금 오브젝트 비활성화하고 새로운 고리 동일한 위치에 활성화
        newGori.transform.position = transform.position;
        newGori.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
