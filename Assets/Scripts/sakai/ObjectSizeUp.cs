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
    CommandManager1P _1P;
    CommandManager2P _2P;
    ThardObjectController _thardObjectController;
    Animator _animator;
  
    public enum Player
    {
        player1,
        player2
    }
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _1P = FindFirstObjectByType<CommandManager1P>();
        _2P = FindFirstObjectByType<CommandManager2P>();
        _thardObjectController = FindFirstObjectByType<ThardObjectController>();
        if(_animator != null)
        {
            _animator.enabled = false;
        }
    }
    public Player PlayerNum;
    private void OnEnable()
    {
        ResetSize();
    }
    private void Update()
    {
        if(_gameManager.AllObjectSizeReset)
        {
            ResetSize();
            _gameManager.AllObjectSizeReset = false;
        }
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
        if(collision.gameObject.tag == "TimeUp" && !_gameManager.OneTimeUp)
        {
            if (PlayerNum == Player.player1)
            {
                _gameManager.OneTimeUp = true;
                _gameManager.TimeUpThard1P = true;   
            }
            else if (PlayerNum == Player.player2)
            {
                _gameManager.OneTimeUp = true;
                _gameManager.TimeUpThard2P = true;
            }

                _thardObjectController.StartAnimationsSequentially();


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
        transform.DOScale(EnlargedScale, EnlargeDuration).SetEase(Ease.OutQuad);
    
    }

}