using Oculus.Interaction.HandGrab;
using Oculus.Interaction.HandGrab.Visuals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGhostMaterial : MonoBehaviour
{
    [SerializeField] private Material ghostMaterial;
    [SerializeField] private HandGrabPose _handGrabPose;
    private HandGhostProvider _handGhostProvider;
    private HandGhost _hand;
    
    void Start()
    {
        _handGrabPose = GetComponent<HandGrabPose>();
        _handGhostProvider = _handGrabPose._handGhostProvider;

        _hand = _handGhostProvider.GetHand(_handGrabPose.HandPose.Handedness);

        SkinnedMeshRenderer skinnedMeshRenderer = _hand.GetComponentInChildren<SkinnedMeshRenderer>();

        if (skinnedMeshRenderer != null)
        {
            skinnedMeshRenderer.material = ghostMaterial;
            Debug.Log("성공적으로 Ghost Material 적용 완료");
        }
    }
}
