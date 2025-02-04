using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;
using UnityEngine.UI;
public class CommandManager1P : MonoBehaviour
{
    public SpriteRenderer FirstImage; // 最初のコマンド画像
    public SpriteRenderer SecondImage; // 二番目のコマンド画像
    public SpriteRenderer ThirdImage; // 三番目のコマンド画像
                                      // メインキャンバスのTransform
    public Transform MainCanvas;
    // コマンドに対応するスプライト
    public Sprite ASprite, BSprite, XSprite, YSprite, RightSprite, LeftSprite, UpSprite, DownSprite;

    // コマンドリストと現在のコマンド順序
    private List<string> _commands; // 使用するコマンドのリスト
    private List<string> _currentSequence = new List<string>(); // 現在のコマンドの順番
    private int _currentIndex; // 現在のコマンドインデックス

    // コマンドごとのスプライトを管理するディクショナリ
    private Dictionary<string, Sprite> _commandSprites; // コマンドに対応するスプライトのディクショナリ

    // プレイヤー1のアニメーター
    [SerializeField]
    public Animator Animator1P;

    // コマンドボックス
    public GameObject FirstBox; // 最初のコマンドボックス
    public GameObject SecondBox; // 二番目のコマンドボックス
    public GameObject ThardRing; // 二番目のコマンドボックス

    // リセット時間の管理
    private float _lastResetTime; // 最後にリセットされた時間

    // プレイヤー2のコマンドマネージャ
    [SerializeField]
    CommandManager2P _command2p;

    // コマンドタイム関連の変数
    public float CommandTime = 0f; // コマンドの現在のタイム
    private float _commandTimeout = 3f; // コマンドタイムアウト時間 (3秒)
    private float _commandTimeoutSecond = 7f; // 第二フェーズのコマンドタイムアウト時間 (7秒)

    // 背景の回転制御
    [SerializeField]
    CircularMovementWithBackground _circular; // 背景を回転させるオブジェクト

    // ゲームマネージャの参照
    GameManager _gameManager;

    // プレイヤー2のコマンドマネージャ
    [SerializeField]
    CommandManager2P _2P;

    // クールダウン状態
    public bool IsCoolDown = false; // クールダウン状態かどうか

    // サードリセット状態
    private bool _oneResetThard = false;

    // コントローラーデータ
    public ControllerData ControllerData;

    // サードオブジェクトコントローラ
    [SerializeField]
    ThardObjectController _thardObjectController;

    // ゲーム開始状態
    private bool _oneStart = false;

    // 回転速度
    public float RotationSpeed = 3f; // 一秒間に二往復する速度

    // 最大回転角度
    public float MaxAngle = 30f;    // 最大角度

    // 回転中フラグ（プレイヤー1）
    private bool _isRotating = false; // 回転中かどうかを判定するフラグ

    // 回転中フラグ（プレイヤー2）
    private bool _isRotating2P = false; // 回転中かどうかを判定するフラグ
    public float Amount = 0f; // 揺れの幅
    public float EnemyAmount = 3f; // 揺れの幅
    public float PlayerAmount = 2f; // 揺れの幅
    public float Duration = 2f; // 揺れる時間
    private Vector3 _rinOriginalPosition;
    private Vector3 _enemyOriginalPosition;
    public GameObject Enemy;
    public GameObject Player1;
    private Vector3 _player1OriginalPosition;
    private Vector3 _player2OriginalPosition;
    public GameObject Player2;
    public GameObject BombEffect;
    public GameObject BombEffect2;
    // スタートメソッド
    private void Start()
    {
        _rinOriginalPosition = ThardRing.transform.position;  // 最初の位置を保存
        _enemyOriginalPosition = Enemy.transform.position;  // 最初の位置を保存
        _player1OriginalPosition = Player1.transform.position;
        _player2OriginalPosition = Player2.transform.position;
        // ゲームマネージャのインスタンス取得
        _gameManager = FindObjectOfType<GameManager>();

        // コマンドリストの初期化
        _commands = new List<string> { "A", "B", "X", "Y", "Right", "Left", "Up", "Down" };

        // コマンドに対応するスプライトのディクショナリ初期化
        _commandSprites = new Dictionary<string, Sprite>
    {
        { "A", ASprite },
        { "B", BSprite },
        { "X", XSprite },
        { "Y", YSprite },
        { "Right", RightSprite },
        { "Left", LeftSprite },
        { "Up", UpSprite },
        { "Down", DownSprite }
    };

        // ゲーム開始時の状態設定
        _gameManager.ThardImagesActive(false); // サードの画像を非表示
        _gameManager.JudgementRing.SetActive(false); // ジャッジメントリングを非表示
        _gameManager.LaneRing.SetActive(false); // レーンリングを非表示

        // コントローラーのデータを購読し、データ受信時の処理を登録
        ControllerManager.Instance.OnControllerData.Subscribe(OnControllerDataReceived).AddTo(this);
    }
    private void Update()
    {
        // コマンドの生成を開始
        StartCommand();

        // 第三フェーズでタイムアップが発生し、ゲームが開始されている場合
        if (_gameManager.TimeUpThard1P && _gameManager.GameStart)
        {
            // 見逃した際の処理をコルーチンで実行
            StartCoroutine(MissTimeUp(ControllerData));
            // タイムアップフラグをリセット
            _gameManager.TimeUpThard1P = false;
        }

        // 第二フェーズがクリアされ、クールダウン状態でない場合
        if (_gameManager.ClearSecond && !IsCoolDown)
        {
            // 第二フェーズのクリア時にタイムをリセット
            _gameManager.SecondCommandTime = 0;
            // 第二フェーズクリア処理をコルーチンで実行
            StartCoroutine(ClearSecond());
        }
        // 第三フェーズがクリアされた場合
        else if (_gameManager.ClearThard)
        {
            // 第三フェーズクリア処理をコルーチンで実行
            StartCoroutine(ClearThard());
            // 第三フェーズクリアフラグをリセット
            _gameManager.ClearThard = false;
        }

        // 一度クリアが完了した場合
        if (_gameManager.OneClear)
        {
            // コマンド画像を非表示にする
            FirstImage.gameObject.SetActive(false);
            SecondImage.gameObject.SetActive(false);
            ThirdImage.gameObject.SetActive(false);
        }
    }

    // コマンドを開始するメソッド
    private void StartCommand()
    {
        // ゲームが開始されており、コマンド生成がまだ行われていない場合
        if (_gameManager.GameStart && !_oneStart)
        {
            // プレイヤー1および2のタイマーを表示
            _gameManager.Timer1P.SetActive(true);
            _gameManager.Timer2P.SetActive(true);

            // コマンドボックスやコマンド画像を表示
            FirstBox.SetActive(true);
            FirstImage.gameObject.SetActive(true);
            SecondImage.gameObject.SetActive(true);
            ThirdImage.gameObject.SetActive(true);

            // コマンド生成を1度だけ開始
            _oneStart = true;

            // コマンド生成をコルーチンで実行
            StartCoroutine(GenerateCommands());
        }
    }
    private IEnumerator GenerateCommands()
    {
        while (true)
        {
            _gameManager.SwitchPlayer = false;
            _currentSequence = new List<string>();
            _currentIndex = 0;
            _gameManager.ClearSecond = false;
            _gameManager.ClearThard = false;
            // ゲームのフェーズカウントが0の時（最初のフェーズ）
            if (_gameManager.PhaseCount == 0)
            {
                // 一度もクリアしていない場合、プレイヤー1のタイマーを表示
                if (!_gameManager.OneClear)
                {
                    _gameManager.Timer1P.SetActive(true);
                }

                // コマンド画像を表示
                FirstImage.gameObject.SetActive(true);
                SecondImage.gameObject.SetActive(true);
                ThirdImage.gameObject.SetActive(true);

                // アニメーターをリセットして開始
                _gameManager.EnableAllAnimators(0);

                // 3つのランダムなコマンドを選択し、それに対応するスプライトを設定
                for (int i = 0; i < 3; i++)
                {
                    string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];  // ランダムにコマンドを選択
                    _currentSequence.Add(randomCommand);  // コマンドシーケンスに追加

                    Sprite commandSprite = _commandSprites[randomCommand];  // コマンドに対応するスプライトを取得
                                                                            // それぞれのコマンドに対応したスプライトを設定
                    if (i == 0)
                        FirstImage.sprite = commandSprite;
                    else if (i == 1)
                        SecondImage.sprite = commandSprite;
                    else if (i == 2)
                        ThirdImage.sprite = commandSprite; // 最大3つまでランダムなボタンスプライトを表示
                }

                //_commandTime = 0f;  // コメントアウトされた行（時間計測の初期化）

                // コマンドのシーケンスが終わるまで処理を続ける
                while (_currentIndex < _currentSequence.Count && _gameManager.GameStart)
                {
                    CommandTime += Time.deltaTime;  // 経過時間を加算

                    // 3秒以上経過した場合
                    if (CommandTime >= _commandTimeout)
                    {
                        // コマンドミス処理を開始
                        StartCoroutine(MissCommand());
                        yield break;  // コルーチンを終了
                    }

                    yield return null;  // 次のフレームを待つ
                }

                // コマンド画像を非表示にする
                FirstImage.gameObject.SetActive(false);
                SecondImage.gameObject.SetActive(false);
                ThirdImage.gameObject.SetActive(false);

                // 敵のダメージアニメーションをトリガー
                _gameManager.EnemyAnim.SetTrigger("damage");

                // プレイヤー1のタイマーを非表示にする
                _gameManager.Timer1P.SetActive(false);
                if (_gameManager.MissCount != 5)
                {
                    ShakeOncePlayer(0);
                }
                // プレイヤーのHPを減少させる（2.5ずつ）
                _gameManager.LeftHP.value -= 2.5f;
                _gameManager.RightHP.value -= 2.5f;
                ShakeForEnemySeconds(0.4f);
                // 毒魔法のSEを再生
                AudioManager.Instance.PlaySE("毒魔法", 0.8f);

                // プレイヤー1の攻撃アニメーションをトリガー
                Animator1P.SetTrigger("Attack");

                CommandTime = 0f;  // 経過時間をリセット

                // 少し待機してから次の処理へ
                yield return new WaitForSeconds(1.0f);
            }
            else if (_gameManager.PhaseCount == 1)
            {
                // フェーズ1の場合、最初のボックスや画像を非表示にする
                FirstBox.SetActive(false);
                FirstImage.gameObject.SetActive(false);
                SecondImage.gameObject.SetActive(false);
                ThirdImage.gameObject.SetActive(false);

                // プレイヤー1および2のタイマーを非表示
                _gameManager.Timer1P.SetActive(false);
                _gameManager.Timer2P.SetActive(false);

                // ランダムにコマンドとプレイヤーの番号を選択
                _gameManager.RandomCommandNum = Random.Range(0, 3);  // 0〜2のランダムなコマンド数
                _gameManager.FirstPlayerRandomNum = Random.Range(0, 2);  // プレイヤー1のランダムな番号（0か1）

                // ミキシングタイマーと画像を表示
                _gameManager.TimerMix.SetActive(true);
                _gameManager.SecondImagesActive(true);  // 2番目の画像をアクティブにする
                _gameManager.SecondBoxImage();  // ボックスの画像を設定

                // ランダムコマンドの数に応じて異なる処理を行う
                if (_gameManager.RandomCommandNum == 0)
                {
                    _gameManager.EnableAllAnimators(2);  // 2番目のアニメーションを有効にする

                    if (_gameManager.FirstPlayerRandomNum == 0)  // プレイヤー1が0の時
                    {
                        // プレイヤー1が0の場合、2つのランダムなコマンドを選択し、それぞれに対応するスプライトを設定
                        for (int i = 0; i < 2; i++)
                        {
                            string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];  // ランダムコマンドを選択
                            _currentSequence.Add(randomCommand);  // コマンドシーケンスに追加

                            Sprite commandSprite = _commandSprites[randomCommand];  // コマンドに対応するスプライトを取得
                            if (i == 0)
                                _gameManager.ThreeCommand[0].sprite = commandSprite;  // 1つ目のコマンドスプライトを設定
                            else if (i == 1)
                                _gameManager.ThreeCommand[2].sprite = commandSprite;  // 2つ目のコマンドスプライトを設定
                        }
                    }
                    else if (_gameManager.FirstPlayerRandomNum == 1)  // プレイヤー1が1の場合
                    {
                        // プレイヤー1が1の場合、1つのランダムなコマンドを選択し、それに対応するスプライトを設定
                        string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                        _currentSequence.Add(randomCommand);

                        Sprite commandSprite = _commandSprites[randomCommand];
                        _gameManager.ThreeCommand[1].sprite = commandSprite;  // 2番目のコマンドスプライトを設定
                    }
                }

                // 同様に、ランダムコマンドの数が1の場合
                if (_gameManager.RandomCommandNum == 1)
                {
                    _gameManager.EnableAllAnimators(3);  // 3番目のアニメーションを有効にする

                    if (_gameManager.FirstPlayerRandomNum == 0)  // プレイヤー1が0の時
                    {
                        // プレイヤー1が0の場合、2つのランダムなコマンドを選択し、それぞれに対応するスプライトを設定
                        for (int i = 0; i < 2; i++)
                        {
                            string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                            _currentSequence.Add(randomCommand);

                            Sprite commandSprite = _commandSprites[randomCommand];
                            if (i == 0)
                                _gameManager.FourCommand[0].sprite = commandSprite;
                            else if (i == 1)
                                _gameManager.FourCommand[2].sprite = commandSprite;
                        }
                    }
                    else if (_gameManager.FirstPlayerRandomNum == 1)  // プレイヤー1が1の場合
                    {
                        // プレイヤー1が1の場合、2つのランダムなコマンドを選択し、それぞれに対応するスプライトを設定
                        for (int i = 0; i < 2; i++)
                        {
                            string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                            _currentSequence.Add(randomCommand);

                            Sprite commandSprite = _commandSprites[randomCommand];
                            if (i == 0)
                                _gameManager.FourCommand[1].sprite = commandSprite;
                            else if (i == 1)
                                _gameManager.FourCommand[3].sprite = commandSprite;
                        }
                    }
                }

                // ランダムコマンドの数が2の場合
                if (_gameManager.RandomCommandNum == 2)
                {
                    _gameManager.EnableAllAnimators(4);  // 4番目のアニメーションを有効にする

                    if (_gameManager.FirstPlayerRandomNum == 0)  // プレイヤー1が0の場合
                    {
                        // プレイヤー1が0の場合、3つのランダムなコマンドを選択し、それぞれに対応するスプライトを設定
                        for (int i = 0; i < 3; i++)
                        {
                            string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                            _currentSequence.Add(randomCommand);

                            Sprite commandSprite = _commandSprites[randomCommand];
                            if (i == 0)
                                _gameManager.FiveCommand[0].sprite = commandSprite;
                            else if (i == 1)
                                _gameManager.FiveCommand[2].sprite = commandSprite;
                            else if (i == 2)
                                _gameManager.FiveCommand[4].sprite = commandSprite;
                        }
                    }
                    else if (_gameManager.FirstPlayerRandomNum == 1)  // プレイヤー1が1の場合
                    {
                        // プレイヤー1が1の場合、2つのランダムなコマンドを選択し、それぞれに対応するスプライトを設定
                        for (int i = 0; i < 2; i++)
                        {
                            string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                            _currentSequence.Add(randomCommand);

                            Sprite commandSprite = _commandSprites[randomCommand];
                            if (i == 0)
                                _gameManager.FiveCommand[1].sprite = commandSprite;
                            else if (i == 1)
                                _gameManager.FiveCommand[3].sprite = commandSprite;
                        }
                    }
                }

                // コマンドシーケンスが終わるまで時間を経過させる
                while (_currentIndex <= _currentSequence.Count)
                {
                    _gameManager.SecondCommandTime += Time.deltaTime; // 経過時間を加算

                    // 5秒経過した場合に処理を実行
                    if (_gameManager.SecondCommandTime >= _commandTimeoutSecond && _gameManager.GameStart)
                    {
                        // プレイヤーが指定通りの入力をしていない場合、ミス処理を開始
                        if (_gameManager.FirstPlayerRandomNum == 0 && !_gameManager.SwitchPlayer || _gameManager.FirstPlayerRandomNum == 1 && _gameManager.SwitchPlayer)
                        {
                            _gameManager.SecondImagesActive(false);  // 画像を非表示

                            // ミス処理を開始
                            StartCoroutine(MissSecond(ControllerData));
                            yield break; // コルーチンを終了
                        }
                    }

                    yield return null; // 次のフレームまで待機
                }
            }
            else if(_gameManager.PhaseCount == 2)
            {
                _thardObjectController.enabled = true;
                if (_oneResetThard)
                {
                    _thardObjectController.ResetObjects();
                }
                _gameManager.TimerMix.SetActive(false);
                _oneResetThard = true;
                _gameManager.SecondBoxSprite.gameObject.SetActive(false);
                FirstBox.SetActive(false);
                FirstImage.gameObject.SetActive(false);
                SecondImage.gameObject.SetActive(false);
                ThirdImage.gameObject.SetActive(false);
                _gameManager.RandomCommandNum = Random.Range(0, 3);
                    _gameManager.FirstPlayerRandomNum = Random.Range(0, 2);

                _gameManager.StartThard = true;
                    _gameManager.ThardImagesActive(true);

                    if (_gameManager.RandomCommandNum == 0)
                    {
                        if (_gameManager.FirstPlayerRandomNum == 0)
                        {

                            for (int i = 0; i < 3; i++)
                            {
                                string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                                _currentSequence.Add(randomCommand);
                                ObjectSizeUp objectSize0 = _gameManager.FiveCommandThard[0].GetComponent<ObjectSizeUp>();
                                ObjectSizeUp objectSize2 = _gameManager.FiveCommandThard[2].GetComponent<ObjectSizeUp>();
                                ObjectSizeUp objectSize4 = _gameManager.FiveCommandThard[4].GetComponent<ObjectSizeUp>();
                                objectSize0.PlayerNum = ObjectSizeUp.Player.player1;
                                objectSize2.PlayerNum = ObjectSizeUp.Player.player1;
                                objectSize4.PlayerNum = ObjectSizeUp.Player.player1;
                            Sprite commandSprite = _commandSprites[randomCommand];
                                if (i == 0)
                                    _gameManager.FiveCommandThard[0].sprite = commandSprite;
                                else if (i == 1)
                                    _gameManager.FiveCommandThard[2].sprite = commandSprite;
                                else if (i == 2)
                                     _gameManager.FiveCommandThard[4].sprite = commandSprite;
                             }
                        }
                        else if (_gameManager.FirstPlayerRandomNum == 1)
                        {
                             for (int i = 0; i < 2; i++)
                             {
                                    string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                                    _currentSequence.Add(randomCommand);
                                ObjectSizeUp objectSize1 = _gameManager.FiveCommandThard[1].GetComponent<ObjectSizeUp>();
                                ObjectSizeUp objectSize3 = _gameManager.FiveCommandThard[3].GetComponent<ObjectSizeUp>();
                                objectSize1.PlayerNum = ObjectSizeUp.Player.player1;
                                objectSize3.PlayerNum = ObjectSizeUp.Player.player1;
                            Sprite commandSprite = _commandSprites[randomCommand];
                                    if (i == 0)
                                        _gameManager.FiveCommandThard[1].sprite = commandSprite;
                                    else if (i == 1)
                                        _gameManager.FiveCommandThard[3].sprite = commandSprite;


                             }
                         }
                    }
                    if (_gameManager.RandomCommandNum == 1)
                    {
                        if (_gameManager.FirstPlayerRandomNum == 0)
                        {

                            for (int i = 0; i < 3; i++)
                            {
                                string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                                _currentSequence.Add(randomCommand);
                            ObjectSizeUp objectSize0 = _gameManager.SixCommand[0].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize2 = _gameManager.SixCommand[2].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize4 = _gameManager.SixCommand[4].GetComponent<ObjectSizeUp>();
                            objectSize0.PlayerNum = ObjectSizeUp.Player.player1;
                            objectSize2.PlayerNum = ObjectSizeUp.Player.player1;
                            objectSize4.PlayerNum = ObjectSizeUp.Player.player1;
                            Sprite commandSprite = _commandSprites[randomCommand];
                                if (i == 0)
                                    _gameManager.SixCommand[0].sprite = commandSprite;
                                else if (i == 1)
                                    _gameManager.SixCommand[2].sprite = commandSprite;
                                 else if (i == 2)
                                     _gameManager.SixCommand[4].sprite = commandSprite;
                             }
                         }
                        else if (_gameManager.FirstPlayerRandomNum == 1)
                        {

                            for (int i = 0; i < 3; i++)
                            {
                                string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                                _currentSequence.Add(randomCommand);
                            ObjectSizeUp objectSize1 = _gameManager.SixCommand[1].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize3 = _gameManager.SixCommand[3].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize5 = _gameManager.SixCommand[5].GetComponent<ObjectSizeUp>();
                            objectSize1.PlayerNum = ObjectSizeUp.Player.player1;
                            objectSize3.PlayerNum = ObjectSizeUp.Player.player1;
                            objectSize5.PlayerNum = ObjectSizeUp.Player.player1;
                            Sprite commandSprite = _commandSprites[randomCommand];
                            if (i == 0)
                                _gameManager.SixCommand[1].sprite = commandSprite;
                            else if (i == 1)
                                _gameManager.SixCommand[3].sprite = commandSprite;
                            else if (i == 2)
                                _gameManager.SixCommand[5].sprite = commandSprite;
                        }
                        }
                    }
                    if (_gameManager.RandomCommandNum == 2)
                    {
                   
                        if (_gameManager.FirstPlayerRandomNum == 0)
                        {

                            for (int i = 0; i < 4; i++)
                            {
                                string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                                _currentSequence.Add(randomCommand);
                            ObjectSizeUp objectSize0 = _gameManager.SevenCommand[0].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize2 = _gameManager.SevenCommand[2].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize4 = _gameManager.SevenCommand[4].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize6 = _gameManager.SevenCommand[6].GetComponent<ObjectSizeUp>();
                            objectSize0.PlayerNum = ObjectSizeUp.Player.player1;
                            objectSize2.PlayerNum = ObjectSizeUp.Player.player1;
                            objectSize4.PlayerNum = ObjectSizeUp.Player.player1;
                            objectSize6.PlayerNum = ObjectSizeUp.Player.player1;
                            Sprite commandSprite = _commandSprites[randomCommand];
                                if (i == 0)
                                    _gameManager.SevenCommand[0].sprite = commandSprite;
                                else if (i == 1)
                                    _gameManager.SevenCommand[2].sprite = commandSprite;
                                else if (i == 2)
                                    _gameManager.SevenCommand[4].sprite = commandSprite;
                            else if (i == 3)
                                _gameManager.SevenCommand[6].sprite = commandSprite;
                        }
                        }
                        else if (_gameManager.FirstPlayerRandomNum == 1)
                        {

                            for (int i = 0; i < 3; i++)
                            {
                                string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                                _currentSequence.Add(randomCommand);
                            ObjectSizeUp objectSize1 = _gameManager.SevenCommand[1].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize3 = _gameManager.SevenCommand[3].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize5 = _gameManager.SevenCommand[5].GetComponent<ObjectSizeUp>();
                            objectSize1.PlayerNum = ObjectSizeUp.Player.player1;
                            objectSize3.PlayerNum = ObjectSizeUp.Player.player1;
                            objectSize5.PlayerNum = ObjectSizeUp.Player.player1;
                            Sprite commandSprite = _commandSprites[randomCommand];
                            if (i == 0)
                                _gameManager.SevenCommand[1].sprite = commandSprite;
                            else if (i == 1)
                                _gameManager.SevenCommand[3].sprite = commandSprite;
                            else if (i == 2)
                                _gameManager.SevenCommand[5].sprite = commandSprite;
                        }

                        }
                    }


                while (_currentIndex <= _currentSequence.Count)
                {
                    _gameManager.SecondCommandTime += Time.deltaTime; // 経過時間を加算

                    if (_gameManager.ThardTimeUp && _gameManager.GameStart) // 5秒経過した場合
                    {
                            _gameManager.ThardImagesActive(false);

                            StartCoroutine(MissThard(ControllerData));
                            yield break; // コルーチンを終了
                    }

                    yield return null;
                }

            }
            }
          
        }
    public void MissEffect(int playerNum)
    {
        Vector3 spawnPosition = Player1.transform.position + new Vector3(-0.3f, 0f, 0f);
        Vector3 spawnPosition2 = Player2.transform.position + new Vector3(+2f, 0f, 0f);
        if (playerNum == 0)
        {
           
            GameObject spawnedObject = Instantiate(BombEffect, MainCanvas);//回復エフェクト出す
            GameObject spawnedObject2 = Instantiate(BombEffect2, MainCanvas);//回復エフェクト出す


            spawnedObject.transform.position = spawnPosition;
            spawnedObject2.transform.position = spawnPosition;
        }
        else if(playerNum == 1)
        {
            GameObject spawnedObject = Instantiate(BombEffect, MainCanvas);//回復エフェクト出す
            GameObject spawnedObject2 = Instantiate(BombEffect2, MainCanvas);//回復エフェクト出す

            spawnedObject.transform.position = spawnPosition2;
            spawnedObject2.transform.position = spawnPosition2;
        }
        else if (playerNum == 2)
        {
            GameObject spawnedObject = Instantiate(BombEffect, MainCanvas);//回復エフェクト出す
            GameObject spawnedObject2 = Instantiate(BombEffect2, MainCanvas);//回復エフェクト出す
            GameObject spawnedObject3 = Instantiate(BombEffect, MainCanvas);//回復エフェクト出す
            GameObject spawnedObject4 = Instantiate(BombEffect2, MainCanvas);//回復エフェクト出す

            spawnedObject.transform.position = spawnPosition;
            spawnedObject2.transform.position = spawnPosition;
            spawnedObject3.transform.position = spawnPosition2;
            spawnedObject4.transform.position = spawnPosition2;
        }
    }
    public IEnumerator MissCommand()
    {
       
        // 1Pタイマーを非表示にする
        _gameManager.Timer1P.SetActive(false);

        // ミスコマンドの表示
        MissCommandBox(0);

        // ミス回数を増加させる
        _gameManager.Miss1pCountMark();

        // ミス回数が5回でない場合、1Pのミスアニメーションをトリガー
        if (_gameManager.MissCount != 5)
        {
            MissEffect(0);
        }

        // キャンセルの音を再生
        AudioManager.Instance.PlaySE("キャンセル3", 1f);

        // イメージの表示を非表示にする
        FirstImage.gameObject.SetActive(false);
        SecondImage.gameObject.SetActive(false);
        ThirdImage.gameObject.SetActive(false);

        // クールダウンを開始
        IsCoolDown = true;
        CommandTime = 0;

        // 2秒間待機
        yield return new WaitForSeconds(2.0f);

        // クールダウン終了
        IsCoolDown = false;

        // コマンドをリセット
        ResetCommands();
    }

    // コマンドのリセット処理
    public void ResetCommands()
    {
        // 経過時間リセット
        CommandTime = 0f;
        _gameManager.SecondCommandTime = 0;

        // 全てのコルーチンを停止し、新たにコマンドを生成するコルーチンを開始
        StopAllCoroutines();
        StartCoroutine(GenerateCommands());

        // 最後のリセット時間を現在の時間に設定
        _lastResetTime = Time.time;
    }

    // 2Pのターン終了時の処理
    public IEnumerator ClearSecond()
    {
        ShakeOncePlayer(0);
        ShakeOncePlayer(1);
        // ミキシングタイマーを非表示
        _gameManager.TimerMix.SetActive(false);

        // HPを回復
        _gameManager.LifeHeal();

        // セカンドイメージを非表示
        _gameManager.SecondImagesActive(false);

        // 敵のアニメーションをトリガー
        _gameManager.EnemyAnim.SetTrigger("damage");
        ShakeForEnemySeconds(0.4f);
        // 毒魔法のSEを再生
        AudioManager.Instance.PlaySE("毒魔法", 0.8f);

        // 左右のHPを減少
        _gameManager.LeftHP.value -= 5f;
        _gameManager.RightHP.value -= 5f; // HP減少処理

        // 1Pと2Pの攻撃アニメーションをトリガー
        Animator1P.SetTrigger("Attack");
        _2P.Animator2P.SetTrigger("Attack2p");

        // セカンドコマンドタイマーをリセット
        _gameManager.SecondCommandTime = 0;
        _gameManager.ClearSecond = false;

        // 1秒間待機
        yield return new WaitForSeconds(1.0f);

        // クールダウン終了
        IsCoolDown = false;
        _2P.IsCoolDown = false;

        // コマンドをリセット
        ResetCommands();
        _2P.ResetCommands();
    }
    // サードコマンドがクリアされた時の処理
    public IEnumerator ClearThard()
    {
        ShakeOncePlayer(0);
        ShakeOncePlayer(1);
        // 毒魔法のSEを再生
        AudioManager.Instance.PlaySE("毒魔法", 0.8f);

        // 左側のHPが0でない場合、敵のダメージアニメーションをトリガー
        if (_gameManager.LeftHP.value != 0)
        {
            ShakeForEnemySeconds(0.4f);
            _gameManager.EnemyAnim.SetTrigger("damage");
        }
       
        // サードイメージを非表示
        _gameManager.ThardImagesActive(false);

        // 左右のHPを減少
        _gameManager.LeftHP.value -= 10f;
        _gameManager.RightHP.value -= 10f; // HP減少処理

        // 1Pと2Pの攻撃アニメーションをトリガー
        Animator1P.SetTrigger("Attack");
        _2P.Animator2P.SetTrigger("Attack2p");

        // セカンドコマンドをリセット
        _gameManager.ClearSecond = false;

        // 1秒間待機
        yield return new WaitForSeconds(1.0f);

        // クールダウン終了
        IsCoolDown = false;
        _2P.IsCoolDown = false;

        // コマンドをリセット
        ResetCommands();
        _2P.ResetCommands();
    }

    // セカンドコマンドがミスした時の処理
    public IEnumerator MissSecond(ControllerData controllerData)
    {
        // ミスコマンドの表示
        MissCommandBox(0);
        MissEffect(2);
        // プレイヤーのタイプによってミス回数を増加させる
        if (controllerData.PlayerType == PlayerType.Player1)
        {
            _gameManager.Miss1pCountMark();
        }
        else if (controllerData.PlayerType == PlayerType.Player2)
        {
            _gameManager.Miss2pCountMark();
        }
        else if (controllerData == null)
        {
            Debug.Log("controllerDataないです" + controllerData);
        }

        // ミス回数が5回でない場合、ミスアニメーションをトリガー

        // キャンセルの音を再生
        AudioManager.Instance.PlaySE("キャンセル3", 1f);

        // セカンドイメージとタイマーを非表示
        _gameManager.SecondImagesActive(false);
        _gameManager.TimerMix.SetActive(false);

        // セカンドコマンドタイマーをリセット
        _gameManager.SecondCommandTime = 0;

        // クールダウンを開始
        IsCoolDown = true;
        _2P.IsCoolDown = true;

        // 2秒間待機
        yield return new WaitForSeconds(2.0f);

        // クールダウン終了
        IsCoolDown = false;
        _2P.IsCoolDown = false;

        // コマンドをリセット
        ResetCommands();
        _2P.ResetCommands();
    }
    
    // サードコマンドがミスした時の処理
    public IEnumerator MissThard(ControllerData controllerData)
    {
        ShakeForSeconds(0.4f);
        // プレイヤーのタイプによってミス回数を増加させる
        if (controllerData.PlayerType == PlayerType.Player1)
        {
            _gameManager.Miss1pCountMark();
        }
        else if (controllerData.PlayerType == PlayerType.Player2)
        {
            _gameManager.Miss2pCountMark();
        }
        else if (controllerData == null)
        {
            Debug.Log("controllerDataないです" + controllerData);
        }
        MissEffect(2);
        // キャンセルの音を再生
        AudioManager.Instance.PlaySE("キャンセル3", 1f);

        // サードイメージを非表示
        _gameManager.ThardImagesActive(false);

        // サークルの全オブジェクトをリセット
        _circular.ResetAllObjects();

        // クールダウンを開始
        IsCoolDown = true;
        _2P.IsCoolDown = true;

        // 2秒間待機
        yield return new WaitForSeconds(2f);

        // オブジェクトのサイズをリセット
        _gameManager.AllObjectSizeReset = true;

        // クールダウン終了
        IsCoolDown = false;
        _2P.IsCoolDown = false;

        // コマンドをリセット
        ResetCommands();
        _2P.ResetCommands();

        // 1回目のターンが終了したことを記録
        _gameManager.OneTimeUp = false;
    }

    // ミスタイムアップ時の処理
    public IEnumerator MissTimeUp(ControllerData controllerData)
    {
        MissEffect(2);

        // キャンセルの音を再生
        AudioManager.Instance.PlaySE("キャンセル3", 1f);

        // プレイヤーのタイプに応じてミス回数を増加させる
        if (controllerData.PlayerType == PlayerType.Player1)
        {
            _gameManager.Miss1pCountMark();
        }
        else if (controllerData.PlayerType == PlayerType.Player2)
        {
            _gameManager.Miss2pCountMark();
        }
        else if (controllerData == null)
        {
            Debug.Log("controllerDataないです" + controllerData);
        }

        // クールダウンを開始
        IsCoolDown = true;
        _2P.IsCoolDown = true;

        // 条件が満たされるまで待機
        yield return new WaitUntil(() => _gameManager.IsConditionMet);

        // サードイメージを非表示
        _gameManager.ThardImagesActive(false);

        // サークルの全オブジェクトをリセット
        _circular.ResetAllObjects();

        // クールダウン終了
        IsCoolDown = false;
        _2P.IsCoolDown = false;

        // コマンドをリセット
        ResetCommands();
        _2P.ResetCommands();

        // 条件をリセット
        _gameManager.IsConditionMet = false;

        // 1回目のターン終了フラグをリセット
        _gameManager.OneTimeUp = false;
    }
    private void OnControllerDataReceived(ControllerData controllerData)
    {
        if (controllerData.PlayerType != PlayerType.Player1)
            return; // プレイヤー1以外の操作は無視

        if (_currentIndex >= _currentSequence.Count)
            return;
        ControllerData = controllerData;

        string expectedCommand = _currentSequence[_currentIndex];


         if (controllerData.ActionType == ActionType.Buttons && _oneStart && !_gameManager.OneClear && !_gameManager.OnCutIn)
        {
            if (IsCorrectCommand(controllerData, expectedCommand) && _gameManager.PhaseCount == 0)
            {
                HandleCommandInput();
             

            }
            else if (IsCorrectCommand(controllerData, expectedCommand) && _gameManager.PhaseCount == 1)
            {
                if (!_gameManager.SwitchPlayer && _gameManager.FirstPlayerRandomNum == 0)
                {
               
                    HandleSecondCommandInput();
                    _gameManager.SwitchPlayer = true; 
                }
              else if (_gameManager.SwitchPlayer && _gameManager.FirstPlayerRandomNum == 1)
                {
                   
                    HandleSecondCommandInput();
                    _gameManager.SwitchPlayer = false;
                }
                else if (_gameManager.SwitchPlayer && _gameManager.FirstPlayerRandomNum == 0 || !_gameManager.SwitchPlayer && _gameManager.FirstPlayerRandomNum == 1 && !IsCoolDown)
                {
                    StartCoroutine(MissSecond(ControllerData));
                }
            }
            else if (IsCorrectCommand(controllerData, expectedCommand) && _gameManager.PhaseCount == 2 && _gameManager.OkPlayer1Thard && !_gameManager.OneTimeUp)
            {
          
                HandleThardCommandInput();
            }
            else
            {
              
                if (_gameManager.PhaseCount == 0 && !IsCoolDown)
                {
                    ExecutePhaseCount();
                }
                if (_gameManager.PhaseCount == 1 && !IsCoolDown)
                {
                    StartCoroutine(MissSecond(ControllerData));
                }
                if (_gameManager.PhaseCount == 2 && !IsCoolDown && !_gameManager.OneTimeUp &&_gameManager.GameStart)
                {
                  
                    StartCoroutine(MissThard(ControllerData));
                   
                }
            }
        }
      
    }
    private void ExecutePhaseCount()
    {

        StartCoroutine(MissCommand());

    }


    private void HandleCommandInput()
    {
        Vector3 firstCommand;

        if (_currentIndex == 0)
        {

            firstCommand = FirstImage.gameObject.transform.position;
            Instantiate(_gameManager.CommandEffect, firstCommand, Quaternion.identity);
            FirstImage.gameObject.SetActive(false);
            AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
        }
        else if (_currentIndex == 1)
        {
            firstCommand = SecondImage.gameObject.transform.position;
            Instantiate(_gameManager.CommandEffect, firstCommand, Quaternion.identity);
            SecondImage.gameObject.SetActive(false);
            AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
        }

        else if (_currentIndex == 2)
        {
            firstCommand = ThirdImage.gameObject.transform.position;
            Instantiate(_gameManager.CommandEffect, firstCommand, Quaternion.identity);
            ThirdImage.gameObject.SetActive(false);
            AudioManager.Instance.PlaySE("ひらめき05", 1f);
        }
        _currentIndex++;

        if (_currentIndex >= _currentSequence.Count)
        {
            Debug.Log("1P Success! All commands entered correctly.");
        }
    }
    private void HandleSecondCommandInput()
    {
        Vector3 secondCommand;
        if (_gameManager.FirstPlayerRandomNum == 0)
        {
            if (_gameManager.RandomCommandNum == 0)
            {
                if (_currentIndex == 0 && !_gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.ThreeCommand[0].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.ThreeCommand[0].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _gameManager.SwitchPlayer = true;
                    _currentIndex++;
                }
                else if (_currentIndex == 1 && !_gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.ThreeCommand[2].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.ThreeCommand[2].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("ひらめき05", 0.8f);
                    _gameManager.ClearSecond = true;
                }

            }
            if (_gameManager.RandomCommandNum == 1)
            {
                if (_currentIndex == 0 && !_gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.FourCommand[0].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.FourCommand[0].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _gameManager.SwitchPlayer = true;
                    _currentIndex++;
                }
                else if (_currentIndex == 1 && !_gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.FourCommand[2].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.FourCommand[2].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _gameManager.SwitchPlayer = true;
                    _currentIndex++;
                }
            }
            if (_gameManager.RandomCommandNum == 2)
            {
                if (_currentIndex == 0 && !_gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.FiveCommand[0].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.FiveCommand[0].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _gameManager.SwitchPlayer = true;
                    _currentIndex++;
                }
                else if (_currentIndex == 1 && !_gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.FiveCommand[2].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.FiveCommand[2].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _gameManager.SwitchPlayer = true;
                    _currentIndex++;
                }
                else if (_currentIndex == 2 && !_gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.FiveCommand[4].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.FiveCommand[4].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("ひらめき05", 0.8f);
                    _gameManager.ClearSecond = true;
                }
               
            }
        }
        if (_gameManager.FirstPlayerRandomNum == 1)
        {
            if (_gameManager.RandomCommandNum == 0)
            {
                
                if (_currentIndex == 0 && _gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.ThreeCommand[1].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.ThreeCommand[1].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _gameManager.SwitchPlayer = false;

                    _currentIndex++;
                }

            }
            if (_gameManager.RandomCommandNum == 1)
            {
                if (_currentIndex == 0 && _gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.FourCommand[1].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.FourCommand[1].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _gameManager.SwitchPlayer = false;

                    _currentIndex++;
                }
                else if (_currentIndex == 1 && _gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.FourCommand[3].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.FourCommand[3].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("ひらめき05", 0.8f);
                    _gameManager.ClearSecond = true;
                }
            }
            if (_gameManager.RandomCommandNum == 2)
            {
                if (_currentIndex == 0 && _gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.FiveCommand[1].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.FiveCommand[1].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _gameManager.SwitchPlayer = false;

                    _currentIndex++;
                }
                else if (_currentIndex == 1 && _gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.FiveCommand[3].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.FiveCommand[3].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _gameManager.SwitchPlayer = false;

                    _currentIndex++;
                }


            }


        }
    }

    private void HandleThardCommandInput()
    {
        Vector3 thardCommand;
        if (_gameManager.FirstPlayerRandomNum == 0)
        {
            if (_gameManager.RandomCommandNum == 0)
            {
                if (_currentIndex == 0 )
                {
                    thardCommand = _gameManager.FiveCommandThard[0].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.FiveCommandThard[0].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _currentIndex++;
                }
                else if (_currentIndex == 1)
                {
                    thardCommand = _gameManager.FiveCommandThard[2].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.FiveCommandThard[2].gameObject.SetActive(false);
                    _currentIndex++;
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                }
                else if (_currentIndex == 2)
                {
                    thardCommand = _gameManager.FiveCommandThard[4].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.FiveCommandThard[4].gameObject.SetActive(false);
                    _currentIndex++;
                    _gameManager.ClearThard = true;
                    AudioManager.Instance.PlaySE("ひらめき05", 0.8f);
                }

            }
            if (_gameManager.RandomCommandNum == 1)
            {
                if (_currentIndex == 0)
                {
                    thardCommand = _gameManager.SixCommand[0].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SixCommand[0].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _currentIndex++;
                }
                else if (_currentIndex == 1 )
                {
                    thardCommand = _gameManager.SixCommand[2].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SixCommand[2].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _currentIndex++;
                }
                else if (_currentIndex == 2)
                {
                    thardCommand = _gameManager.SixCommand[4].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SixCommand[4].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _currentIndex++;
                }
            }
            if (_gameManager.RandomCommandNum == 2)
            {
                if (_currentIndex == 0)
                {
                    thardCommand = _gameManager.SevenCommand[0].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SevenCommand[0].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _currentIndex++;
                }
                else if (_currentIndex == 1)
                {
                    thardCommand = _gameManager.SevenCommand[2].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SevenCommand[2].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _currentIndex++;
                }
                else if (_currentIndex == 2)
                {
                    thardCommand = _gameManager.SevenCommand[4].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SevenCommand[4].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _currentIndex++;
                }
                else if (_currentIndex == 3)
                {
                    thardCommand = _gameManager.SevenCommand[6].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SevenCommand[6].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("ひらめき05", 0.8f);
                    _currentIndex++;
                    _gameManager.ClearThard = true;
                }

            }
        }
        if (_gameManager.FirstPlayerRandomNum == 1)
        {
            if (_gameManager.RandomCommandNum == 0)
            {

                if (_currentIndex == 0)
                {
                    thardCommand = _gameManager.FiveCommandThard[1].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.FiveCommandThard[1].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _currentIndex++;
                }
                else if (_currentIndex == 1)
                {
                    thardCommand = _gameManager.FiveCommandThard[3].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.FiveCommandThard[3].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _currentIndex++;
                }


            }
            if (_gameManager.RandomCommandNum == 1)
            {
                if (_currentIndex == 0)
                {
                    thardCommand = _gameManager.SixCommand[1].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SixCommand[1].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _currentIndex++;
                }
                else if (_currentIndex == 1)
                {
                    thardCommand = _gameManager.SixCommand[3].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SixCommand[3].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _currentIndex++;
                }
                else if (_currentIndex == 2)
                {
                    thardCommand = _gameManager.SixCommand[5].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SixCommand[5].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("ひらめき05", 0.8f);
                    _currentIndex++;
                    _gameManager.ClearThard = true;
                }
            }
            if (_gameManager.RandomCommandNum == 2)
            {
                if (_currentIndex == 0)
                {
                    thardCommand = _gameManager.SevenCommand[1].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SevenCommand[1].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _currentIndex++;
                }
                else if (_currentIndex == 1)
                {
                    thardCommand = _gameManager.SevenCommand[3].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SevenCommand[3].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _currentIndex++;
                }
                else if (_currentIndex == 2)
                {
                    thardCommand = _gameManager.SevenCommand[5].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SevenCommand[5].gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE("電子ルーレット停止ボタンを押す", 0.8f);
                    _currentIndex++;
                }


            }


        }
    }
    private bool IsCorrectCommand(ControllerData controllerData, string expectedCommand)
    {
        if (controllerData.ActionType == ActionType.Buttons)
        {
            switch (expectedCommand)
            {
                case "A":
                    return controllerData.ButtonType == ButtonType.South;
                case "B":
                    return controllerData.ButtonType == ButtonType.East;
                case "X":
                    return controllerData.ButtonType == ButtonType.West;
                case "Y":
                    return controllerData.ButtonType == ButtonType.North;
                case "Right":
                    return controllerData.ButtonType == ButtonType.Right;
                case "Up":
                    return controllerData.ButtonType == ButtonType.Up;
                case "Down":
                    return controllerData.ButtonType == ButtonType.Down;
                case "Left":
                    return controllerData.ButtonType == ButtonType.Left;
                default:
                    return false;
            }
        }

        return false;
    }
    public void MissCommandBox(int playerNum)
    {
        // コルーチンを開始して回転処理を実行
        if (!_isRotating && playerNum == 0) // 二重起動を防ぐ
        {
            StartCoroutine(RotateForOneSecond(playerNum));
        }
        if(!_isRotating2P && playerNum == 1)
        {
            StartCoroutine(RotateForOneSecond(playerNum));
        }
    }
    private System.Collections.IEnumerator RotateForOneSecond(int playerNum)
    {
        if (playerNum == 0)
        {
            _isRotating = true; // 回転中に設定
        }
        if(playerNum == 1)
        {
            _isRotating2P = true;
        }
        float timer = 0f;

        while (timer < 0.5f)
        {
            
            float angle = Mathf.Sin(timer * RotationSpeed * Mathf.PI * 2) * MaxAngle;
        
            if (_gameManager.PhaseCount == 0)
            {
                if (playerNum == 0)
                {
                    FirstBox.transform.rotation = Quaternion.Euler(0, 0, angle);
                }
                else if (playerNum == 1)
                {
                    _2P.FirstBox.transform.rotation = Quaternion.Euler(0, 0, angle);
                }
            }
            else if(_gameManager.PhaseCount == 1)
            {
                SecondBox.transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            // 時間を更新
            timer += Time.deltaTime;
            yield return null; // 次のフレームまで待機
        }
        if (_gameManager.PhaseCount == 0)
        {
            if (playerNum == 0)
            {
                // 一秒経過後に回転を0にリセット
                FirstBox.transform.rotation = Quaternion.Euler(0, 0, 0);
                _isRotating = false; // 回転終了
            }
            else if (playerNum == 1)
            {
                _2P.FirstBox.transform.rotation = Quaternion.Euler(0, 0, 0);
                _isRotating2P = false;
            }
           
        }else if(_gameManager.PhaseCount == 1)
        {
            SecondBox.transform.rotation = Quaternion.Euler(0, 0, 0);
            _isRotating = false; // 回転終了
        }


    }
    public void ShakeForSeconds(float duration)
    {
     
        ThardRing.transform.DOLocalMoveX(Amount, 0.04f)
             .SetLoops(Mathf.RoundToInt(duration / 0.1f), LoopType.Yoyo)
             .SetEase(Ease.Linear)
             .OnKill(() =>
             {
                 // 揺れ終了後に元の位置に戻す
                 ThardRing.transform.position = _rinOriginalPosition;
             });
    }
    public void ShakeForEnemySeconds(float duration)
    {

        Enemy.transform.DOLocalMoveX(EnemyAmount, 0.1f)
             .SetLoops(Mathf.RoundToInt(duration / 0.1f), LoopType.Yoyo)
             .SetEase(Ease.Linear)
             .OnKill(() =>
             {
                 // 揺れ終了後に元の位置に戻す
                 Enemy.transform.position = _enemyOriginalPosition;
             });
    }
    public void ShakeOncePlayer(int playerNum1)
    {
        StartCoroutine(ShakeOncePlayerDelay(playerNum1));
    }
    public IEnumerator ShakeOncePlayerDelay(int playerNum)
    {
        yield return new WaitForSeconds(0.8f);
        if (playerNum == 0)
        {
           Player1.transform.DOLocalMoveX(_player1OriginalPosition.x - PlayerAmount, 0.2f)  // 右に移動
                .SetLoops(2, LoopType.Yoyo)  // 1回だけ往復させる
                .SetEase(Ease.InOutSine)  // 滑らかな加速と減速
                .OnKill(() =>Player1.transform.localPosition = _player1OriginalPosition);  // 終了後に元の位置に戻す
        }else if(playerNum == 1)
        {
            Player2.transform.DOLocalMoveX(_player2OriginalPosition.x + PlayerAmount, 0.2f)  // 右に移動
              .SetLoops(2, LoopType.Yoyo)  // 1回だけ往復させる
              .SetEase(Ease.InOutSine)  // 滑らかな加速と減速
              .OnKill(() => Player2.transform.localPosition = _player2OriginalPosition);  // 終了後に元の位置に戻す
        }

    }
}