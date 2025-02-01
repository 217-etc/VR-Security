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
    //딕셔너리 생성하기
    public Dictionary<string, DialogueStructure> dialougeDictionary = new Dictionary<string, DialogueStructure>();

    async void Start()
    {
        await ParseCSV();
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
        List<string> currentDialogues = new List<string>(); // 현재 대사 리스트

        // 헤더 값 저장 (6번째 줄이 헤더, 7번째 줄부터 데이터 시작)
        for (int i = 6; i < datas.Length; i++)
        {
            string[] values = datas[i].Split(',');

            if (values.Length == 0) continue; // 빈 줄 방지

            string key = values[0].Trim(); // 첫 번째 열이 key
            string dialogue = values.Length > 1 ? values[1].Trim() : ""; // 두 번째 열이 대사

            if (!string.IsNullOrEmpty(key)) // 새로운 대사 시작 (key가 존재)
            {
                if (!string.IsNullOrEmpty(currentKey)) // 이전 대사가 있다면 저장
                {
                    dialougeDictionary[currentKey] = new DialogueStructure
                    {
                        key = currentKey,
                        dialogue_texts = currentDialogues.ToArray()
                    };
                }

                // 새로운 대사 키와 초기화된 대사 리스트 설정
                currentKey = key;
                currentDialogues = new List<string> { dialogue };
            }
            else if (!string.IsNullOrEmpty(dialogue)) // 기존 대사 추가 (key가 비어 있음)
            {
                currentDialogues.Add(dialogue);
            }
        }

        // 마지막 대사 저장
        if (!string.IsNullOrEmpty(currentKey))
        {
            dialougeDictionary[currentKey] = new DialogueStructure
            {
                key = currentKey,
                dialogue_texts = currentDialogues.ToArray()
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
    void ShowDialogue()
    {
        
    }
}
