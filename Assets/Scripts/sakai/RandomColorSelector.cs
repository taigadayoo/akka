﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomColorSelector : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Color[] _colors; // カラーパレットを設定

    void Start()
    {
        if (_particleSystem == null)
            _particleSystem = GetComponent<ParticleSystem>();

        // ランダムな色を選択
        SetRandomColor();
        StartCoroutine(DestroyObject());
    }
   IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(2f);

        Destroy(this.gameObject);
    }
    void SetRandomColor()
    {
        if (_colors.Length == 0)
            return; // 色が設定されていない場合は終了

        var main = _particleSystem.main;

        // 配列からランダムな色を選択
        Color randomColor = _colors[Random.Range(0, _colors.Length)];
        main.startColor = randomColor;
    }
    public void PlayAnim()
    {
        _particleSystem.Play();
    }
}
