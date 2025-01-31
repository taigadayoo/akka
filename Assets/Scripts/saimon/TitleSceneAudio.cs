using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneAudio : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.PlayBGM("この訓練、方向性合ってます的なBGM", 0.1f);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            AudioManager.Instance.PlaySE("決定ボタンを押す20", 1f);
        }
        if (Input.GetKeyDown(KeyCode.Joystick2Button1))
        {
            AudioManager.Instance.PlaySE("決定ボタンを押す20", 1f);
        }
    }
}
