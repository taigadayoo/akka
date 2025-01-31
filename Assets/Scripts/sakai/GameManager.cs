using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // クリアタイムを計測する変数
    private float _clearTIme = 0;

    // ゲームクリアかどうかを判定するフラグ
    private bool _isClear = false;

    // ミスを追跡する1Pと2Pの配列
    [SerializeField]
    public Image[] Miss1P;
    [SerializeField]
    public Image[] Miss2P;

    // 第二フェーズ時のスプライトを格納する配列
    [SerializeField]
    public Sprite[] SecondPhaseSprite;

    // コマンド表示用のスプライトレンダラーの配列
    [SerializeField]
    public SpriteRenderer[] ThreeCommand;
    [SerializeField]
    public SpriteRenderer[] FourCommand;
    [SerializeField]
    public SpriteRenderer[] FiveCommand;
    [SerializeField]
    public SpriteRenderer[] FiveCommandThard;
    [SerializeField]
    public SpriteRenderer[] SixCommand;
    [SerializeField]
    public SpriteRenderer[] SevenCommand;

    // ミス回数のカウント
    public int MissCount = 0;

    // 第二フェーズ用のボックススプライト
    public SpriteRenderer SecondBoxSprite;

    // コマンドエフェクトやヒールエフェクトを格納するオブジェクト
    [SerializeField]
    public GameObject CommandEffect;
    [SerializeField]
    public GameObject HealEffect;

    // テキストアニメーションの管理クラス
    [SerializeField]
    StartTextAnimation _textAnimation;

    // ゲームオーバー時の状態を追跡するフラグ
    public bool OnGameOver = false;

    // 敵のHPスライダー
    public Slider LeftHP;
    public Slider RightHP;

    // フェーズのカウント
    public int PhaseCount = 0;

    // 先行プレイヤーを決めるためのランダム値
    public int FirstPlayerRandomNum = 0;

    // 第二フェーズのランダムコマンドの数
    public int RandomCommandNum = 2;

    // 第二フェーズでのコマンド表示時間
    public float SecondCommandTime = 0f;

    // 第二フェーズのクリアフラグ
    public bool ClearSecond = false;
    public bool ClearThard = false;

    // ラインリングや判定リングのゲームオブジェクト
    public GameObject LaneRing;
    public GameObject JudgementRing;

    // 1Pと2Pのコマンド管理オブジェクト
    [SerializeField]
    CommandManager1P _1P;
    [SerializeField]
    CommandManager2P _2P;

    // 各アニメーターの配列
    public Animator[] First1PAnim;
    public Animator[] First2PAnim;
    public Animator[] SecondThreeAnim;
    public Animator[] SecondFourAnim;
    public Animator[] SecondFiveAnim;

    // ゲームスタート時のパネル
    public GameObject StartPanel;

    // プレイヤーの交代に関するフラグ
    public bool SwitchPlayer = false;

    // フェーズ移行に関するフラグ
    public bool ChangeNext = false;
    public bool ChangeLast = false;

    // サードフェーズのタイムアップに関するフラグ
    public bool ThardTimeUp = false;
    public bool StartThard = false;

    // サードフェーズのOK判定
    public bool OkPlayer1Thard = false;
    public bool OkPlayer2Thard = false;

    // サードフェーズのタイムアップ
    public bool TimeUpThard1P = false;
    public bool TimeUpThard2P = false;

    // ゲームクリア関連のフラグ
    public bool OneTimeUp = false;
    public bool OneClear = false;

    // すべてのオブジェクトサイズをリセットするフラグ
    public bool AllObjectSizeReset = false;

    // タイムアップ用のコライダー
    public Collider2D OnTimeUpCol;

    // メインキャンバスのTransform
    public Transform MainCanvas;

    // ゲーム開始のフラグ
    public bool GameStart = false;

    // カットイン処理に関するフラグ
    public bool OneCutIn = false;
    public GameObject Timer1P;
    public GameObject Timer2P;
    public GameObject TimerMix;
    public bool OnCutIn = false;

    // 各フェーズのセットを格納するゲームオブジェクト
    public GameObject FirstSet;
    public GameObject SecondSet;
    public GameObject ThardSet;

    // 条件が満たされたかどうかのフラグ
    public bool IsConditionMet = false;

    // カットインアニメーションを管理するクラス
    public CutInAnimationManager CutInAnimation;

    // 敵のアニメーションを管理するアニメーター
    [SerializeField]
    public Animator EnemyAnim;

    void Start()
    {
        // 初期化処理。1Pと2Pの画像を非表示にし、SecondBoxSpriteを非表示にする
        Set1pImagesActive(false);
        Set2pImagesActive(false); // 配列内のミスマークは非表示に
        SecondBoxSprite.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (OnCutIn) // カットイン中はコマンドを消す処理
        {
            _1P.enabled = false;
            _2P.enabled = false;
            FirstSet.SetActive(false);
            SecondSet.SetActive(false);
            ThardSet.SetActive(false);
            Timer1P.SetActive(false);
            Timer2P.SetActive(false);
            TimerMix.SetActive(false);
        }

        if (!_isClear && GameStart)
        {
            _clearTIme += Time.deltaTime; // ゲーム開始後、クリア時間をカウント
        }

        if (GameStart)
        {
            StartPanel.SetActive(false); // ゲームスタート時に開始パネルを非表示
        }

        if (MissCount == 5) // ミスが5回になった場合、ゲームオーバー処理
        {
            if (!OneClear) // まだゲームオーバー処理が行われていない場合
            {
                _1P.Animator1P.SetTrigger("GameOver");
                _2P.Animator2P.SetTrigger("GameOver");
                AudioManager.Instance.PlaySE("HP吸収魔法2", 1f); // 効果音再生
                _isClear = false;
                _textAnimation.StartText(); // テキストアニメーション開始
                AudioManager.Instance.StopBGM(); // BGM停止
                ClearEnabled(); // クリア関連の処理を行う
                Timer1P.SetActive(false);
                Timer2P.SetActive(false);
                OneClear = true;
            }
        }

        // 第1フェーズ時の処理
        if (LeftHP.value <= 40 && LeftHP.value > 20)
        {
            if (!OneCutIn && !OneClear) // まだ第1カットインが行われていない場合
            {
                OnCutIn = true;
                OneCutIn = true;
                CutInAnimation.StartFirstCutIn(); // 第1カットインアニメーション開始
            }
            EnemyAnim.SetBool("damage1", true);
            PhaseCount = 1;
            SecondBoxSprite.gameObject.SetActive(true); // 第1フェーズ開始時にSecondBoxSpriteを表示
        }
        // 第2フェーズ時の処理
        else if (LeftHP.value <= 20 && LeftHP.value > 0 && !OneClear)
        {
            if (OneCutIn) // 既に第1カットインが行われている場合
            {
                OnCutIn = true;
                OneCutIn = false;
                CutInAnimation.StartSecondCutIn(); // 第2カットインアニメーション開始
            }
            EnemyAnim.SetBool("damage2", true);
            PhaseCount = 2;
            JudgementRing.gameObject.SetActive(true); // ジャッジメントリングを表示
            LaneRing.gameObject.SetActive(true); // レーンリングを表示
        }
        // クリア時の処理
        else if (LeftHP.value == 0)
        {
            if (!_isClear)
            {
                EnemyAnim.SetBool("damage3", true);
                Debug.Log("aaa");
                _isClear = true;
                ClearEnabled(); // クリア処理
                _textAnimation.StartText(); // テキストアニメーション開始
                AudioManager.Instance.StopBGM(); // BGM停止
                if (SampleSoundManager.Instance != null)
                {
                    SampleSoundManager.Instance.StopBgm(); // サウンドの停止
                }
                OneClear = true;
            }
        }

        // フェーズ移行時の処理
        if (PhaseCount == 1 && !ChangeNext)
        {
            _1P.ResetCommands(); // 1Pコマンドをリセット
            _2P.ResetCommands(); // 2Pコマンドをリセット
            _1P.IsCoolDown = false;
            _2P.IsCoolDown = false;
            ChangeNext = true; // フェーズ1から2へ進行
        }
        else if (PhaseCount == 2 && !ChangeLast)
        {
            StartCoroutine(DelayCol()); // フェーズ2移行時に遅延処理を開始
            ClearSecond = false;
            _1P.ResetCommands(); // 1Pコマンドをリセット
            _2P.ResetCommands(); // 2Pコマンドをリセット
            _1P.IsCoolDown = false;
            _2P.IsCoolDown = false;
            ChangeLast = true; // フェーズ2からクリアへ移行
        }
    }

    // クリア時に行う処理
    public void ClearEnabled()
    {
        // 各コマンドのスプライトを非表示にする
        foreach (SpriteRenderer renderer in FiveCommandThard)
        {
            if (renderer != null) // nullチェック
            {
                renderer.enabled = false;
            }
        }
        foreach (SpriteRenderer renderer in SixCommand)
        {
            if (renderer != null) // nullチェック
            {
                renderer.enabled = false;
            }
        }
        foreach (SpriteRenderer renderer in SevenCommand)
        {
            if (renderer != null) // nullチェック
            {
                renderer.enabled = false;
            }
        }

        SaveDate(); // リザルトデータ保存
        JudgementRing.SetActive(false); // ジャッジメントリングを非表示
        LaneRing.SetActive(false); // レーンリングを非表示
        SecondImagesActive(false); // セカンドイメージを非表示
        _1P.FirstBox.SetActive(false); // 1Pのボックスを非表示
        _2P.FirstBox.SetActive(false); // 2Pのボックスを非表示
    }
    public void Miss1pCountMark()
    {
        if (_isClear)
            return;
            
            Miss1P[MissCount].gameObject.SetActive(true);//失敗時1pの×付ける
            MissCount += 1;
    }
    public void LifeHeal()
    {
        if (MissCount != 0)
        {
            MissCount -= 1;//失敗回数減らす
            GameObject spawnedObject = Instantiate(HealEffect, MainCanvas);//回復エフェクト出す
            RectTransform rectTransform = spawnedObject.GetComponent<RectTransform>();
            AudioManager.Instance.PlaySE("HP吸収魔法2", 1f);

            if (rectTransform != null)
            {
                Vector2 newPosition = Miss2P[MissCount].rectTransform.anchoredPosition;//エフェクトの中心がズレているので合わせる
                newPosition.x -= 30; // 丁度いい位置に合わせるためx座標から30引く
                newPosition.y -= 40;
                rectTransform.anchoredPosition = newPosition;
            }
            Miss2P[MissCount].gameObject.SetActive(false);
            Miss1P[MissCount].gameObject.SetActive(false);
        }
   
    }
    IEnumerator DelayCol()
    {
        yield return new WaitForSeconds(2f);

        OnTimeUpCol.enabled = true;
    }
    public void Miss2pCountMark()//ミスしたときの2pの×付ける
    {
        if (_isClear)
            return;           
            Miss2P[MissCount].gameObject.SetActive(true);
            MissCount += 1;
    }
    // 配列内の全てのImageオブジェクトを一括でSetActiveを切り替えるメソッド
    public void Set1pImagesActive(bool isActive)
    {
        foreach (Image img in Miss1P)
        {
            img.gameObject.SetActive(isActive);
        }
    }
    public void Set2pImagesActive(bool isActive)
    {
        foreach (Image img in Miss2P)
        {
            img.gameObject.SetActive(isActive);
        }
    }
    public void SecondBoxImage()//ランダムなコマンドのスプライトをアタッチ
    {
        if (RandomCommandNum == 0)
        {
            SecondPhaseSprite[0] = SecondBoxSprite.sprite;
        }
        if (RandomCommandNum == 1)
        {
            SecondPhaseSprite[1] = SecondBoxSprite.sprite;
        }
        if (RandomCommandNum == 2)
        {
            SecondPhaseSprite[2] = SecondBoxSprite.sprite;
        }
    }
    public void SecondImagesActive(bool isActive)//個数に合わせたランダムなコマンドのスプライトをアタッチ
    {
        if (RandomCommandNum == 0)
        {
            foreach (SpriteRenderer sp in ThreeCommand)
            {
                sp.gameObject.SetActive(isActive);
            }
        }
        if (RandomCommandNum == 1)
        {
            foreach (SpriteRenderer sp in FourCommand)
            {
                sp.gameObject.SetActive(isActive);
            }
        }
        if (RandomCommandNum == 2)
        {
            foreach (SpriteRenderer sp in FiveCommand)
            {
                sp.gameObject.SetActive(isActive);
            }
        }
        
    }
    public void ThardImagesActive(bool isActive)//個数に合わせたランダムなコマンドのスプライトをアタッチ
    {
        if (RandomCommandNum == 0)
        {
            foreach (SpriteRenderer sp in FiveCommandThard)
            {
                sp.gameObject.SetActive(isActive);
            }
        }
        if (RandomCommandNum == 1)
        {
            foreach (SpriteRenderer sp in SixCommand)
            {
                sp.gameObject.SetActive(isActive);
            }
        }
        if (RandomCommandNum == 2)
        {
            foreach (SpriteRenderer sp in SevenCommand)
            {
                sp.gameObject.SetActive(isActive);
            }
        }

    }

    public void StartGameOver()
    {


        SceneManager.Instance.LoadScene(SceneName.Result);
        //ゲームオーバー処理
    }
    private void SaveDate()
    {
        int key = _isClear ? 0: 1;//クリア判定のメソッド
        PlayerPrefs.SetFloat("ClearTime", _clearTIme);//クリアタイム保存
        PlayerPrefs.SetInt("MissCount", MissCount);//ミスした回数
       
        PlayerPrefs.SetInt("IsClear", key);
    }
   public void EnableAllAnimators(int num)
    {
        if (num == 0)
        {
            foreach (Animator animator in First1PAnim)
            {
                if (animator != null) // Nullチェック
                {
                    animator.SetTrigger("Zoom");
                }

            }
        }
        else if (num == 1)
        {
            foreach (Animator animator in First2PAnim)
            {
                if (animator != null) // Nullチェック
                {
                    animator.SetTrigger("Zoom");
                }

            }
        }
        else if (num == 2)
        {
            foreach (Animator animator in SecondThreeAnim)
            {
                if (animator != null) // Nullチェック
                {
                    animator.SetTrigger("Zoom");
                }

            }
        }
        else if (num == 3)
        {
            foreach (Animator animator in SecondFourAnim)
            {
                if (animator != null) // Nullチェック
                {
                    animator.SetTrigger("Zoom");
                }

            }
        }
        else if (num == 4)
        {
            foreach (Animator animator in SecondFiveAnim)
            {
                if (animator != null) // Nullチェック
                {
                    animator.SetTrigger("Zoom");
                }

            }
        }
    }
}
