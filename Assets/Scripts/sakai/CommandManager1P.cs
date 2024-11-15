using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
public class CommandManager1P : MonoBehaviour
{
    public SpriteRenderer FirstImage;
    public SpriteRenderer SecondImage;
    public SpriteRenderer ThirdImage;

    public Sprite ASprite, BSprite, XSprite, YSprite, RightSprite, LeftSprite, UpSprite, DownSprite;

    private List<string> _commands;
    private List<string> _currentSequence = new List<string>();
    private int _currentIndex;

    private Dictionary<string, Sprite> _commandSprites;

    public GameObject FirstBox;
    private float _lastResetTime;
    private float _resetCooldown = 0.2f;

    [SerializeField]
    CommandManager2P _command2p;
    public float CommandTime = 0f;
    private float _commandTimeout = 3f; // 3秒
    private float _commandTimeoutSecond = 7f;
    [SerializeField]
    CircularMovementWithBackground _circular;
    GameManager _gameManager;
    [SerializeField]
    CommandManager2P _2P;
    public bool IsCoolDown = false;
    private ControllerData _controllerData;


    private void Start()
    {

        _gameManager = FindObjectOfType<GameManager>();

        _commands = new List<string> { "A", "B", "X", "Y", "Right", "Left", "Up", "Down" };

        _commandSprites = new Dictionary<string, Sprite>
        {
            { "A", ASprite },
            { "B", BSprite },
            { "X", XSprite },
            { "Y", YSprite },
            { "Right", RightSprite },
            { "Left", LeftSprite },
            { "Up", UpSprite },
            { "Down", DownSprite }
        };
        _gameManager.ThardImagesActive(false);
        _gameManager.JudgementRing.SetActive(false);
        _gameManager.LaneRing.SetActive(false);
        StartCoroutine(GenerateCommands());
        ControllerManager.Instance.OnControllerData.Subscribe(OnControllerDataReceived).AddTo(this);
    }
    private void Update()
    {

      
        if(_gameManager.ClearSecond)
        {
           
            _gameManager.SecondCommandTime = 0;
            StartCoroutine(ClearSecond());
        }
        else if (_gameManager.ClearThard)
        {
            StartCoroutine(ClearThard());
            _gameManager.ClearThard = false;
        }
    }
    private IEnumerator GenerateCommands()
    {
        while (true)
        {
            _gameManager.SwitchPlayer = false;
            _currentSequence = new List<string>();
            _currentIndex = 0;
            _gameManager.ClearSecond = false;
            _gameManager.ClearThard = false;
            if (_gameManager.PhaseCount == 0)
            {

                FirstImage.gameObject.SetActive(true);
                SecondImage.gameObject.SetActive(true);
                ThirdImage.gameObject.SetActive(true);

                for (int i = 0; i < 3; i++)
                {
                    string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                    _currentSequence.Add(randomCommand);

                    Sprite commandSprite = _commandSprites[randomCommand];
                    if (i == 0)
                        FirstImage.sprite = commandSprite;
                    else if (i == 1)
                        SecondImage.sprite = commandSprite;
                    else if (i == 2)
                        ThirdImage.sprite = commandSprite; //3個までランダムでボタンスプライト表示
                }

                //_commandTime = 0f;

                while (_currentIndex < _currentSequence.Count)
                {
                    CommandTime += Time.deltaTime; // 経過時間を加算

                    if (CommandTime >= _commandTimeout) // 3秒経過した場合
                    {
                        ResetCommands();
                        yield break; // コルーチンを終了
                    }

                    yield return null;
                }


                FirstImage.gameObject.SetActive(false);
                SecondImage.gameObject.SetActive(false);
                ThirdImage.gameObject.SetActive(false);

               _gameManager.LeftHP.value -= 2.5f;
               _gameManager.RightHP.value -= 2.5f; //HP減少処理

                    CommandTime = 0f; // 経過時間リセット
              

                yield return new WaitForSeconds(1.0f);
            }
            else if(_gameManager.PhaseCount == 1)
            {
                FirstBox.SetActive(false);
                FirstImage.gameObject.SetActive(false);
                SecondImage.gameObject.SetActive(false);
                ThirdImage.gameObject.SetActive(false);

                    _gameManager.RandomCommandNum = Random.Range(0, 3);
                    _gameManager.FirstPlayerRandomNum = Random.Range(0, 2);

                _gameManager.SecondImagesActive(true);
                _gameManager.SecondBoxImage();

                if(_gameManager.RandomCommandNum == 0)
                {
                    if (_gameManager.FirstPlayerRandomNum == 0)
                    {
                       
                        for (int i = 0; i < 2; i++)
                        {
                            string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                            _currentSequence.Add(randomCommand);

                            Sprite commandSprite = _commandSprites[randomCommand];
                            if (i == 0)
                                _gameManager.ThreeCommand[0].sprite = commandSprite;
                            else if (i == 1)
                                _gameManager.ThreeCommand[2].sprite = commandSprite;
                        }
                    }else if(_gameManager.FirstPlayerRandomNum == 1)
                    {
                      
                        string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                            _currentSequence.Add(randomCommand);

                            Sprite commandSprite = _commandSprites[randomCommand];
                          
                                _gameManager.ThreeCommand[1].sprite = commandSprite;

                        
                    }
                }
                if (_gameManager.RandomCommandNum == 1)
                {
                    if (_gameManager.FirstPlayerRandomNum == 0)
                    {
                       
                        for (int i = 0; i < 2; i++)
                        {
                            string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                            _currentSequence.Add(randomCommand);

                            Sprite commandSprite = _commandSprites[randomCommand];
                            if (i == 0)
                                _gameManager.FourCommand[0].sprite = commandSprite;
                            else if (i == 1)
                                _gameManager.FourCommand[2].sprite = commandSprite;
                        }
                    }
                    else if (_gameManager.FirstPlayerRandomNum == 1)
                    {
                      
                        for (int i = 0; i < 2; i++)
                        {
                            string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                            _currentSequence.Add(randomCommand);

                            Sprite commandSprite = _commandSprites[randomCommand];
                            if (i == 0)
                                _gameManager.FourCommand[1].sprite = commandSprite;
                            else if (i == 1)
                                _gameManager.FourCommand[3].sprite = commandSprite;
                        }
                    }
                }
                if (_gameManager.RandomCommandNum == 2)
                {
                    if (_gameManager.FirstPlayerRandomNum == 0)
                    {
                    
                        for (int i = 0; i < 3; i++)
                        {
                            string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                            _currentSequence.Add(randomCommand);

                            Sprite commandSprite = _commandSprites[randomCommand];
                            if (i == 0)
                                _gameManager.FiveCommand[0].sprite = commandSprite;
                            else if (i == 1)
                                _gameManager.FiveCommand[2].sprite = commandSprite;
                            else if (i == 2)
                                _gameManager.FiveCommand[4].sprite = commandSprite;
                        }
                    }
                    else if (_gameManager.FirstPlayerRandomNum == 1)
                    {
                      
                        for (int i = 0; i < 2; i++)
                        {
                            string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                            _currentSequence.Add(randomCommand);

                            Sprite commandSprite = _commandSprites[randomCommand];
                            if (i == 0)
                                _gameManager.FiveCommand[1].sprite = commandSprite;
                            else if (i == 1)
                                _gameManager.FiveCommand[3].sprite = commandSprite;
                        }
                    }
                }

                
                while (_currentIndex <= _currentSequence.Count )
                {
                    _gameManager.SecondCommandTime  += Time.deltaTime; // 経過時間を加算
                    
                    if (_gameManager.SecondCommandTime  >= _commandTimeoutSecond ) // 5秒経過した場合
                    {
                        if (_gameManager.FirstPlayerRandomNum == 0 && !_gameManager.SwitchPlayer || _gameManager.FirstPlayerRandomNum == 1 && _gameManager.SwitchPlayer)
                        {
                            _gameManager.SecondImagesActive(false);

                            StartCoroutine(MissSecond(_controllerData));
                            yield break; // コルーチンを終了
                        }
                    }

                    yield return null;
                }
               
            }
            else if(_gameManager.PhaseCount == 2)
            {
                _gameManager.SecondBoxSprite.gameObject.SetActive(false);
                FirstBox.SetActive(false);
                FirstImage.gameObject.SetActive(false);
                SecondImage.gameObject.SetActive(false);
                ThirdImage.gameObject.SetActive(false);
                _gameManager.RandomCommandNum = Random.Range(0, 3);
                    _gameManager.FirstPlayerRandomNum = Random.Range(0, 2);

                _gameManager.StartThard = true;
                    _gameManager.ThardImagesActive(true);

                    if (_gameManager.RandomCommandNum == 0)
                    {
                        if (_gameManager.FirstPlayerRandomNum == 0)
                        {

                            for (int i = 0; i < 3; i++)
                            {
                                string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                                _currentSequence.Add(randomCommand);
                                ObjectSizeUp objectSize0 = _gameManager.FiveCommandThard[0].GetComponent<ObjectSizeUp>();
                                ObjectSizeUp objectSize2 = _gameManager.FiveCommandThard[2].GetComponent<ObjectSizeUp>();
                                ObjectSizeUp objectSize4 = _gameManager.FiveCommandThard[4].GetComponent<ObjectSizeUp>();
                                objectSize0.PlayerNum = ObjectSizeUp.Player.player1;
                                objectSize2.PlayerNum = ObjectSizeUp.Player.player1;
                                objectSize4.PlayerNum = ObjectSizeUp.Player.player1;
                            Sprite commandSprite = _commandSprites[randomCommand];
                                if (i == 0)
                                    _gameManager.FiveCommandThard[0].sprite = commandSprite;
                                else if (i == 1)
                                    _gameManager.FiveCommandThard[2].sprite = commandSprite;
                                else if (i == 2)
                                     _gameManager.FiveCommandThard[4].sprite = commandSprite;
                             }
                        }
                        else if (_gameManager.FirstPlayerRandomNum == 1)
                        {
                             for (int i = 0; i < 2; i++)
                             {
                                    string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                                    _currentSequence.Add(randomCommand);
                                ObjectSizeUp objectSize1 = _gameManager.FiveCommandThard[1].GetComponent<ObjectSizeUp>();
                                ObjectSizeUp objectSize3 = _gameManager.FiveCommandThard[3].GetComponent<ObjectSizeUp>();
                                objectSize1.PlayerNum = ObjectSizeUp.Player.player1;
                                objectSize3.PlayerNum = ObjectSizeUp.Player.player1;
                            Sprite commandSprite = _commandSprites[randomCommand];
                                    if (i == 0)
                                        _gameManager.FiveCommandThard[1].sprite = commandSprite;
                                    else if (i == 1)
                                        _gameManager.FiveCommandThard[3].sprite = commandSprite;


                             }
                         }
                    }
                    if (_gameManager.RandomCommandNum == 1)
                    {
                        if (_gameManager.FirstPlayerRandomNum == 0)
                        {

                            for (int i = 0; i < 3; i++)
                            {
                                string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                                _currentSequence.Add(randomCommand);
                            ObjectSizeUp objectSize0 = _gameManager.SixCommand[0].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize2 = _gameManager.SixCommand[2].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize4 = _gameManager.SixCommand[4].GetComponent<ObjectSizeUp>();
                            objectSize0.PlayerNum = ObjectSizeUp.Player.player1;
                            objectSize2.PlayerNum = ObjectSizeUp.Player.player1;
                            objectSize4.PlayerNum = ObjectSizeUp.Player.player1;
                            Sprite commandSprite = _commandSprites[randomCommand];
                                if (i == 0)
                                    _gameManager.SixCommand[0].sprite = commandSprite;
                                else if (i == 1)
                                    _gameManager.SixCommand[2].sprite = commandSprite;
                                 else if (i == 2)
                                     _gameManager.SixCommand[4].sprite = commandSprite;
                             }
                         }
                        else if (_gameManager.FirstPlayerRandomNum == 1)
                        {

                            for (int i = 0; i < 3; i++)
                            {
                                string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                                _currentSequence.Add(randomCommand);
                            ObjectSizeUp objectSize1 = _gameManager.SixCommand[1].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize3 = _gameManager.SixCommand[3].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize5 = _gameManager.SixCommand[5].GetComponent<ObjectSizeUp>();
                            objectSize1.PlayerNum = ObjectSizeUp.Player.player1;
                            objectSize3.PlayerNum = ObjectSizeUp.Player.player1;
                            objectSize5.PlayerNum = ObjectSizeUp.Player.player1;
                            Sprite commandSprite = _commandSprites[randomCommand];
                            if (i == 0)
                                _gameManager.SixCommand[1].sprite = commandSprite;
                            else if (i == 1)
                                _gameManager.SixCommand[3].sprite = commandSprite;
                            else if (i == 2)
                                _gameManager.SixCommand[5].sprite = commandSprite;
                        }
                        }
                    }
                    if (_gameManager.RandomCommandNum == 2)
                    {
                        if (_gameManager.FirstPlayerRandomNum == 0)
                        {

                            for (int i = 0; i < 4; i++)
                            {
                                string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                                _currentSequence.Add(randomCommand);
                            ObjectSizeUp objectSize0 = _gameManager.SevenCommand[0].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize2 = _gameManager.SevenCommand[2].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize4 = _gameManager.SevenCommand[4].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize6 = _gameManager.SevenCommand[6].GetComponent<ObjectSizeUp>();
                            objectSize0.PlayerNum = ObjectSizeUp.Player.player1;
                            objectSize2.PlayerNum = ObjectSizeUp.Player.player1;
                            objectSize4.PlayerNum = ObjectSizeUp.Player.player1;
                            objectSize6.PlayerNum = ObjectSizeUp.Player.player1;
                            Sprite commandSprite = _commandSprites[randomCommand];
                                if (i == 0)
                                    _gameManager.SevenCommand[0].sprite = commandSprite;
                                else if (i == 1)
                                    _gameManager.SevenCommand[2].sprite = commandSprite;
                                else if (i == 2)
                                    _gameManager.SevenCommand[4].sprite = commandSprite;
                            else if (i == 3)
                                _gameManager.SevenCommand[6].sprite = commandSprite;
                        }
                        }
                        else if (_gameManager.FirstPlayerRandomNum == 1)
                        {

                            for (int i = 0; i < 3; i++)
                            {
                                string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                                _currentSequence.Add(randomCommand);
                            ObjectSizeUp objectSize1 = _gameManager.SevenCommand[1].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize3 = _gameManager.SevenCommand[3].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize5 = _gameManager.SevenCommand[5].GetComponent<ObjectSizeUp>();
                            objectSize1.PlayerNum = ObjectSizeUp.Player.player1;
                            objectSize3.PlayerNum = ObjectSizeUp.Player.player1;
                            objectSize5.PlayerNum = ObjectSizeUp.Player.player1;
                            Sprite commandSprite = _commandSprites[randomCommand];
                            if (i == 0)
                                _gameManager.SevenCommand[1].sprite = commandSprite;
                            else if (i == 1)
                                _gameManager.SevenCommand[3].sprite = commandSprite;
                            else if (i == 2)
                                _gameManager.SevenCommand[5].sprite = commandSprite;
                        }

                        }
                    }


                while (_currentIndex <= _currentSequence.Count)
                {
                    _gameManager.SecondCommandTime += Time.deltaTime; // 経過時間を加算

                    if (_gameManager.ThardTimeUp) // 5秒経過した場合
                    {
                            _gameManager.ThardImagesActive(false);

                            StartCoroutine(MissThard(_controllerData));
                            yield break; // コルーチンを終了
                    }

                    yield return null;
                }

            }
            }
          
        }    

   public IEnumerator MissCommand()
    {
        FirstImage.gameObject.SetActive(false);
        SecondImage.gameObject.SetActive(false);
        ThirdImage.gameObject.SetActive(false);
        // クールダウンを開始
        IsCoolDown = true;

        _gameManager.Miss1pCountMark();
        yield return new WaitForSeconds(2.0f);
        IsCoolDown = false;
        ResetCommands();
    }
    //public IEnumerator MissSecondCommand()
    //{
    //    _gameManager.SecondImagesActive(false);
    //    CommandTime = 0;
    //    IsCoolDown = true;
    //        _gameManager.Miss1pCountMark();

    //    _2P.CommandTime = 0;
        
    //    yield return new WaitForSeconds(2.0f);
    //    IsCoolDown = false;
    //    _2P.ResetCommands();
    //    ResetCommands();
    //}

    public void ResetCommands()
    {
        //if (Time.time - _lastResetTime < _resetCooldown)
        //    return;
        CommandTime = 0f; // 経過時間リセット
        _gameManager.SecondCommandTime = 0;

        StopAllCoroutines();
        StartCoroutine(GenerateCommands());
        _lastResetTime = Time.time;
    }
    public IEnumerator ClearSecond()
    {

            _gameManager.SecondImagesActive(false);

            _gameManager.LeftHP.value -= 5f;
            _gameManager.RightHP.value -= 5f; //HP減少処理

        _gameManager.SecondCommandTime = 0;
        _gameManager.ClearSecond = false;
            yield return new WaitForSeconds(1.0f);
        IsCoolDown = false;
        _2P.IsCoolDown = false;
        ResetCommands();
        _2P.ResetCommands();

    }
    public IEnumerator ClearThard()
    {

        _gameManager.ThardImagesActive(false);

        _gameManager.LeftHP.value -= 10f;
        _gameManager.RightHP.value -= 10f; //HP減少処理

        _gameManager.ClearSecond = false;
        yield return new WaitForSeconds(1.0f);
        IsCoolDown = false;
        _2P.IsCoolDown = false;
        ResetCommands();
        _2P.ResetCommands();

    }
    public IEnumerator MissSecond(ControllerData controllerData)
    {

        _gameManager.SecondImagesActive(false);
        if (controllerData.PlayerType == PlayerType.Player1)
        {
            _gameManager.Miss1pCountMark();
        }
        else if(controllerData.PlayerType == PlayerType.Player2)
        {
            _gameManager.Miss2pCountMark();
        }
        else if(controllerData == null)
        {
            Debug.Log("controllerDataないです" + controllerData);
        }
        _gameManager.SecondCommandTime = 0;
        IsCoolDown = true;
        _2P.IsCoolDown = true;
        yield return new WaitForSeconds(2.0f);
        IsCoolDown = false;
        _2P.IsCoolDown = false;
        ResetCommands();
        _2P.ResetCommands();

    }
    public IEnumerator MissThard(ControllerData controllerData)
    {
        _circular.ResetAllObjects();
        _gameManager.ThardImagesActive(false);
        if (controllerData.PlayerType == PlayerType.Player1)
        {
            _gameManager.Miss1pCountMark();
        }
        else if (controllerData.PlayerType == PlayerType.Player2)
        {
            _gameManager.Miss2pCountMark();
        }
        else if (controllerData == null)
        {
            Debug.Log("controllerDataないです" + controllerData);
        }
        IsCoolDown = true;
        _2P.IsCoolDown = true;
        yield return new WaitForSeconds(2.0f);
        IsCoolDown = false;
        _2P.IsCoolDown = false;
        ResetCommands();
        _2P.ResetCommands();

    }
    private void OnControllerDataReceived(ControllerData controllerData)
    {
        if (controllerData.PlayerType != PlayerType.Player1)
            return; // プレイヤー1以外の操作は無視

        if (_currentIndex >= _currentSequence.Count)
            return;
        _controllerData = controllerData;

        string expectedCommand = _currentSequence[_currentIndex];


         if (controllerData.ActionType == ActionType.Buttons)
        {
            if (IsCorrectCommand(controllerData, expectedCommand) && _gameManager.PhaseCount == 0)
            {
                HandleCommandInput();
               
            }
            else if (IsCorrectCommand(controllerData, expectedCommand) && _gameManager.PhaseCount == 1)
            {
                if (!_gameManager.SwitchPlayer && _gameManager.FirstPlayerRandomNum == 0)
                {
                    HandleSecondCommandInput();
                    _gameManager.SwitchPlayer = true; 
                }
              else if (_gameManager.SwitchPlayer && _gameManager.FirstPlayerRandomNum == 1)
                {
                    HandleSecondCommandInput();
                    _gameManager.SwitchPlayer = false;
                }
                else if (_gameManager.SwitchPlayer && _gameManager.FirstPlayerRandomNum == 0 || !_gameManager.SwitchPlayer && _gameManager.FirstPlayerRandomNum == 1 && !IsCoolDown)
                {
                    StartCoroutine(MissSecond(_controllerData));
                }
            }
            else if (IsCorrectCommand(controllerData, expectedCommand) && _gameManager.PhaseCount == 2 && _gameManager.OkPlayer1Thard)
            {
                HandleThardCommandInput();
            }
            else
            {
                if (_gameManager.PhaseCount == 0 && !IsCoolDown)
                {
                    ExecutePhaseCount();
                }
                if (_gameManager.PhaseCount == 1 && !IsCoolDown)
                {
                    StartCoroutine(MissSecond(_controllerData));
                }
                if (_gameManager.PhaseCount == 2 && !IsCoolDown)
                {
                  
                    StartCoroutine(MissThard(_controllerData));
                   
                }
            }
        }
      
    }
    private void ExecutePhaseCount()
    {

        StartCoroutine(MissCommand());

    }
    //private void  ExecutePhaseCountSecond()
    //{

    //    StartCoroutine(MissSecondCommand());

    //}

    private void HandleCommandInput()
    {
        if (_currentIndex == 0)
            FirstImage.gameObject.SetActive(false);
        else if (_currentIndex == 1)
            SecondImage.gameObject.SetActive(false);
        else if (_currentIndex == 2)
            ThirdImage.gameObject.SetActive(false);

        _currentIndex++;

        if (_currentIndex >= _currentSequence.Count)
        {
            Debug.Log("1P Success! All commands entered correctly.");
        }
    }
    private void HandleSecondCommandInput()
    {
        if(_gameManager.FirstPlayerRandomNum == 0)
        {
            if (_gameManager.RandomCommandNum == 0)
            {
                if (_currentIndex == 0 && !_gameManager.SwitchPlayer)
                {
                    _gameManager.ThreeCommand[0].gameObject.SetActive(false);

                    _gameManager.SwitchPlayer = true;
                    _currentIndex++;
                }
                else if (_currentIndex == 1 && !_gameManager.SwitchPlayer)
                {
                    _gameManager.ThreeCommand[2].gameObject.SetActive(false);

                    _gameManager.ClearSecond = true;
                }

            }
            if (_gameManager.RandomCommandNum == 1)
            {
                if (_currentIndex == 0 && !_gameManager.SwitchPlayer)
                {
                    _gameManager.FourCommand[0].gameObject.SetActive(false);

                    _gameManager.SwitchPlayer = true;
                    _currentIndex++;
                }
                else if (_currentIndex == 1 && !_gameManager.SwitchPlayer)
                {
                    _gameManager.FourCommand[2].gameObject.SetActive(false);

                    _gameManager.SwitchPlayer = true;
                    _currentIndex++;
                }
            }
            if (_gameManager.RandomCommandNum == 2)
            {
                if (_currentIndex == 0 && !_gameManager.SwitchPlayer)
                {
                    _gameManager.FiveCommand[0].gameObject.SetActive(false);

                    _gameManager.SwitchPlayer = true;
                    _currentIndex++;
                }
                else if (_currentIndex == 1 && !_gameManager.SwitchPlayer)
                {
                    _gameManager.FiveCommand[2].gameObject.SetActive(false);

                    _gameManager.SwitchPlayer = true;
                    _currentIndex++;
                }
                else if (_currentIndex == 2 && !_gameManager.SwitchPlayer)
                {
                    _gameManager.FiveCommand[4].gameObject.SetActive(false);

                    _gameManager.ClearSecond = true;
                }
               
            }
        }
        if (_gameManager.FirstPlayerRandomNum == 1)
        {
            if (_gameManager.RandomCommandNum == 0)
            {
                
                if (_currentIndex == 0 && _gameManager.SwitchPlayer)
                {
                    _gameManager.ThreeCommand[1].gameObject.SetActive(false);

                    _gameManager.SwitchPlayer = false;

                    _currentIndex++;
                }

            }
            if (_gameManager.RandomCommandNum == 1)
            {
                if (_currentIndex == 0 && _gameManager.SwitchPlayer)
                {
                    _gameManager.FourCommand[1].gameObject.SetActive(false);

                    _gameManager.SwitchPlayer = false;

                    _currentIndex++;
                }
                else if (_currentIndex == 1 && _gameManager.SwitchPlayer)
                {
                    _gameManager.FourCommand[3].gameObject.SetActive(false);

                    _gameManager.ClearSecond = true;
                }
            }
            if (_gameManager.RandomCommandNum == 2)
            {
                if (_currentIndex == 0 && _gameManager.SwitchPlayer)
                {
                    _gameManager.FiveCommand[1].gameObject.SetActive(false);

                    _gameManager.SwitchPlayer = false;

                    _currentIndex++;
                }
                else if (_currentIndex == 1 && _gameManager.SwitchPlayer)
                {
                    _gameManager.FiveCommand[3].gameObject.SetActive(false);

                    _gameManager.SwitchPlayer = false;

                    _currentIndex++;
                }


            }


        }
    }

    private void HandleThardCommandInput()
    {
        if (_gameManager.FirstPlayerRandomNum == 0)
        {
            if (_gameManager.RandomCommandNum == 0)
            {
                if (_currentIndex == 0 )
                {
                    _gameManager.FiveCommandThard[0].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 1)
                {
                    _gameManager.FiveCommandThard[2].gameObject.SetActive(false);
                    _currentIndex++;
                }
                else if (_currentIndex == 2)
                {
                    _gameManager.FiveCommandThard[4].gameObject.SetActive(false);
                    _currentIndex++;
                    _gameManager.ClearThard = true;
                }

            }
            if (_gameManager.RandomCommandNum == 1)
            {
                if (_currentIndex == 0)
                {
                    _gameManager.SixCommand[0].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 1 )
                {
                    _gameManager.SixCommand[2].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 2)
                {
                    _gameManager.SixCommand[4].gameObject.SetActive(false);

                    _currentIndex++;
                }
            }
            if (_gameManager.RandomCommandNum == 2)
            {
                if (_currentIndex == 0)
                {
                    _gameManager.SevenCommand[0].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 1)
                {
                    _gameManager.SevenCommand[2].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 2)
                {
                    _gameManager.SevenCommand[4].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 3)
                {
                    _gameManager.SevenCommand[6].gameObject.SetActive(false);

                    _currentIndex++;
                    _gameManager.ClearThard = true;
                }

            }
        }
        if (_gameManager.FirstPlayerRandomNum == 1)
        {
            if (_gameManager.RandomCommandNum == 0)
            {

                if (_currentIndex == 0)
                {
                    _gameManager.FiveCommandThard[1].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 1)
                {
                    _gameManager.FiveCommandThard[3].gameObject.SetActive(false);
                    _currentIndex++;
                }


            }
            if (_gameManager.RandomCommandNum == 1)
            {
                if (_currentIndex == 0)
                {
                    _gameManager.SixCommand[1].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 1)
                {
                    _gameManager.SixCommand[3].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 2)
                {
                    _gameManager.SixCommand[5].gameObject.SetActive(false);

                    _currentIndex++;
                    _gameManager.ClearThard = true;
                }
            }
            if (_gameManager.RandomCommandNum == 2)
            {
                if (_currentIndex == 0)
                {
                    _gameManager.SevenCommand[1].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 1)
                {
                    _gameManager.SevenCommand[3].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 2)
                {
                    _gameManager.SevenCommand[5].gameObject.SetActive(false);

                    _currentIndex++;
                }


            }


        }
    }
    private bool IsCorrectCommand(ControllerData controllerData, string expectedCommand)
    {
        if (controllerData.ActionType == ActionType.Buttons)
        {
            switch (expectedCommand)
            {
                case "A":
                    return controllerData.ButtonType == ButtonType.South;
                case "B":
                    return controllerData.ButtonType == ButtonType.East;
                case "X":
                    return controllerData.ButtonType == ButtonType.West;
                case "Y":
                    return controllerData.ButtonType == ButtonType.North;
                case "Right":
                    return controllerData.ButtonType == ButtonType.Right;
                case "Up":
                    return controllerData.ButtonType == ButtonType.Up;
                case "Down":
                    return controllerData.ButtonType == ButtonType.Down;
                case "Left":
                    return controllerData.ButtonType == ButtonType.Left;
                default:
                    return false;
            }
        }

        return false;
    }
}