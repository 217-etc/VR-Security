using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolUIManager : MonoBehaviour
{
    public string toolName;
    public ToolGrabManager toolGrabManager;
    public Animator animator;

    void Start()
    {
        toolGrabManager = FindObjectOfType<ToolGrabManager>();
    }
    public void DisappearUI()
    {
        if (toolGrabManager.IsGrabEnd(toolName)) return;
        animator.SetTrigger("Disappear");
        toolGrabManager.GrabTool(toolName);


        /*
        // Rigidbody 키네틱 해제
        Debug.LogWarning($"키네틱 값 변경 전 : {GetComponent<Rigidbody>().isKinematic}");
        GetComponent<Rigidbody>().isKinematic = false;
        Debug.LogWarning($"키네틱 값 변경 후 : {GetComponent<Rigidbody>().isKinematic}");
        GetComponent<Rigidbody>().useGravity = true;
        */
    }
}
