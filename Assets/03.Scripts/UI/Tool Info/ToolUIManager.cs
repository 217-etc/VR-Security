using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolUIManager : MonoBehaviour
{
    public Animator animator;
    public void DisappearUI()
    {
        animator.SetTrigger("Disappear");
    }
}
