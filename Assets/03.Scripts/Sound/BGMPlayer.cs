using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    void Start()
    {
        PlaySceneBGM();
    }

    void PlaySceneBGM()
    {
        SoundManager.Instance.PlayBGM("FireTruck1");
        SoundManager.Instance.PlayBGM("FireTruck2");
        SoundManager.Instance.PlayBGM("Wind");
    }
}
