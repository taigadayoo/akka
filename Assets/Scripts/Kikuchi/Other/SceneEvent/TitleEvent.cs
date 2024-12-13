using UnityEngine;
using UniRx;


/// <summary>
/// タイトル画面のボタンイベントを処理するクラス
/// </summary>
public class TitleEvent : MonoBehaviour
{
    /// <summary>
    /// オブジェクトが初期化されたときに呼び出されるメソッド
    /// </summary>
    void Start()
    {
        AudioManager.Instance.PlayBGM("この訓練、方向性合ってます的なBGM", 0.5f);
        // ControllerManager からの ControllerData を購読
        ControllerManager.Instance.OnControllerData
        .Where(_ => !SceneManager.Instance.IsFading)
        .Subscribe(controllerData =>
        {
            Debug.Log($"Player: {controllerData.PlayerType}, Input: {controllerData.ActionType}");

            // 入力されたアクションのタイプに応じて処理を分岐
            switch (controllerData.ActionType)
            {
                case ActionType.Buttons:
                    HandleButtonInput(controllerData.ButtonType);
                    break;
                // case ActionType.Sticks:
                //     HandleStickInput(controllerData.StickDirection);
                // break;
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
    /// <param name="buttonType">押されたボタンのタイプ</param>
    private void HandleButtonInput(ButtonType? buttonType)
    {
        if(buttonType == ButtonType.East)
        {
            AudioManager.Instance.PlaySE("決定ボタンを押す20", 0.8f);
            AudioManager.Instance.StopBGM();
            Debug.Log("FadeStart");
            SceneManager.Instance.LoadScene(SceneName.Setting);
            // SampleSoundManager.Instance.StopBgm();
        }
    }

    // /// <summary>
    // /// スティック入力を処理します
    // /// </summary>
    // /// <param name="stickDirection">スティックの方向</param>
    // private void HandleStickInput(StickDirection? stickDirection)
    // {
    //     Debug.Log($"Stick Direction: {stickDirection}");
    // }
}
