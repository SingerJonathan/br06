using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameLoopController : MonoBehaviour
{
    public enum GameMode
    {
        Standard, KingOfTheHill, CaptureTheFlag
    }

    public GameObject MainMenuCanvasGameObject;
    public GameObject GameSetupCanvasGameObject;
    public GameObject LoadoutCanvasGameObject;
    public GameObject HUDCanvasGameObject;
    public GameObject RoundList;

    public Text RedWinsText;
    public Text BlueWinsText;
    public Text RedHPText;
    public Text BlueHPText;
    public Text RedScoreText;
    public Text BlueScoreText;
    public Text RoundTimeText;
    public Text NotificationText;
    public Text GameModeText;

    public GameObject KingOfTheHillObjects;
    public KingOfTheHillCollisionController HillCollisionController;
    public GameObject CaptureTheFlagObjects;

    public CharacterStatsController RedCharacterStatsController;
    public CharacterStatsController BlueCharacterStatsController;
    public CharacterLoadoutController RedCharacterLoadoutController;
    public CharacterAnimationController RedCharacterAnimationController;
    public CharacterLoadoutController BlueCharacterLoadoutController;
    public CharacterAnimationController BlueCharacterAnimationController;

    private Vector3 _initialRedCharacterPosition;
    private Vector3 _initialBlueCharacterPosition;
    private Quaternion _initialRedCharacterRotation;
    private Quaternion _initialBlueCharacterRotation;
    private int _initialRedCharacterHitPoints;
    private int _initialBlueCharacterHitPoints;

    private GameObject _confirmPanelGameObject;
    private Button _confirmPanelYesButton;
    private Button _quitButton;
    private Dropdown _roundsDropdown;
    private Dropdown _roundDurationDropdown;
    private int _maxRounds;
    private float _roundDuration;
    private int _currentRound;
    private int _redWins;
    private int _blueWins;
    private float _currentRoundTime;
    private float _countdown;
    private float _winCountdown;

    // DEVNOTE: Remove hard-coded value when changing game modes is implemented
    private List<Round> _rounds;
    private GameMode _currentGameMode = GameMode.KingOfTheHill;
    private float _redScore;
    private float _blueScore;

    private static string _timeFormat = "{0:D1}:{1:D2}";

    public GameMode CurrentGameMode
    {
        get
        {
            return _currentGameMode;
        }

        set
        {
            _currentGameMode = value;
        }
    }

    public float RedScore
    {
        get
        {
            return _redScore;
        }

        set
        {
            _redScore = value;
        }
    }

    public float BlueScore
    {
        get
        {
            return _blueScore;
        }

        set
        {
            _blueScore = value;
        }
    }

    public int CurrentRound
    {
        get
        {
            return _currentRound;
        }

        set
        {
            _currentRound = value;
            if (CurrentRound > 0)
            {
                CurrentGameMode = _rounds[_currentRound - 1].GameMode;
                GameModeText.text = _rounds[0].RoundModeDropdown.options[(int)CurrentGameMode].text;
            }
            else
            {
                CurrentGameMode = GameMode.Standard;
                GameModeText.text = "Standard";
            }
            SwitchGameModeObjects(_currentGameMode);
        }
    }

    private void DisableCharacterAnimations()
    {
        if (RedCharacterAnimationController.enabled)
        {
            RedCharacterAnimationController._animator.SetInteger("walking", 0);
            RedCharacterAnimationController._animator.SetBool("running", false);
            BlueCharacterAnimationController._animator.SetInteger("walking", 0);
            BlueCharacterAnimationController._animator.SetBool("running", false);
            RedCharacterAnimationController.enabled = false;
            BlueCharacterAnimationController.enabled = false;
        }
    }

    private void SetupGame()
    {
        _confirmPanelGameObject.SetActive(false);
        MainMenuCanvasGameObject.SetActive(false);
        GameSetupCanvasGameObject.SetActive(true);
        LoadoutCanvasGameObject.SetActive(false);
        HUDCanvasGameObject.SetActive(false);
        NotificationText.gameObject.SetActive(false);
    }

    public void CancelNewGame()
    {
        MainMenuCanvasGameObject.SetActive(true);
        GameSetupCanvasGameObject.SetActive(false);
        LoadoutCanvasGameObject.SetActive(false);
        HUDCanvasGameObject.SetActive(false);
        NotificationText.gameObject.SetActive(false);
    }

    public void NewGame()
    {
        CurrentRound = 1;
        _redWins = 0;
        _blueWins = 0;
        _maxRounds = Int32.Parse(_roundsDropdown.options[_roundsDropdown.value].text);
        _roundDuration = 60*(_roundDurationDropdown.value + 1);
        MainMenuCanvasGameObject.SetActive(false);
        GameSetupCanvasGameObject.SetActive(false);
        LoadoutCanvasGameObject.SetActive(true);
        HUDCanvasGameObject.SetActive(true);
        NotificationText.gameObject.SetActive(false);
        RedWinsText.text = "" + _redWins;
        BlueWinsText.text = "" + _blueWins;
        TimeSpan time = TimeSpan.FromSeconds(_roundDuration);
        RoundTimeText.text = string.Format(_timeFormat, time.Minutes, time.Seconds);
        _quitButton.interactable = true;
        RedCharacterStatsController.HitPoints = _initialRedCharacterHitPoints;
        BlueCharacterStatsController.HitPoints = _initialBlueCharacterHitPoints;
        RedScore = 0;
        BlueScore = 0;
        RedHPText.text = "" + RedCharacterStatsController.HitPoints;
        BlueHPText.text = "" + BlueCharacterStatsController.HitPoints;
        RedCharacterLoadoutController.transform.SetPositionAndRotation(_initialRedCharacterPosition, _initialRedCharacterRotation);
        BlueCharacterLoadoutController.transform.SetPositionAndRotation(_initialBlueCharacterPosition, _initialBlueCharacterRotation);
        RedCharacterLoadoutController.ReadyToggle.isOn = false;
        BlueCharacterLoadoutController.ReadyToggle.isOn = false;
        RedCharacterAnimationController.enabled = false;
        BlueCharacterAnimationController.enabled = false;
        _confirmPanelYesButton.onClick.RemoveAllListeners();
        _confirmPanelGameObject.SetActive(false);
    }

    private void QuitToMainMenu()
    {
        _confirmPanelGameObject.SetActive(false); 
        CurrentRound = 0;
        _quitButton.interactable = false;
        RedCharacterAnimationController.enabled = false;
        BlueCharacterAnimationController.enabled = false;
        MainMenuCanvasGameObject.SetActive(true);
        GameSetupCanvasGameObject.SetActive(false);
        LoadoutCanvasGameObject.SetActive(false);
        HUDCanvasGameObject.SetActive(false);
        _confirmPanelYesButton.onClick.RemoveAllListeners();
        _confirmPanelGameObject.SetActive(false);
        RedCharacterLoadoutController.transform.SetPositionAndRotation(_initialRedCharacterPosition, _initialRedCharacterRotation);
        BlueCharacterLoadoutController.transform.SetPositionAndRotation(_initialBlueCharacterPosition, _initialBlueCharacterRotation);
        DisableCharacterAnimations();
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    private void SwitchGameModeObjects(GameMode gameMode)
    {
        switch (gameMode)
        {
            case GameMode.Standard:
                KingOfTheHillObjects.SetActive(false);
                CaptureTheFlagObjects.SetActive(false);
                break;
            case GameMode.KingOfTheHill:
                KingOfTheHillObjects.SetActive(true);
                CaptureTheFlagObjects.SetActive(false);
                break;
            case GameMode.CaptureTheFlag:
                KingOfTheHillObjects.SetActive(false);
                CaptureTheFlagObjects.SetActive(true);
                break;
        }
    }

    public void OpenConfirmDialogue(string context)
    {
        _confirmPanelGameObject.SetActive(true);
        UnityAction call;
        switch (context)
        {
            case "New":
                call = SetupGame;
                break;
            case "Quit":
                call = QuitToMainMenu;
                break;
            case "Exit":
                call = ExitGame;
                break;
            default:
                call = delegate { };
                break;
        }
        _confirmPanelYesButton.onClick.AddListener(call);
    }

    public void CloseConfirmDialogue()
    {
        _confirmPanelGameObject.SetActive(false);
    }

    public void RefreshRoundList()
    {
        _maxRounds = Int32.Parse(_roundsDropdown.options[_roundsDropdown.value].text);
        int roundGameObjects = RoundList.transform.childCount;
        while (roundGameObjects < _maxRounds)
        {
            GameObject roundGameObject = (GameObject)Instantiate(Resources.Load("Round"));
            roundGameObject.transform.SetParent(RoundList.transform);
            _rounds.Add(roundGameObject.GetComponent<Round>());
            roundGameObjects = RoundList.transform.childCount;
        }
        while (_maxRounds < roundGameObjects)
        {
            GameObject roundGameObject = RoundList.transform.GetChild(roundGameObjects - 1).gameObject;
            _rounds.RemoveAt(roundGameObjects - 1);
            Destroy(roundGameObject);
            roundGameObjects--;
        }
    }

    void Start()
    {
        _quitButton = MainMenuCanvasGameObject.transform.Find("Main Panel").Find("Quit Button").GetComponent<Button>();
        _roundsDropdown = GameSetupCanvasGameObject.transform.Find("Panel").Find("Rounds Dropdown").GetComponent<Dropdown>();
        _roundDurationDropdown = GameSetupCanvasGameObject.transform.Find("Panel").Find("Minutes Dropdown").GetComponent<Dropdown>();
        _confirmPanelGameObject = MainMenuCanvasGameObject.transform.Find("Confirm Panel").gameObject;
        _confirmPanelYesButton = _confirmPanelGameObject.transform.Find("Yes Button").GetComponent<Button>();
        _initialRedCharacterPosition = RedCharacterLoadoutController.transform.position;
        _initialBlueCharacterPosition = BlueCharacterLoadoutController.transform.position;
        _initialRedCharacterRotation = RedCharacterLoadoutController.transform.rotation;
        _initialBlueCharacterRotation = BlueCharacterLoadoutController.transform.rotation;
        _initialRedCharacterHitPoints = RedCharacterStatsController.HitPoints;
        _initialBlueCharacterHitPoints = BlueCharacterStatsController.HitPoints;
        _rounds = new List<Round>
        {
            RoundList.transform.GetChild(0).GetComponent<Round>()
        };
    }

    void Update()
    {
        // Game is in progress
        if (CurrentRound > 0)
        {
            // In Loadout menu
            if (LoadoutCanvasGameObject.activeInHierarchy)
            {
                if (RedCharacterLoadoutController.Ready && BlueCharacterLoadoutController.Ready)
                {
                    NotificationText.gameObject.SetActive(true);
                    _countdown = 3;
                    NotificationText.text = "" + 3;
                    LoadoutCanvasGameObject.SetActive(false);
                }
            }
            // Start countdown after both players are ready
            else if (_countdown > 0.0f && !MainMenuCanvasGameObject.activeInHierarchy)
            {
                _countdown -= Time.deltaTime;
                NotificationText.text = "" + (int)(_countdown + 1);
                if (_countdown <= 0.0f)
                {
                    NotificationText.gameObject.SetActive(false);
                    RedCharacterAnimationController.enabled = true;
                    BlueCharacterAnimationController.enabled = true;
                    _currentRoundTime = _roundDuration;
                }
            }
            // Countdown finished, round started
            else if (_currentRoundTime > 0.0f && !NotificationText.gameObject.activeInHierarchy && !MainMenuCanvasGameObject.activeInHierarchy)
            {
                RedHPText.text = "" + RedCharacterStatsController.HitPoints;
                BlueHPText.text = "" + BlueCharacterStatsController.HitPoints;
                _currentRoundTime -= Time.deltaTime;
                TimeSpan time = TimeSpan.FromSeconds(_currentRoundTime);
                switch (_currentGameMode)
                {
                    case GameMode.Standard:
                        _redScore = RedCharacterStatsController.HitPoints;
                        _blueScore = BlueCharacterStatsController.HitPoints;
                        break;
                    case GameMode.KingOfTheHill:
                        if (HillCollisionController.RedColliding && HillCollisionController.BlueColliding)
                        { }
                        else if (HillCollisionController.RedColliding)
                            _redScore += Time.deltaTime;
                        else if (HillCollisionController.BlueColliding)
                            _blueScore += Time.deltaTime;
                        break;
                    case GameMode.CaptureTheFlag:
                        break;
                }
                RoundTimeText.text = string.Format(_timeFormat, time.Minutes, time.Seconds);
                RedScoreText.text = "" + (int)_redScore;
                BlueScoreText.text = "" + (int)_blueScore;
                // Round end conditions
                if (_currentRoundTime <= 0.0f || RedCharacterStatsController.HitPoints == 0 || BlueCharacterStatsController.HitPoints == 0)
                {
                    DisableCharacterAnimations();
                    if ((int)_redScore > (int)_blueScore)
                    {
                        _redWins++;
                        RedWinsText.text = "" + _redWins;
                        NotificationText.text = string.Format("ROUND {0}\nRED WINS", CurrentRound);
                    }
                    else if ((int)_blueScore > (int)_redScore)
                    {
                        _blueWins++;
                        BlueWinsText.text = "" + _blueWins;
                        NotificationText.text = string.Format("ROUND {0}\nBLUE WINS", CurrentRound);
                    }
                    else
                    {
                        NotificationText.text = string.Format("ROUND {0}\nDRAW", CurrentRound);
                    }
                    _winCountdown = 5;
                    NotificationText.gameObject.SetActive(true);
                }
            }
            // Message after round or game
            else if (_winCountdown > 0.0f && !MainMenuCanvasGameObject.activeInHierarchy)
            {
                _winCountdown -= Time.deltaTime;
                if (_winCountdown <= 0.0f)
                {
                    NotificationText.gameObject.SetActive(false);
                    // After message after game
                    if (NotificationText.text.Contains("GAME OVER"))
                    {
                        QuitToMainMenu();
                    }
                    // After message after round
                    else if (!(CurrentRound == _maxRounds || _redWins > _maxRounds / 2 || _blueWins > _maxRounds / 2))
                    {
                        RedCharacterStatsController.HitPoints = _initialRedCharacterHitPoints;
                        BlueCharacterStatsController.HitPoints = _initialBlueCharacterHitPoints;
                        RedScore = 0;
                        BlueScore = 0;
                        CurrentRound++;
                        TimeSpan newTime = TimeSpan.FromSeconds(_roundDuration);
                        RoundTimeText.text = string.Format(_timeFormat, newTime.Minutes, newTime.Seconds);
                        LoadoutCanvasGameObject.SetActive(true);
                        RedCharacterLoadoutController.ReadyToggle.isOn = false;
                        BlueCharacterLoadoutController.ReadyToggle.isOn = false;
                        RedCharacterLoadoutController.transform.SetPositionAndRotation(_initialRedCharacterPosition, _initialRedCharacterRotation);
                        BlueCharacterLoadoutController.transform.SetPositionAndRotation(_initialBlueCharacterPosition, _initialBlueCharacterRotation);
                    }
                    // Game end conditions
                    else
                    {
                        if (_redWins > _maxRounds / 2 || CurrentRound == _maxRounds && _redWins > _blueWins)
                            NotificationText.text = "GAME OVER\nRED WINS";
                        else if (_blueWins > _maxRounds / 2 || CurrentRound == _maxRounds && _blueWins > _redWins)
                            NotificationText.text = "GAME OVER\nBLUE WINS";
                        else
                            NotificationText.text = "GAME OVER\nDRAW";
                        _winCountdown = 5;
                        NotificationText.gameObject.SetActive(true);
                    }
                }
            }
            if (Input.GetButtonDown("Main Menu"))
            {
                if (!MainMenuCanvasGameObject.activeInHierarchy)
                {
                    MainMenuCanvasGameObject.SetActive(true);
                    DisableCharacterAnimations();
                }
                else
                {
                    MainMenuCanvasGameObject.SetActive(false);
                    RedCharacterAnimationController.enabled = true;
                    BlueCharacterAnimationController.enabled = true;
                }
            }
        }
    }
}
