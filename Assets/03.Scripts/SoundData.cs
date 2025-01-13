using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundData", menuName = "Sound System/Sound Data", order = 1)]
public class SoundData : ScriptableObject
{
    public string soundName;        // ������ �̸� (ȿ������ �̸����� ã�� ����)
    public AudioClip clip;          // ������ ���� AudioClip ����
    public float volume = 1f;       // ���� (0 ~ 1)
    public bool loop = false;       // ���� ���� (������� ���� ��� ���� �ʿ�)
}

