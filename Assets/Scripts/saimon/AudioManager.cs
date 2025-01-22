using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource _bgmAudioSource;

    private AudioSource _seAudioSource;

    private Dictionary<string, AudioClip> _bgmClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _seClips = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _bgmAudioSource = gameObject.AddComponent<AudioSource>();
        _bgmAudioSource.loop = true; // BGMループ再生
        _seAudioSource = gameObject.AddComponent<AudioSource>();
    }

    // BGMをロード
    public void LoadBGM(string name)
    {
        if (!_bgmClips.ContainsKey(name))
        {
            AudioClip clip = Resources.Load<AudioClip>($"Audio/BGM/{name}");
            if (clip != null)
            {
                _bgmClips[name] = clip;
            }
            else
            {
                Debug.LogWarning($"BGM '{name}' が見つかりませんでした");
            }
        }
    }

    // SEをロード
    public void LoadSE(string name)
    {
        if (!_seClips.ContainsKey(name))
        {
            AudioClip clip = Resources.Load<AudioClip>($"Audio/SE/{name}");
            if (clip != null)
            {
                _seClips[name] = clip;
            }
            else
            {
                Debug.LogWarning($"SE '{name}' が見つかりませんでした");
            }
        }
    }

    // BGMを再生
    public void PlayBGM(string name, float volume = 1f)
    {
        if (!_bgmClips.ContainsKey(name))
        {
            LoadBGM(name);
        }

        if (_bgmClips.ContainsKey(name))
        {
            _bgmAudioSource.clip = _bgmClips[name];
            _bgmAudioSource.volume = volume;
            _bgmAudioSource.Play();
        }
        else
        {
            Debug.LogWarning($"BGM '{name}' がロードされていません");
        }
    }

    // BGMを停止
    public void StopBGM()
    {
        _bgmAudioSource.Stop();
    }

    // SEを再生
    public void PlaySE(string name, float volume = 1f)
    {
        if (!_seClips.ContainsKey(name))
        {
            LoadSE(name);
        }

        if (_seClips.ContainsKey(name))
        {
            _seAudioSource.PlayOneShot(_seClips[name], volume);
        }
        else
        {
            Debug.LogWarning($"SE '{name}' がロードされていません");
        }
    }
}
