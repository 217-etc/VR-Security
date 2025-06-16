using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TouchWindow : MonoBehaviour
{
    Vector3 initPos;

    private void Start()
    {
        initPos = transform.position;
    }

    private void Update()
    {
        transform.position = initPos;
    }
    public void WhenTouchWindow()
    {
        SceneManager.LoadScene("MDemoStage_FallDown");
    }
}
