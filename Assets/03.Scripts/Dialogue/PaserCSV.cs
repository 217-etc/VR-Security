using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class DialogueStructure
{
    public string key;
    public string dialogue_text;
    public string next_dialogue_key;
}

public class PaserCSV : MonoBehaviour
{
    private const string GOOGLE_SHEET_URL = "https://docs.google.com/spreadsheets/d/1cI32XyRtkvbBQhcp7X-bqHLbGvLRB0LizHmtGTCLAZA/export?format=csv&gid=0";
    public async Task<Dictionary<string, T>> Parse<T>() where T : new()
    {
        //딕셔너리 생성하기
        Dictionary<string, T> dictionary = new Dictionary<string, T>();

        //CSV 데이터 가져오기
        string csvData = await FetchGoogleSheetData(GOOGLE_SHEET_URL);

        if (csvData == null)
        {
            Debug.Log(_CSVFileName + " 이름의 csv file을 찾을 수 없음.");
            return dictionary;
        }

        //엔터를 기준으로 줄 나누기
        string[] datas = csvData.text.Split('\n');
        //헤더 값 저장하기
        string[] headers = datas[5].Split(',');
        //7번째 줄부터 읽어오기(1~5번째 줄은 설명, 6번째 줄은 헤더)
        for (int i = 6; i < datas.Length; i++)
        {
            //빈 줄이라면 다음 줄로 넘어가기
            if (string.IsNullOrWhiteSpace(datas[i])) continue;
            //쉼표를 기준으로 분리하기
            string[] values = datas[i].Split(',');
            //첫번째 값은 키 값으로 사용
            string key = values[0];
            //제너릭 객체 생성
            T entry = new T();

            //나머지 값들을 T class 내의 필드들에 저장하기
            for (int j = 1; j < headers.Length && j < values.Length; j++)
            {
                FieldInfo field = typeof(T).GetField(headers[j], BindingFlags.Public);

                if (field != null)
                {
                    object convertedValue = ConvertValue(field.FieldType, values[j]);
                    field.SetValue(entry, convertedValue);
                }
            }

            //딕셔너리에 추가하기
            dictionary[key] = entry;
        }
        return dictionary;
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

    private object ConvertValue(System.Type fieldType, string value)
    {
        if (fieldType == typeof(int)) return int.Parse(value);
        if (fieldType == typeof(float)) return float.Parse(value);
        if (fieldType == typeof(bool)) return bool.Parse(value);
        return value;
    }
}
