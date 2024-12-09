using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class CommandManager2P : MonoBehaviour
{
    public SpriteRenderer FirstImage;
    public SpriteRenderer SecondImage;
    public SpriteRenderer ThirdImage;
    public Sprite ASprite, BSprite, XSprite, YSprite, RightSprite, LeftSprite, UpSprite, DownSprite;
    [SerializeField]
    CircularMovementWithBackground _circular;
    private List<string> _commands;
    private List<string> _currentSequence= new List<string>();
    private int _currentIndex;

    [SerializeField]
    CommandManager1P _command1P;
    private Dictionary<string, Sprite> _commandSprites;

    private float _lastResetTime;
    [SerializeField]
    public Animator Animator2P;
    public GameObject FirstBox;
    public float CommandTime = 0f;
    private float _commandTimeout = 3f; // 3秒
    private float _commandTimeoutSecond = 7f;
    [SerializeField]
    CommandManager1P _1P;
    public bool ChangeNext = false;
    GameManager _gameManager;
    public bool IsCoolDown = false;
    public ControllerData ControllerData;
    [SerializeField]
    ThardObjectController _thardObjectController;
    private bool _oneStart = false;
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
        
    
        ControllerManager.Instance.OnControllerData.Subscribe(OnControllerDataReceived).AddTo(this);
    }
    private void Update()
    {
        StartCommand();
        if (_gameManager.TimeUpThard2P)
        {
            StartCoroutine(_1P.MissTimeUp(ControllerData));
            _gameManager.TimeUpThard2P = false;
        }
        if (_gameManager.OneClear)
        {
            FirstImage.gameObject.SetActive(false);
            SecondImage.gameObject.SetActive(false);
            ThirdImage.gameObject.SetActive(false);
        }
    }
    private void StartCommand()
    {
        if (_gameManager.GameStart && !_oneStart)
        {
            FirstBox.SetActive(true);
            FirstImage.gameObject.SetActive(true);
            SecondImage.gameObject.SetActive(true);
            ThirdImage.gameObject.SetActive(true);
            _oneStart = true;
            StartCoroutine(GenerateCommands());
        }
    }
    private IEnumerator GenerateCommands()
    {
        while (true)
        {
            _currentSequence = new List<string>();
            _currentIndex = 0;


            if (_gameManager.PhaseCount == 0)
            {
                if (_gameManager.PhaseCount == 1)
                {
                    yield return null;
                }
                FirstImage.gameObject.SetActive(true);
                SecondImage.gameObject.SetActive(true);
                ThirdImage.gameObject.SetActive(true);
                _gameManager.EnableAllAnimators(1);
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
                if (_gameManager.PhaseCount == 1)
                {
                    yield return null;
                }
                //_commandTime = 0f;

                while (_currentIndex < _currentSequence.Count)
                {
                    CommandTime += Time.deltaTime; // 経過時間を加算

                    if (CommandTime >= _commandTimeout && _gameManager.GameStart) // 3秒経過した場合
                    {
                        StartCoroutine(MissCommand());
                        yield break; // コルーチンを終了
                    }

                    yield return null;
                }


                FirstImage.gameObject.SetActive(false);
                SecondImage.gameObject.SetActive(false);
                ThirdImage.gameObject.SetActive(false);

                _gameManager.LeftHP.value -= 2.5f;
                _gameManager.RightHP.value -= 2.5f; //HP減少処理
                Animator2P.SetTrigger("Attack2p");
                CommandTime = 0f; // 経過時間リセット


                yield return new WaitForSeconds(1.0f);

            }
            else if (_gameManager.PhaseCount == 1)
            {
                FirstBox.SetActive(false);
                FirstImage.gameObject.SetActive(false);
                SecondImage.gameObject.SetActive(false);
                ThirdImage.gameObject.SetActive(false);



                if (_gameManager.RandomCommandNum == 0)
                {

                    if (_gameManager.FirstPlayerRandomNum == 0)
                    {

                        string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                        _currentSequence.Add(randomCommand);

                        Sprite commandSprite = _commandSprites[randomCommand];

                        _gameManager.ThreeCommand[1].sprite = commandSprite;

                    }
                    else if (_gameManager.FirstPlayerRandomNum == 1)
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
                                _gameManager.FourCommand[1].sprite = commandSprite;
                            else if (i == 1)
                                _gameManager.FourCommand[3].sprite = commandSprite;
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
                                _gameManager.FourCommand[0].sprite = commandSprite;
                            else if (i == 1)
                                _gameManager.FourCommand[2].sprite = commandSprite;
                        }
                    }
                }
                if (_gameManager.RandomCommandNum == 2)
                {
                    if (_gameManager.FirstPlayerRandomNum == 0)
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
                    else if (_gameManager.FirstPlayerRandomNum == 1)
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
                }

                while (_currentIndex <= _currentSequence.Count)
                {
                    _gameManager.SecondCommandTime += Time.deltaTime; // 経過時間を加算

                    if (_gameManager.SecondCommandTime >= _commandTimeoutSecond && _gameManager.GameStart) // 5秒経過した場合
                    {
                        if (_gameManager.FirstPlayerRandomNum == 0 && _gameManager.SwitchPlayer || _gameManager.FirstPlayerRandomNum == 1 && !_gameManager.SwitchPlayer)
                        {
                            StartCoroutine(_1P.MissSecond(ControllerData));
                            yield break; // コルーチンを終了
                        }
                    }

                    yield return null;
                }

            }
            else if (_gameManager.PhaseCount == 2)
            {
                _gameManager.SecondBoxSprite.gameObject.SetActive(false);
               
                FirstBox.SetActive(false);
                FirstImage.gameObject.SetActive(false);
                SecondImage.gameObject.SetActive(false);
                ThirdImage.gameObject.SetActive(false);
                _gameManager.StartThard = true;
                _gameManager.ThardImagesActive(true);

                if (_gameManager.RandomCommandNum == 0)
                {
                    if (_gameManager.FirstPlayerRandomNum == 1)
                    {

                        for (int i = 0; i < 3; i++)
                        {
                            string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                            _currentSequence.Add(randomCommand);
                            ObjectSizeUp objectSize0 = _gameManager.FiveCommandThard[0].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize2 = _gameManager.FiveCommandThard[2].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize4 = _gameManager.FiveCommandThard[4].GetComponent<ObjectSizeUp>();
                            objectSize0.PlayerNum = ObjectSizeUp.Player.player2;
                            objectSize2.PlayerNum = ObjectSizeUp.Player.player2;
                            objectSize4.PlayerNum = ObjectSizeUp.Player.player2;
                            Sprite commandSprite = _commandSprites[randomCommand];
                            if (i == 0)
                                _gameManager.FiveCommandThard[0].sprite = commandSprite;
                            else if (i == 1)
                                _gameManager.FiveCommandThard[2].sprite = commandSprite;
                            else if (i == 2)
                                _gameManager.FiveCommandThard[4].sprite = commandSprite;
                        }
                    }
                    else if (_gameManager.FirstPlayerRandomNum == 0)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                            _currentSequence.Add(randomCommand);
                            ObjectSizeUp objectSize1 = _gameManager.FiveCommandThard[1].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize3 = _gameManager.FiveCommandThard[3].GetComponent<ObjectSizeUp>();
                            objectSize1.PlayerNum = ObjectSizeUp.Player.player2;
                            objectSize3.PlayerNum = ObjectSizeUp.Player.player2;
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
                    if (_gameManager.FirstPlayerRandomNum == 1)
                    {

                        for (int i = 0; i < 3; i++)
                        {
                            string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                            _currentSequence.Add(randomCommand);
                            ObjectSizeUp objectSize0 = _gameManager.SixCommand[0].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize2 = _gameManager.SixCommand[2].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize4 = _gameManager.SixCommand[4].GetComponent<ObjectSizeUp>();
                            objectSize0.PlayerNum = ObjectSizeUp.Player.player2;
                            objectSize2.PlayerNum = ObjectSizeUp.Player.player2;
                            objectSize4.PlayerNum = ObjectSizeUp.Player.player2;
                            Sprite commandSprite = _commandSprites[randomCommand];
                            if (i == 0)
                                _gameManager.SixCommand[0].sprite = commandSprite;
                            else if (i == 1)
                                _gameManager.SixCommand[2].sprite = commandSprite;
                            else if (i == 2)
                                _gameManager.SixCommand[4].sprite = commandSprite;
                        }
                    }
                    else if (_gameManager.FirstPlayerRandomNum == 0)
                    {

                        for (int i = 0; i < 3; i++)
                        {
                            string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                            _currentSequence.Add(randomCommand);
                            ObjectSizeUp objectSize1 = _gameManager.SixCommand[1].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize3 = _gameManager.SixCommand[3].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize5 = _gameManager.SixCommand[5].GetComponent<ObjectSizeUp>();
                            objectSize1.PlayerNum = ObjectSizeUp.Player.player2;
                            objectSize3.PlayerNum = ObjectSizeUp.Player.player2;
                            objectSize5.PlayerNum = ObjectSizeUp.Player.player2;
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
                    if (_gameManager.FirstPlayerRandomNum == 1)
                    {

                        for (int i = 0; i < 4; i++)
                        {
                            string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                            _currentSequence.Add(randomCommand);
                            ObjectSizeUp objectSize0 = _gameManager.SevenCommand[0].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize2 = _gameManager.SevenCommand[2].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize4 = _gameManager.SevenCommand[4].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize6 = _gameManager.SevenCommand[6].GetComponent<ObjectSizeUp>();
                            objectSize0.PlayerNum = ObjectSizeUp.Player.player2;
                            objectSize2.PlayerNum = ObjectSizeUp.Player.player2;
                            objectSize4.PlayerNum = ObjectSizeUp.Player.player2;
                            objectSize6.PlayerNum = ObjectSizeUp.Player.player2;
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
                    else if (_gameManager.FirstPlayerRandomNum == 0)
                    {

                        for (int i = 0; i < 3; i++)
                        {
                            string randomCommand = _commands[UnityEngine.Random.Range(0, _commands.Count)];
                            _currentSequence.Add(randomCommand);
                            ObjectSizeUp objectSize1 = _gameManager.SevenCommand[1].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize3 = _gameManager.SevenCommand[3].GetComponent<ObjectSizeUp>();
                            ObjectSizeUp objectSize5 = _gameManager.SevenCommand[5].GetComponent<ObjectSizeUp>();
                            objectSize1.PlayerNum = ObjectSizeUp.Player.player2;
                            objectSize3.PlayerNum = ObjectSizeUp.Player.player2;
                            objectSize5.PlayerNum = ObjectSizeUp.Player.player2;
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

                    if (_gameManager.ThardTimeUp && _gameManager.GameStart) // 5秒経過した場合
                    {
                        _gameManager.ThardImagesActive(false);

                        StartCoroutine(_1P.MissThard(ControllerData));
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
        CommandTime = 0;
        _gameManager.Miss2pCountMark();
        yield return new WaitForSeconds(2.0f);
        IsCoolDown = false;
        ResetCommands();
    }
    //public IEnumerator MissSecondCommand()
    //{
    //    _gameManager.SecondImagesActive(false);

    //    IsCoolDown = true;
    //    CommandTime = 0;
    //    _1P.CommandTime = 0;
    //    _gameManager.Miss2pCountMark();
    //    yield return new WaitForSeconds(2.0f);
    //    IsCoolDown = false;
    //    ResetCommands();
    //    _1P.ResetCommands();
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

    private void OnControllerDataReceived(ControllerData controllerData)
    {
        if (controllerData.PlayerType != PlayerType.Player2)
            return; // プレイヤー1以外の操作は無視

        if (_currentIndex >= _currentSequence.Count)
            return;

        string expectedCommand = _currentSequence[_currentIndex];

        ControllerData = controllerData;

        if (controllerData.ActionType == ActionType.Buttons && _oneStart && !_gameManager.OneClear)
        {
            if (IsCorrectCommand(controllerData, expectedCommand) && _gameManager.PhaseCount == 0)
            {
                HandleCommandInput();

            }
            else if (IsCorrectCommand(controllerData, expectedCommand) && _gameManager.PhaseCount == 1)
            {
                if (_gameManager.SwitchPlayer &&  _gameManager.FirstPlayerRandomNum == 0)
                {
                    HandleSecondCommandInput();
                    _gameManager.SwitchPlayer = false;
                }
                else if (!_gameManager.SwitchPlayer && _gameManager.FirstPlayerRandomNum == 1)
                {
                    HandleSecondCommandInput();
                    _gameManager.SwitchPlayer = true;
                }
                else if(_gameManager.SwitchPlayer && _gameManager.FirstPlayerRandomNum == 1 || !_gameManager.SwitchPlayer && _gameManager.FirstPlayerRandomNum == 0 && !IsCoolDown)
                {
                    StartCoroutine(_1P.MissSecond(ControllerData));
                }

            }
            else if (IsCorrectCommand(controllerData, expectedCommand) && _gameManager.PhaseCount == 2 && _gameManager.OkPlayer2Thard && !_gameManager.OneTimeUp)
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

                    StartCoroutine(_1P.MissSecond(ControllerData));
                }
                if (_gameManager.PhaseCount == 2 && !IsCoolDown && !_gameManager.OneTimeUp && _gameManager.GameStart)
                {
               
                    StartCoroutine(_1P.MissThard(ControllerData));
                   
                }
            }
        }
    }
    private void ExecutePhaseCount()
    {

        StartCoroutine(MissCommand());
    }
    //private void ExecutePhaseCountSecond()
    //{

    //    StartCoroutine(MissSecondCommand());

    //}
        private void HandleCommandInput()
    {
        Vector3 firstCommand;
        if (_currentIndex == 0)
        {

            firstCommand = FirstImage.gameObject.transform.position;
            Instantiate(_gameManager.CommandEffect, firstCommand, Quaternion.identity);
            FirstImage.gameObject.SetActive(false);
        }
        else if (_currentIndex == 1)
        {
            firstCommand = SecondImage.gameObject.transform.position;
            Instantiate(_gameManager.CommandEffect, firstCommand, Quaternion.identity);
            SecondImage.gameObject.SetActive(false);
        }

        else if (_currentIndex == 2)
        {
            firstCommand = ThirdImage.gameObject.transform.position;
            Instantiate(_gameManager.CommandEffect, firstCommand, Quaternion.identity);
            ThirdImage.gameObject.SetActive(false);
        }
        _currentIndex++;

        if (_currentIndex >= _currentSequence.Count)
        {
            Debug.Log("1P Success! All commands entered correctly.");
        }
    }
    private void HandleSecondCommandInput()
    {
        Vector3 secondCommand;
        if (_gameManager.FirstPlayerRandomNum == 0)
        {

            if (_gameManager.RandomCommandNum == 0)
            {
                if (_currentIndex == 0 && _gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.ThreeCommand[1].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.ThreeCommand[1].gameObject.SetActive(false);

                    _gameManager.SwitchPlayer = false;

                    _currentIndex++;
                }

            }
            if (_gameManager.RandomCommandNum == 1)
            {
                if (_currentIndex == 0 && _gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.FourCommand[1].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.FourCommand[1].gameObject.SetActive(false);

                    _gameManager.SwitchPlayer = false;

                    _currentIndex++;
                }
                else if (_currentIndex == 1 && _gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.FourCommand[3].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.FourCommand[3].gameObject.SetActive(false);
                    _gameManager.ClearSecond = true;
                    _currentIndex++;
                }
            }
            if (_gameManager.RandomCommandNum == 2)
            {
                if (_currentIndex == 0 && _gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.FiveCommand[1].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.FiveCommand[1].gameObject.SetActive(false);

                    _gameManager.SwitchPlayer = false;

                    _currentIndex++;
                }
                else if (_currentIndex == 1 && _gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.FiveCommand[3].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.FiveCommand[3].gameObject.SetActive(false);

                    _gameManager.SwitchPlayer = false;

                    _currentIndex++;
                }
            }
        }
        if (_gameManager.FirstPlayerRandomNum == 1)
        {
            if (_gameManager.RandomCommandNum == 0)
            {
                if (_currentIndex == 0 && !_gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.ThreeCommand[0].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.ThreeCommand[0].gameObject.SetActive(false);

                    _gameManager.SwitchPlayer = true;
                    _currentIndex++;
                }
                else if (_currentIndex == 1 && !_gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.ThreeCommand[2].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.ThreeCommand[2].gameObject.SetActive(false);

                    _gameManager.ClearSecond = true;
                }

            }
            if (_gameManager.RandomCommandNum == 1)
            {
                if (_currentIndex == 0 && !_gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.FourCommand[0].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.FourCommand[0].gameObject.SetActive(false);

                    _gameManager.SwitchPlayer = true;
                    _currentIndex++;
                }
                else if (_currentIndex == 1 && !_gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.FourCommand[2].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.FourCommand[2].gameObject.SetActive(false);

                    _gameManager.SwitchPlayer = true;
                    _currentIndex++;
                }
            }
            if (_gameManager.RandomCommandNum == 2)
            {
                if (_currentIndex == 0 && !_gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.FiveCommand[0].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.FiveCommand[0].gameObject.SetActive(false);

                    _gameManager.SwitchPlayer = true;
                    _currentIndex++;
                }
                else if (_currentIndex == 1 && !_gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.FiveCommand[2].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.FiveCommand[2].gameObject.SetActive(false);

                    _gameManager.SwitchPlayer = true;
                    _currentIndex++;
                }
                else if (_currentIndex == 2 && !_gameManager.SwitchPlayer)
                {
                    secondCommand = _gameManager.FiveCommand[4].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, secondCommand, Quaternion.identity);
                    _gameManager.FiveCommand[4].gameObject.SetActive(false);
                    _gameManager.ClearSecond = true;
                }

            }
        }
    }
    private void HandleThardCommandInput()
    {
        Vector3 thardCommand;
        if (_gameManager.FirstPlayerRandomNum == 1)
        {
            if (_gameManager.RandomCommandNum == 0)
            {
                if (_currentIndex == 0)
                {
                    thardCommand = _gameManager.FiveCommandThard[0].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.FiveCommandThard[0].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 1)
                {
                    thardCommand = _gameManager.FiveCommandThard[2].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.FiveCommandThard[2].gameObject.SetActive(false);
                    _currentIndex++;
                }
                else if (_currentIndex == 2)
                {
                    thardCommand = _gameManager.FiveCommandThard[4].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.FiveCommandThard[4].gameObject.SetActive(false);
                    _currentIndex++;
                    _gameManager.ClearThard = true;
                }

            }
            if (_gameManager.RandomCommandNum == 1)
            {
                if (_currentIndex == 0)
                {
                    thardCommand = _gameManager.SixCommand[0].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SixCommand[0].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 1)
                {
                    thardCommand = _gameManager.SixCommand[2].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SixCommand[2].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 2)
                {
                    thardCommand = _gameManager.SixCommand[4].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SixCommand[4].gameObject.SetActive(false);

                    _currentIndex++;
                }
            }
            if (_gameManager.RandomCommandNum == 2)
            {
                if (_currentIndex == 0)
                {
                    thardCommand = _gameManager.SevenCommand[0].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SevenCommand[0].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 1)
                {
                    thardCommand = _gameManager.SevenCommand[2].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SevenCommand[2].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 2)
                {
                    thardCommand = _gameManager.SevenCommand[4].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SevenCommand[4].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 3)
                {
                    thardCommand = _gameManager.SevenCommand[6].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SevenCommand[6].gameObject.SetActive(false);

                    _currentIndex++;
                    _gameManager.ClearThard = true;
                }

            }
        }
        if (_gameManager.FirstPlayerRandomNum == 0)
        {
            if (_gameManager.RandomCommandNum == 0)
            {

                if (_currentIndex == 0)
                {
                    thardCommand = _gameManager.FiveCommandThard[1].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.FiveCommandThard[1].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 1)
                {
                    thardCommand = _gameManager.FiveCommandThard[3].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.FiveCommandThard[3].gameObject.SetActive(false);
                    _currentIndex++;
                }


            }
            if (_gameManager.RandomCommandNum == 1)
            {
                if (_currentIndex == 0)
                {
                    thardCommand = _gameManager.SixCommand[1].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SixCommand[1].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 1)
                {
                    thardCommand = _gameManager.SixCommand[3].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SixCommand[3].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 2)
                {
                    thardCommand = _gameManager.SixCommand[5].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SixCommand[5].gameObject.SetActive(false);

                    _currentIndex++;
                    _gameManager.ClearThard = true;
                }
            }
            if (_gameManager.RandomCommandNum == 2)
            {
                if (_currentIndex == 0)
                {
                    thardCommand = _gameManager.SevenCommand[1].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SevenCommand[1].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 1)
                {
                    thardCommand = _gameManager.SevenCommand[3].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
                    _gameManager.SevenCommand[3].gameObject.SetActive(false);

                    _currentIndex++;
                }
                else if (_currentIndex == 2)
                {
                    thardCommand = _gameManager.SevenCommand[5].gameObject.transform.position;
                    Instantiate(_gameManager.CommandEffect, thardCommand, Quaternion.identity);
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