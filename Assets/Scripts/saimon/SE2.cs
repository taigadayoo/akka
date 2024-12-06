using UnityEngine;

public class SE2 : MonoBehaviour
{
    public AudioClip SoundEffect;
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;

        PlaySound();
    }

    void PlaySound()
    {
        if (SoundEffect != null)
        {
            _audioSource.PlayOneShot(SoundEffect);
        }
    }
}
