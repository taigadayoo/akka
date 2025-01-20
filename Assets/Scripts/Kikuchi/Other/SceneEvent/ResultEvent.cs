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
    private bool _isClear = false;          // ゲームクリアフラグ
    private float _clearTime = 0.0f;       // クリアタイム
    private int _missCount = 0;            // ミス回数

    [SerializeField]
    private List<float> _clearTimeLimit = new List<float>();  // クリアタイム制限のリスト

    [SerializeField]
    private List<ResultSheetData> _resultSpriteCollection = new List<ResultSheetData>();  // リザルトシートデータのリスト
    private ResultSheetData _currentResultSheetData = null;  // 現在のリザルトシートデータ

    private Tween _tween = null;  // DOTweenのアニメーショントゥイーン


    void Start()
    {
        // スコアを取得する
        GetScore();

        // ランクを取得する
        GetRank();

        // プレイヤーパネルを非表示にする
        DisablePlayerPanel();

        // テキストを設定する
        SetText();

        // パネルをアニメーションさせる
        PanelTween();

        // リザルトイベントハンドラを設定する
        ResultEventHandler();
    }

    // リザルトイベントハンドラ
    private void ResultEventHandler()
    {
        // コントローラーの入力イベントを購読する
        ControllerManager.Instance.OnControllerData
            .Where(_ => !SceneManager.Instance.IsFading)  // シーン遷移中でないことを確認
            .Where(_ => _tween == null)  // アニメーション中でないことを確認
            .Subscribe(controllerData =>
            {
                Debug.Log($"Player: {controllerData.PlayerType}, Input: {controllerData.ActionType}");

                // 入力タイプに応じて処理を分岐
                switch (controllerData.ActionType)
                {
                    case ActionType.Buttons:
                        // ボタン入力の場合
                        HandleButtonInput(controllerData);

                        // 全プレイヤーのパネルが表示されていたらタイトルシーンに遷移する
                        if (CheckPlayerPanelActive())
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

    // ボタン入力処理
    private void HandleButtonInput(ControllerData controllerData)
    {
        // Eastボタンが押されたら対応するプレイヤーのパネルを表示する
        if (controllerData.ButtonType == ButtonType.East)
        {
            ShowPlayerPanel(controllerData.PlayerType);
        }
    }

    // 全プレイヤーのパネルが表示されているかチェックする
    private bool CheckPlayerPanelActive()
    {
        return _currentResultSheetData.PL1OkImage.activeSelf && _currentResultSheetData.PL2OkImage.activeSelf;
    }

    // プレイヤーのパネルを表示する
    private void ShowPlayerPanel(PlayerType playerType)
    {
        // プレイヤータイプに応じて対応するパネルを表示する
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

    // プレイヤーパネルを非表示にする
    private void DisablePlayerPanel()
    {
        _currentResultSheetData.PL1OkImage.SetActive(false);
        _currentResultSheetData.PL2OkImage.SetActive(false);
    }

    // スコアを取得する
    private void GetScore()
    {
        _isClear = PlayerPrefs.GetInt("IsClear") == 0; // 0: クリア, 1: 失敗
        _clearTime = PlayerPrefs.GetFloat("ClearTime");
        _missCount = PlayerPrefs.GetInt("MissCount");
    }

    // ランクを取得する
    private void GetRank()
    {
        // クリアしている場合
        if (_isClear)
        {
            // クリアタイム制限リストをループして、クリアタイムが制限時間内であれば対応するリザルトシートデータを設定する
            for (int i = 0; i < _clearTimeLimit.Count; i++)
            {
                if (_clearTime < _clearTimeLimit[i])
                {
                    _currentResultSheetData = _resultSpriteCollection[i];
                    break;
                }
            }
        }
        // 失敗している場合
        else
        {
            // 失敗用のリザルトシートデータを設定する
            _currentResultSheetData = _resultSpriteCollection[_resultSpriteCollection.Count - 1];
        }
    }

    // テキストを設定する
    private void SetText()
    {
        // クリアテキスト、タイムテキスト、ミステキストを設定する
        _currentResultSheetData.ClearText.text = _isClear ? "成功！" : "失敗...";
        _currentResultSheetData.TimeText.text = ConvertToTime(_clearTime);
        _currentResultSheetData.MissText.text = _missCount.ToString() + "回";
    }

    // 時間を文字列に変換する
    private string ConvertToTime(float time)
    {
        int minute = (int)time / 60;
        int second = (int)time % 60;
        return minute.ToString("00") + ":" + second.ToString("00") + "秒";
    }

    // パネルをアニメーションさせる
    private void PanelTween()
    {
        // リザルトパネルを下から上に移動させるアニメーション
        RectTransform rect = _currentResultSheetData.gameObject.GetComponent<RectTransform>();
        _tween = rect.DOMoveY(0, 1.0f)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                _tween = null;  // アニメーション完了後にトゥイーンをnullにする
                SetText();  // アニメーション後にテキストを更新
            });
    }
}