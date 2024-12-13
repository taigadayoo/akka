using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTextAnimation : MonoBehaviour
{
    GameManager _gameManager;
    // Start is called before the first frame update
    public List<GameObject> TextImages;
    public List<GameObject> ClearTextImages;

    void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
        StartCoroutine(TextAnim());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator TextAnim()
    {
        yield return new WaitForSeconds(0.8f);
        for (int i = 0; i < TextImages.Count; i++)
        {
            GameObject obj = TextImages[i];
            obj.SetActive(true);
            AudioManager.Instance.PlaySE("ボールをミットでキャッチ1", 1f);
            if (obj.TryGetComponent(out Animator animator))
            {
                animator.enabled = true;

            }
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlaySE("ホイッスル", 0.5f);
        yield return new WaitForSeconds(0.5f);
        _gameManager.GameStart = true;
        AudioManager.Instance.PlayBGM("ビッグモンキー", 0.5f);
    }
 public void StartText()
    {
        StartCoroutine(ClearTextAnim());
    }
    IEnumerator ClearTextAnim()
    {
        for (int i = 0; i < TextImages.Count; i++)
        {
            GameObject obj = TextImages[i];
            obj.SetActive(false);

        }
        _gameManager.GameStart = false;
        yield return new WaitForSeconds(2f);
        _gameManager.StartPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < ClearTextImages.Count; i++)
        {
            GameObject obj = ClearTextImages[i];
            obj.SetActive(true);
            AudioManager.Instance.PlaySE("ボールをミットでキャッチ1", 1f);
            if (obj.TryGetComponent(out Animator animator))
            {
                animator.enabled = true;

            }
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlaySE("ホイッスル・連続", 0.5f);
        yield return new WaitForSeconds(0.5f);
        _gameManager.StartGameOver();
    }
}
