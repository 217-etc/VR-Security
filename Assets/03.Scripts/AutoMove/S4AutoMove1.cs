using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class S4AutoMove1 : MonoBehaviour
{
    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;

    public StepManager stepManager;
    public S3AutoMove s3AutoMove;

    private float moveDuration = 1.0f;
    private bool S41Moved = false;
    private bool S42Moved = false;
    private float prePosY = 0.0f;
    private bool actOnce = false;
    public Outline outline41;

    void Start()
    {
        ActivateHandMoveObjects();
        if (stepManager == null) Debug.LogError("StepManager가 할당되지 않았습니다!");
        if (s3AutoMove == null) Debug.LogError("S3AutoMove가 할당되지 않았습니다!");
    }

    void Update()
    {
        bool S3Moved = s3AutoMove != null && s3AutoMove.GetS3Moved();

        if (!S41Moved && transform.localEulerAngles.y >= 0.1f)
        {
            DeactivateHandMoveObjects();
            StartCoroutine(MoveToPosition(new Vector3(0f, transform.localPosition.y, transform.localPosition.z), false));
            prePosY = transform.localEulerAngles.y;
        }

        if (S3Moved && S41Moved && !actOnce)
        {
            ActivateHandMoveObjects();
            actOnce = true;
        }

        if (S3Moved && S41Moved && transform.localEulerAngles.y > prePosY + 1.0f)
        {
            StartCoroutine(MoveToPosition(new Vector3(1.5f, transform.localPosition.y, transform.localPosition.z), true));
            S42Moved = true;
        }
    }

    IEnumerator MoveToPosition(Vector3 targetPosition, bool enableGrabAfterMove)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.localPosition;
        if (!S42Moved) { DeactivateHandMoveObjects(); }

        while (elapsedTime < moveDuration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition;
        S41Moved = true;

        if (enableGrabAfterMove) { ActivateHandMoveObjects(); }
    }

    public void ActivateHandMoveObjects()
    {
        HandMoveObject.SetActive(true);
        HandMoveObject_mirror.SetActive(true);
    }

    public void DeactivateHandMoveObjects()
    {
        HandMoveObject.SetActive(false);
        HandMoveObject_mirror.SetActive(false);
    }

    public bool GetS41Moved() { return S41Moved; }
    public bool GetS42Moved() { return S42Moved; }
}

