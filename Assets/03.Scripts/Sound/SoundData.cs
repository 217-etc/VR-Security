using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundData", menuName = "Sound System/Sound Data", order = 1)]
public class SoundData : ScriptableObject
{
    public string soundName;        // 사운드의 이름 (효과음을 이름으로 찾기 위함)
    public AudioClip clip;          // 사운드의 실제 AudioClip 파일
    public float volume = 1f;       // 볼륨 (0 ~ 1)
    public bool loop = false;       // 루프 여부 (배경음악 같은 경우 루프 필요)
}

