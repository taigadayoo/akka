using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
   public Image[] Miss1P;
    [SerializeField]
    public Image[] Miss2P;

    public int MissCount = 0;

    public bool OnGameOver = false;
    void Start()
    {
        Set1pImagesActive(false);
        Set2pImagesActive(false);//配列内のミスマークは非表示に
    }

    // Update is called once per frame
    void Update()
    {
        if(MissCount == 5)
        {
            StartCoroutine(GameOver());
        }
    }
    public void Miss1pCountMark()
    {
        Miss1P[MissCount].gameObject.SetActive(true);
        MissCount += 1;
    }
    public void Miss2pCountMark()
    {
        Miss2P[MissCount].gameObject.SetActive(true);
        MissCount += 1;
    }
    // 配列内の全てのImageオブジェクトを一括でSetActiveを切り替えるメソッド
    public void Set1pImagesActive(bool isActive)
    {
        foreach (Image img in Miss1P)
        {
            img.gameObject.SetActive(isActive);
        }
    }
    public void Set2pImagesActive(bool isActive)
    {
        foreach (Image img in Miss2P)
        {
            img.gameObject.SetActive(isActive);
        }
    }
    IEnumerator GameOver()
    {

        yield return new WaitForSeconds(2.0f);

        SceneManager.Instance.LoadScene(SceneName.Result);
        //ゲームオーバー処理
    }
}
