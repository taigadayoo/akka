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
    [SerializeField]
    GameManager _gameManager;
    public enum Player
    {
        player1,
        player2
    }
    public Player PlayerNum;
    private void OnEnable()
    {
        ResetSize();
    }
    void ResetSize()
    {
        transform.localScale = DefaultScale;  // デフォルトのサイズにリセット
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // オブジェクトがアクティブになったときに、まずサイズを大きくする
        if (gameObject.activeSelf && collision.gameObject.tag == "SizeUp")
        {
            EnlargeAndResize();
        }
        if (collision.gameObject.tag == "Judge")
        {
            if (PlayerNum == Player.player1)
            {
                _gameManager.OkPlayer1Thard = true;
            }
            else if (PlayerNum == Player.player2)
            {
                _gameManager.OkPlayer2Thard = true;
            }
        }
        // サイズを徐々に大きくしてから元に戻す処理     
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Judge")
        {
            if (PlayerNum == Player.player1)
            {
                _gameManager.OkPlayer1Thard = false;
            }
            else if (PlayerNum == Player.player2)
            {
                _gameManager.OkPlayer2Thard = false;
            }
        }
    }
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
}