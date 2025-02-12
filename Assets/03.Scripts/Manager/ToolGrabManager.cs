using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolGrabManager : MonoBehaviour
{
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
        }
        else
        {
            if (!_isEnd)
            {
                ExecuteResult();
                _isEnd = true;
            }
        }
    }

    public void GrabTool(string toolName)
    {
        _toolGrabDictionary[toolName] = true;
        Debug.Log($"{toolName} 도구를 집음.");
    }

    private bool CheckAllTrue()
    {
        foreach (bool value in _toolGrabDictionary.Values)
        {
            if (!value) return false;
        }
        return true;
    }

    private void ExecuteResult()
    {
        Debug.Log("모든 완강기 도구를 집음");
    }
}
