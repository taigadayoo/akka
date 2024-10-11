using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections;
using System.Collections.Generic;

public class CommandManager : MonoBehaviour
{
    public SpriteRenderer FirstImage; // 1つ目のコマンド画像を表示するSpriteRenderer
    public SpriteRenderer SecondImage; // 2つ目のコマンド画像を表示するSpriteRenderer
    public SpriteRenderer ThirdImage; // 3つ目のコマンド画像を表示するSpriteRenderer

    public Sprite ASprite, BSprite, XSprite, YSprite, RightSprite, LeftSprite, UpSprite, DownSprite; // 各コマンドの画像

    private List<string> _commands; // コマンドリスト
    private List<string> _currentSequence; // 現在のシーケンス
    private int _currentIndex; // 現在のインデックス
    private bool _canChangeCommands; // コマンドを変えられるかどうかのフラグ

    private Dictionary<string, Sprite> _commandSprites; // コマンドとスプライトのマッピング

    private string _lastStickDirection; // 最後に認識されたスティックの方向

    void Start()
    {
        _commands = new List<string> { "A", "B", "X", "Y", "Right", "Left", "Up", "Down" };

        // コマンドとスプライトの対応をマッピング
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

        StartCoroutine(GenerateCommands()); // コマンドを生成
        ControllerManager.Instance.OnControllerData.Subscribe(OnControllerDataReceived).AddTo(this);
    }

    // コマンドを生成して設定
    private IEnumerator GenerateCommands()
    {
        while (true) // 繰り返しコマンドを生成
        {
            // コマンドをリセット
            _currentSequence = new List<string>();
            _currentIndex = 0;
            _canChangeCommands = true; // コマンドを変えられるようにする

            // すべてのSpriteRendererを表示
            FirstImage.gameObject.SetActive(true);
            SecondImage.gameObject.SetActive(true);
            ThirdImage.gameObject.SetActive(true);

            for (int i = 0; i < 3; i++)
            {
                string randomCommand = _commands[Random.Range(0, _commands.Count)];
                _currentSequence.Add(randomCommand);

                // SpriteRendererに画像を設定
                Sprite commandSprite = _commandSprites[randomCommand];
                if (i == 0)
                    FirstImage.sprite = commandSprite; // 1つ目のSpriteRendererに設定
                else if (i == 1)
                    SecondImage.sprite = commandSprite; // 2つ目のSpriteRendererに設定
                else if (i == 2)
                    ThirdImage.sprite = commandSprite; // 3つ目のSpriteRendererに設定
            }

            // すべてのコマンドが入力されるまで待機
            while (_currentIndex < _currentSequence.Count)
            {
                yield return null; // 次のフレームまで待つ
            }

            // すべてのコマンドが入力された場合、SpriteRendererを非表示にする
            FirstImage.gameObject.SetActive(false);
            SecondImage.gameObject.SetActive(false);
            ThirdImage.gameObject.SetActive(false);

            // 1秒待機してから新しいコマンドを生成
            yield return new WaitForSeconds(0.1f);
        }
    }

    // コマンドをリセットする
    public void ResetCommands()
    {
        StopAllCoroutines(); // 現在のコルーチンを停止
        StartCoroutine(GenerateCommands()); // 新しいコマンドを生成
    }

    // コントローラーからのデータを受け取る
    private void OnControllerDataReceived(ControllerData controllerData)
    {
        if (_currentIndex >= _currentSequence.Count)
            return;

        string expectedCommand = _currentSequence[_currentIndex];

        // スティックコマンドの処理
        if (controllerData.ActionType == ActionType.Sticks)
        {
            // スティックの方向が変わっているか確認
            if (controllerData.StickDirection.ToString() != _lastStickDirection)
            {
                if (IsCorrectCommand(controllerData, expectedCommand))
                {
                    // 正しいコマンドが入力された場合、対応するSpriteRendererを非表示にする
                    HandleCommandInput();
                }
            }

            // スティックが戻った場合、方向をリセット
            if (controllerData.StickDirection == StickDirection.None) // スティックが戻ったと仮定
            {
                _lastStickDirection = null; // 方向をリセット
            }
            else
            {
                // 現在の方向を更新
                _lastStickDirection = controllerData.StickDirection.ToString();
            }
        }
        // ボタンコマンドの処理
        else if (controllerData.ActionType == ActionType.Buttons)
        {
            if (IsCorrectCommand(controllerData, expectedCommand))
            {
                HandleCommandInput(); // ボタンコマンドも処理
            }
        }
    }

    // コマンド入力を処理する
    private void HandleCommandInput()
    {
        // 正しいコマンドが入力された場合、対応するSpriteRendererを非表示にする
        if (_currentIndex == 0)
            FirstImage.gameObject.SetActive(false);
        else if (_currentIndex == 1)
            SecondImage.gameObject.SetActive(false);
        else if (_currentIndex == 2)
            ThirdImage.gameObject.SetActive(false);

        _currentIndex++; // 次のコマンドに進む

        // すべてのコマンドが入力されたら、新しいコマンドを生成
        if (_currentIndex >= _currentSequence.Count)
        {
            Debug.Log("Success! All commands entered correctly.");
        }
    }

    // 入力が正しいかどうかを確認
    private bool IsCorrectCommand(ControllerData controllerData, string expectedCommand)
    {
        // 期待されるコマンドに基づいて、正しいボタン入力を確認
        if (controllerData.ActionType == ActionType.Buttons)
        {
            switch (expectedCommand)
            {
                case "A":
                    return controllerData.ButtonType == ButtonType.South; // ButtonTypeは実際の型名に合わせて変更
                case "B":
                    return controllerData.ButtonType == ButtonType.East;
                case "X":
                    return controllerData.ButtonType == ButtonType.West;
                case "Y":
                    return controllerData.ButtonType == ButtonType.North;
                default:
                    return false;
            }
        }
        else if (controllerData.ActionType == ActionType.Sticks)
        {
            switch (expectedCommand)
            {
                case "Right":
                    return controllerData.StickDirection == StickDirection.Right; // Directionは実際の型名に合わせて変更
                case "Left":
                    return controllerData.StickDirection == StickDirection.Left;
                case "Up":
                    return controllerData.StickDirection == StickDirection.Up;
                case "Down":
                    return controllerData.StickDirection == StickDirection.Down;
                default:
                    return false;
            }
        }

        return false; // それ以外は不正なコマンド
    }
}