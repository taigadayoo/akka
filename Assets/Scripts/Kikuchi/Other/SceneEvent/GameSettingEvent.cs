using UnityEngine;
using UniRx;

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

    /// <summary>
    /// オブジェクトが初期化されたときに呼び出されるメソッド
    /// </summary>
    private void Start()
    {
        // SampleSoundManager.Instance.PlayBgm(BgmType.BGM2);
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
                    HandleButtonInput(controllerData);
                    if (CheckPlayerPanelActive())
                    {
                        HandlePhaseAction();
                    }
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
            ShowPlayerPanel(controllerData.PlayerType);
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
                _player1Panel.SetActive(true);
                break;
            case PlayerType.Player2:
                _player2Panel.SetActive(true);
                break;
            default:
                Debug.LogWarning($"Unsupported PlayerType: {playerType}");
                break;
        }
    }

    /// <summary>
    /// プレイヤー1およびプレイヤー2のパネルがアクティブかどうかをチェックします
    /// </summary>
    /// <returns>両方のパネルがアクティブな場合は true、それ以外は false</returns>
    private bool CheckPlayerPanelActive()
    {
        return _player1Panel.activeSelf && _player2Panel.activeSelf;
    }

    private void DisablePlayerPanel()
    {
        _player1Panel.SetActive(false);
        _player2Panel.SetActive(false);
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
        switch (_settingPhase)
        {
            case SettingPhase.First:
                _secondPanel.SetActive(true);
                DisablePlayerPanel();
                NextPhase();
                break;
            case SettingPhase.Second:
                _secondPanel.SetActive(false);
                Debug.Log("FadeStart");
                SceneManager.Instance.LoadScene(SceneName.Game);
                NextPhase();
                break;
            case SettingPhase.Start:
                break;
            default:
                Debug.LogWarning($"Unsupported SettingPhase: {_settingPhase}");
                break;
        }
    }
}
