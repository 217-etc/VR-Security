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
        animator.SetTrigger("Disappear");
        toolGrabManager.GrabTool(toolName);
    }
}
