using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CutInAnimationManager : MonoBehaviour
{
    public GameObject CutInpanel;
    public RectTransform CutBack1;
    public RectTransform CutBack2;
    public RectTransform Player1;
    public RectTransform Player2;
    public RectTransform Akka1;
    public RectTransform Akka2;
    public GameObject Ink1;
    public GameObject Ink3;
    public GameObject Ink2;
    public GameObject Ink4;
    public GameObject FirstCutInSet;
    public GameObject SecondCutInSet;
    [SerializeField]
    CommandManager1P _1P;
    [SerializeField]
    CommandManager2P _2P;
    [SerializeField]
    GameManager _gameManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartFirstCutIn()
    {
        StartCoroutine(FirstCutIn());
    }
    public void StartSecondCutIn()
    {
        StartCoroutine(SecondCutIn());
    }
    public IEnumerator FirstCutIn()
    {
        CutInpanel.SetActive(true);

        yield return new WaitForSeconds(0.3f);
       
        CutBack1.DOAnchorPos(new Vector2(-28, 0), 0.5f).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(0.5f);

        Player1.DOAnchorPos(new Vector2(-468, -164), 0.5f).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(0.5f);

        Akka1.DOAnchorPos(new Vector2(538, 109), 0.5f).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(0.5f);

        Ink1.SetActive(true);
        Ink1.transform.DOScale(new Vector3(0.2f, 0.2f, 1f), 0.2f)
                     .SetEase(Ease.OutBounce); // イージングを設定

        yield return new WaitForSeconds(0.3f);

        Ink2.SetActive(true);
        Ink2.transform.DOScale(new Vector3(0.15f, 0.15f, 1f), 0.2f)
                    .SetEase(Ease.OutBounce); // イージングを設定
        yield return new WaitForSeconds(0.8f);
        FirstCutInSet.SetActive(false);
        CutInpanel.SetActive(false);
        _gameManager.OnCutIn = false;
        _1P.enabled = true;
        _2P.enabled = true;
        _gameManager.FirstSet.SetActive(true);
        _gameManager.SecondSet.SetActive(true);
        _gameManager.ThardSet.SetActive(true);
        _gameManager.SecondCommandTime = 0;
       _gameManager.TimerMix.SetActive(true);
    }
    public IEnumerator SecondCutIn()
    {
        CutInpanel.SetActive(true);

        yield return new WaitForSeconds(0.3f);

        CutBack2.DOAnchorPos(new Vector2(-28, 0), 0.5f).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(0.5f);

        Player2.DOAnchorPos(new Vector2(587, -164), 0.5f).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(0.5f);

        Akka2.DOAnchorPos(new Vector2(-422, 86), 0.5f).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(0.5f);

        Ink3.SetActive(true);
        Ink3.transform.DOScale(new Vector3(0.2f, 0.2f, 1f), 0.2f)
                     .SetEase(Ease.OutBounce); // イージングを設定

        yield return new WaitForSeconds(0.3f);

        Ink4.SetActive(true);
        Ink4.transform.DOScale(new Vector3(0.15f, 0.15f, 1f), 0.2f)
                    .SetEase(Ease.OutBounce); // イージングを設定
        yield return new WaitForSeconds(0.8f);
        SecondCutInSet.SetActive(false);
        CutInpanel.SetActive(false);
        _gameManager.OnCutIn = false;
        _1P.enabled = true;
        _2P.enabled = true;
        _gameManager.FirstSet.SetActive(true);
        _gameManager.SecondSet.SetActive(true);
        _gameManager.ThardSet.SetActive(true);
    }
}
