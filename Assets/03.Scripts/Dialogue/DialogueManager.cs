using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public struct DialogueTextType
{
    public string text;
    public string display_behaviour;
    public string only_audio;
}
public class DialogueStructure
{
    public string key;
    public DialogueTextType[] dialogues;

}
public class DialogueManager : Singleton<DialogueManager>
{
    private const string GOOGLE_SHEET_URL = "https://docs.google.com/spreadsheets/d/1cI32XyRtkvbBQhcp7X-bqHLbGvLRB0LizHmtGTCLAZA/export?format=csv&gid=0";
    private const string TTS_URL = "https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen=32&client=tw-ob&q=";
    //딕셔너리 생성하기
    public Dictionary<string, DialogueStructure> dialougeDictionary = new Dictionary<string, DialogueStructure>();
    public GameObject noticeUI; // 안내문 UI
    public Animator _animator; // 안내문 Animator
    private AudioSource _audioSource;

    public bool isDialogueActive = false;
    public float autoDialgoueDuration = 0.5f;
    private string _currentKey;
    private int _currentIndex = 0;
    public Action ShowNext;
    private bool _waitingForAction = false;

    // 파싱 로딩
    public TextMeshProUGUI progressText;
    public GameObject loadingUI;
    public GameObject selectUI;
    private bool isParsingComplete = false;

    // 관절
    public OVRHand ovrHand; // OVRHand 참조
    public OVRSkeleton ovrSkeleton; // OVRSkeleton 참조

    public Dictionary<string, Transform> boneTransforms = new Dictionary<string, Transform>();
    public Transform wristTransform; // 손목 Transform

    public GameObject debugSpherePrefab;
    public Dictionary<string, GameObject> debugSpheres = new Dictionary<string, GameObject>();
    public bool isInitializeComplete = false;
    public GameObject boneParent;

    async void Start()
    {
        await ParseCSV();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!isInitializeComplete || DialogueManager.Instance.boneTransforms.Count == 0 || DialogueManager.Instance.wristTransform == null) return;

        // 디버그 Sphere 위치 업데이트 (손가락 관절을 따라감)
        foreach (var key in DialogueManager.Instance.boneTransforms.Keys)
        {
            if (DialogueManager.Instance.debugSpheres.ContainsKey(key))
            {
                DialogueManager.Instance.debugSpheres[key].transform.position = DialogueManager.Instance.boneTransforms[key].position;
            }
        }
    }

    public async Task ParseCSV()
    {
        // CSV 데이터 가져오기
        string csvData = await FetchGoogleSheetData(GOOGLE_SHEET_URL);

        if (csvData == null)
        {
            Debug.LogWarning("csv file을 찾을 수 없음.");

            return;
        }

        // 엔터를 기준으로 줄 나누기
        string[] datas = csvData.Split('\n');
        int totalLines = datas.Length - 6;

        string currentKey = ""; // 현재 대사 키 값
        List<DialogueTextType> currentDialogues = new List<DialogueTextType>(); // 현재 대사 리스트

        // 6번째 줄부터 데이터 시작 (헤더는 5번째 줄)
        for (int i = 6; i < datas.Length; i++)
        {
            string[] values = datas[i].Split(',');

            if (values.Length < 2) continue; // 빈 줄 또는 유효하지 않은 데이터 방지

            string key = values[0].Trim(); // 첫 번째 열이 key
            string text = values[1].Trim(); // 두 번째 열이 대사
            string displayBehaviour = values.Length > 2 ? values[2].Trim() : ""; // 세 번째 열이 display_behaviour
            string onlyAudio = values.Length > 3 ? values[3].Trim() : ""; // 네 번째 열이 only_audio

            if (!string.IsNullOrEmpty(key)) // 새로운 대사 시작 (key가 존재)
            {
                if (!string.IsNullOrEmpty(currentKey)) // 이전 대사가 있다면 저장
                {
                    dialougeDictionary[currentKey] = new DialogueStructure
                    {
                        key = currentKey,
                        dialogues = currentDialogues.ToArray()
                    };
                }

                // 새로운 대사 키와 초기화된 대사 리스트 설정
                currentKey = key;
                currentDialogues = new List<DialogueTextType>
                {
                    new DialogueTextType { text = text, display_behaviour = displayBehaviour, only_audio = onlyAudio}
                };
            }
            else if (!string.IsNullOrEmpty(text)) // 기존 대사 추가 (key가 비어 있음)
            {
                currentDialogues.Add(new DialogueTextType { text = text, display_behaviour = displayBehaviour, only_audio = onlyAudio });
            }
            await Task.Delay(10);
        }

        // 마지막 대사 저장
        if (!string.IsNullOrEmpty(currentKey))
        {
            dialougeDictionary[currentKey] = new DialogueStructure
            {
                key = currentKey,
                dialogues = currentDialogues.ToArray()
            };
        }
        isParsingComplete = false;
        StartCoroutine(VisualLoadingCoroutine(() => isParsingComplete));

        // 관절 값 다 읽어오기
        while (ovrSkeleton.Bones == null || ovrSkeleton.Bones.Count == 0)
        {
            await Task.Yield();
        }

        // 손가락 이름 리스트 (Pinky 대신 Little 사용)
        string[] fingerNames = { "Thumb", "Index", "Middle", "Ring", "Little" };

        foreach (var bone in ovrSkeleton.Bones)
        {
            string boneName = bone.Transform.name; // Transform의 실제 이름 가져오기

            foreach (var finger in fingerNames)
            {
                if (boneName == $"XRHand_{finger}Proximal") // Base 관절
                {
                    boneTransforms[$"{finger}_Base"] = bone.Transform;
                }
                else if (boneName == $"XRHand_{finger}Tip") // Tip 손끝
                {
                    boneTransforms[$"{finger}_Tip"] = bone.Transform;
                }
                else if (boneName == "XRHand_Wrist") // 손목
                {
                    wristTransform = bone.Transform;
                }
            }
        }

        // 디버그 Sphere 생성 및 매핑
        foreach (var boneKey in boneTransforms.Keys)
        {
            Transform boneTransform = DialogueManager.Instance.boneTransforms[boneKey];

            if (boneTransform != null)
            {
                GameObject debugSphere = Instantiate(debugSpherePrefab, boneTransform.position, Quaternion.identity);
                debugSphere.transform.parent = boneParent.transform;
                debugSphere.transform.localScale = Vector3.one * 0.02f; // Sphere 크기 조정
                debugSpheres[boneKey] = debugSphere;
            }
        }
        isInitializeComplete = true;
        await Task.Yield();

        Debug.LogWarning("CSV 데이터 파싱 완료! 대사 개수: " + dialougeDictionary.Count);
        loadingUI.SetActive(false);
        selectUI.SetActive(true);
    }

    public IEnumerator VisualLoadingCoroutine(System.Func<bool> isCompleteCheck)
    {
        float progress = 0f;

        while (!isCompleteCheck())
        {
            progress = Mathf.Min(progress + Time.deltaTime * 0.3f, 0.95f);
            loadingUI.GetComponent<Slider>().value = progress;
            progressText.text = $"{(progress * 100f):F1}%";
            yield return null;
        }

        while (progress < 1f)
        {
            progress = Mathf.Min(progress + Time.deltaTime * 1f, 1f);
            loadingUI.GetComponent<Slider>().value = progress;
            progressText.text = $"{(progress * 100f):F1}%";
            yield return null;
        }
    }

    private async Task<string> FetchGoogleSheetData(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            var operation = webRequest.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
                return null;
            }

            return webRequest.downloadHandler.text;
        }
    }

    //다이얼로그 띄우기
    public void StartDialogue(string dialogueKey)
    {
        if (isDialogueActive)
        {
            Debug.LogWarning("이미 실행 중인 대사가 있습니다.");
            return;
        }
        // 대사 정보 저장하기
        _currentKey = dialogueKey;
        _currentIndex = 0;
        isDialogueActive = true;

        // 대사 시작하기
        StartCoroutine(ShowDialogue());
    }

    IEnumerator ShowDialogue()
    {
        if (!isDialogueActive) yield break;
        DialogueStructure dialogueStructure = dialougeDictionary[_currentKey];

        // 출력할 다음 대사가 존재한다면
        if (_currentIndex < dialogueStructure.dialogues.Length)
        {
            string text = dialogueStructure.dialogues[_currentIndex].text;
            string audioType = dialogueStructure.dialogues[_currentIndex].only_audio;

            Debug.Log("대사는 전 UI창 활성화 여부 : " + noticeUI.activeSelf);
            if (audioType == "N")
            {
                // UI 텍스트 교체
                TextMeshProUGUI textUI = noticeUI.GetComponentInChildren<TextMeshProUGUI>();
                textUI.text = text;

                // UI가 이미 떠 있지 않은 경우 새로 띄워야 함.
                if (!noticeUI.activeSelf)
                {
                    //_animator.SetTrigger("NoticeAppear");
                    noticeUI.SetActive(true);
                    
                    Debug.Log("대사는 activeSelf false 라서 창 나타나게 함");
                }
            }
            else
            {
                // 오디오만 재생되는 경우
                // 현재 UI가 떠있는지 체크
                if (noticeUI.activeSelf)
                {
                    //_animator.SetTrigger("NoticeDisappear");
                    noticeUI.SetActive(false);
                    
                    Debug.Log("대사는 activeSelf True 라서 창 사라지게 함");
                }
            }
            Debug.Log("대사 후 UI창 활성화 여부 : " + noticeUI.activeSelf);

            yield return StartCoroutine(PlayTTS(ChangeStringForTTS(text)));

            Debug.LogWarning($"대사의 행동 타입 : {dialogueStructure.dialogues[_currentIndex].display_behaviour}");
            // 타입이 auto면
            if (dialogueStructure.dialogues[_currentIndex].display_behaviour == "auto")
            {
                yield return new WaitForSeconds(autoDialgoueDuration);
            }
            else
            {
                // act 타입이면, 플레이어가 act할 때까지 대기
                ShowNext += OnPlayerAct;
                _waitingForAction = true;
                yield return new WaitUntil(() => !_waitingForAction);
                Debug.LogWarning("플레이어가 행동을 함!");
            }
            ShowNextDialogue();
        }
        else // 다음 대사가 없다면
        {
            // 대사 출력 종료
            EndDialogue();
        }
    }

    void ShowNextDialogue()
    {
        if (!isDialogueActive) return;

        ShowNext -= OnPlayerAct;

        _currentIndex++;

        if (_currentIndex >= dialougeDictionary[_currentKey].dialogues.Length)
        {
            EndDialogue();
        }
        else
        {
            StartCoroutine(ShowDialogue());
        }
    }

    void EndDialogue()
    {
        // 안내문 UI 비활성화
        if (noticeUI.activeSelf)
        {
            _animator.SetTrigger("NoticeDisappear");
        }
        Debug.LogWarning("안내문 종료");
        _currentKey = "";
        _currentIndex = 0;
        isDialogueActive = false;
    }

    IEnumerator PlayTTS(string data)
    {
        string ttsData = TTS_URL + data;

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(ttsData, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"TTS 요청 실패: {www.error}");
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            _audioSource.clip = clip;
            _audioSource.Play();
            Debug.Log($"{clip.length}");
            yield return new WaitForSeconds(clip.length);
            Debug.LogWarning("오디오 다 출력됨");
        }
    }
    string ChangeStringForTTS(string text)
    {
        return text + "&tl=Ko-gb";
    }

    void OnPlayerAct()
    {
        _waitingForAction = false;
    }
}
