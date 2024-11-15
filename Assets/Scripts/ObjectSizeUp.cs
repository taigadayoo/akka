using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObjectSizeUp : MonoBehaviour
{
    public Vector3 DefaultScale = new Vector3(0.5f, 0.5f, 0.5f);   // 元のサイズ（リセットするサイズ）
    public Vector3 EnlargedScale = new Vector3(1f, 1f, 1f);  // 大きくするサイズ
    public float EnlargeDuration = 1f;  // 2秒かけて大きくする
    public float DelayBeforeShrinking = 1.3f;  // 大きくなった後、縮小を開始するまでの遅延時間（秒）
    public float ResizeDuration = 1f;  // 2秒かけて元の大きさに戻す

    private void OnEnable()
    {
        ResetSize();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // オブジェクトがアクティブになったときに、まずサイズを大きくする
        if (gameObject.activeSelf)
        {
            EnlargeAndResize();
        }
    }
    // サイズを徐々に大きくしてから元に戻す処理
    void EnlargeAndResize()
    {
        // 最初は元のサイズにセット
        transform.localScale = DefaultScale;

        // 2秒間かけて大きくする
        transform.DOScale(EnlargedScale, EnlargeDuration).SetEase(Ease.OutQuad)
            .OnComplete(() => StartShrinkAfterDelay());
    }
    void StartShrinkAfterDelay()
    {
        // delayBeforeShrinking（遅延）秒後に縮小を開始
        DOVirtual.DelayedCall(DelayBeforeShrinking, ShrinkBackToDefault);
    }
    // 大きくした後、元の大きさに戻す処理
    void ShrinkBackToDefault()
    {
        // 2秒間かけて元の大きさに戻す
        transform.DOScale(DefaultScale, ResizeDuration).SetEase(Ease.OutQuad);
    }
    void ResetSize()
    {
        transform.localScale = DefaultScale;  // デフォルトのサイズにリセット
    }
}