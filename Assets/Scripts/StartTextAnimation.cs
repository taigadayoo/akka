using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTextAnimation : MonoBehaviour
{
    GameManager _gameManager;
    // Start is called before the first frame update
    public List<GameObject> TextImages;
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

            if (obj.TryGetComponent(out Animator animator))
            {
                animator.enabled = true;

            }
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1f);
        _gameManager.GameStart = true;
    }
}
