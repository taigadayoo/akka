using UnityEngine;

public class SE1_3 : MonoBehaviour
{
    public AudioClip SoundEffect;
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            PlaySound();
        }
        if (Input.GetKeyDown(KeyCode.Joystick2Button1))
        {
            PlaySound();
        }
    }

    void PlaySound()
    {
        if (SoundEffect != null)
        {
            _audioSource.PlayOneShot(SoundEffect);
        }
    }
}
