using UnityEngine;
using DG.Tweening;

public class CircularMovementWithBackground : MonoBehaviour
{
    public GameObject[] Objects;    // 動かすオブジェクトの配列（7つのオブジェクト）
    public Transform CircleBackground; // 背景の円の位置（円の中心）
    public float Radius = 3.05f;    // 円の半径
    public float Duration = 5f;     // 一周するのにかかる時間
    public bool Loop = true;        // 繰り返すかどうか
    [SerializeField]
    GameManager _gameManager;

    private Vector3[] initialPositions;  // 各オブジェクトの初期位置
    private Tween[] objectTweens;  // 各オブジェクトのTweenを保持する配列

    void Start()
    {
        initialPositions = new Vector3[Objects.Length];
        objectTweens = new Tween[Objects.Length];  // Tween配列の初期化
        PositionObjectsOnCircle();  // オブジェクトを円周上に配置
    }

    void Update()
    {
        if (_gameManager.StartThard)
        {
            StartCircularMovement();  // 円運動を開始
            _gameManager.StartThard = false;  // 一度だけ呼ばれるようにフラグをリセット
        }
    }

    // オブジェクトを背景の円周上に120度から330度の範囲で均等に配置
    void PositionObjectsOnCircle()
    {
        int objectCount = Objects.Length;

        // 配置する範囲を設定（120度から330度）
        float startAngle = 2 * Mathf.PI / 3;  // 120度（2π/3ラジアン）
        float endAngle = 11 * Mathf.PI / 6;  // 330度（11π/6ラジアン）

        // 各オブジェクトの配置角度のステップを計算
        float angleStep = (endAngle - startAngle) / (objectCount - 1);  // 120度から330度の範囲を等間隔に分割

        for (int i = 0; i < objectCount; i++)
        {
            // 配置する角度を計算
            float angle = startAngle + i * angleStep;

            // 背景の円の中心位置を基準にオブジェクトの初期位置を計算
            initialPositions[i] = new Vector3(Mathf.Cos(angle) * Radius, Mathf.Sin(angle) * Radius, 0) + CircleBackground.position;

            // 指定した角度に基づきオブジェクトの位置を設定
            Objects[i].transform.position = initialPositions[i];
        }
    }

    // 各オブジェクトを円周上に沿って回転させる
    void StartCircularMovement()
    {
        int objectCount = Objects.Length;

        // 各オブジェクトが円運動を開始
        for (int i = 0; i < objectCount; i++)
        {
            // 現在のオブジェクトの位置から円運動を開始する
            Vector3[] path = CreateCircularPath(initialPositions[i]);

            // DOPathを使って円形の経路に沿って動かす
            objectTweens[i] = Objects[i].transform.DOPath(path, Duration, PathType.CatmullRom)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);  // 無限ループ
        }
    }

    // すべてのオブジェクトが背景の円周上で動くようにパスを作成
    Vector3[] CreateCircularPath(Vector3 startPosition)
    {
        int points = 36;  // 円周上のポイント数（増やすとより滑らかになる）
        Vector3[] path = new Vector3[points];

        // オブジェクトが初期位置からその場で円運動を開始する
        // 初期位置の角度を計算する
        float startAngle = Mathf.Atan2(startPosition.y - CircleBackground.position.y, startPosition.x - CircleBackground.position.x);

        // 円周上の位置を計算
        for (int i = 0; i < points; i++)
        {
            // 時計回りにするため角度を反時計回りのマイナスにする
            float angle = startAngle - 2 * Mathf.PI * i / points;

            // 背景の円周上の位置を計算
            path[i] = new Vector3(Mathf.Cos(angle) * Radius, Mathf.Sin(angle) * Radius, 0) + CircleBackground.position;
        }

        return path;
    }

    // 円運動を停止させる関数
    public void StopCircularMovement()
    {
        // すべてのオブジェクトの円運動を停止
        foreach (var tween in objectTweens)
        {
            if (tween != null && tween.IsPlaying())
            {
                tween.Kill();  // 円運動を停止
            }
        }
    }
}
