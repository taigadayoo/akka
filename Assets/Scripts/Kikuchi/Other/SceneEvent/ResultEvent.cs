using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class ResultEvent : MonoBehaviour
{
    private bool _isClear = false;
    private float _clearTime = 0.0f;
    private int _missCount = 0;


    [SerializeField]
    private List<float> _clearTimeLimit = new List<float>();


    [SerializeField]
    private TextMeshProUGUI _clearText = null;
    [SerializeField]
    private TextMeshProUGUI _timeText = null;
    [SerializeField]
    private TextMeshProUGUI _missText = null;
    [SerializeField]
    private Image _rankImage = null;


    [SerializeField]
    private List<Sprite> _rankSprite = new List<Sprite>();

    [SerializeField]
    private GameObject _player1OKImage = null;
    [SerializeField]
    private GameObject _player2OKImage = null;

    private Tween _tween = null;


    //Debug用
    [SerializeField]
    private int _isDebug = 1;
    [SerializeField]
    private float _debugTime = 0.0f;
    [SerializeField]
    private int _debugMiss = 0;
    //


    // Start is called before the first frame update
    void Start()
    {
        SetText();
        DisablePlayerPanel();
        PanelTween();

        ResultEventHandler();

    }

    private void ResultEventHandler()
    {
        ControllerManager.Instance.OnControllerData
        .Where(_ => !SceneManager.Instance.IsFading)
        .Where(_ => _tween == null)
        .Subscribe(controllerData =>
        {
            Debug.Log($"Player: {controllerData.PlayerType}, Input: {controllerData.ActionType}");

            switch (controllerData.ActionType)
            {
                case ActionType.Buttons:
                    HandleButtonInput(controllerData);
                    if(CheckPlayerPanelActive())
                    {
                        SceneManager.Instance.LoadScene(SceneName.Title);
                    }
                    break;
                default:
                    Debug.LogError("Invalid Action Type");
                    break;
            }

        })
        .AddTo(this);
    }

    private void HandleButtonInput(ControllerData controllerData)
    {
        if (controllerData.ButtonType == ButtonType.East)
        {
            ShowPlayerPanel(controllerData.PlayerType);
        }
    }

    private bool CheckPlayerPanelActive()
    {
        return _player1OKImage.activeSelf && _player2OKImage.activeSelf;
    }

    private void ShowPlayerPanel(PlayerType playerType)
    {
        switch (playerType)
        {
            case PlayerType.Player1:
                _player1OKImage.SetActive(true);
                break;
            case PlayerType.Player2:
                _player2OKImage.SetActive(true);
                break;
            default:
                Debug.LogWarning($"Unsupported PlayerType: {playerType}");
                break;
        }
    }

    private void DisablePlayerPanel()
    {
        _player1OKImage.SetActive(false);
        _player2OKImage.SetActive(false);
    }


    private void GetScore()
    {
        _isClear = PlayerPrefs.GetInt("IsClear", _isDebug) == 1;
        _clearTime = PlayerPrefs.GetFloat("ClearTime", _debugTime);
        _missCount = PlayerPrefs.GetInt("MissCount", _debugMiss);
    }

    private void SetText()
    {
        GetScore();

        _clearText.text = _isClear ? "成功！" : "失敗...";
        _timeText.text =　_clearTime.ToString("F0");
        _missText.text = _missCount.ToString();

        for (int i = 0; i < _clearTimeLimit.Count; i++)
        {
            if (_clearTime <= _clearTimeLimit[i])
            {
                _rankImage.sprite = _rankSprite[i];
                break;
            }
        }
    }

    private void PanelTween()
    {
        RectTransform rect = _rankImage.GetComponent<RectTransform>();
        _tween = rect.DOMoveY(0, 1.0f).SetEase(Ease.OutBounce).OnComplete(() => _tween = null);
    }


}
