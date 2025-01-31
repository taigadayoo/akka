using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultSceneAudio : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.PlayBGM("ポケットワールド", 0.1f);
        AudioManager.Instance.PlaySE("チラシ01", 1f);
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
