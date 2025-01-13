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
        //��ųʸ� �����ϱ�
        Dictionary<string, T> dictionary = new Dictionary<string, T>();

        //CSV ������ ��������
        string csvData = await FetchGoogleSheetData(GOOGLE_SHEET_URL);

        if (csvData == null)
        {
            Debug.Log(_CSVFileName + " �̸��� csv file�� ã�� �� ����.");
            return dictionary;
        }

        //���͸� �������� �� ������
        string[] datas = csvData.text.Split('\n');
        //��� �� �����ϱ�
        string[] headers = datas[5].Split(',');
        //7��° �ٺ��� �о����(1~5��° ���� ����, 6��° ���� ���)
        for (int i = 6; i < datas.Length; i++)
        {
            //�� ���̶�� ���� �ٷ� �Ѿ��
            if (string.IsNullOrWhiteSpace(datas[i])) continue;
            //��ǥ�� �������� �и��ϱ�
            string[] values = datas[i].Split(',');
            //ù��° ���� Ű ������ ���
            string key = values[0];
            //���ʸ� ��ü ����
            T entry = new T();

            //������ ������ T class ���� �ʵ�鿡 �����ϱ�
            for (int j = 1; j < headers.Length && j < values.Length; j++)
            {
                FieldInfo field = typeof(T).GetField(headers[j], BindingFlags.Public);

                if (field != null)
                {
                    object convertedValue = ConvertValue(field.FieldType, values[j]);
                    field.SetValue(entry, convertedValue);
                }
            }

            //��ųʸ��� �߰��ϱ�
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
