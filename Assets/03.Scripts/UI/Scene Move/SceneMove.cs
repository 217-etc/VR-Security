using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneMove : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    public void MoveToScene()
    {
        SceneManager.LoadScene(_sceneName);
    }

    public void MoveToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
