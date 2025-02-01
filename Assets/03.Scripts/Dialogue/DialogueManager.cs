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
    //��ųʸ� �����ϱ�
    public Dictionary<string, DialogueStructure> dialougeDictionary = new Dictionary<string, DialogueStructure>();
    public GameObject noticeUI; // �ȳ��� UI
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
        // CSV ������ ��������
        string csvData = await FetchGoogleSheetData(GOOGLE_SHEET_URL);

        if (csvData == null)
        {
            Debug.LogWarning("csv file�� ã�� �� ����.");
            return;
        }

        // ���͸� �������� �� ������
        string[] datas = csvData.Split('\n');

        string currentKey = ""; // ���� ��� Ű ��
        List<DialogueTextType> currentDialogues = new List<DialogueTextType>(); // ���� ��� ����Ʈ

        // 6��° �ٺ��� ������ ���� (����� 5��° ��)
        for (int i = 6; i < datas.Length; i++)
        {
            string[] values = datas[i].Split(',');

            if (values.Length < 2) continue; // �� �� �Ǵ� ��ȿ���� ���� ������ ����

            string key = values[0].Trim(); // ù ��° ���� key
            string text = values[1].Trim(); // �� ��° ���� ���
            string displayBehaviour = values.Length > 2 ? values[2].Trim() : ""; // �� ��° ���� display_behaviour

            if (!string.IsNullOrEmpty(key)) // ���ο� ��� ���� (key�� ����)
            {
                if (!string.IsNullOrEmpty(currentKey)) // ���� ��簡 �ִٸ� ����
                {
                    dialougeDictionary[currentKey] = new DialogueStructure
                    {
                        key = currentKey,
                        dialogues = currentDialogues.ToArray()
                    };
                }

                // ���ο� ��� Ű�� �ʱ�ȭ�� ��� ����Ʈ ����
                currentKey = key;
                currentDialogues = new List<DialogueTextType>
                {
                    new DialogueTextType { text = text, display_behaviour = displayBehaviour }
                };
            }
            else if (!string.IsNullOrEmpty(text)) // ���� ��� �߰� (key�� ��� ����)
            {
                currentDialogues.Add(new DialogueTextType { text = text, display_behaviour = displayBehaviour });
            }
        }

        // ������ ��� ����
        if (!string.IsNullOrEmpty(currentKey))
        {
            dialougeDictionary[currentKey] = new DialogueStructure
            {
                key = currentKey,
                dialogues = currentDialogues.ToArray()
            };
        }

        Debug.LogWarning("CSV ������ �Ľ� �Ϸ�! ��� ����: " + dialougeDictionary.Count);
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

    //���̾�α� ����
    public void StartDialogue(string dialogueKey)
    {
        if (isDialogueActive)
        {
            Debug.LogWarning("�̹� ���� ���� ��簡 �ֽ��ϴ�.");
            return;
        }
        // ��� ���� �����ϱ�
        _currentKey = dialogueKey;
        _currentIndex = 0;
        isDialogueActive = true;

        // ��� �����ϱ�
        _animator.SetTrigger("NoticeAppear");
        StartCoroutine(ShowDialogue());
    }

    IEnumerator ShowDialogue()
    {
        while (isDialogueActive)
        {
            DialogueStructure dialogueStructure = dialougeDictionary[_currentKey];
            
            // ����� ���� ��簡 �����Ѵٸ�
            if (_currentIndex < dialogueStructure.dialogues.Length)
            {
                // UI �ؽ�Ʈ ��ü
                TextMeshProUGUI textUI = noticeUI.GetComponentInChildren<TextMeshProUGUI>();
                textUI.text = dialogueStructure.dialogues[_currentIndex].text;

                // Ÿ���� auto��
                if (dialogueStructure.dialogues[_currentIndex].display_behaviour == "auto")
                {
                    yield return new WaitForSeconds(autoDialgoueDuration);
                    ShowNextDialogue();
                }
                else
                {
                    // act Ÿ���̸�, �÷��̾ act�� ������ ���
                    ShowNext += ShowNextDialogue;
                    _waitingForAction = true;
                    yield return new WaitUntil(() => !_waitingForAction);
                }
            }
            else // ���� ��簡 ���ٸ�
            {
                // ��� ��� ����
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
        // �ȳ��� UI ��Ȱ��ȭ
        _animator.SetTrigger("NoticeDisappear");
        Debug.LogWarning("�ȳ��� ����");
        _currentKey = "";
        _currentIndex = 0;
        isDialogueActive = false;
    }
}
