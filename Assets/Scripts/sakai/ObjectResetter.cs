using UnityEngine;

public class ObjectResetter : MonoBehaviour
{
    public int ObjectIndex;  // リスト内のオブジェクトのインデックス
    private CircularMovementWithBackground _circularMovement;

    void Awake()
    {
        // CircularMovementWithBackground スクリプトを取得
        _circularMovement = FindObjectOfType<CircularMovementWithBackground>();
    }

    void OnEnable()
    {
        // アクティブになった際にリセットを実行
        if (_circularMovement != null)
        {
            _circularMovement.ResetObjectToStart(ObjectIndex);
        }
    }
}
