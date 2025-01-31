using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region BGM関係

[System.Serializable]
public class BgmData
{
    public BgmType   Type = BgmType.None;
    public AudioClip Clip;
    [Range(0f, 1f)]
    public float Volume = 0.5f;
}

public enum BgmType
{
    None = 0,
    BGM1,
    BGM2,
    BGM3,
    BGM4
    // 他のBGMタイプをここに追加
}

#endregion

#region SE関係

[System.Serializable]
public class SeData
{
    public SeType    Type = SeType.None;
    public AudioClip Clip;
    [Range(0f, 1f)]
    public float 　　 Volume = 0.5f;
}

public enum SeType
{
    None = 0,
    SE1,
    SE2,
    SE3,
}

#endregion

public class SampleSoundManager : MonoBehaviour
{
    public static SampleSoundManager Instance { get; private set; }
    
    private AudioSource _audioSource;
    
    [SerializeField] 
    private List<BgmData> _bgmList        = new List<BgmData>(); //BGMのリスト
    [SerializeField]
    private List<SeData>  _seList         = new List<SeData>();  //SEのリスト
    private BgmType       _currentBgmType = BgmType.None;        //現在再生しているBGM
    [SerializeField,Header("フェードにかかる時間")]
    private float         _fadeDuration   = 1.0f;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _audioSource = gameObject.AddComponent<AudioSource>();
        //BGMがループするように設定
        _audioSource.loop = true;
        _audioSource.volume = 0;
    }

    /// <summary>
    /// BGMの再生
    /// </summary>
    /// <param name="type">再生したいタイプ</param>
    public void PlayBgm(BgmType type)
    {
        BgmData bgmData = _bgmList.Find(s => s.Type == type);
        
        if (_currentBgmType == type)
        {
            //再生中の場合は何もしない
            if (_audioSource.isPlaying)
            {
                return;
            }
            
            // 同じBGMがすでに再生されている場合は、フェードインして再生
            StartCoroutine(FadeInCurrentBgm(bgmData.Volume));
            Debug.Log(_audioSource.isPlaying);
            return;
        }
        
        if (bgmData == null || bgmData.Type == BgmType.None)
        {
            Debug.LogError("指定のTypeの音源が見つかりませんでした。\nSoundManagerに登録しておください。");
            return;
        }

        StartCoroutine(ChangeBgm(bgmData));
    }

    /// <summary>
    /// BGMの停止
    /// </summary>
    public void StopBgm()
    {
        StartCoroutine(FadeOutCurrentBgm());
    }

    /// <summary>
    /// SEの再生
    /// </summary>
    /// <param name="type">再生したいSEのタイプ</param>
    public void PlaySe(SeType type)
    {
        SeData seData = _seList.Find(s => s.Type == type);
        if (seData == null || seData.Type == SeType.None)
        {
            Debug.LogError("指定のTypeのSEが見つかりませんでした。\nSoundManagerに登録しておください。");
            return;
        }
        
        //SEを鳴らすAudioSourceの作成
        var seAudioSource = gameObject.AddComponent<AudioSource>();
        seAudioSource.clip = seData.Clip;
        seAudioSource.volume = seData.Volume;
        seAudioSource.Play();

        //SEの再生が終了したときAudioSourceを削除するコルーチンの開始
        StartCoroutine(DestroyAudioSourceAfterPlay(seAudioSource));
    }
    
    #region BGM関連クラス

    /// <summary>
    /// BGMを変えるときのフェードアウト、インを行う
    /// </summary>
    /// <param name="newBGMData"></param>
    /// <returns></returns>
    private IEnumerator ChangeBgm(BgmData newBGMData)
    {
        yield return StartCoroutine(FadeOutCurrentBgm());
        
        _audioSource.clip = newBGMData.Clip;
        _audioSource.volume = 0;
        _audioSource.Play();

        yield return StartCoroutine(FadeInCurrentBgm(newBGMData.Volume));
        
        _currentBgmType = newBGMData.Type;
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeOutCurrentBgm()
    {
        float startVolume = _audioSource.volume;
        float elapsedTime = 0;

        while (_audioSource.volume > 0)
        {
            elapsedTime += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(startVolume, 0, elapsedTime / _fadeDuration);
            yield return null;
        }

        _audioSource.Stop();
        _currentBgmType = BgmType.None;
    }

    /// <summary>
    /// フェードイン
    /// </summary>
    /// <param name="targetVolume"></param>
    /// <returns></returns>
    private IEnumerator FadeInCurrentBgm(float targetVolume)
    {
        float startVolume = 0;
        float elapsedTime = 0;

        _audioSource.volume = startVolume;

        while (_audioSource.volume < targetVolume)
        {
            elapsedTime += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / _fadeDuration);
            yield return null;
        }
    }

    #endregion
    
    #region Se関連クラス

    /// <summary>
    /// 再生が終了したAudioSourceの削除
    /// </summary>
    /// <param name="audioSource">削除するAudioSource</param>
    /// <returns></returns>
    private IEnumerator DestroyAudioSourceAfterPlay(AudioSource audioSource)
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
        Destroy(audioSource);
    }

    #endregion
}
