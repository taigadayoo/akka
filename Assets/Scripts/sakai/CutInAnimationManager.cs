using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CutInAnimationManager : MonoBehaviour
{
    // パネルやキャラクターなどのUI要素を格納するためのpublic変数
    public GameObject CutInpanel;  // カットインパネル
    public RectTransform CutBack1; // カットバック1
    public RectTransform CutBack2; // カットバック2
    public RectTransform Player1;  // プレイヤー1
    public RectTransform Player2;  // プレイヤー2
    public RectTransform Akka1;   // アッカ1
    public RectTransform Akka2;   // アッカ2
    public GameObject Ink1;       // インク1
    public GameObject Ink3;       // インク3
    public GameObject Ink2;       // インク2
    public GameObject Ink4;       // インク4
    public GameObject FirstCutInSet;  // 1stカットインセット
    public GameObject SecondCutInSet; // 2ndカットインセット

    // シーン内でのゲーム管理やコマンド管理を行うスクリプト
    [SerializeField]
    CommandManager1P _1P;  // プレイヤー1用コマンド管理
    [SerializeField]
    CommandManager2P _2P;  // プレイヤー2用コマンド管理
    [SerializeField]
    GameManager _gameManager;  // ゲーム管理

    // Start is called before the first frame update
    void Start()
    {
        // 初期化の処理が必要な場合に記述
    }

    // Update is called once per frame
    void Update()
    {
        // 毎フレーム更新が必要な場合に記述
    }

    // 最初のカットインを開始するメソッド
    public void StartFirstCutIn()
    {
        StartCoroutine(FirstCutIn());  // FirstCutInコルーチンを開始
    }

    // 2番目のカットインを開始するメソッド
    public void StartSecondCutIn()
    {
        StartCoroutine(SecondCutIn());  // SecondCutInコルーチンを開始
    }

    // 1番目のカットインのアニメーション処理
    public IEnumerator FirstCutIn()
    {
        CutInpanel.SetActive(true); // カットインパネルを表示

        yield return new WaitForSeconds(0.3f); // 0.3秒待機
        AudioManager.Instance.PlaySE("flash-of-light-4", 1f); // 音を再生
        CutBack1.DOAnchorPos(new Vector2(-28, 0), 0.5f).SetEase(Ease.OutQuad); // カットバック1をアニメーションで移動

        yield return new WaitForSeconds(0.5f); // 0.5秒待機

        Player1.DOAnchorPos(new Vector2(-468, -164), 0.5f).SetEase(Ease.OutQuad); // プレイヤー1を移動

        yield return new WaitForSeconds(0.5f); // 0.5秒待機

        Akka1.DOAnchorPos(new Vector2(538, 109), 0.5f).SetEase(Ease.OutQuad); // アッカ1を移動

        yield return new WaitForSeconds(0.5f); // 0.5秒待機

        // インクを表示してアニメーション
        Ink1.SetActive(true);
        AudioManager.Instance.PlaySE("魚を釣り上げる", 1f); // 音を再生
        Ink1.transform.DOScale(new Vector3(0.2f, 0.2f, 1f), 0.2f).SetEase(Ease.OutBounce); // イージングを設定してインクをスケール

        yield return new WaitForSeconds(0.3f); // 0.3秒待機

        Ink2.SetActive(true); // インク2を表示
        AudioManager.Instance.PlaySE("魚を釣り上げる", 1f); // 音を再生
        Ink2.transform.DOScale(new Vector3(0.15f, 0.15f, 1f), 0.2f).SetEase(Ease.OutBounce); // イージングを設定してインクをスケール

        yield return new WaitForSeconds(0.8f); // 0.8秒待機

        // カットインセットを非表示にし、ゲーム管理を再開
        FirstCutInSet.SetActive(false);
        CutInpanel.SetActive(false);
        _gameManager.OnCutIn = false; // カットインフラグをオフ
        _1P.enabled = true; // プレイヤー1の操作を有効化
        _2P.enabled = true; // プレイヤー2の操作を有効化
        _gameManager.FirstSet.SetActive(true); // 最初のセットを表示
        _gameManager.SecondSet.SetActive(true); // 2番目のセットを表示
        _gameManager.ThardSet.SetActive(true); // 3番目のセットを表示
        _gameManager.SecondCommandTime = 0; // セカンドコマンドタイムをリセット
        _gameManager.TimerMix.SetActive(true); // タイマーを表示
    }

    // 2番目のカットインのアニメーション処理
    public IEnumerator SecondCutIn()
    {
        CutInpanel.SetActive(true); // カットインパネルを表示

        yield return new WaitForSeconds(0.3f); // 0.3秒待機
        AudioManager.Instance.PlaySE("flash-of-light-4", 1f); // 音を再生
        CutBack2.DOAnchorPos(new Vector2(-28, 0), 0.5f).SetEase(Ease.OutQuad); // カットバック2をアニメーションで移動

        yield return new WaitForSeconds(0.5f); // 0.5秒待機

        Player2.DOAnchorPos(new Vector2(587, -164), 0.5f).SetEase(Ease.OutQuad); // プレイヤー2を移動

        yield return new WaitForSeconds(0.5f); // 0.5秒待機

        Akka2.DOAnchorPos(new Vector2(-422, 86), 0.5f).SetEase(Ease.OutQuad); // アッカ2を移動

        yield return new WaitForSeconds(0.5f); // 0.5秒待機

        // インクを表示してアニメーション
        Ink3.SetActive(true);
        AudioManager.Instance.PlaySE("魚を釣り上げる", 1f); // 音を再生
        Ink3.transform.DOScale(new Vector3(0.2f, 0.2f, 1f), 0.2f).SetEase(Ease.OutBounce); // イージングを設定してインクをスケール

        yield return new WaitForSeconds(0.3f); // 0.3秒待機

        Ink4.SetActive(true); // インク4を表示
        AudioManager.Instance.PlaySE("魚を釣り上げる", 1f); // 音を再生
        Ink4.transform.DOScale(new Vector3(0.15f, 0.15f, 1f), 0.2f).SetEase(Ease.OutBounce); // イージングを設定してインクをスケール

        yield return new WaitForSeconds(0.8f); // 0.8秒待機

        // カットインセットを非表示にし、ゲーム管理を再開
        SecondCutInSet.SetActive(false);
        CutInpanel.SetActive(false);
        _gameManager.OnCutIn = false; // カットインフラグをオフ
        _1P.enabled = true; // プレイヤー1の操作を有効化
        _2P.enabled = true; // プレイヤー2の操作を有効化
        _gameManager.FirstSet.SetActive(true); // 最初のセットを表示
        _gameManager.SecondSet.SetActive(true); // 2番目のセットを表示
        _gameManager.ThardSet.SetActive(true); // 3番目のセットを表示
    }
}
