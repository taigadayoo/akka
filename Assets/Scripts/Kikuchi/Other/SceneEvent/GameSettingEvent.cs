using UnityEngine;
using UniRx;
using System;

/// <summary>
/// ゲーム設定画面のボタンイベントを処理するクラス
/// </summary>
public class GameSettingEvent : MonoBehaviour
{
    // プレイヤー1のパネルオブジェクト
    [SerializeField]
    private GameObject _player1Panel;

    // プレイヤー2のパネルオブジェクト
    [SerializeField]
    private GameObject _player2Panel;

    [SerializeField]
    private GameObject _firstPanel;
    [SerializeField]
    private GameObject _secondPanel;


    enum SettingPhase
    {
        First,
        Second,
        Start,
    }

    private SettingPhase _settingPhase = SettingPhase.First;

    // 各プレイヤーのボタン押下状態を保持するフラグ
    private bool _isPlayer1ButtonPressed = false;
    private bool _isPlayer2ButtonPressed = false;

    // タイマーを保持するための変数
    private IDisposable _timerDisposable;

    /// <summary>
    /// オブジェクトが初期化されたときに呼び出されるメソッド
    /// </summary>
    private void Start()
    {
        AudioManager.Instance.PlayBGM("Sunny_Days_Song", 0.9f); // BGMを再生
        // プレイヤー1およびプレイヤー2のパネルを非アクティブに設定
        _player1Panel.SetActive(false);
        _player2Panel.SetActive(false);

        // ControllerManager からの ControllerData を購読
        ControllerManager.Instance.OnControllerData.Subscribe(controllerData =>
        {
            Debug.Log($"Player: {controllerData.PlayerType}, Input: {controllerData.ActionType}");

            switch (controllerData.ActionType)
            {
                case ActionType.Buttons:
                    HandleButtonInput(controllerData); // ボタン入力を処理
                    break;
                default:
                    Debug.LogError("Invalid Action Type");
                    break;
            }

        })
        // MonoBehaviour のライフサイクルに合わせて購読を解除
        .AddTo(this);
    }



    /// <summary>
    /// ボタン入力を処理します
    /// </summary>
    /// <param name="controllerData">入力されたコントローラーのデータ</param>
    private void HandleButtonInput(ControllerData controllerData)
    {
        // East ボタンが押された場合にプレイヤーパネルを表示
        if (controllerData.ButtonType == ButtonType.East)
        {
            // ボタン押下状態を更新
            if (controllerData.PlayerType == PlayerType.Player1)
            {
                _isPlayer1ButtonPressed = true;
            }
            else if (controllerData.PlayerType == PlayerType.Player2)
            {
                _isPlayer2ButtonPressed = true;
            }

            ShowPlayerPanel(controllerData.PlayerType); // プレイヤーパネルを表示

            // 両方のプレイヤーがボタンを押下したら HandlePhaseAction() を実行
            if (_isPlayer1ButtonPressed && _isPlayer2ButtonPressed)
            {
                HandlePhaseAction(); // フェーズ遷移処理
                // フラグをリセット
                _isPlayer1ButtonPressed = false;
                _isPlayer2ButtonPressed = false;
            }
        }
    }



    /// <summary>
    /// 指定されたプレイヤーのパネルを表示します
    /// </summary>
    /// <param name="playerType">表示するプレイヤーのタイプ</param>
    public void ShowPlayerPanel(PlayerType playerType)
    {
        switch (playerType)
        {
            case PlayerType.Player1:
                AudioManager.Instance.PlaySE("はんこ", 1f); // SEを再生
                _player1Panel.SetActive(true); // プレイヤー1のパネルをアクティブ化
                break;
            case PlayerType.Player2:
                AudioManager.Instance.PlaySE("はんこ", 1f); // SEを再生
                _player2Panel.SetActive(true); // プレイヤー2のパネルをアクティブ化
                break;
            default:
                Debug.LogWarning($"Unsupported PlayerType: {playerType}");
                break;
        }
    }


    private void DisablePlayerPanel()
    {

        _player1Panel.SetActive(false); // プレイヤー1のパネルを非アクティブ化
        _player2Panel.SetActive(false); // プレイヤー2のパネルを非アクティブ化
    }

    /// <summary>
    /// フェーズの状態を次に進めます
    /// </summary>
    private void NextPhase()
    {
        switch (_settingPhase)
        {
            case SettingPhase.First:
                _settingPhase = SettingPhase.Second;
                break;
            case SettingPhase.Second:
                _settingPhase = SettingPhase.Start;
                break;
            case SettingPhase.Start:
                break;
            default:
                Debug.LogWarning($"Unsupported SettingPhase: {_settingPhase}");
                break;
        }
    }

    private void HandlePhaseAction()
    {
        // 既存のタイマーを破棄
        _timerDisposable?.Dispose();

        switch (_settingPhase)
        {
            case SettingPhase.First:
                _timerDisposable = Observable.Timer(System.TimeSpan.FromSeconds(0.5f))
                    .Subscribe(_ =>
                    {
                        _secondPanel.SetActive(true);
                        DisablePlayerPanel();
                        NextPhase();
                    })
                    .AddTo(this);
                break;
            case SettingPhase.Second:
                _secondPanel.SetActive(false);
                Debug.Log("FadeStart");
                _timerDisposable = Observable.Timer(System.TimeSpan.FromSeconds(0.5f))
                    .Subscribe(_ => {
                        SceneManager.Instance.LoadScene(SceneName.Game);
                        AudioManager.Instance.StopBGM();
                        NextPhase();
                    })
                    .AddTo(this);
                break;
            case SettingPhase.Start:
                break;
            default:
                Debug.LogWarning($"Unsupported SettingPhase: {_settingPhase}");
                break;
        }
    }
}