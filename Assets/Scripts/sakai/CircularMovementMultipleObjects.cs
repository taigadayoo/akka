using UnityEngine;

public class CircularMovementWithBackground : MonoBehaviour
{
    public GameObject[] Objects;    // 動かすオブジェクトの配列
    public Transform CircleBackground; // 背景の円の位置（円の中心）
    public float Radius = 3.05f;    // 円の半径
    public float Duration = 5f;     // 一周するのにかかる時間
    public bool Loop = true;        // 繰り返すかどうか
    [SerializeField]
    GameManager _gameManager;

    private Vector3[] _initialPositions;  // 各オブジェクトの初期位置
    private float[] _angles;  // 各オブジェクトの現在の角度
    private float[] _speeds;  // 各オブジェクトの回転速度

    void Start()
    {
        _initialPositions = new Vector3[Objects.Length];
        _angles = new float[Objects.Length];
        _speeds = new float[Objects.Length];
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

        UpdateCircularMovement();  // 各オブジェクトの円運動を更新
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
            _initialPositions[i] = new Vector3(Mathf.Cos(angle) * Radius, Mathf.Sin(angle) * Radius, 0) + CircleBackground.position;
            Objects[i].transform.position = _initialPositions[i];
        }
    }

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
            _angles[i] = Mathf.Atan2(_initialPositions[i].y - CircleBackground.position.y, _initialPositions[i].x - CircleBackground.position.x);
            _speeds[i] = -2 * Mathf.PI / Duration;  // 一周する時間に基づいて回転速度を設定
        }
    }

    void UpdateCircularMovement()
    {
        for (int i = 0; i < Objects.Length; i++)
        {
            _angles[i] += _speeds[i] * Time.deltaTime;  // 角度を更新
            Vector3 newPosition = new Vector3(Mathf.Cos(_angles[i]) * Radius, Mathf.Sin(_angles[i]) * Radius, 0) + CircleBackground.position;
            Objects[i].transform.position = newPosition;  // 新しい位置にオブジェクトを移動
        }
    }

    public void ResetObjectToStart(int index)
    {
        if (index < 0 || index >= Objects.Length)
        {
            Debug.LogWarning("Index out of range.");
            return;
        }

        // 初期位置に瞬時に戻す
        Objects[index].transform.position = _initialPositions[index];

        // 角度と回転速度をリセット
        _angles[index] = Mathf.Atan2(_initialPositions[index].y - CircleBackground.position.y, _initialPositions[index].x - CircleBackground.position.x);
        _speeds[index] = 2 * Mathf.PI / Duration;
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
