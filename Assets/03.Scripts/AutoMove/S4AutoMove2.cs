using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class S4AutoMove2 : MonoBehaviour
{
    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;
    public GameObject GaugeImage;
    public GameObject GuideHand;

    public StepManager stepManager;
    public S4AutoMove1 s4AutoMove1;
    public S3AutoMove s3AutoMove;
    public Outline outline42;

    private bool S4Done = false;
    public bool S4setting = false;

    void Update()
    {
        bool S3Moved = s3AutoMove != null && s3AutoMove.GetS3Moved();
        bool S41Moved = s4AutoMove1 != null && s4AutoMove1.GetS41Moved();

        if (S3Moved && S41Moved && transform.localEulerAngles.y < 85.0f && !S4setting)
        {
            StartCoroutine(DelayFunction42());
            S4setting = true;
        }

        if (S3Moved && S41Moved && transform.localEulerAngles.y >= 85.0f)
        {
            if (S4Done) { return; } 
            DeactivateHandMoveObjects();
            Debug.Log("S4 모든 행동 완료");
            SoundManager.Instance.PlaySFX("Supporter");
            Destroy(GuideHand);
            GaugeImage.SetActive(false);
            outline42.enabled = false;
            S4Done = true;
            stepManager?.OnPlayerActionCompleted();
        }
    }

    IEnumerator DelayFunction42()
    {
        // 1초 대기
        yield return new WaitForSeconds(2f);
        GaugeImage.SetActive(true);
        outline42.enabled = true;
        GuideHand.SetActive(true);
    }

    public void DeactivateHandMoveObjects()
    {
        HandMoveObject.SetActive(false);
        HandMoveObject_mirror.SetActive(false);
    }
}

