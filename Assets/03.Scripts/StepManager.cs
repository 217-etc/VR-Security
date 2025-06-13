using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StepManager : MonoBehaviour
{
    public StepUIManager stepUI;                    // UI 매니저 연결
    public List<Step> steps = new List<Step>();     // 모든 단계 정보 저장
    private int currentStepIndex = -1;              // 현재 단계 인덱스 (-1부터 시작)
    private bool isPlayerActionCompleted = false;   // 플레이어 행동 완료 여부
    private bool isStepInProgress = false;          // 중복 실행 방지용 플래그

    // Feedback 부분
    private Coroutine feedbackCoroutine;
    private bool isGrabbing = false;
    private float timeSinceRelease = 0f;
    private bool isFeedbackPlaying = false;
    private bool feedbackCooldown = false;

    [SerializeField] GameObject _noticeUI;

    void Start()
    {
        DialogueManager.Instance.noticeUI = _noticeUI;
        DialogueManager.Instance._animator = _noticeUI.GetComponent<Animator>();

        NextStep(); // 첫 번째 단계 시작
    }

    void Update()
    {

    }

    void NextStep()
    {
        if (isStepInProgress) return;           // 중복 실행 방지
        isStepInProgress = true;

        currentStepIndex++;

        if (currentStepIndex >= steps.Count)
        {
            Debug.Log("모든 단계를 완료했습니다.");
            return;
        }

        isPlayerActionCompleted = false;

        StartStep(steps[currentStepIndex]);     // 새로운 단계 시작
        isStepInProgress = false;
    }

    void StartStep(Step step)
    {
        Debug.Log($"현재 단계: {step.stepName}");

        isPlayerActionCompleted = false;        // 새로운 단계에서 플레이어 행동 초기화

        if (stepUI == null)
        {
            stepUI = FindObjectOfType<StepUIManager>();
        }
        if (stepUI != null)
        {
            stepUI.UpdateStepText(step.stepName);
        }

        foreach (GameObject obj in step.target)
        {
            // 1. 아웃라인 켜기
            Outline outline = obj?.GetComponentInChildren<Outline>();
            if (outline != null) outline.enabled = true;

            // 2. HandGrabInteractable / GuideHand 활성화
            Transform parentTransform = obj.transform;
            for (int i = 0; i < parentTransform.childCount; i++)
            {
                Transform child = parentTransform.GetChild(i);

                if (child.name.Contains("HandGrabInteractable") || child.name.Contains("HandGrabInteractable_Mirror"))
                {
                    child.gameObject.SetActive(true);
                }

                if (child.name.Contains("GuideHand"))
                {
                    child.gameObject.SetActive(true);
                }

                /*if (child.name.Contains("Outline"))
                {
                    child.GetComponent<Outline>().enabled = true;
                }*/
            }
        }
        // 3. UI & 음성 활성화
        if (!String.IsNullOrWhiteSpace(step.dialogueKey))
        {
            DialogueManager.Instance.StartDialogue(step.dialogueKey);

            // 0. 자동완료 할지, 특정 키 값 입력
            if (step.dialogueKey == "Dialogue_A003-2")
            {
                StartCoroutine(AutoCompleteAfterDelay(3f)); // 5초 뒤 자동 완료
            }

            // 0. Feedback 타이머 시작
            if (feedbackCoroutine != null) StopCoroutine(feedbackCoroutine);
            feedbackCoroutine = StartCoroutine(FeedbackLoop());
        }

        // 4. 게이지 UI 활성화
        if (step.gaugeUI != null)
        {
            step.gaugeUI.SetActive(true);
        }

        // 대사 진행 대기
        StartCoroutine(WaitForInitialDialogue(step));
    }

    void EndStep(Step step)
    {
        Debug.Log($"단계 종료: {step.stepName}");

        foreach (GameObject obj in step.target)
        {
            // 6. 아웃라인 끄기
            Outline outline = obj?.GetComponentInChildren<Outline>();
            if (outline != null) outline.enabled = false;

            // 7. HandGrabInteractable 비활성화
            Transform parentTransform = obj.transform;
            for (int i = 0; i < parentTransform.childCount; i++)
            {
                Transform child = parentTransform.GetChild(i);

                if (child.name.Contains("HandGrabInteractable") || child.name.Contains("HandGrabInteractable_Mirror"))
                {
                    child.gameObject.SetActive(false);
                }

                if (child.name.Contains("GuideHand"))
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        if (DialogueManager.Instance.dialougeDictionary.ContainsKey(step.dialogueKey + "_act"))
        {
            DialogueManager.Instance.StartDialogue(step.dialogueKey + "_act");
        }

        // 게이지 UI 비활성화는 WaitForDialogueThenProceed에서
    }

    public void CompleteCurrentStep()
    {
        if (isPlayerActionCompleted) return;    // 중복 실행 방지
        isPlayerActionCompleted = true;

        StartCoroutine(WaitForDialogueThenProceed());
    }

    // 단계 시작부분 대사 대기
    private IEnumerator WaitForInitialDialogue(Step step)
    {
        while (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
        {
            //Debug.Log("단계 시작 대사 진행 중...");
            yield return null;
        }
        //Debug.Log("단계 시작 대사 종료");
    }

    // 단계 마무리부분 대사 대기
    private IEnumerator WaitForDialogueThenProceed()
    {
        // 대사 진행 중이면 대기
        while (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
        {
            //Debug.Log("대사가 아직 진행 중입니다.");
            yield return null;
        }
        //Debug.Log("대사 종료");

        // 8. 게이지 UI 비활성화
        Step step = steps[currentStepIndex];
        if (step.gaugeUI != null)
        {
            step.gaugeUI.SetActive(false);
        }

        // 다음 단계로 이동
        Invoke(nameof(NextStep), 0.1f);
    }

    // 5. 플레이어 행동 완료
    public void OnPlayerActionCompleted()      // 해당 함수는 다른 스크립트에서 플레이어 행동이 완료될시 호출
    {
        if (isPlayerActionCompleted) return;

        SoundManager.Instance.PlaySFX("0.Suc_bell");
        //Debug.LogWarning(" OnPlayerActionCompleted() 호출됨!");
        Debug.LogWarning(Environment.StackTrace);

        EndStep(steps[currentStepIndex]);
        CompleteCurrentStep();
    }

    // 물체를 Grab했을 때
    // 7. GuideHand 비활성화
    public void WhenGrabbedObject()
    {
        // 0. Feedback 타이머 멈추기
        isGrabbing = true;
        timeSinceRelease = 0f;
        Debug.Log($"[FeedbackLoop] isGrabbing: {isGrabbing}");
        if (feedbackCoroutine != null) StopCoroutine(feedbackCoroutine);

        SoundManager.Instance.PlaySFX("0.Grabbing");
        if (currentStepIndex >= 0 && currentStepIndex < steps.Count)
        {
            foreach (GameObject obj in steps[currentStepIndex].target)
            {
                Transform parentTransform = obj.transform;
                for (int i = 0; i < parentTransform.childCount; i++)
                {
                    Transform child = parentTransform.GetChild(i);

                    if (child.name.Contains("GuideHand"))
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    // 물체에서 손을 뗐을 때
    public void WhenReleasedObject()
    {
        Debug.Log("손을 뗌");
        isGrabbing = false;
        timeSinceRelease = 0f;
        Debug.Log($"[FeedbackLoop] isGrabbing: {isGrabbing}");

        if (feedbackCoroutine != null) StopCoroutine(feedbackCoroutine);
        feedbackCoroutine = StartCoroutine(FeedbackLoop());
        Debug.Log("FeedbackCoroutine 재시작됨 (손을 뗐을 때)");
    }

    // Feedback 코루틴
    private IEnumerator FeedbackLoop()
    {
        // 대사가 끝날 때까지 대기
        while (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
        {
            Debug.Log("대사가 아직 진행 중입니다.");
            yield return null;
        }
        Debug.Log("대사 종료");

        while (!isPlayerActionCompleted)
        {
            Debug.Log($"[FeedbackLoop 안쪽] isGrabbing: {isGrabbing}");
            if (!feedbackCooldown)
            {
                if (isGrabbing)
                {
                    timeSinceRelease = 0f;
                }
                else
                {
                    timeSinceRelease += Time.deltaTime;
                    //Debug.Log($"대사 대기 경과 시간: {timeSinceRelease:F2}");
                    
                    if (timeSinceRelease >= 7f)
                    {
                        Debug.Log("feedback 7초 넘어서 실행");
                        StartCoroutine(PlayFeedback());
                        timeSinceRelease = 0f;
                    }
                }
            }

            yield return null;
        }
    }

    private IEnumerator PlayFeedback()
    {
        feedbackCooldown = true;
        isFeedbackPlaying = true;

        Step step = steps[currentStepIndex];
        if (DialogueManager.Instance.dialougeDictionary.ContainsKey(step.dialogueKey + "_fb"))
        {
            DialogueManager.Instance.StartDialogue(step.dialogueKey + "_fb");
        }

        foreach (GameObject obj in step.target)
        {
            Transform parentTransform = obj.transform;
            for (int i = 0; i < parentTransform.childCount; i++)
            {
                Transform child = parentTransform.GetChild(i);

                if (child.name.Contains("GuideHand"))
                {
                    child.gameObject.SetActive(true);
                }
               
            }
        }

        // 피드백 대사 끝날 때까지 기다림
        yield return new WaitUntil(() => !DialogueManager.Instance.isDialogueActive);

        feedbackCooldown = false;
        isFeedbackPlaying = false;
        timeSinceRelease = 0f; // 대사 끝나고 다시 0초부터
    }

    // 자동완료
    private IEnumerator AutoCompleteAfterDelay(float delay)
    {
        // 대사가 끝날 때까지 기다림
        while (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
            yield return null;

        yield return new WaitForSeconds(delay);

        if (!isPlayerActionCompleted)
        {
            //Debug.Log("자동 완료 타이머 종료 – 다음 단계로 진행");
            CompleteCurrentStep();
        }
    }

}
