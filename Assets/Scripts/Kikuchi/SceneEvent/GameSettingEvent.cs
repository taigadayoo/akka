using UnityEngine;
using UniRx;

public class GameSettingEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject _player1Panel;
    [SerializeField]
    private GameObject _player2Panel;

    private void Start()
    {
        _player1Panel.SetActive(false);
        _player2Panel.SetActive(false);

        // ControllerManager からの ControllerData を購読
        ControllerManager.Instance.OnControllerData.Subscribe(controllerData =>
        {
            Debug.Log($"Player: {controllerData.PlayerType}, Input: {controllerData.ActionType}");

            // 入力されたアクションのタイプに応じて処理を分岐
            switch (controllerData.ActionType)
            {
                case ActionType.Buttons:
                    HandleButtonInput(controllerData);
                    if(CheckPlayerPanelActive())
                    {
                        Debug.Log("FadeStart");
                        SceneManager.Instance.LoadScene(SceneName.Game);
                    }
                    break;
                // case ActionType.Sticks:
                //     HandleStickInput(controllerData.StickDirection);
                // break;
                default:
                    Debug.LogError("Invalid Action Type");
                    break;
            }

        }).AddTo(this);
    }

    private void HandleButtonInput(ControllerData controllerData)
    {
        if (controllerData.ButtonType == ButtonType.East)
        {
            ShowPlayerPanel(controllerData.PlayerType);
        }
    }

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
                break;
        }
    }

    private bool CheckPlayerPanelActive()
    {
        return _player1Panel.activeSelf && _player2Panel.activeSelf;
    }

}
