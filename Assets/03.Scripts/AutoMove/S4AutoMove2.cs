using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class S4AutoMove2 : MonoBehaviour
{
    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;

    public StepManager stepManager;
    public S4AutoMove1 s4AutoMove1;
    public S3AutoMove s3AutoMove;

    public AudioClip sound1;
    private AudioSource audioSource;

    void Update()
    {
        bool S3Moved = s3AutoMove != null && s3AutoMove.GetS3Moved();
        bool S41Moved = s4AutoMove1 != null && s4AutoMove1.GetS41Moved();

        if (S3Moved && S41Moved && transform.localEulerAngles.y >= 85.0f)
        {
            
            DeactivateHandMoveObjects();
            Debug.Log("S4 모든 행동 완료");
            stepManager.OnPlayerActionCompleted();
            // LockWindow();
        }
        
    }

    public void DeactivateHandMoveObjects()
    {
        HandMoveObject.SetActive(false);
        HandMoveObject_mirror.SetActive(false);
    }

    /* void LockWindow()
    {
        if (isLocked) { return; }
        isLocked = true;
        if (audioSource != null && sound1 != null)
        {
            audioSource.clip = sound1;
            audioSource.Play();
        }
        GaugeImage.SetActive(false); // 게이지 비활성화 삭제
    }*/
}

