using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;


public class PlayerControllerObserver : MonoBehaviour
{
    private PlayerInput _playerInput;

    private Subject<ControllerData> _controllerDataSubject = new Subject<ControllerData>();
    public IObservable<ControllerData> OnControllerData => _controllerDataSubject;

    private float ControllerStickThreshold => ControllerManager.Instance.ControllerStickThreshold;

    private void OnEnable()
    {
        _playerInput = GetComponent<PlayerInput>();
        transform.parent = ControllerManager.Instance.gameObject.transform;
        _playerInput.onActionTriggered += OnButtonActionTriggered;
        EnableActionMap();
        ControllerManager.Instance.EnablePlayerControllerEventHandle(this);
    }
    public void OnDisable()
    {
        _playerInput.onActionTriggered -= OnButtonActionTriggered;
        DisableActionMap();
    }

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

    private void EnableActionMap()
    {
        var buttonMap = _playerInput.actions.FindActionMap("Buttons", true);
        buttonMap.Enable();

        var stickMap = _playerInput.actions.FindActionMap("Sticks", true);
        stickMap.Enable();
    }

    private void DisableActionMap()
    {
        var buttonMap = _playerInput.actions.FindActionMap("Buttons", true);
        buttonMap.Disable();

        var stickMap = _playerInput.actions.FindActionMap("Sticks", true);
        stickMap.Disable();
    }

}
