using System;
using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;

public class ControllerManager : SingletonMonoBehaviour<ControllerManager>
{
    protected override bool DontDestroyOnLoad => true;
    [SerializeField]
    private GameObject _playerControllerPrefab;
    [SerializeField]
    private float _controllerStickThreshold = 0.5f;
    public float ControllerStickThreshold => _controllerStickThreshold;

    private PlayerInputManager _playerInputManager;
    private Subject<ControllerData> _controllerDataSubject = new Subject<ControllerData>();
    public IObservable<ControllerData> OnControllerData => _controllerDataSubject;
    protected override void Awake()
    {
        base.Awake();
        _playerInputManager = GetComponent<PlayerInputManager>();
    }

    private void OnEnable()
    {
        _playerInputManager.onPlayerJoined += OnPlayerJoined;
        _playerInputManager.onPlayerLeft += OnPlayerLeft;
    }

    private void OnDisable()
    {
        _playerInputManager.onPlayerJoined -= OnPlayerJoined;
        _playerInputManager.onPlayerLeft -= OnPlayerLeft;
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("Player Joined");
    }

    private void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.Log("Player Left");
    }

    public void EnablePlayerControllerEventHandle(PlayerControllerObserver playerControllerObserver)
    {
        playerControllerObserver.OnControllerData.Subscribe(controllerData =>
        {
            Debug.Log($"Player: {controllerData.PlayerType}, Input: {controllerData.ActionType}");
            switch (controllerData.ActionType)
            {
                case ActionType.Buttons:
                    Debug.Log($"Button: {controllerData.ButtonType}");
                    break;
                case ActionType.Sticks:
                    Debug.Log($"Stick Direction: {controllerData.StickDirection}");
                    break;
                default:
                    Debug.LogError("Invalid Action Type");
                    break;
            }
            _controllerDataSubject.OnNext(controllerData);
        }).AddTo(playerControllerObserver);
    }

}