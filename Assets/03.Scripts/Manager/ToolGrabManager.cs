using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolGrabManager : MonoBehaviour
{
    public StepManager stepManager;
    public List<string> toolNameList = new List<string>();
    private Dictionary<string,bool> _toolGrabDictionary = new Dictionary<string,bool>();
    private bool _isComplete = false;
    private bool _isEnd = false;

    
    void Awake()
    {
        foreach (string toolName in toolNameList)
        {
            _toolGrabDictionary.Add(toolName, false);
        }
    }

    private void Update()
    {
        if (!_isComplete)
        {
            _isComplete = CheckAllTrue();
            Debug.Log($"통 : [체크] 현재 상태: {_isComplete}");
        }
        else
        {
            if (!_isEnd)
            {
                Debug.Log("통 : [성공] ExecuteResult 실행");
                ExecuteResult();
                _isEnd = true;
            }
        }
    }

    public void GrabTool(string toolName)
    {
        //_toolGrabDictionary[toolName] = true;
        //Debug.Log($"{toolName} 도구를 집음.");
        if (_toolGrabDictionary.ContainsKey(toolName))
        {
            _toolGrabDictionary[toolName] = true;
            Debug.Log($"통 : {toolName} 도구를 집음.");
        }
        else
        {
            Debug.LogError($"통 : {toolName} 은(는) toolGrabDictionary에 존재하지 않음!");
        }
    }

    public bool IsGrabEnd(string toolName)
    {
        return _toolGrabDictionary[toolName];
    }

    private bool CheckAllTrue()
    {
        foreach (KeyValuePair<string, bool> pair in _toolGrabDictionary)
        {
            Debug.Log($"[도구 체크] {pair.Key} = {pair.Value}");
            if (!pair.Value) return false;
        }
        return true;
    }

    private void ExecuteResult()
    {
        if (stepManager == null)
        {
            Debug.LogError("통 : [에러] stepManager가 null입니다!");
        }
        Debug.Log("통 : 모든 완강기 도구를 집음");
        stepManager?.OnPlayerActionCompleted();
    }
}
