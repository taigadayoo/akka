using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThardObjectController : MonoBehaviour
{
    public List<GameObject> Objects; // インスペクターで指定するオブジェクトのリスト
    private List<GameObject> _initialObjectsState; // 初期のオブジェクトリストを保持
    public float DelayBetweenActivations = 0.1f; // 各アニメーション開始の遅延

    void Start()
    {
        // 初期状態のオブジェクトリストをコピーして保存
        _initialObjectsState = new List<GameObject>(Objects);
    }

    void Update()
    {
        // オブジェクトが非アクティブになった場合にリストから削除
        for (int i = Objects.Count - 1; i >= 0; i--)
        {
            if (Objects[i] != null && !Objects[i].activeSelf)
            {
                Objects.RemoveAt(i); // 非アクティブなオブジェクトをリストから削除
            }
        }
    }

    // 指定した関数を呼び出し、オブジェクトのアニメーションを1秒ごとに順次再生
    public void StartAnimationsSequentially()
    {
        StartCoroutine(ActivateObjectsWithAnimation());
    }

    private IEnumerator ActivateObjectsWithAnimation()
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            GameObject obj = Objects[i];
            obj.SetActive(true);

            if (obj.TryGetComponent(out Animator animator))
            {
                animator.enabled = true;
               
            }
            animator.SetTrigger("SizeUp");
            yield return new WaitForSeconds(DelayBetweenActivations);
        }
    }

    // オブジェクトリストを初期状態にリセット
    public void ResetObjects()
    {
        StopAllCoroutines(); // アニメーション再生を停止

        // 現在のリストを初期の状態に戻す

            Objects = new List<GameObject>(_initialObjectsState);


        // オブジェクトを非表示にしてAnimatorを無効化
        foreach (var obj in Objects)
        {
            obj.SetActive(false);
            if (obj.TryGetComponent(out Animator animator))
            {
                animator.enabled = false;
            }
        }
    }
}