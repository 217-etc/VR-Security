using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class S2AutoMove1 : MonoBehaviour
{
    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;
    public GameObject GaugeImage;
    public StepManager stepManager;

    private bool hasMoved = false;
    private bool halfMoved = false;
    private float targetY1 = -2.0f;
    private float moveDuration = 1.0f;

    public bool HalfMoved => halfMoved; // 외부에서 읽기 전용 접근 허용
    public bool HasMoved => hasMoved;

    void Start()
    {
        if (stepManager == null)
        { Debug.LogError("StepManager가 할당되지 않았습니다! Unity 인스펙터에서 할당하세요."); }
    }

    void Update()
    {
        if (transform.localEulerAngles.x <= 359.0f && transform.localPosition.y <= -3f && !hasMoved)
        {
            GaugeImage.SetActive(true);
            HandMoveObject.SetActive(false);
            HandMoveObject_mirror.SetActive(false);
            StartCoroutine(MoveS2Smoothly1());
            hasMoved = true;
        }

        if (transform.localEulerAngles.x >= 269.0f && transform.localEulerAngles.x <= 271.0f)
        {
            halfMoved = true;
        }
    }

    IEnumerator MoveS2Smoothly1()
    {
        float elapsedTime = 0f;
        Vector3 start = transform.localPosition;
        Vector3 target = new Vector3(start.x, targetY1, start.z);

        while (elapsedTime < moveDuration)
        {
            transform.localPosition = Vector3.Lerp(start, target, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = target;
        HandMoveObject.SetActive(true);
        HandMoveObject_mirror.SetActive(true);
    }
}
