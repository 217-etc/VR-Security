using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;


    public List<SoundData> soundDatabase; // ScriptableObject�κ��� ���� �����ͺ��̽� ������
    private Dictionary<string, SoundData> soundDictionary; // ���带 ������ ã�� ���� ��ųʸ�
    private List<AudioSource> sfxSources = new List<AudioSource>(); // ȿ���� AudioSource ����Ʈ

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // ���� ������ �ʱ�ȭ
        soundDictionary = new Dictionary<string, SoundData>();
        foreach (var soundData in soundDatabase)
        {
            soundDictionary[soundData.soundName] = soundData;
        }
    }

    void Start()
    {
        PlayBGM("Wind");
        PlayBGM("FireTruck1");
        PlayBGM("FireTruck2");
    }

    /// <summary> ������� (BGM) ��� </summary>
    public void PlayBGM(string soundName)
    {
        if (!soundDictionary.ContainsKey(soundName))
        {
            Debug.LogWarning($"BGM '{soundName}' not found in SoundDatabase.");
            return;
        }
        
        AudioSource bgmSource = gameObject.AddComponent<AudioSource>();

        SoundData data = soundDictionary[soundName];
        AudioClip clip = data.clip;
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.volume = data.volume;
        bgmSource.Play();
    }

    /// <summary> ȿ���� (SFX) ��� </summary>
    public void PlaySFX(string soundName)
    {
        if (!soundDictionary.ContainsKey(soundName))
        {
            Debug.LogWarning($"SFX '{soundName}' not found in SoundDatabase.");
            return;
        }

        SoundData data = soundDictionary[soundName];
        AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.clip = data.clip;
        sfxSource.volume = data.volume;
        sfxSource.Play();

        sfxSources.Add(sfxSource);
        StartCoroutine(DestroyAfterPlay(sfxSource, data.clip.length));
    }

    /// <summary> Ư�� �ð� �Ŀ� ����� �ҽ��� �����մϴ�. </summary>
    private IEnumerator DestroyAfterPlay(AudioSource source, float time)
    {
        yield return new WaitForSeconds(time);
        sfxSources.Remove(source);
        Destroy(source);
    }

    /// <summary> ��� ȿ���� (SFX) ���� </summary>
    public void StopAllSFX()
    {
        foreach (var source in sfxSources)
        {
            source.Stop();
            Destroy(source);
        }
        sfxSources.Clear();
    }
}
