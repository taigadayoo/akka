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
    [SerializeField]
    public Sprite[] SecondPhaseSprite;
    [SerializeField]
    public SpriteRenderer[] ThreeCommand;
    [SerializeField]
    public SpriteRenderer[] FourCommand;
    [SerializeField]
    public SpriteRenderer[] FiveCommand;
    [SerializeField]
    public SpriteRenderer[] FiveCommandThard;
    [SerializeField]
    public SpriteRenderer[] SixCommand;
    [SerializeField]
    public SpriteRenderer[] SevenCommand;
    public int MissCount = 0;
    public SpriteRenderer SecondBoxSprite;
    public bool OnGameOver = false;
    public Slider LeftHP;
    public Slider RightHP;
    public int PhaseCount = 0;
    public int FirstPlayerRandomNum = 0;//先行がどちらか決める値
    public int RandomCommandNum = 2; //第二フェーズの３～５個のランダムの個数
    public float SecondCommandTime = 0f;
    public bool ClearSecond = false;
    public bool ClearThard = false;
    public GameObject LaneRing;
    public GameObject JudgementRing;
    [SerializeField]
    CommandManager1P _1P;
    [SerializeField]
    CommandManager2P _2P;
    public bool SwitchPlayer = false;
    public bool ChangeNext = false;
    public bool ChangeLast = false;
    public bool ThardTimeUp = false;
    public bool StartThard = false;
    public bool OkPlayer1Thard = false;
    public bool OkPlayer2Thard = false;
    public bool TimeUpThard1P = false;
    public bool TimeUpThard2P = false;
    public bool OneTimeUp = false;
    public bool AllObjectSizeReset = false;
    public Collider2D OnTimeUpCol;
    void Start()
    {
        Set1pImagesActive(false);
        Set2pImagesActive(false);//配列内のミスマークは非表示に
        SecondBoxSprite.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if(MissCount == 5)
        {
            StartCoroutine(GameOver());
        }
       if(LeftHP.value <= 40 && LeftHP.value > 20)
        {
            PhaseCount = 1;
            SecondBoxSprite.gameObject.SetActive(true);
        }
       else if(LeftHP.value <= 20 && LeftHP.value > 0)
        {
            PhaseCount = 2;
            JudgementRing.gameObject.SetActive(true);
            LaneRing.gameObject.SetActive(true);
        }
       else if(LeftHP.value == 0)
        {
            StartCoroutine(GameOver());
        }
        if (PhaseCount == 1 && !ChangeNext)
        {
            _1P.ResetCommands();
            _2P.ResetCommands();
            _1P.IsCoolDown = false;
            _2P.IsCoolDown = false;
            ChangeNext = true;
        }
        else if (PhaseCount == 2 && !ChangeLast)
        {
            StartCoroutine(DelayCol());
            ClearSecond = false;    
            _1P.ResetCommands();
            _2P.ResetCommands();
            _1P.IsCoolDown = false;
            _2P.IsCoolDown = false;
            ChangeLast = true;
        }
    }
    public void Miss1pCountMark()
    {
        Miss1P[MissCount].gameObject.SetActive(true);
        MissCount += 1;
    }
    IEnumerator DelayCol()
    {
        yield return new WaitForSeconds(2f);

        OnTimeUpCol.enabled = true;
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
    public void SecondBoxImage()
    {
        if (RandomCommandNum == 0)
        {
            SecondPhaseSprite[0] = SecondBoxSprite.sprite;
        }
        if (RandomCommandNum == 1)
        {
            SecondPhaseSprite[1] = SecondBoxSprite.sprite;
        }
        if (RandomCommandNum == 2)
        {
            SecondPhaseSprite[2] = SecondBoxSprite.sprite;
        }
    }
    public void SecondImagesActive(bool isActive)
    {
        if (RandomCommandNum == 0)
        {
            foreach (SpriteRenderer sp in ThreeCommand)
            {
                sp.gameObject.SetActive(isActive);
            }
        }
        if (RandomCommandNum == 1)
        {
            foreach (SpriteRenderer sp in FourCommand)
            {
                sp.gameObject.SetActive(isActive);
            }
        }
        if (RandomCommandNum == 2)
        {
            foreach (SpriteRenderer sp in FiveCommand)
            {
                sp.gameObject.SetActive(isActive);
            }
        }
        
    }
    public void ThardImagesActive(bool isActive)
    {
        if (RandomCommandNum == 0)
        {
            foreach (SpriteRenderer sp in FiveCommandThard)
            {
                sp.gameObject.SetActive(isActive);
            }
        }
        if (RandomCommandNum == 1)
        {
            foreach (SpriteRenderer sp in SixCommand)
            {
                sp.gameObject.SetActive(isActive);
            }
        }
        if (RandomCommandNum == 2)
        {
            foreach (SpriteRenderer sp in SevenCommand)
            {
                sp.gameObject.SetActive(isActive);
            }
        }

    }
    IEnumerator GameOver()
    {

        yield return new WaitForSeconds(2.0f);

        SceneManager.Instance.LoadScene(SceneName.Result);
        //ゲームオーバー処理
    }
}
