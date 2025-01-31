using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneAudio : MonoBehaviour
{
    public GameObject TargetObject1;
    public GameObject TargetObject2;
    public GameObject TargetObject3;
    public GameObject TargetObject4;
    public GameObject TargetObject5;
    public GameObject TargetObject6;
    public GameObject TargetObject7;
    public GameObject TargetObject8;
    public GameObject TargetObject9;
    public GameObject TargetObject10;
    public GameObject TargetObject11;
    public GameObject TargetObject12;
    public GameObject TargetObject13;
    public GameObject TargetObject14;
    public GameObject TargetObject15;
    public GameObject TargetObject16;
    public GameObject TargetObject17;
    public GameObject TargetObject18;
    public GameObject TargetObject19;
    public GameObject TargetObject20;
    private bool _wasActive1;
    private bool _wasActive2;
    private bool _wasActive3;
    private bool _wasActive4;
    private bool _wasActive5;
    private bool _wasActive6;
    private bool _wasActive7;
    private bool _wasActive8;
    private bool _wasActive9;
    private bool _wasActive10;
    private bool _wasActive11;
    private bool _wasActive12;
    private bool _wasActive13;
    private bool _wasActive14;
    private bool _wasActive15;
    private bool _wasActive16;
    private bool _wasActive17;
    private bool _wasActive18;
    private bool _wasActive19;
    private bool _wasActive20;

    public float Delay = 1.0f;

    public Slider Slider;
    public Slider Slider1;

    private float _previousValue;
    private float _previousValue1;
    void Start()
    {
        AudioManager.Instance.PlayBGM("ビッグモンキー", 0.1f);

        if (TargetObject1 != null)
        {
            _wasActive1 = TargetObject1.activeSelf;
        }
        if (TargetObject2 != null)
        {
            _wasActive2 = TargetObject2.activeSelf;
        }
        if (TargetObject3 != null)
        {
            _wasActive3 = TargetObject3.activeSelf;
        }
        if (TargetObject4 != null)
        {
            _wasActive4 = TargetObject4.activeSelf;
        }
        if (TargetObject5 != null)
        {
            _wasActive5 = TargetObject5.activeSelf;
        }
        if (TargetObject6 != null)
        {
            _wasActive6 = TargetObject6.activeSelf;
        }
        if (TargetObject7 != null)
        {
            _wasActive7 = TargetObject7.activeSelf;
        }
        if (TargetObject8 != null)
        {
            _wasActive8 = TargetObject8.activeSelf;
        }
        if (TargetObject9 != null)
        {
            _wasActive9 = TargetObject9.activeSelf;
        }
        if (TargetObject10 != null)
        {
            _wasActive10 = TargetObject10.activeSelf;
        }




        if (TargetObject11 != null)
        {
            _wasActive11 = TargetObject11.activeSelf;
        }
        if (TargetObject12 != null)
        {
            _wasActive12 = TargetObject12.activeSelf;
        }
        if (TargetObject13 != null)
        {
            _wasActive13 = TargetObject13.activeSelf;
        }
        if (TargetObject14 != null)
        {
            _wasActive14 = TargetObject14.activeSelf;
        }
        if (TargetObject15 != null)
        {
            _wasActive15 = TargetObject15.activeSelf;
        }
        if (TargetObject16 != null)
        {
            _wasActive16 = TargetObject16.activeSelf;
        }
        if (TargetObject17 != null)
        {
            _wasActive17 = TargetObject17.activeSelf;
        }
        if (TargetObject18 != null)
        {
            _wasActive18 = TargetObject18.activeSelf;
        }
        if (TargetObject19 != null)
        {
            _wasActive19 = TargetObject19.activeSelf;
        }
        if (TargetObject20 != null)
        {
            _wasActive20 = TargetObject20.activeSelf;
        }




        _previousValue = Slider.value;
        _previousValue1 = Slider1.value;
    }

    void Update()
    {
        if (TargetObject1.activeSelf && !_wasActive1)
        {
            AudioManager.Instance.PlaySE("ボールをミットでキャッチ1", 1f);
        }

        _wasActive1 = TargetObject1.activeSelf;

        if (TargetObject2.activeSelf && !_wasActive2)
        {
            AudioManager.Instance.PlaySE("ボールをミットでキャッチ1", 1f);
        }

        _wasActive2 = TargetObject2.activeSelf;

        if (TargetObject3.activeSelf && !_wasActive3)
        {
            AudioManager.Instance.PlaySE("ボールをミットでキャッチ1", 1f);
        }

        _wasActive3 = TargetObject3.activeSelf;

        if (TargetObject4.activeSelf && !_wasActive4)
        {
            AudioManager.Instance.PlaySE("ボールをミットでキャッチ1", 1f);
        }

        _wasActive4 = TargetObject4.activeSelf;

        if (TargetObject5.activeSelf && !_wasActive5)
        {
            AudioManager.Instance.PlaySE("ボールをミットでキャッチ1", 1f);

            StartCoroutine(SoundPlay1());
        }

        _wasActive5 = TargetObject5.activeSelf;

        if (TargetObject6.activeSelf && !_wasActive6)
        {
            AudioManager.Instance.PlaySE("ボールをミットでキャッチ1", 1f);
        }

        _wasActive6 = TargetObject6.activeSelf;

        if (TargetObject7.activeSelf && !_wasActive7)
        {
            AudioManager.Instance.PlaySE("ボールをミットでキャッチ1", 1f);
        }

        _wasActive7 = TargetObject7.activeSelf;

        if (TargetObject8.activeSelf && !_wasActive8)
        {
            AudioManager.Instance.PlaySE("ボールをミットでキャッチ1", 1f);
        }

        _wasActive8 = TargetObject8.activeSelf;

        if (TargetObject9.activeSelf && !_wasActive9)
        {
            AudioManager.Instance.PlaySE("ボールをミットでキャッチ1", 1f);
        }

        _wasActive9 = TargetObject9.activeSelf;

        if (TargetObject10.activeSelf && !_wasActive10)
        {
            AudioManager.Instance.PlaySE("ボールをミットでキャッチ1", 1f);

            StartCoroutine(SoundPlay2());
        }

        _wasActive10 = TargetObject10.activeSelf;



        if (Input.anyKeyDown)
        {
            AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 1f);

            if (TargetObject11.activeSelf && !_wasActive11)
            {
                AudioManager.Instance.PlaySE("キャンセル3", 1f);
            }
            if (TargetObject12.activeSelf && !_wasActive12)
            {
                AudioManager.Instance.PlaySE("キャンセル3", 1f);
            }
            if (TargetObject13.activeSelf && !_wasActive13)
            {
                AudioManager.Instance.PlaySE("キャンセル3", 1f);
            }
            if (TargetObject14.activeSelf && !_wasActive14)
            {
                AudioManager.Instance.PlaySE("キャンセル3", 1f);
            }
            if (TargetObject15.activeSelf && !_wasActive15)
            {
                AudioManager.Instance.PlaySE("キャンセル3", 1f);
            }
            if (TargetObject16.activeSelf && !_wasActive16)
            {
                AudioManager.Instance.PlaySE("キャンセル3", 1f);
            }
            if (TargetObject17.activeSelf && !_wasActive17)
            {
                AudioManager.Instance.PlaySE("キャンセル3", 1f);
            }
            if (TargetObject18.activeSelf && !_wasActive18)
            {
                AudioManager.Instance.PlaySE("キャンセル3", 1f);
            }
            if (TargetObject19.activeSelf && !_wasActive19)
            {
                AudioManager.Instance.PlaySE("キャンセル3", 1f);
            }
            if (TargetObject20.activeSelf && !_wasActive20)
            {
                AudioManager.Instance.PlaySE("キャンセル3", 1f);
            }
            else
            {
                AudioManager.Instance.PlaySE("ひらめき05", 1f);
            }

            _wasActive11 = TargetObject11.activeSelf;
            _wasActive12 = TargetObject12.activeSelf;
            _wasActive13 = TargetObject13.activeSelf;
            _wasActive14 = TargetObject14.activeSelf;
            _wasActive15 = TargetObject15.activeSelf;
            _wasActive16 = TargetObject16.activeSelf;
            _wasActive17 = TargetObject17.activeSelf;
            _wasActive18 = TargetObject18.activeSelf;
            _wasActive19 = TargetObject19.activeSelf;
            _wasActive20 = TargetObject20.activeSelf;
        }




        if (Slider.value < _previousValue)
        {
            AudioManager.Instance.PlaySE("毒魔法1", 1f);
        }
        if (Slider1.value < _previousValue1)
        {
            AudioManager.Instance.PlaySE("毒魔法1", 1f);

        }

        if (Slider.value == 0)
        {
            AudioManager.Instance.PlaySE("HP吸収魔法2", 1f);

        }
        if (Slider1.value == 0)
        {
            AudioManager.Instance.PlaySE("HP吸収魔法2", 1f);

        }





    }

    IEnumerator SoundPlay1()
    {
        //Debug.Log("なった");

        yield return new WaitForSeconds(Delay);

        AudioManager.Instance.PlaySE("ホイッスル", 1f);
    }

    IEnumerator SoundPlay2()
    {
        //Debug.Log("なった");

        yield return new WaitForSeconds(Delay);

        AudioManager.Instance.PlaySE("ホイッスル・連続", 1f);
    }
}
