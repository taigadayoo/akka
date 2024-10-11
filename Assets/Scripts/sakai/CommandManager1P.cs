using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
public class CommandManager1P : MonoBehaviour
{
    public SpriteRenderer FirstImage;
    public SpriteRenderer SecondImage;
    public SpriteRenderer ThirdImage;
    public Slider LeftHP;
    public Slider RightHP;
    public Sprite ASprite, BSprite, XSprite, YSprite, RightSprite, LeftSprite, UpSprite, DownSprite;

    private List<string> _commands;
    private List<string> _currentSequence;
    private int _currentIndex;
    private bool _canChangeCommands;

    private Dictionary<string, Sprite> _commandSprites;

    private StickDirection _previousStickDirection = StickDirection.None;
    private bool _isStickStrengthReset = true;
    private float _lastResetTime;
    private float _resetCooldown = 0.2f;
    private float _stickInputThreshold = 0.6f;
    private float _stickReleaseThreshold = 0.3f;

    private float _commandTime = 0f;
    private float _commandTimeout = 3f; // 3秒
    private void Start()
    {
        _commands = new List<string> { "A", "B", "X", "Y", "Right", "Left", "Up", "Down" };

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

        StartCoroutine(GenerateCommands());
        ControllerManager.Instance.OnControllerData.Subscribe(OnControllerDataReceived).AddTo(this);
    }

    private IEnumerator GenerateCommands()
    {
        while (true)
        {
            _currentSequence = new List<string>();
            _currentIndex = 0;
            _canChangeCommands = true;

            FirstImage.gameObject.SetActive(true);
            SecondImage.gameObject.SetActive(true);
            ThirdImage.gameObject.SetActive(true);

            for (int i = 0; i < 3; i++)
            {
                string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                _currentSequence.Add(randomCommand);

                Sprite commandSprite = _commandSprites[randomCommand];
                if (i == 0)
                    FirstImage.sprite = commandSprite;
                else if (i == 1)
                    SecondImage.sprite = commandSprite;
                else if (i == 2)
                    ThirdImage.sprite = commandSprite;
            }

            _commandTime = 0f;

            while (_currentIndex < _currentSequence.Count)
            {
                _commandTime += Time.deltaTime; // 経過時間を加算

                if (_commandTime >= _commandTimeout) // 3秒経過した場合
                {
                    ResetCommands();
                    yield break; // コルーチンを終了
                }

                yield return null;
            }


            FirstImage.gameObject.SetActive(false);
            SecondImage.gameObject.SetActive(false);
            ThirdImage.gameObject.SetActive(false);

            LeftHP.value -= 2.5f;
            RightHP.value -=2.5f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ResetCommands()
    {
        if (Time.time - _lastResetTime < _resetCooldown)
            return;
        _commandTime = 0f; // 経過時間リセット
        StopAllCoroutines();
        StartCoroutine(GenerateCommands());
        _lastResetTime = Time.time;
    }

    private void OnControllerDataReceived(ControllerData controllerData)
    {
        if (controllerData.PlayerType != PlayerType.Player1)
            return; // プレイヤー1以外の操作は無視

        if (_currentIndex >= _currentSequence.Count)
            return;

        string expectedCommand = _currentSequence[_currentIndex];

        if (controllerData.ActionType == ActionType.Sticks)
        {
            StickDirection currentDirection = controllerData.StickDirection ?? StickDirection.None;
            float stickStrength = controllerData.StickStrength;

            if (_previousStickDirection == currentDirection)
            {
                if (stickStrength <= _stickReleaseThreshold)
                {
                    _isStickStrengthReset = true;
                }
            }

            if (currentDirection != StickDirection.None && (_isStickStrengthReset && stickStrength >= _stickInputThreshold))
            {
                if (IsCorrectStickCommand(expectedCommand, currentDirection))
                {
                    HandleCommandInput();
                    _isStickStrengthReset = false;
                    _commandTime = 0f; // コマンドが正しく入力された場合、経過時間をリセット
                }
                else
                {
                    ResetCommands();
                }
            }

            _previousStickDirection = currentDirection;
        }
        else if (controllerData.ActionType == ActionType.Buttons)
        {
            if (IsCorrectCommand(controllerData, expectedCommand))
            {
                HandleCommandInput();
            }
            else
            {
                ResetCommands();
            }
        }
    }

    private bool IsCorrectStickCommand(string expectedCommand, StickDirection currentDirection)
    {
        float angle = GetStickDirectionAngle(currentDirection);

        switch (expectedCommand)
        {
            case "Right":
                return IsInAngleRange(angle, 0); // 0度
            case "Left":
                return IsInAngleRange(angle, 180); // 180度
            case "Up":
                return IsInAngleRange(angle, 90); // 90度
            case "Down":
                return IsInAngleRange(angle, 270); // 270度
            default:
                return false;
        }
    }

    private float GetStickDirectionAngle(StickDirection direction)
    {
        switch (direction)
        {
            case StickDirection.Right:
                return 0f;
            case StickDirection.Up:
                return 90f;
            case StickDirection.Left:
                return 180f;
            case StickDirection.Down:
                return 270f;
            // 他の方向が必要なら追加
            default:
                return -1f; // 無効な方向
        }
    }

    private bool IsInAngleRange(float inputAngle, float targetAngle)
    {
        float lowerBound = targetAngle - 45;
        float upperBound = targetAngle + 45;

        // 角度のラップアラウンド処理
        if (lowerBound < 0)
            lowerBound += 360;
        if (upperBound >= 360)
            upperBound -= 360;

        if (lowerBound < upperBound)
        {
            return inputAngle >= lowerBound && inputAngle <= upperBound;
        }
        else // 角度が360度を跨いでいる場合
        {
            return inputAngle >= lowerBound || inputAngle <= upperBound;
        }
    }

    private void HandleCommandInput()
    {
        if (_currentIndex == 0)
            FirstImage.gameObject.SetActive(false);
        else if (_currentIndex == 1)
            SecondImage.gameObject.SetActive(false);
        else if (_currentIndex == 2)
            ThirdImage.gameObject.SetActive(false);

        _currentIndex++;

        if (_currentIndex >= _currentSequence.Count)
        {
            Debug.Log("1P Success! All commands entered correctly.");
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
                default:
                    return false;
            }
        }

        return false;
    }
}