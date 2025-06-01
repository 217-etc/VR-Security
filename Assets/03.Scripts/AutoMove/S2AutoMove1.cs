using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class S2AutoMove1 : MonoBehaviour
{
    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;
    public StepManager stepManager;
    public SupCheck supCheck;
    public Outline outline21;

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
        outline21.enabled = true;
    }

    void Update()
    {
        //Debug.Log("섭첵 값: " + supCheck.SupChecked + " / S2의 x축 회전값: " + transform.localEulerAngles.x + " / y축 이동값: " +transform.localPosition.y + " / hasMoved: " + hasMoved);

        if (supCheck.SupChecked && transform.localEulerAngles.x <= 359.0f && transform.localPosition.y <= -3f && !hasMoved)
        {
            HandMoveObject.SetActive(false);
            HandMoveObject_mirror.SetActive(false);
            StartCoroutine(MoveS2Smoothly1());
            hasMoved = true;
        }

        if (transform.localEulerAngles.x >= 265.0f && transform.localEulerAngles.x <= 275.0f)
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
