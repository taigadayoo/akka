using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public float Delay = 1.0f;
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
