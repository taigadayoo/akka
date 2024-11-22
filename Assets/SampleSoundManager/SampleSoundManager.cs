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
    
    private AudioSource audioSource;
    
    [SerializeField] 
    private List<BgmData> bgmList        = new List<BgmData>(); //BGMのリスト
    [SerializeField]
    private List<SeData>  seList         = new List<SeData>();  //SEのリスト
    private BgmType       currentBgmType = BgmType.None;        //現在再生しているBGM
    [SerializeField,Header("フェードにかかる時間")]
    private float         fadeDuration   = 1.0f;
    
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

        audioSource = gameObject.AddComponent<AudioSource>();
        //BGMがループするように設定
        audioSource.loop = true;
        audioSource.volume = 0;
    }

    /// <summary>
    /// BGMの再生
    /// </summary>
    /// <param name="type">再生したいタイプ</param>
    public void PlayBgm(BgmType type)
    {
        BgmData bgmData = bgmList.Find(s => s.Type == type);
        
        if (currentBgmType == type)
        {
            //再生中の場合は何もしない
            if (audioSource.isPlaying)
            {
                return;
            }
            
            // 同じBGMがすでに再生されている場合は、フェードインして再生
            StartCoroutine(FadeInCurrentBgm(bgmData.Volume));
            Debug.Log(audioSource.isPlaying);
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
        SeData seData = seList.Find(s => s.Type == type);
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
        
        audioSource.clip = newBGMData.Clip;
        audioSource.volume = 0;
        audioSource.Play();

        yield return StartCoroutine(FadeInCurrentBgm(newBGMData.Volume));
        
        currentBgmType = newBGMData.Type;
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeOutCurrentBgm()
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0;

        while (audioSource.volume > 0)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
        currentBgmType = BgmType.None;
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

        audioSource.volume = startVolume;

        while (audioSource.volume < targetVolume)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
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
