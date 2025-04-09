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


        // Rigidbody 키네틱 해제
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
    }
}
