using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameLoopController : MonoBehaviour
{
    public enum GameMode
    {
        Standard, KingOfTheHill, CaptureTheFlag
    }

    public EventSystem EventSystem;

    public Animator CameraAnimator;
    public Vector3 RedCharacterGamePosition;
    public Vector3 BlueCharacterGamePosition;
    public Vector3 RedCharacterGameRotation;
    public Vector3 BlueCharacterGameRotation;

    public GameObject MainMenuCanvasGameObject;
    public GameObject GameSetupCanvasGameObject;
    public GameObject LoadoutCanvasGameObject;
    public GameObject HUDCanvasGameObject;
    public GameObject RoundList;

    public Text RedWinsText;
    public Text BlueWinsText;
    public Text RedHPText;
    public Text BlueHPText;
    public Slider RedHPSlider;
    public Slider BlueHPSlider;
    public Text RedScoreText;
    public Text BlueScoreText;
    public Text RoundTimeText;
    public Text RedDodgeCooldownText;
    public Text BlueDodgeCooldownText;
    public Image RedDodgeDarkMask;
    public Image BlueDodgeDarkMask;
    public Text RedNotificationText;
    public Text BlueNotificationText;
    public Text NotificationText;
    public Text GameModeText;

    public GameObject[,] LoadoutStatePanels = new GameObject[2,6];
    public GameObject RedState1Panel;
    public GameObject RedState2Panel;
    public GameObject RedState5Panel;
    public GameObject BlueState1Panel;
    public GameObject BlueState2Panel;
    public GameObject BlueState5Panel;

    public HitboxTriggerController[] RedHitboxControllers;
    public HitboxTriggerController[] BlueHitboxControllers;

    public GameObject KingOfTheHillObjects;
    public KingOfTheHillCollisionController HillCollisionController;
    public GameObject CaptureTheFlagObjects;
    public CaptureTheFlagTriggerController RedCaptureTheFlagTriggerController;
    public CaptureTheFlagTriggerController BlueCaptureTheFlagTriggerController;
    public FlagController FlagController;
    public Text FlagRespawnText;

    public RandomEnvironmentController RandomEnvironmentController;

    public CharacterStatsController RedCharacterStatsController;
    public CharacterStatsController BlueCharacterStatsController;
    public CharacterAnimationController RedCharacterAnimationController;
    public CharacterAnimationController BlueCharacterAnimationController;
    public CharacterLoadoutController[] CharacterLoadoutControllers;

    public Dropdown[] MainWeaponDropdowns;
    public Dropdown[] OffhandWeaponDropdowns;

    private Vector3 _initialFlagPosition;
    private Quaternion _initialFlagRotation;
    private Vector3 _initialRedCharacterPosition;
    private Vector3 _initialBlueCharacterPosition;
    private Quaternion _initialRedCharacterRotation;
    private Quaternion _initialBlueCharacterRotation;

    private Vector3 _targetRedCharacterPosition;
    private Vector3 _targetBlueCharacterPosition;
    private Vector3 _targetRedCharacterRotation;
    private Vector3 _targetBlueCharacterRotation;

    private CanvasGroup _loadoutCanvasGroup;
    private GameObject _newGameButtonGameObject;
    private GameObject _mainMenuPanelGameObject;
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
    private float _redCountdown;
    private float _blueCountdown;
    
    private List<Round> _rounds;
    private GameMode _currentGameMode = GameMode.Standard;
    private float _redScore;
    private float _blueScore;

    private bool[] _loadoutAxisTriggered = new bool[2];
    private int[] _loadoutNavigationStates = new int[2];

    private static string _timeFormat = "{0:D1}:{1:D2}";

    private static float _respawnTime = 3;

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
            switch (_currentGameMode)
            {
                case GameMode.Standard:
                    RedScoreText.transform.parent.gameObject.SetActive(false);
                    BlueScoreText.transform.parent.gameObject.SetActive(false);
                    break;
                default:
                    RedScoreText.transform.parent.gameObject.SetActive(true);
                    BlueScoreText.transform.parent.gameObject.SetActive(true);
                    break;
            }
        }
    }

    public Vector3 InitialFlagPosition
    {
        get
        {
            return _initialFlagPosition;
        }

        set
        {
            _initialFlagPosition = value;
        }
    }

    public Quaternion InitialFlagRotation
    {
        get
        {
            return _initialFlagRotation;
        }

        set
        {
            _initialFlagRotation = value;
        }
    }

    private void DisableCharacterAnimations()
    {
        if (RedCharacterAnimationController.enabled)
        {
            RedCharacterAnimationController._animator.SetBool("running", false);
            BlueCharacterAnimationController._animator.SetBool("running", false);
            RedCharacterAnimationController.enabled = false;
            BlueCharacterAnimationController.enabled = false;
        }
    }

    private void SetupGame()
    {
        //if (CurrentRound > 0 && (_currentRoundTime > 0.0f || _countdown > 0.0f))
        StartTransition(false, true);
        CurrentRound = 0;
        _confirmPanelGameObject.SetActive(false);
        MainMenuCanvasGameObject.SetActive(false);
        GameSetupCanvasGameObject.SetActive(true);
        LoadoutCanvasGameObject.SetActive(false);
        HUDCanvasGameObject.SetActive(false);
        NotificationText.gameObject.SetActive(false);
        LoadoutCanvasGameObject.GetComponent<CanvasGroup>().interactable = true;
        EventSystem.SetSelectedGameObject(_roundsDropdown.gameObject);
    }

    public void CancelNewGame()
    {
        MainMenuCanvasGameObject.SetActive(true);
        GameSetupCanvasGameObject.SetActive(false);
        LoadoutCanvasGameObject.SetActive(false);
        HUDCanvasGameObject.SetActive(false);
        NotificationText.gameObject.SetActive(false);
        _mainMenuPanelGameObject.GetComponent<CanvasGroup>().interactable = true;
        EventSystem.SetSelectedGameObject(_newGameButtonGameObject.gameObject);
    }

    public void NewGame()
    {
        _loadoutNavigationStates[0] = 0;
        _loadoutNavigationStates[1] = 0;
        HillCollisionController.RedColliding = false;
        HillCollisionController.BlueColliding = false;
        HillCollisionController.DetermineMaterial();
        RedCharacterStatsController.gameObject.SetActive(true);
        _redCountdown = 0;
        RedNotificationText.gameObject.SetActive(false);
        BlueCharacterStatsController.gameObject.SetActive(true);
        _blueCountdown = 0;
        BlueNotificationText.gameObject.SetActive(false);
        CurrentRound = 1;
        RandomEnvironmentController.SpawnRandomEnvironmentObjects(CurrentGameMode);
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
        RedCharacterStatsController.HitPoints = RedCharacterStatsController.MaxHitPoints;
        BlueCharacterStatsController.HitPoints = BlueCharacterStatsController.MaxHitPoints;
        RedCharacterStatsController.Potions = CharacterStatsController.InitialPotions;
        BlueCharacterStatsController.Potions = CharacterStatsController.InitialPotions;
        RedCharacterAnimationController.DodgeCountdown = 0;
        BlueCharacterAnimationController.DodgeCountdown = 0;
        RedDodgeCooldownText.gameObject.SetActive(false);
        BlueDodgeCooldownText.gameObject.SetActive(false);
        RedScore = 0;
        BlueScore = 0;
        RedHPText.text = "" + RedCharacterStatsController.HitPoints;
        BlueHPText.text = "" + BlueCharacterStatsController.HitPoints;
        RedHPSlider.value = ((float)RedCharacterStatsController.HitPoints / RedCharacterStatsController.MaxHitPoints) * 100;
        BlueHPSlider.value = ((float)BlueCharacterStatsController.HitPoints / BlueCharacterStatsController.MaxHitPoints) * 100;
        CharacterLoadoutControllers[0].transform.SetPositionAndRotation(_initialRedCharacterPosition, _initialRedCharacterRotation);
        CharacterLoadoutControllers[1].transform.SetPositionAndRotation(_initialBlueCharacterPosition, _initialBlueCharacterRotation);
        FlagController.transform.SetParent(CaptureTheFlagObjects.transform);
        FlagController.transform.localPosition = InitialFlagPosition;
        FlagController.transform.rotation = InitialFlagRotation;
        FlagController.ResetMaterial();
        FlagController.GetComponent<CapsuleCollider>().enabled = true;
        CharacterLoadoutControllers[0].ReadyToggle.isOn = false;
        CharacterLoadoutControllers[1].ReadyToggle.isOn = false;
        _confirmPanelYesButton.onClick.RemoveAllListeners();
        _confirmPanelGameObject.SetActive(false);
        _loadoutCanvasGroup.interactable = true;
        RedDodgeDarkMask.fillAmount = 0;
        BlueDodgeDarkMask.fillAmount = 0;
        DisableCharacterAnimations();
        foreach (CharacterLoadoutController loadoutController in CharacterLoadoutControllers)
            loadoutController.SetAbilitiesActive(false);
    }

    private void QuitToMainMenu()
    {
        StartTransition(false, true);
        _confirmPanelGameObject.SetActive(false);
        _loadoutCanvasGroup.interactable = false;
        CurrentRound = 0;
        _quitButton.interactable = false;
        RedCharacterAnimationController.enabled = false;
        BlueCharacterAnimationController.enabled = false;
        MainMenuCanvasGameObject.SetActive(true);
        GameSetupCanvasGameObject.SetActive(false);
        LoadoutCanvasGameObject.SetActive(false);
        HUDCanvasGameObject.SetActive(false);
        _mainMenuPanelGameObject.GetComponent<CanvasGroup>().interactable = true;
        EventSystem.SetSelectedGameObject(_newGameButtonGameObject.gameObject);
        _confirmPanelYesButton.onClick.RemoveAllListeners();
        _confirmPanelGameObject.SetActive(false);
        //CharacterLoadoutControllers[0].transform.SetPositionAndRotation(_initialRedCharacterPosition, _initialRedCharacterRotation);
        //CharacterLoadoutControllers[1].transform.SetPositionAndRotation(_initialBlueCharacterPosition, _initialBlueCharacterRotation);
        FlagController.transform.SetParent(CaptureTheFlagObjects.transform);
        FlagController.transform.localPosition = InitialFlagPosition;
        FlagController.transform.rotation = InitialFlagRotation;
        FlagController.ResetMaterial();
        FlagController.GetComponent<CapsuleCollider>().enabled = true;
        DisableCharacterAnimations();
        StartCoroutine(RandomEnvironmentController.SinkAndDeleteObjects());
        foreach (CharacterLoadoutController loadoutController in CharacterLoadoutControllers)
            loadoutController.SetAbilitiesActive(false);
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
                StartCoroutine(SinkAndRaiseObjects(KingOfTheHillObjects));
                StartCoroutine(SinkAndRaiseObjects(CaptureTheFlagObjects));
                break;
            case GameMode.KingOfTheHill:
                StartCoroutine(SinkAndRaiseObjects(CaptureTheFlagObjects, KingOfTheHillObjects));
                break;
            case GameMode.CaptureTheFlag:
                StartCoroutine(SinkAndRaiseObjects(KingOfTheHillObjects, CaptureTheFlagObjects));
                break;
        }
    }

    private IEnumerator SinkAndRaiseObjects(GameObject objectToSink, GameObject objectToRaise = null)
    {
        if (objectToSink != null && !objectToSink.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Sink"))
        {
            objectToSink.GetComponent<Animator>().SetTrigger("sink");
            objectToSink.GetComponent<Animator>().ResetTrigger("raise");
            yield return new WaitForSeconds(3);
            objectToSink.SetActive(false);
        }
        if (objectToRaise != null && !objectToSink.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Raise"))
        {
            objectToRaise.SetActive(true);
            objectToRaise.GetComponent<Animator>().SetTrigger("raise");
            objectToSink.GetComponent<Animator>().ResetTrigger("sink");
        }
    }

    public void OpenConfirmDialogue(string context)
    {
        _confirmPanelGameObject.SetActive(true);
        _mainMenuPanelGameObject.GetComponent<CanvasGroup>().interactable = false;
        _confirmPanelGameObject.GetComponent<CanvasGroup>().interactable = true;
        EventSystem.SetSelectedGameObject(_confirmPanelYesButton.gameObject);
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
        _mainMenuPanelGameObject.GetComponent<CanvasGroup>().interactable = true;
        EventSystem.SetSelectedGameObject(_newGameButtonGameObject.gameObject);
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

    private void SwitchActiveLoadoutStatePanel(int playerNumber, int state)
    {
        for (int index = 0; index < LoadoutStatePanels.GetLength(1); index++)
            if (LoadoutStatePanels[playerNumber, index] != null)
                if (index == state)
                    LoadoutStatePanels[playerNumber, index].SetActive(true);
                else
                    LoadoutStatePanels[playerNumber, index].SetActive(false);
    }

    private void HandleLoadoutInput(int playerNumber)
    {
        // State 1: Weapon Dropdown
        if (_loadoutNavigationStates[playerNumber] == 1)
        {
            if (Input.GetAxisRaw("Vertical" + (playerNumber + 1)) == 0 && Input.GetAxisRaw("VerticalAlt" + (playerNumber + 1)) == 0)
                _loadoutAxisTriggered[playerNumber] = false;
            if (Input.GetAxisRaw("Vertical" + (playerNumber + 1)) < 0 || Input.GetAxisRaw("VerticalAlt" + (playerNumber + 1)) < 0)
            {
                if (!_loadoutAxisTriggered[playerNumber])
                {
                    MainWeaponDropdowns[playerNumber].value++;
                    _loadoutAxisTriggered[playerNumber] = true;
                }
            }
            else if (Input.GetAxisRaw("Vertical" + (playerNumber + 1)) > 0 || Input.GetAxisRaw("VerticalAlt" + (playerNumber + 1)) > 0)
            {
                if (!_loadoutAxisTriggered[playerNumber])
                {
                    MainWeaponDropdowns[playerNumber].value--;
                    _loadoutAxisTriggered[playerNumber] = true;
                }
            }
            if (Input.GetButtonDown("Submit" + (playerNumber + 1)) || Input.GetButtonDown("SubmitAlt" + (playerNumber + 1)))
                if (OffhandWeaponDropdowns[playerNumber].interactable)
                {
                    _loadoutNavigationStates[playerNumber] = 2;
                    SwitchActiveLoadoutStatePanel(playerNumber, 2);
                }
                else // Two-handed
                {
                    _loadoutNavigationStates[playerNumber] = 5;
                    SwitchActiveLoadoutStatePanel(playerNumber, 5);
                }
        }
        // State 2: Offhand Weapon Dropdown
        else if (_loadoutNavigationStates[playerNumber] == 2)
        {
            if (Input.GetAxisRaw("Vertical" + (playerNumber + 1)) == 0 && Input.GetAxisRaw("VerticalAlt" + (playerNumber + 1)) == 0)
                _loadoutAxisTriggered[playerNumber] = false;
            if (Input.GetAxisRaw("Vertical" + (playerNumber + 1)) < 0 || Input.GetAxisRaw("VerticalAlt" + (playerNumber + 1)) < 0)
            {
                if (!_loadoutAxisTriggered[playerNumber])
                {
                    OffhandWeaponDropdowns[playerNumber].value++;
                    _loadoutAxisTriggered[playerNumber] = true;
                }
            }
            else if (Input.GetAxisRaw("Vertical" + (playerNumber + 1)) > 0 || Input.GetAxisRaw("VerticalAlt" + (playerNumber + 1)) > 0)
            {
                if (!_loadoutAxisTriggered[playerNumber])
                {
                    OffhandWeaponDropdowns[playerNumber].value--;
                    _loadoutAxisTriggered[playerNumber] = true;
                }
            }
            if (Input.GetButtonDown("Submit" + (playerNumber + 1)) || Input.GetButtonDown("SubmitAlt" + (playerNumber + 1)))
            {
                _loadoutNavigationStates[playerNumber] = 5;
                SwitchActiveLoadoutStatePanel(playerNumber, 5);
            }
            else if (Input.GetButtonDown("Cancel" + (playerNumber + 1)) || Input.GetButtonDown("CancelAlt" + (playerNumber + 1)))
            {
                _loadoutNavigationStates[playerNumber] = 1;
                SwitchActiveLoadoutStatePanel(playerNumber, 1);
            }
        }
        // State 3: Detailed Ability View
        // State 4: Ability Mutation
        // State 5: Ready
        else if (_loadoutNavigationStates[playerNumber] == 5)
        {
            if (Input.GetButtonDown("Submit" + (playerNumber + 1)) || Input.GetButtonDown("SubmitAlt" + (playerNumber + 1)))
                CharacterLoadoutControllers[playerNumber].ReadyToggle.isOn = true;
            else if (Input.GetButtonDown("Cancel" + (playerNumber + 1)) || Input.GetButtonDown("CancelAlt" + (playerNumber + 1)))
            {
                if (CharacterLoadoutControllers[playerNumber].ReadyToggle.isOn)
                    CharacterLoadoutControllers[playerNumber].ReadyToggle.isOn = false;
                else if (CurrentRound == 1)
                {
                    if (OffhandWeaponDropdowns[playerNumber].interactable)
                    {
                        _loadoutNavigationStates[playerNumber] = 2;
                        SwitchActiveLoadoutStatePanel(playerNumber, 2);
                    }
                    else // Two-handed
                    {
                        _loadoutNavigationStates[playerNumber] = 1;
                        SwitchActiveLoadoutStatePanel(playerNumber, 1);
                    }
                }
            }
        }
    }

    void Start()
    {
        LoadoutStatePanels[0, 1] = RedState1Panel;
        LoadoutStatePanels[0, 2] = RedState2Panel;
        LoadoutStatePanels[0, 5] = RedState5Panel;
        LoadoutStatePanels[1, 1] = BlueState1Panel;
        LoadoutStatePanels[1, 2] = BlueState2Panel;
        LoadoutStatePanels[1, 5] = BlueState5Panel;
        _quitButton = MainMenuCanvasGameObject.transform.Find("Main Panel/Quit Button").GetComponent<Button>();
        _roundsDropdown = GameSetupCanvasGameObject.transform.Find("Panel/Rounds Dropdown").GetComponent<Dropdown>();
        _roundDurationDropdown = GameSetupCanvasGameObject.transform.Find("Panel/Minutes Dropdown").GetComponent<Dropdown>();
        _mainMenuPanelGameObject = MainMenuCanvasGameObject.transform.Find("Main Panel").gameObject;
        _newGameButtonGameObject = _mainMenuPanelGameObject.transform.Find("New Game Button").gameObject;
        _confirmPanelGameObject = MainMenuCanvasGameObject.transform.Find("Confirm Panel").gameObject;
        _confirmPanelYesButton = _confirmPanelGameObject.transform.Find("Buttons/Yes Button").GetComponent<Button>();
        _loadoutCanvasGroup = LoadoutCanvasGameObject.GetComponent<CanvasGroup>();
        _initialRedCharacterPosition = CharacterLoadoutControllers[0].transform.position;
        _initialBlueCharacterPosition = CharacterLoadoutControllers[1].transform.position;
        _initialRedCharacterRotation = CharacterLoadoutControllers[0].transform.rotation;
        _initialBlueCharacterRotation = CharacterLoadoutControllers[1].transform.rotation;
        InitialFlagPosition = FlagController.transform.localPosition;
        InitialFlagRotation = FlagController.transform.rotation;
        foreach (CharacterLoadoutController loadoutController in CharacterLoadoutControllers)
            loadoutController.SetAbilitiesActive(false);
        _rounds = new List<Round>
        {
            RoundList.transform.GetChild(0).GetComponent<Round>()
        };
    }

    private bool _transitionActive;

    private void StartTransition(bool toGame, bool animateCamera)
    {
        if ((toGame && !CameraAnimator.GetCurrentAnimatorStateInfo(0).IsName("Menu To Game")) || (toGame && !animateCamera))
        {
            _targetRedCharacterPosition = RedCharacterGamePosition;
            _targetRedCharacterRotation = RedCharacterGameRotation;
            _targetBlueCharacterPosition = BlueCharacterGamePosition;
            _targetBlueCharacterRotation = BlueCharacterGameRotation;
            if (animateCamera)
                CameraAnimator.SetTrigger("animate");
            _transitionActive = true;
            RedCharacterStatsController.GetComponent<Animator>().SetBool("running", true);
            BlueCharacterStatsController.GetComponent<Animator>().SetBool("running", true);
        }
        else if ((!toGame && CameraAnimator.GetCurrentAnimatorStateInfo(0).IsName("Menu To Game")) || (!toGame && !animateCamera))
        {
            _targetRedCharacterPosition = _initialRedCharacterPosition;
            _targetRedCharacterRotation = _initialRedCharacterRotation.eulerAngles;
            _targetBlueCharacterPosition = _initialBlueCharacterPosition;
            _targetBlueCharacterRotation = _initialBlueCharacterRotation.eulerAngles;
            if (animateCamera)
                CameraAnimator.SetTrigger("animate");
            _transitionActive = true;
            RedCharacterStatsController.GetComponent<Animator>().SetBool("running", true);
            BlueCharacterStatsController.GetComponent<Animator>().SetBool("running", true);
        }
    }

    void Update()
    {
        if (_transitionActive)
        {
            float step = (RedCharacterAnimationController.RunSpeed*1.5f) * Time.deltaTime;
            RedCharacterStatsController.transform.position = Vector3.MoveTowards(RedCharacterStatsController.transform.position, _targetRedCharacterPosition, step);
            BlueCharacterStatsController.transform.position = Vector3.MoveTowards(BlueCharacterStatsController.transform.position, _targetBlueCharacterPosition, step);
            Vector3 redDir;
            Vector3 blueDir;
            if (RedCharacterStatsController.transform.position == _targetRedCharacterPosition)
            {
                RedCharacterStatsController.GetComponent<Animator>().SetBool("running", false);
                redDir = Vector3.RotateTowards(RedCharacterStatsController.transform.forward, _targetBlueCharacterPosition - _targetRedCharacterPosition, step, 0.0F);
            }
            else
                redDir = Vector3.RotateTowards(RedCharacterStatsController.transform.forward, _targetRedCharacterPosition - RedCharacterStatsController.transform.position, step, 0.0F);
            if (BlueCharacterStatsController.transform.position == _targetBlueCharacterPosition)
            {
                BlueCharacterStatsController.GetComponent<Animator>().SetBool("running", false);
                blueDir = Vector3.RotateTowards(BlueCharacterStatsController.transform.forward, _targetRedCharacterPosition - _targetBlueCharacterPosition, step, 0.0F);
            }
            else
                blueDir = Vector3.RotateTowards(BlueCharacterStatsController.transform.forward, _targetBlueCharacterPosition - BlueCharacterStatsController.transform.position, step, 0.0F);
            RedCharacterStatsController.transform.rotation = Quaternion.LookRotation(redDir);
            BlueCharacterStatsController.transform.rotation = Quaternion.LookRotation(blueDir);
            if (RedCharacterStatsController.transform.eulerAngles.y >= _targetRedCharacterRotation.y - 0.03f && RedCharacterStatsController.transform.eulerAngles.y <= _targetRedCharacterRotation.y + 0.03f
            && BlueCharacterStatsController.transform.eulerAngles.y >= _targetBlueCharacterRotation.y - 0.03f && BlueCharacterStatsController.transform.eulerAngles.y <= _targetBlueCharacterRotation.y + 0.03f)
            {
                _transitionActive = false;
            }
        }
        // Game is in progress
        if (CurrentRound > 0)
        {
            if (_loadoutCanvasGroup.interactable)
            {
                if (!(_loadoutNavigationStates[0] == 0 && (Input.GetButtonDown("Submit1") || Input.GetButtonDown("SubmitAlt1"))))
                {
                    HandleLoadoutInput(0);
                    HandleLoadoutInput(1);
                }
                else
                {
                    _loadoutNavigationStates[0] = 1;
                    _loadoutNavigationStates[1] = 1;
                    SwitchActiveLoadoutStatePanel(0, 1);
                    SwitchActiveLoadoutStatePanel(1, 1);
                }
            }
            // Listen for ready up
            if (LoadoutCanvasGameObject.activeInHierarchy)
            {
                if (CharacterLoadoutControllers[0].Ready && CharacterLoadoutControllers[1].Ready)
                {
                    _countdown = 3;
                    NotificationText.text = "" + 3;
                    LoadoutCanvasGameObject.SetActive(false);
                    if (CurrentRound == 1)
                        StartTransition(true, true);
                    _loadoutCanvasGroup.interactable = false;
                }
            }
            // Start countdown after both players are ready
            else if (_countdown > 0.0f && !MainMenuCanvasGameObject.activeInHierarchy && !GameSetupCanvasGameObject.activeInHierarchy && !_transitionActive && !RandomEnvironmentController.SinkOrRaiseActive)
            {
                NotificationText.gameObject.SetActive(true);
                _countdown -= Time.deltaTime;
                NotificationText.text = "" + (int)(_countdown + 1);
                if (_countdown <= 0.0f)
                {
                    NotificationText.gameObject.SetActive(false);
                    RedCharacterAnimationController.enabled = true;
                    BlueCharacterAnimationController.enabled = true;
                    _currentRoundTime = _roundDuration;
                    foreach (CharacterLoadoutController loadoutController in CharacterLoadoutControllers)
                        loadoutController.SetAbilitiesActive(true);
                }
            }
            // Countdown finished, round started
            else if (_currentRoundTime > 0.0f && !NotificationText.gameObject.activeInHierarchy && !MainMenuCanvasGameObject.activeInHierarchy)
            {
                RedHPText.text = "" + RedCharacterStatsController.HitPoints;
                BlueHPText.text = "" + BlueCharacterStatsController.HitPoints;
                RedHPSlider.value = ((float)RedCharacterStatsController.HitPoints / RedCharacterStatsController.MaxHitPoints) * 100;
                BlueHPSlider.value = ((float)BlueCharacterStatsController.HitPoints / BlueCharacterStatsController.MaxHitPoints) * 100;
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
                // Dodge Cooldown HUD updating
                if (RedCharacterAnimationController.DodgeCountdown > 0.0f)
                {
                    RedDodgeCooldownText.gameObject.SetActive(true);
                    RedDodgeDarkMask.fillAmount = RedCharacterAnimationController.DodgeCountdown / RedCharacterStatsController.DodgeCooldown;
                    RedDodgeCooldownText.text = "" + Mathf.Round(RedCharacterAnimationController.DodgeCountdown);
                    if (RedCharacterAnimationController.DodgeCountdown <= 0.05f)
                        RedDodgeCooldownText.gameObject.SetActive(false);
                }
                if (BlueCharacterAnimationController.DodgeCountdown > 0.0f)
                {
                    BlueDodgeCooldownText.gameObject.SetActive(true);
                    BlueDodgeDarkMask.fillAmount = BlueCharacterAnimationController.DodgeCountdown / BlueCharacterStatsController.DodgeCooldown;
                    BlueDodgeCooldownText.text = "" + Mathf.Round(BlueCharacterAnimationController.DodgeCountdown);
                    if (BlueCharacterAnimationController.DodgeCountdown <= 0.05f)
                        BlueDodgeCooldownText.gameObject.SetActive(false);
                }
                // Red Respawning
                if (RedCharacterStatsController.HitPoints <= 0 && CurrentGameMode != GameMode.Standard)
                {
                    if (_redCountdown > 0.0f && !MainMenuCanvasGameObject.activeInHierarchy)
                    {
                        _redCountdown -= Time.deltaTime;
                        RedNotificationText.text = "" + (int)(_redCountdown + 1);
                        if (_redCountdown <= 0.0f)
                        {
                            CharacterLoadoutControllers[0].transform.SetPositionAndRotation(RedCharacterGamePosition, Quaternion.Euler(RedCharacterGameRotation));
                            RedCharacterStatsController.HitPoints = RedCharacterStatsController.MaxHitPoints;
                            RedCharacterStatsController.gameObject.SetActive(true);
                            RedNotificationText.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        if (CurrentGameMode == GameMode.CaptureTheFlag && RedCharacterStatsController.transform.Find("mixamorig:Hips/mixamorig:Spine/Flagpole") != null)
                        {
                            FlagController.transform.SetParent(CaptureTheFlagObjects.transform);
                            FlagController.transform.rotation = InitialFlagRotation;
                            FlagController.transform.Translate(0, -3.2f, 0);
                            FlagController.ResetMaterial();
                            FlagController.GetComponent<CapsuleCollider>().enabled = true;
                        }
                        else if (CurrentGameMode == GameMode.KingOfTheHill)
                        {
                            HillCollisionController.RedColliding = false;
                            HillCollisionController.DetermineMaterial();
                        }
                        RedCharacterStatsController.gameObject.SetActive(false);
                        foreach (HitboxTriggerController hitbox in BlueHitboxControllers)
                            hitbox.CollidingObjects.Remove(RedCharacterStatsController.gameObject);
                        foreach (HitboxTriggerController hitbox in RedHitboxControllers)
                            hitbox.CollidingObjects.Remove(BlueCharacterStatsController.gameObject);
                        _redCountdown = _respawnTime;
                        RedNotificationText.gameObject.SetActive(true);
                    }
                }
                // Blue Respawning
                if (BlueCharacterStatsController.HitPoints <= 0 && CurrentGameMode != GameMode.Standard)
                {
                    if (_blueCountdown > 0.0f && !MainMenuCanvasGameObject.activeInHierarchy)
                    {
                        _blueCountdown -= Time.deltaTime;
                        BlueNotificationText.text = "" + (int)(_blueCountdown + 1);
                        if (_blueCountdown <= 0.0f)
                        {
                            CharacterLoadoutControllers[1].transform.SetPositionAndRotation(BlueCharacterGamePosition, Quaternion.Euler(BlueCharacterGameRotation));
                            BlueCharacterStatsController.HitPoints = BlueCharacterStatsController.MaxHitPoints;
                            BlueCharacterStatsController.gameObject.SetActive(true);
                            BlueNotificationText.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        if (CurrentGameMode == GameMode.CaptureTheFlag && BlueCharacterStatsController.transform.Find("mixamorig:Hips/mixamorig:Spine/Flagpole") != null)
                        {
                            FlagController.transform.SetParent(CaptureTheFlagObjects.transform);
                            FlagController.transform.rotation = InitialFlagRotation;
                            FlagController.transform.Translate(0, -3.2f, 0);
                            FlagController.ResetMaterial();
                            FlagController.GetComponent<CapsuleCollider>().enabled = true;
                        }
                        else if (CurrentGameMode == GameMode.KingOfTheHill)
                        {
                            HillCollisionController.BlueColliding = false;
                            HillCollisionController.DetermineMaterial();
                        }
                        BlueCharacterStatsController.gameObject.SetActive(false);
                        foreach (HitboxTriggerController hitbox in BlueHitboxControllers)
                            hitbox.CollidingObjects.Remove(RedCharacterStatsController.gameObject);
                        foreach (HitboxTriggerController hitbox in RedHitboxControllers)
                            hitbox.CollidingObjects.Remove(BlueCharacterStatsController.gameObject);
                        _blueCountdown = _respawnTime;
                        BlueNotificationText.gameObject.SetActive(true);
                    }
                }
                // Round end conditions
                if (_currentRoundTime <= 0.0f || (CurrentGameMode == GameMode.Standard && (RedCharacterStatsController.HitPoints <= 0 || BlueCharacterStatsController.HitPoints <= 0)))
                {
                    foreach (HitboxTriggerController hitbox in BlueHitboxControllers)
                        hitbox.CollidingObjects.Remove(RedCharacterStatsController.gameObject);
                    foreach (HitboxTriggerController hitbox in RedHitboxControllers)
                        hitbox.CollidingObjects.Remove(BlueCharacterStatsController.gameObject);
                    DisableCharacterAnimations();
                    foreach (CharacterLoadoutController loadoutController in CharacterLoadoutControllers)
                        loadoutController.SetAbilitiesActive(false);
                    if (CurrentGameMode == GameMode.KingOfTheHill)
                    {
                        HillCollisionController.RedColliding = false;
                        HillCollisionController.BlueColliding = false;
                        HillCollisionController.DetermineMaterial();
                    }
                    if ((int)_redScore > (int)_blueScore)
                    {
                        _redWins++;
                        RedWinsText.text = "" + _redWins;
                        NotificationText.text = string.Format("ROUND {0}\nPINK WINS", CurrentRound);
                    }
                    else if ((int)_blueScore > (int)_redScore)
                    {
                        _blueWins++;
                        BlueWinsText.text = "" + _blueWins;
                        NotificationText.text = string.Format("ROUND {0}\nGREEN WINS", CurrentRound);
                    }
                    else
                    {
                        NotificationText.text = string.Format("ROUND {0}\nDRAW", CurrentRound);
                    }
                    _winCountdown = 5;
                    NotificationText.gameObject.SetActive(true);
                    StartTransition(true, false);
                    StartCoroutine(RandomEnvironmentController.SinkAndDeleteObjects());
                    StartCoroutine(SinkAndRaiseObjects(KingOfTheHillObjects));
                    StartCoroutine(SinkAndRaiseObjects(CaptureTheFlagObjects));
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
                        RedCharacterStatsController.HitPoints = RedCharacterStatsController.MaxHitPoints;
                        BlueCharacterStatsController.HitPoints = BlueCharacterStatsController.MaxHitPoints;
                        RedHPText.text = "" + RedCharacterStatsController.HitPoints;
                        BlueHPText.text = "" + BlueCharacterStatsController.HitPoints;
                        RedHPSlider.value = ((float)RedCharacterStatsController.HitPoints / RedCharacterStatsController.MaxHitPoints) * 100;
                        BlueHPSlider.value = ((float)BlueCharacterStatsController.HitPoints / BlueCharacterStatsController.MaxHitPoints) * 100;
                        RedCharacterAnimationController.DodgeCountdown = 0;
                        BlueCharacterAnimationController.DodgeCountdown = 0;
                        RedCharacterStatsController.Potions++;
                        BlueCharacterStatsController.Potions++;
                        RedDodgeCooldownText.gameObject.SetActive(false);
                        BlueDodgeCooldownText.gameObject.SetActive(false);
                        RedDodgeDarkMask.fillAmount = 0;
                        BlueDodgeDarkMask.fillAmount = 0;
                        RedScore = 0;
                        BlueScore = 0;
                        RedScoreText.text = "" + (int)_redScore;
                        BlueScoreText.text = "" + (int)_blueScore;
                        TimeSpan newTime = TimeSpan.FromSeconds(_roundDuration);
                        RoundTimeText.text = string.Format(_timeFormat, newTime.Minutes, newTime.Seconds);
                        LoadoutCanvasGameObject.SetActive(true);
                        _loadoutCanvasGroup.interactable = true;
                        CharacterLoadoutControllers[0].ReadyToggle.isOn = false;
                        CharacterLoadoutControllers[1].ReadyToggle.isOn = false;
                        //CharacterLoadoutControllers[0].transform.SetPositionAndRotation(_initialRedCharacterPosition, _initialRedCharacterRotation);
                        //CharacterLoadoutControllers[1].transform.SetPositionAndRotation(_initialBlueCharacterPosition, _initialBlueCharacterRotation);
                        CharacterLoadoutControllers[0].ResetAbilityCooldowns();
                        CharacterLoadoutControllers[1].ResetAbilityCooldowns();
                        RedCharacterStatsController.gameObject.SetActive(true);
                        _redCountdown = 0;
                        RedNotificationText.gameObject.SetActive(false);
                        BlueCharacterStatsController.gameObject.SetActive(true);
                        _blueCountdown = 0;
                        BlueNotificationText.gameObject.SetActive(false);
                        if (CurrentGameMode == GameMode.CaptureTheFlag)
                        {
                            FlagController.transform.SetParent(CaptureTheFlagObjects.transform);
                            FlagController.transform.localPosition = InitialFlagPosition;
                            FlagController.transform.rotation = InitialFlagRotation;
                            FlagController.ResetMaterial();
                            FlagController.GetComponent<CapsuleCollider>().enabled = true;
                        }
                        CurrentRound++;
                        RandomEnvironmentController.SpawnRandomEnvironmentObjects(CurrentGameMode);
                    }
                    // Game end conditions
                    else
                    {
                        if (_redWins > _maxRounds / 2 || CurrentRound == _maxRounds && _redWins > _blueWins)
                            NotificationText.text = "GAME OVER\nPINK WINS";
                        else if (_blueWins > _maxRounds / 2 || CurrentRound == _maxRounds && _blueWins > _redWins)
                            NotificationText.text = "GAME OVER\nGREEN WINS";
                        else
                            NotificationText.text = "GAME OVER\nDRAW";
                        _winCountdown = 5;
                        NotificationText.gameObject.SetActive(true);
                    }
                }
            }
            if (Input.GetButtonDown("Main Menu"))
            {
                if (!MainMenuCanvasGameObject.activeInHierarchy && !_transitionActive)
                {
                    MainMenuCanvasGameObject.SetActive(true);
                    DisableCharacterAnimations();
                    _mainMenuPanelGameObject.GetComponent<CanvasGroup>().interactable = true;
                    _loadoutCanvasGroup.interactable = false;
                    EventSystem.SetSelectedGameObject(_newGameButtonGameObject.gameObject);
                    foreach (CharacterLoadoutController loadoutController in CharacterLoadoutControllers)
                        loadoutController.SetAbilitiesActive(false);
                }
                else
                {
                    MainMenuCanvasGameObject.SetActive(false);
                    _mainMenuPanelGameObject.GetComponent<CanvasGroup>().interactable = false;
                    _loadoutCanvasGroup.interactable = true;
                    if (!LoadoutCanvasGameObject.activeInHierarchy && _currentRoundTime > 0.0f && !NotificationText.gameObject.activeInHierarchy)
                    {
                        RedCharacterAnimationController.enabled = true;
                        BlueCharacterAnimationController.enabled = true;
                    foreach (CharacterLoadoutController loadoutController in CharacterLoadoutControllers)
                        loadoutController.SetAbilitiesActive(true);
                    }
                }
            }
        }
    }
}
