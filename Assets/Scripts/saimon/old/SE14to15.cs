using UnityEngine;
using UnityEngine.UI;

public class SE14to15 : MonoBehaviour
{
    public Slider Slider;          // 対象のスライダー
    public Slider Slider1;          // 対象のスライダー
    public AudioClip SoundEffect;   // 再生する効果音
    public AudioClip ZeroSoundEffect; // ゼロになったときの特別な効果音
    private AudioSource _audioSource;


    private float _previousValue;    // 前回のスライダーの値
    private float _previousValue1;    // 前回のスライダーの値

    void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;

        // スライダーの初期値をセット
        _previousValue = Slider.value;
        _previousValue1 = Slider1.value;
    }

    void Update()
    {
        // スライダーの値が減少している場合に効果音を再生
        if (Slider.value < _previousValue)
        {
            PlaySound();
        }
        if (Slider1.value < _previousValue1)
        {
            PlaySound();
        }

        // スライダーの値がゼロになったときに特別な効果音を再生
        if (Slider.value == 0)
        {
            PlayZeroSoundEffect();
        }
        if (Slider1.value == 0)
        {
            PlayZeroSoundEffect();
        }

        // 前回の値を更新
        _previousValue = Slider.value;
        _previousValue1 = Slider1.value;
    }

    void PlaySound()
    {
        if (SoundEffect != null)
        {
            _audioSource.PlayOneShot(SoundEffect);
        }
    }

    void PlayZeroSoundEffect()
    {
        // スライダーがゼロになったときの特別な音を再生
        if (_audioSource != null && ZeroSoundEffect != null)
        {
            _audioSource.PlayOneShot(ZeroSoundEffect);
        }
    }
}
