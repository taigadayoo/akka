using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// フェードインおよびフェードアウトのアニメーションを制御するクラス
/// </summary>
public class FadeAnimation : MonoBehaviour
{
    // フェードアニメーションにかかる時間（秒）
    [SerializeField]
    private float _fadeTime = 1.0f;

    // フェード対象の CanvasGroup コンポーネント
    [SerializeField]
    private CanvasGroup _image;

    // フェード中かどうかを示すフラグ
    private bool _isFading = false;

    /// <summary>
    /// 現在フェード中であるかどうかを取得します
    /// </summary>
    public bool IsFading => _isFading;

    /// <summary>
    /// フェードイン（透明から不透明）を非同期で実行します
    /// </summary>
    /// <returns>フェードインが完了するまでの UniTask</returns>
    public async UniTask FadeIn()
    {
        _isFading = true;
        try
        {
            // CanvasGroup の alpha を 0.0f から 1.0f に線形で変化させる
            await _image.DOFade(0.0f, _fadeTime)
                        .SetEase(Ease.Linear)
                        .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
        }
        catch (OperationCanceledException)
        {
            Debug.Log("FadeIn Canceled");
        }
        _isFading = false;
    }

    /// <summary>
    /// フェードアウト（不透明から透明）を非同期で実行します
    /// </summary>
    /// <returns>フェードアウトが完了するまでの UniTask</returns>
    public async UniTask FadeOut()
    {
        _isFading = true;
        try
        {
            // CanvasGroup の alpha を 1.0f から 0.0f に線形で変化させる
            await _image.DOFade(1.0f, _fadeTime)
                        .SetEase(Ease.Linear)
                        .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
        }
        catch (OperationCanceledException)
        {
            Debug.Log("FadeOut Canceled");
        }
        _isFading = false;
    }
}
