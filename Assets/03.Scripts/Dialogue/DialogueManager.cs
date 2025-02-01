using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public struct DialogueTextType
{
    public string text;
    public string display_behaviour;
}
public class DialogueStructure
{
    public string key;
    public DialogueTextType[] dialogues;

}
public class DialogueManager : Singleton<DialogueManager>
{
    private const string GOOGLE_SHEET_URL = "https://docs.google.com/spreadsheets/d/1cI32XyRtkvbBQhcp7X-bqHLbGvLRB0LizHmtGTCLAZA/export?format=csv&gid=0";
    //딕셔너리 생성하기
    public Dictionary<string, DialogueStructure> dialougeDictionary = new Dictionary<string, DialogueStructure>();
    public GameObject noticeUI; // 안내문 UI
    private Animator _animator;

    public bool isDialogueActive = false;
    public float autoDialgoueDuration = 2f;
    private string _currentKey;
    private int _currentIndex = 0;
    public Action ShowNext;
    private bool _waitingForAction;
    
    async void Start()
    {
        await ParseCSV();
        _animator = noticeUI.GetComponentInParent<Animator>();
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
                    new DialogueTextType { text = text, display_behaviour = displayBehaviour }
                };
            }
            else if (!string.IsNullOrEmpty(text)) // 기존 대사 추가 (key가 비어 있음)
            {
                currentDialogues.Add(new DialogueTextType { text = text, display_behaviour = displayBehaviour });
            }
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

        Debug.LogWarning("CSV 데이터 파싱 완료! 대사 개수: " + dialougeDictionary.Count);
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
        _animator.SetTrigger("NoticeAppear");
        StartCoroutine(ShowDialogue());
    }

    IEnumerator ShowDialogue()
    {
        while (isDialogueActive)
        {
            DialogueStructure dialogueStructure = dialougeDictionary[_currentKey];
            
            // 출력할 다음 대사가 존재한다면
            if (_currentIndex < dialogueStructure.dialogues.Length)
            {
                // UI 텍스트 교체
                TextMeshProUGUI textUI = noticeUI.GetComponentInChildren<TextMeshProUGUI>();
                textUI.text = dialogueStructure.dialogues[_currentIndex].text;

                // 타입이 auto면
                if (dialogueStructure.dialogues[_currentIndex].display_behaviour == "auto")
                {
                    yield return new WaitForSeconds(autoDialgoueDuration);
                    ShowNextDialogue();
                }
                else
                {
                    // act 타입이면, 플레이어가 act할 때까지 대기
                    ShowNext += ShowNextDialogue;
                    _waitingForAction = true;
                    yield return new WaitUntil(() => !_waitingForAction);
                }
            }
            else // 다음 대사가 없다면
            {
                // 대사 출력 종료
                EndDialogue();
            }
        }
    }

    void ShowNextDialogue()
    {
        if (!isDialogueActive) return;

        _waitingForAction = false;
        ShowNext -= ShowNextDialogue;

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
        _animator.SetTrigger("NoticeDisappear");
        Debug.LogWarning("안내문 종료");
        _currentKey = "";
        _currentIndex = 0;
        isDialogueActive = false;
    }
}
