using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
public class DialogueStructure
{
    public string key;
    public string[] dialogue_texts;
}
public class DialogueManager : Singleton<DialogueManager>
{
    private const string GOOGLE_SHEET_URL = "https://docs.google.com/spreadsheets/d/1cI32XyRtkvbBQhcp7X-bqHLbGvLRB0LizHmtGTCLAZA/export?format=csv&gid=0";
    //��ųʸ� �����ϱ�
    public Dictionary<string, DialogueStructure> dialougeDictionary = new Dictionary<string, DialogueStructure>();

    async void Start()
    {
        await ParseCSV();
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
        List<string> currentDialogues = new List<string>(); // ���� ��� ����Ʈ

        // ��� �� ���� (6��° ���� ���, 7��° �ٺ��� ������ ����)
        for (int i = 6; i < datas.Length; i++)
        {
            string[] values = datas[i].Split(',');

            if (values.Length == 0) continue; // �� �� ����

            string key = values[0].Trim(); // ù ��° ���� key
            string dialogue = values.Length > 1 ? values[1].Trim() : ""; // �� ��° ���� ���

            if (!string.IsNullOrEmpty(key)) // ���ο� ��� ���� (key�� ����)
            {
                if (!string.IsNullOrEmpty(currentKey)) // ���� ��簡 �ִٸ� ����
                {
                    dialougeDictionary[currentKey] = new DialogueStructure
                    {
                        key = currentKey,
                        dialogue_texts = currentDialogues.ToArray()
                    };
                }

                // ���ο� ��� Ű�� �ʱ�ȭ�� ��� ����Ʈ ����
                currentKey = key;
                currentDialogues = new List<string> { dialogue };
            }
            else if (!string.IsNullOrEmpty(dialogue)) // ���� ��� �߰� (key�� ��� ����)
            {
                currentDialogues.Add(dialogue);
            }
        }

        // ������ ��� ����
        if (!string.IsNullOrEmpty(currentKey))
        {
            dialougeDictionary[currentKey] = new DialogueStructure
            {
                key = currentKey,
                dialogue_texts = currentDialogues.ToArray()
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
    void ShowDialogue()
    {
        
    }
}
