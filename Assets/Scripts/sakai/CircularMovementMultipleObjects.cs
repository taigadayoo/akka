using UnityEngine;
using DG.Tweening;

public class CircularMovementWithBackground : MonoBehaviour
{
    public GameObject[] Objects;    // 動かすオブジェクトの配列
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
        objectTweens = new Tween[Objects.Length];
        PositionObjectsOnCircle();  // オブジェクトを円周上に配置
        AddCollidersAndRigidbodyToChildren(); // 子オブジェクトにColliderとRigidbodyを追加
    }

    void Update()
    {
        if (_gameManager.StartThard)
        {
            StartCircularMovement();  // 円運動を開始
            _gameManager.StartThard = false;  // 一度だけ呼ばれるようにフラグをリセット
        }
    }

    void PositionObjectsOnCircle()
    {
        int objectCount = Objects.Length;
        float startAngle = 2 * Mathf.PI / 3;
        float endAngle = 11 * Mathf.PI / 6;
        float angleStep = (endAngle - startAngle) / (objectCount - 1);

        for (int i = 0; i < objectCount; i++)
        {
            float angle = startAngle + i * angleStep;
            initialPositions[i] = new Vector3(Mathf.Cos(angle) * Radius, Mathf.Sin(angle) * Radius, 0) + CircleBackground.position;
            Objects[i].transform.position = initialPositions[i];
        }
    }

    // 子オブジェクトにColliderとRigidbodyを追加するメソッド
    void AddCollidersAndRigidbodyToChildren()
    {
        foreach (var obj in Objects)
        {
            if (obj.transform.childCount > 0)
            {
                foreach (Transform child in obj.transform)
                {
                    // 子オブジェクトにRigidbodyがない場合、Rigidbodyを追加
                    if (child.GetComponent<Rigidbody>() == null)
                    {
                        Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
                        rb.isKinematic = true;  // 物理エンジンの影響を受けないように
                    }
                }
            }
        }
    }

    void StartCircularMovement()
    {
        int objectCount = Objects.Length;
        for (int i = 0; i < objectCount; i++)
        {
            Vector3[] path = CreateCircularPath(initialPositions[i]);

            // 一周後に初期位置に戻す
            objectTweens[i] = Objects[i].transform.DOPath(path, Duration, PathType.CatmullRom)
                .SetEase(Ease.Linear)
                .SetLoops(Loop ? -1 : 1, LoopType.Restart)  // Loopがtrueなら無限ループ
                .OnComplete(() => ResetObjectToStart(i));
        }
    }

    Vector3[] CreateCircularPath(Vector3 startPosition)
    {
        int points = 36;  // 円周上の分割数
        Vector3[] path = new Vector3[points];
        float startAngle = Mathf.Atan2(startPosition.y - CircleBackground.position.y, startPosition.x - CircleBackground.position.x);

        for (int i = 0; i < points; i++)
        {
            float angle = startAngle - 2 * Mathf.PI * i / points;
            path[i] = new Vector3(Mathf.Cos(angle) * Radius, Mathf.Sin(angle) * Radius, 0) + CircleBackground.position;
        }

        return path;
    }

    // 他のスクリプトから呼び出せるようにpublicに変更
    public void ResetObjectToStart(int index)
    {
        if (index < 0 || index >= Objects.Length)
        {
            Debug.LogWarning("Index out of range.");
            return;
        }

        Objects[index].transform.position = initialPositions[index];  // 初期位置に戻す

        // 再度円運動を開始
        Vector3[] path = CreateCircularPath(initialPositions[index]);
        objectTweens[index] = Objects[index].transform.DOPath(path, Duration, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .SetLoops(1, LoopType.Restart)
            .OnComplete(() => ResetObjectToStart(index));  // 一周したら再度リセット
    }

    // 他のスクリプトから全オブジェクトをリセットできるメソッド
    public void ResetAllObjects()
    {
        for (int i = 0; i < Objects.Length; i++)
        {
            ResetObjectToStart(i);
        }
    }

}
