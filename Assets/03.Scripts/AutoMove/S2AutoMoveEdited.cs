using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class S2AutoMoveEdited : MonoBehaviour
{
    private bool hasMoved = false; // S2가 한 번 이동했는지 체크
    private bool halfMoved = false;
    private bool hasDone = false; // S2가 두 번째 이동했는지 체크
    private bool isLocked = false;
    private bool S2Setting = false;
    private float targetY1 = -2.0f;
    private float targetY2 = -5.0f;
    private float moveDuration = 1.0f;
    public bool HasDone => hasDone;

    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;
    public StepManager stepManager;
    public SupCheck supCheck;
    public Outline outline2; // 아웃라인 하나로 퉁침
    public GameObject GaugeImage;
    public GameObject GuideHand211; // 섭첵 가이드손
    public GameObject GuideHand; // S2 가이드손
    public GameObject S2CKUI; // 섭첵 확인 UI
    public GameObject S2UI; // S2 UI

    void Update()
    {
        if (isLocked) return;

        if (supCheck.SupChecked && !S2Setting) // 섭첵 완료되면
        {
            StartCoroutine(DelayFunction4e());
            S2UI.SetActive(true); // S2 UI 켜기
            GuideHand211.SetActive(true); // 첫번째 가이드손 켜기
            S2Setting = true;
            StartCoroutine(WaitForInitialDialogue());
        }

        // 섭첵 완료되면 앞으로 이동. 이동 후 게이지, 가이드손 켜기
        // 나중에 섭첵 추가. 섭첵에서 섭첵, S2 가이드손 끄고 켬
        // 맨앞에 supCheck.SupChecked &&  추가하면 됨
        if (supCheck.SupChecked && transform.localEulerAngles.x <=359.0f && transform.localPosition.y <= -3f && !hasMoved)
        {
            HandMoveObject.SetActive(false);
            HandMoveObject_mirror.SetActive(false);
            StartCoroutine(MoveS2Smoothly1());
            hasMoved = true;
        }

        if (transform.localEulerAngles.x >= 260.0f && transform.localEulerAngles.x <= 280.0f)
        {
            halfMoved = true;
            // Debug.Log("S2 절반 넘어감");
        }

        // 회전 끝까지 이동
        if (transform.localEulerAngles.x >= 340.0f && hasMoved && halfMoved)
        {
            StartCoroutine(MoveS2RotateToTarget(359.5f));
        }

        // 다시 넣기
        //여기서 모든 아웃라인, 가이드손 끄기
        if (hasDone)
        {
            StartCoroutine(MoveS2Smoothly2());
        }
    }

    IEnumerator MoveS2Smoothly1()
    {
        float elapsedTime = 0f;
        Vector3 S2startPosition = transform.localPosition;
        Vector3 S2targetPosition = new Vector3(S2startPosition.x, targetY1, S2startPosition.z);

        while (elapsedTime < moveDuration)
        {
            if (isLocked) yield break;
            transform.localPosition = Vector3.Lerp(S2startPosition, S2targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = S2targetPosition;
        HandMoveObject.SetActive(true);
        HandMoveObject_mirror.SetActive(true);
        GaugeImage.SetActive(true); // 게이지 이미지 띄움
        GuideHand.SetActive(true); // S2 가이드손 켜기
        DialogueManager.Instance.StartDialogue("Dialogue_A004-02-2"); // 대사 출력
        StartCoroutine(WaitForInitialDialogue());
    }

    IEnumerator MoveS2Smoothly2()
    {
        Destroy(HandMoveObject);
        Destroy(HandMoveObject_mirror);

        float elapsedTime = 0f;
        Vector3 S2startPosition2 = transform.localPosition;
        Vector3 S2targetPosition2 = new Vector3(S2startPosition2.x, targetY2, S2startPosition2.z);

        while (elapsedTime < moveDuration)
        {
            if (isLocked) yield break;
            transform.localPosition = Vector3.Lerp(S2startPosition2, S2targetPosition2, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = S2targetPosition2; 
        LockWindow();
        SoundManager.Instance.PlaySFX("Supporter");
        SoundManager.Instance.PlaySFX("0.Suc_bell");
        GaugeImage.SetActive(false); // 게이지 끄기
        GuideHand.SetActive(false); // 가이드손 끄기
        outline2.enabled = false; // 아웃라인 끄기
        S2UI.SetActive(false); // UI 끄기
    }

    IEnumerator MoveS2RotateToTarget(float targetXRotation)
    {
        float elapsedTime = 0f;
        float moveDuration = 1.0f;
        float startXRotation = transform.localEulerAngles.x;

        while (elapsedTime < moveDuration)
        {
            if (isLocked) yield break;

            float newXRotation = Mathf.Lerp(startXRotation, targetXRotation, elapsedTime / moveDuration);
            transform.localEulerAngles = new Vector3(newXRotation, transform.localEulerAngles.y, transform.localEulerAngles.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localEulerAngles = new Vector3(targetXRotation, transform.localEulerAngles.y, transform.localEulerAngles.z);
        hasDone = true;
    }

    void LockWindow()
    {
        isLocked = true;
    }

    //대사 대기 함수
    private IEnumerator WaitForInitialDialogue()
    {
        HandMoveObject.SetActive(false);
        HandMoveObject_mirror.SetActive(false);
        while (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
        {
            yield return null;
        }
        HandMoveObject.SetActive(true);
        HandMoveObject_mirror.SetActive(true);
    }

    IEnumerator DelayFunction4e()
    {
        yield return new WaitForSeconds(7f);
        DialogueManager.Instance.StartDialogue("Dialogue_A004-02-1"); // 대사 출력
        WaitForInitialDialogue();
    }
}