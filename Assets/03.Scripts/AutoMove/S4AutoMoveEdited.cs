using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Grab;

public class S4AutoMoveEdited : MonoBehaviour
{
    public GameObject HandMoveObject;
    public GameObject HandMoveObject_mirror;
    public StepManager stepManager;
    // public S2AutoMove2 s2AutoMove2;
    public S2AutoMoveEdited s2AutoMoveEdited; // 수정
    public GameObject GaugeImage;
    public GameObject GuideHand;
    public GameObject GuideHand40;
    public GameObject S4UI; // 수정: S4 UI 추가
    public GameObject StepUI; // 수정: 스텝 UI 조정용 추가
    public Outline outline4;

    private float moveDuration = 1.0f;
    private bool S41Moved = false;
    private bool S4Done = false;
    private bool S4setting = false;

    public GameObject newSupporter;

    void Start()
    {
        //ActivateHandMoveObjects(); // 처음엔 그랩 활성화
        if (stepManager == null)
        {
            Debug.LogError("StepManager가 할당되지 않았습니다! Unity 인스펙터에서 할당하세요.");
        }
    }

    void Update()
    {
        // Debug.Log("S4 y축 회전값: " + transform.localEulerAngles.y);

        if (s2AutoMoveEdited.HasDone && !S4setting) // 수정: 원래 s2AutoMove2 였음
        {
            StartCoroutine(DelayFunction4e());
            S4setting = true;
        }

        // S4 올리기
        if (!S41Moved && transform.localEulerAngles.y >= 0.1f)
        {
            GuideHand40.SetActive(false);
            DeactivateHandMoveObjects();
            StartCoroutine(MoveToPosition(new Vector3(0f, transform.localPosition.y, transform.localPosition.z)));
        }

        // S41Moved가 참이고 S4의 y축 회전값이 85도보다 크면 → S4 모든 행동 완료. 그랩 비활성화 후 스텝매니저 호출
        if (S41Moved && transform.localEulerAngles.y >= 85.0f && !S4Done)
        {
            // SoundManager.Instance.PlaySFX("Supporter");
            // SoundManager.Instance.PlaySFX("0.Suc_bell");
            DeactivateHandMoveObjects();
            SoundManager.Instance.PlaySFX("Supporter");
            SoundManager.Instance.PlaySFX("0.Suc_bell");
            outline4.enabled = false;
            GaugeImage.SetActive(false);
            GuideHand.SetActive(false);
            S4UI.SetActive(false); // 수정: UI 끄기
            StepUI.SetActive(true); // 수정: 스텝 UI 켜기
            S4Done = true;
            // stepManager.OnPlayerActionCompleted(); // 수정
            stepManager.gameObject.SetActive(true); // 수정: 스텝매니저 다시 켜기
            newSupporter.SetActive(true); // 새로운 지지대 켜기
            this.gameObject.SetActive(false);
        }
    }

    IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.localPosition;
        
        while (elapsedTime < moveDuration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPosition;
        S41Moved = true;
        ActivateHandMoveObjects();
        GuideHand.SetActive(true);
        GaugeImage.SetActive(true);
        DialogueManager.Instance.StartDialogue("Dialogue_A004-02-3-1"); // 대사 출력
        StartCoroutine(WaitForInitialDialogue());
    }

    IEnumerator DelayFunction4e()
    {
        yield return new WaitForSeconds(2f);
        ActivateHandMoveObjects();
        DialogueManager.Instance.StartDialogue("Dialogue_A004-02-3"); // 대사 출력
        outline4.enabled = true;
        GuideHand40.SetActive(true);
        S4UI.SetActive(true); // 수정: UI 켜기
        StartCoroutine(WaitForInitialDialogue());
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

    public bool GetS4Done() { return S4Done; }

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
}
