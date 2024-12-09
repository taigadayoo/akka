using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSceneAudio : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.PlayBGM("Sunny_Days_Song", 0.1f);
        AudioManager.Instance.PlaySE("カーソル移動11", 1f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            AudioManager.Instance.PlaySE("はんこ", 1f);
        }
        if (Input.GetKeyDown(KeyCode.Joystick2Button1))
        {
            AudioManager.Instance.PlaySE("はんこ", 1f);
        }
    }
}
