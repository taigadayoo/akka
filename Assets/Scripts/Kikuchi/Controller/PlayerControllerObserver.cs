using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

/// <summary>
/// プレイヤーのコントローラー入力を監視し、入力データを発行するクラス
/// </summary>
public class PlayerControllerObserver : MonoBehaviour
{
    // プレイヤーの入力を管理する PlayerInput コンポーネント
    private PlayerInput _playerInput;

    // ControllerData を発行する Subject
    private Subject<ControllerData> _controllerDataSubject = new Subject<ControllerData>();

    // ControllerData の購読可能な Observable
    public IObservable<ControllerData> OnControllerData => _controllerDataSubject;

    // スティック入力の閾値
    private float ControllerStickThreshold => ControllerManager.Instance.ControllerStickThreshold;

    /// <summary>
    /// オブジェクトが有効になったときに呼び出されるメソッド
    /// </summary>
    private void OnEnable()
    {
        _playerInput = GetComponent<PlayerInput>();
        transform.parent = ControllerManager.Instance.gameObject.transform;
        _playerInput.onActionTriggered += OnButtonActionTriggered;
        EnableActionMap();
        ControllerManager.Instance.EnablePlayerControllerEventHandle(this);
    }

    /// <summary>
    /// オブジェクトが無効になったときに呼び出されるメソッド
    /// </summary>
    public void OnDisable()
    {
        _playerInput.onActionTriggered -= OnButtonActionTriggered;
        DisableActionMap();
    }

    /// <summary>
    /// 入力アクションがトリガーされたときに呼び出されるコールバックメソッド
    /// </summary>
    /// <param name="context">入力アクションのコンテキスト</param>
    private void OnButtonActionTriggered(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Debug.Log($"Action Triggered: {context.action.actionMap.name}");
            var playerType = _playerInput.playerIndex == 0 ? PlayerType.Player1 : PlayerType.Player2;
            ControllerData controllerData;
            if (context.action.actionMap.name == "Buttons")
            {
                var buttonType = GetButtonType(context.action.name);
                controllerData = new ControllerData(playerType, buttonType);
            }
            else if (context.action.actionMap.name == "Sticks")
            {
                var stickDirection = GetStickDirection(context.ReadValue<Vector2>());
                controllerData = new ControllerData(playerType, stickDirection);
            }
            else
            {
                Debug.LogError("Invalid Action Map");
                return;
            }
            _controllerDataSubject.OnNext(controllerData);
        }
    }

    /// <summary>
    /// アクション名から ButtonType を取得します
    /// </summary>
    /// <param name="actionName">アクションの名前</param>
    /// <returns>対応する ButtonType</returns>
    /// <exception cref="ArgumentOutOfRangeException">無効なアクション名が渡された場合にスローされます</exception>
    private ButtonType GetButtonType(string actionName)
    {
        return actionName switch
        {
            "South" => ButtonType.South,
            "East" => ButtonType.East,
            "West" => ButtonType.West,
            "North" => ButtonType.North,
            "L1" => ButtonType.L1,
            "R1" => ButtonType.R1,
            "L2" => ButtonType.L2,
            "R2" => ButtonType.R2,
            "Up" => ButtonType.Up,
            "Down" => ButtonType.Down,
            "Left" => ButtonType.Left,
            "Right" => ButtonType.Right,
            "Option" => ButtonType.Option,
            "Select" => ButtonType.Select,
            _ => throw new ArgumentOutOfRangeException(nameof(actionName), actionName, null),
        };
    }

    /// <summary>
    /// スティックの入力値から StickDirection を取得します
    /// </summary>
    /// <param name="value">スティックの入力値（Vector2）</param>
    /// <returns>対応する StickDirection</returns>
    private StickDirection GetStickDirection(Vector2 value)
    {
        if (value.y > ControllerStickThreshold)
        {
            return StickDirection.Up;
        }
        if (value.y < -ControllerStickThreshold)
        {
            return StickDirection.Down;
        }
        if (value.x < -ControllerStickThreshold)
        {
            return StickDirection.Left;
        }
        if (value.x > ControllerStickThreshold)
        {
            return StickDirection.Right;
        }
        return StickDirection.None;
    }

    /// <summary>
    /// ボタンおよびスティックのアクションマップを有効にします
    /// </summary>
    private void EnableActionMap()
    {
        var buttonMap = _playerInput.actions.FindActionMap("Buttons", true);
        buttonMap.Enable();

        var stickMap = _playerInput.actions.FindActionMap("Sticks", true);
        stickMap.Enable();
    }

    /// <summary>
    /// ボタンおよびスティックのアクションマップを無効にします
    /// </summary>
    private void DisableActionMap()
    {
        var buttonMap = _playerInput.actions.FindActionMap("Buttons", true);
        buttonMap.Disable();

        var stickMap = _playerInput.actions.FindActionMap("Sticks", true);
        stickMap.Disable();
    }
}
