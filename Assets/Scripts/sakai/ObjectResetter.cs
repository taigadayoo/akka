using UnityEngine;

public class ObjectResetter : MonoBehaviour
{
    public int ObjectIndex;  // リスト内のオブジェクトのインデックス
    private CircularMovementWithBackground circularMovement;

    void Awake()
    {
        // CircularMovementWithBackground スクリプトを取得
        circularMovement = FindObjectOfType<CircularMovementWithBackground>();
    }

    void OnEnable()
    {
        // アクティブになった際にリセットを実行
        if (circularMovement != null)
        {
            circularMovement.ResetObjectToStart(ObjectIndex);
        }
    }
}
