using UnityEngine;
using DG.Tweening;

/// <summary>
/// オブジェクトのサイズをアニメーションで拡大・縮小するクラス
/// </summary>
public class SizeAnimation : MonoBehaviour
{
    // アニメーションで拡大する倍率
    [SerializeField]
    private float _scale = 1.2f;

    // 現在のTweenを保持する変数
    private Tween _tween;

    /// <summary>
    /// オブジェクトが初期化されたときに呼び出されるメソッド
    /// </summary>
    void Start()
    {
        // 現在のローカルスケールを取得
        var mySize = transform.localScale;

        // 最大スケールを計算（xとyを拡大、zはそのまま）
        var maxSize = new Vector3(mySize.x * _scale, mySize.y * _scale, mySize.z);

        // スケールアニメーションを設定
        // 最大スケールまで1秒かけて拡大し、無限ループで往復する
        _tween = transform.DOScale(maxSize, 1.0f)
                          .SetLoops(-1, LoopType.Yoyo)
                          .SetEase(Ease.Linear);
    }

    /// <summary>
    /// オブジェクトが破棄されるときに呼び出されるメソッド
    /// </summary>
    private void OnDestroy()
    {
        // Tweenを停止し、リソースを解放
        if (_tween != null && _tween.IsActive())
        {
            _tween.Kill();
        }
    }
}
