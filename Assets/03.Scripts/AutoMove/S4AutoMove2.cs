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

    private bool S4Done = false;

    void Update()
    {
        bool S3Moved = s3AutoMove != null && s3AutoMove.GetS3Moved();
        bool S41Moved = s4AutoMove1 != null && s4AutoMove1.GetS41Moved();

        if (S3Moved && S41Moved && transform.localEulerAngles.y >= 85.0f)
        {
            if (S4Done) { return; } 
            DeactivateHandMoveObjects();
            Debug.Log("S4 모든 행동 완료");
            S4Done = true;
            // Debug.Log("현재 S42Moved 값: " + S4Done);
            SoundManager.Instance.PlaySFX("Supporter");
            stepManager.OnPlayerActionCompleted();            
        }
    }

    public void DeactivateHandMoveObjects()
    {
        HandMoveObject.SetActive(false);
        HandMoveObject_mirror.SetActive(false);
    }
}

