using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSound : MonoBehaviour
{
    public GameObject obj;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("충돌한 오브젝트는 " + other.gameObject.name + "--------------------------------------------------");
        BoxCollider objCollider = obj.GetComponent<BoxCollider>();
        if (other.gameObject == objCollider)
        {
            Debug.Log("고리 충돌 감지됨-----------------------------------------------------");
            SoundManager.Instance.PlaySFX("Link"); // 소리 재생 코드
        }
    }
}
