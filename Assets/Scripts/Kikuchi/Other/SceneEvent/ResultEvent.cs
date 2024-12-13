using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using Unity.VisualScripting;

public class ResultEvent : MonoBehaviour
{
    private bool _isClear = false;
    private float _clearTime = 0.0f;
    private int _missCount = 0;


    [SerializeField]
    private List<float> _clearTimeLimit = new List<float>();

    private List<ResultSheetData> _resultSpriteCollection = new List<ResultSheetData>();
    private ResultSheetData _currentResultSheetData = null;

    private Tween _tween = null;

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
        return _currentResultSheetData.PL1OkImage.activeSelf && _currentResultSheetData.PL2OkImage.activeSelf;
    }

    private void ShowPlayerPanel(PlayerType playerType)
    {
        switch (playerType)
        {
            case PlayerType.Player1:
                _currentResultSheetData.PL1OkImage.SetActive(true);
                break;
            case PlayerType.Player2:
                _currentResultSheetData.PL2OkImage.SetActive(true);
                break;
            default:
                Debug.LogWarning($"Unsupported PlayerType: {playerType}");
                break;
        }
    }

    private void DisablePlayerPanel()
    {
        _currentResultSheetData.PL1OkImage.SetActive(false);
        _currentResultSheetData.PL2OkImage.SetActive(false);
    }



    private void GetScore()
    {
        _isClear = PlayerPrefs.GetInt("IsClear") == 0;
        _clearTime = PlayerPrefs.GetFloat("ClearTime");
        _missCount = PlayerPrefs.GetInt("MissCount");
    }

    private void GetRank()
    {
        if(_isClear)
        {
            for(int i = 0; i < _clearTimeLimit.Count; i++)
            {
                if(_clearTime < _clearTimeLimit[i])
                {
                    _currentResultSheetData = _resultSpriteCollection[i];
                    break;
                }
            }
        }
        else
        {
            _currentResultSheetData = _resultSpriteCollection[_resultSpriteCollection.Count - 1];
        }
    }

    private void SetText()
    {
        GetScore();

        _currentResultSheetData.ClearText.text = _isClear ? "失敗..." : "成功！";
        _currentResultSheetData.TimeText.text =　ConvertToTime(_clearTime);
        _currentResultSheetData.MissText.text = _missCount.ToString() + "回";
    }

    private string ConvertToTime(float time)
    {
        int minute = (int)time / 60;
        int second = (int)time % 60;
        return minute.ToString("00") + ":" + second.ToString("00") + "秒";
    }

    private void PanelTween()
    {
        RectTransform rect = _currentResultSheetData.gameObject.GetComponent<RectTransform>();
        _tween = rect.DOMoveY(0, 1.0f).SetEase(Ease.OutBounce).OnComplete(() => _tween = null);
    }


}
