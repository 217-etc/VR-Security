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
    //��ųʸ� �����ϱ�
    public Dictionary<string, DialogueStructure> dialougeDictionary = new Dictionary<string, DialogueStructure>();
    public GameObject noticeUI; // �ȳ��� UI
    private Animator _animator; // �ȳ��� Animator
    private AudioSource _audioSource;

    public bool isDialogueActive = false;
    public float autoDialgoueDuration = 2f;
    private string _currentKey;
    private int _currentIndex = 0;
    public Action ShowNext;
    private bool _waitingForAction = true;
    
    async void Start()
    {
        await ParseCSV();
        _animator = noticeUI.GetComponentInParent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DialogueManager.Instance.StartDialogue("Dialogue_A001");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DialogueManager.Instance.StartDialogue("Dialogue_A002");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            DialogueManager.Instance.StartDialogue("Dialogue_A003");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if( DialogueManager.Instance.ShowNext != null)
            {
                Debug.LogWarning("�÷��̾ �ൿ�� ��");
                DialogueManager.Instance.ShowNext?.Invoke();
            }
        }
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
            string onlyAudio = values.Length > 3 ? values[3].Trim() : ""; // �� ��° ���� only_audio

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
                    new DialogueTextType { text = text, display_behaviour = displayBehaviour, only_audio = onlyAudio}
                };
            }
            else if (!string.IsNullOrEmpty(text)) // ���� ��� �߰� (key�� ��� ����)
            {
                currentDialogues.Add(new DialogueTextType { text = text, display_behaviour = displayBehaviour, only_audio = onlyAudio });
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
        StartCoroutine(ShowDialogue());
    }

    IEnumerator ShowDialogue()
    {
        if (!isDialogueActive) yield break;
        DialogueStructure dialogueStructure = dialougeDictionary[_currentKey];

        // ����� ���� ��簡 �����Ѵٸ�
        if (_currentIndex < dialogueStructure.dialogues.Length)
        {
            string text = dialogueStructure.dialogues[_currentIndex].text;
            string audioType = dialogueStructure.dialogues[_currentIndex].only_audio;
            if (audioType == "N")
            {
                // UI �ؽ�Ʈ ��ü
                TextMeshProUGUI textUI = noticeUI.GetComponentInChildren<TextMeshProUGUI>();
                textUI.text = text;

                // UI�� �̹� �� ���� ���� ��� ���� ����� ��.
                if (!noticeUI.activeSelf)
                {
                    _animator.SetTrigger("NoticeAppear");
                }
            }
            else
            {
                // ������� ����Ǵ� ���
                // ���� UI�� ���ִ��� üũ
                if (noticeUI.activeSelf)
                {
                    _animator.SetTrigger("NoticeDisappear");
                }
            }
            StartCoroutine(PlayTTS(ChangeStringForTTS(text)));

            Debug.LogWarning($"����� �ൿ Ÿ�� : {dialogueStructure.dialogues[_currentIndex].display_behaviour}");
            // Ÿ���� auto��
            if (dialogueStructure.dialogues[_currentIndex].display_behaviour == "auto")
            {
                yield return new WaitForSeconds(autoDialgoueDuration);
            }
            else
            {
                // act Ÿ���̸�, �÷��̾ act�� ������ ���
                ShowNext += OnPlayerAct;
                _waitingForAction = true;
                yield return new WaitUntil(() => !_waitingForAction);
                Debug.LogWarning("�÷��̾ �ൿ�� ��!");
            }
            ShowNextDialogue();
        }
        else // ���� ��簡 ���ٸ�
        {
            // ��� ��� ����
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
        // �ȳ��� UI ��Ȱ��ȭ
        if (noticeUI.activeSelf)
        {
            _animator.SetTrigger("NoticeDisappear");
        }
        Debug.LogWarning("�ȳ��� ����");
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
                Debug.LogError($"TTS ��û ����: {www.error}");
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            _audioSource.clip = clip;
            _audioSource.Play();
            Debug.Log($"{clip.length}");
            yield return new WaitForSeconds(clip.length);
            Debug.LogWarning("����� �� ��µ�");
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
