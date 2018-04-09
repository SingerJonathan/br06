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

    public AudioSource MenuMusic;

    public Text RoomNumberText;

    public Animator CameraAnimator;
    public Vector3 RedCharacterGamePosition;
    public Vector3 BlueCharacterGamePosition;
    public Vector3 RedCharacterGameRotation;
    public Vector3 BlueCharacterGameRotation;

    public GameObject MainMenuCanvasGameObject;
    public GameObject ControlsPanelGameObject;
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
    public GameObject[] RedStatePanels;
    public GameObject[] BlueStatePanels;

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

    private bool _firstCountdownSoundPlayed;
    
    private List<Round> _rounds;
    private GameMode _currentGameMode = GameMode.Standard;
    private float _redScore;
    private float _blueScore;

    private bool[] _loadoutAxisTriggered = new bool[2];
    private int[] _loadoutNavigationStates = new int[2];

    private bool QuitToMainMenuPressed;
    private bool NewGamePressed;

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
            if (CurrentRound > 1)
            {
                // Disable ability description list and enable ability mutation after first round
                CharacterLoadoutControllers[0].LoadoutGUIDescriptionIcons[0].transform.parent.parent.gameObject.SetActive(false);
                CharacterLoadoutControllers[1].LoadoutGUIDescriptionIcons[0].transform.parent.parent.gameObject.SetActive(false);
                CharacterLoadoutControllers[0].LoadoutGUIAbilityIcons[0].transform.parent.parent.gameObject.SetActive(true);
                CharacterLoadoutControllers[1].LoadoutGUIAbilityIcons[0].transform.parent.parent.gameObject.SetActive(true);
                // Disable weapon dropdowns after first round
                foreach (Dropdown dropdown in MainWeaponDropdowns)
                    dropdown.interactable = false;
                foreach (Dropdown dropdown in OffhandWeaponDropdowns)
                    dropdown.interactable = false;
            }
            else
            {
                CharacterLoadoutControllers[0].LoadoutGUIDescriptionIcons[0].transform.parent.parent.gameObject.SetActive(true);
                CharacterLoadoutControllers[1].LoadoutGUIDescriptionIcons[0].transform.parent.parent.gameObject.SetActive(true);
                CharacterLoadoutControllers[0].LoadoutGUIAbilityIcons[0].transform.parent.parent.gameObject.SetActive(false);
                CharacterLoadoutControllers[1].LoadoutGUIAbilityIcons[0].transform.parent.parent.gameObject.SetActive(false);
                foreach (Dropdown dropdown in MainWeaponDropdowns)
                    dropdown.interactable = true;
                foreach (Dropdown dropdown in OffhandWeaponDropdowns)
                    dropdown.interactable = true;
            }
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

    private void DestroyTraps()
    {
        foreach (GameObject trap in GameObject.FindGameObjectsWithTag("Trap"))
            Destroy(trap.transform.parent.gameObject);
    }

    private Coroutine CurrentFadeMusicCoroutine;

    private void FadeMusic(float targetVolume)
    {
        if (CurrentFadeMusicCoroutine != null)
            StopCoroutine(CurrentFadeMusicCoroutine);
        CurrentFadeMusicCoroutine = StartCoroutine(FadeMusicCoroutine(targetVolume));
    }

    private IEnumerator FadeMusicCoroutine(float targetVolume)
    {
        while (Math.Abs(MenuMusic.volume - targetVolume) > 0.005)
        {
            if (targetVolume > MenuMusic.volume)
                MenuMusic.volume += 0.005f;
            else
                MenuMusic.volume -= 0.005f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SetupGame()
    {
        FadeMusic(0.2f);
        BlueCharacterStatsController.RemoveShieldWall();
        RedCharacterStatsController.RemoveShieldWall();
        NewGamePressed = true;
        DestroyTraps();
        StartTransition(false, true);
        if (CurrentRound != 0)
            StartCoroutine(RandomEnvironmentController.SinkAndDeleteObjects());
        CurrentRound = 0;
        SwitchGameModeObjects(GameMode.Standard);
        _confirmPanelGameObject.SetActive(false);
        MainMenuCanvasGameObject.SetActive(false);
        GameSetupCanvasGameObject.SetActive(false);
        LoadoutCanvasGameObject.SetActive(false);
        HUDCanvasGameObject.SetActive(false);
        NotificationText.gameObject.SetActive(false);
        LoadoutCanvasGameObject.GetComponent<CanvasGroup>().interactable = true;
    }

    public void CancelNewGame()
    {
        FadeMusic(0.2f);
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
        FadeMusic(0.2f);
        BlueCharacterStatsController.RemoveShieldWall();
        RedCharacterStatsController.RemoveShieldWall();
        foreach (Dropdown dropdown in MainWeaponDropdowns)
            dropdown.value = 0;
        foreach (Dropdown dropdown in OffhandWeaponDropdowns)
            dropdown.value = 1;
        foreach (CharacterLoadoutController loadout in CharacterLoadoutControllers)
            loadout.MutationsAvailable = 0;
        _redScore = 0;
        _blueScore = 0;
        RedScoreText.text = "" + (int)_redScore;
        BlueScoreText.text = "" + (int)_blueScore;
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
        FadeMusic(0.2f);
        BlueCharacterStatsController.RemoveShieldWall();
        RedCharacterStatsController.RemoveShieldWall();
        QuitToMainMenuPressed = true;
        DestroyTraps();
        MainMenuCanvasGameObject.SetActive(false);
        StartTransition(false, true);
        _confirmPanelGameObject.SetActive(false);
        _loadoutCanvasGroup.interactable = false;
        CurrentRound = 0;
        SwitchGameModeObjects(GameMode.Standard);
        _quitButton.interactable = false;
        RedCharacterAnimationController.enabled = false;
        BlueCharacterAnimationController.enabled = false;
        GameSetupCanvasGameObject.SetActive(false);
        LoadoutCanvasGameObject.SetActive(false);
        HUDCanvasGameObject.SetActive(false);
        _confirmPanelYesButton.onClick.RemoveAllListeners();
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
        if (context == "New" && CurrentRound == 0)
            SetupGame();
        else
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
        GameObject.FindGameObjectWithTag("Click Sound").GetComponent<AudioSource>().Play();
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
        else if (_loadoutNavigationStates[playerNumber] == 3)
        {
            if (Input.GetAxisRaw("Horizontal" + (playerNumber + 1)) == 0 && Input.GetAxisRaw("HorizontalAlt" + (playerNumber + 1)) == 0
            && Input.GetAxisRaw("Vertical" + (playerNumber + 1)) == 0 && Input.GetAxisRaw("VerticalAlt" + (playerNumber + 1)) == 0)
                _loadoutAxisTriggered[playerNumber] = false;
            if (Input.GetAxisRaw("Horizontal" + (playerNumber + 1)) < 0 || Input.GetAxisRaw("HorizontalAlt" + (playerNumber + 1)) < 0)
            {
                if (!_loadoutAxisTriggered[playerNumber])
                {
                    if (CharacterLoadoutControllers[playerNumber].AbilityIndex > 0)
                    {
                        LoadoutStatePanels[playerNumber,3].transform.GetChild(CharacterLoadoutControllers[playerNumber].AbilityIndex).gameObject.SetActive(false);
                        CharacterLoadoutControllers[playerNumber].AbilityIndex--;
                        LoadoutStatePanels[playerNumber,3].transform.GetChild(CharacterLoadoutControllers[playerNumber].AbilityIndex).gameObject.SetActive(true);
                        CharacterLoadoutControllers[playerNumber].LoadoutGUIAbilityIcons[CharacterLoadoutControllers[playerNumber].AbilityIndex].transform.parent.GetComponent<AbilityDescriptionSwitcher>().OnSelect(null);
                        GameObject.FindGameObjectWithTag("Select Sound").GetComponent<AudioSource>().Play();
                    }
                    _loadoutAxisTriggered[playerNumber] = true;
                }
            }
            else if (Input.GetAxisRaw("Horizontal" + (playerNumber + 1)) > 0 || Input.GetAxisRaw("HorizontalAlt" + (playerNumber + 1)) > 0)
            {
                if (!_loadoutAxisTriggered[playerNumber])
                {
                    if (CharacterLoadoutControllers[playerNumber].AbilityIndex < CharacterLoadoutControllers[playerNumber].Abilities.Length - 1)
                    {
                        LoadoutStatePanels[playerNumber,3].transform.GetChild(CharacterLoadoutControllers[playerNumber].AbilityIndex).gameObject.SetActive(false);
                        CharacterLoadoutControllers[playerNumber].AbilityIndex++;
                        LoadoutStatePanels[playerNumber,3].transform.GetChild(CharacterLoadoutControllers[playerNumber].AbilityIndex).gameObject.SetActive(true);
                        CharacterLoadoutControllers[playerNumber].LoadoutGUIAbilityIcons[CharacterLoadoutControllers[playerNumber].AbilityIndex].transform.parent.GetComponent<AbilityDescriptionSwitcher>().OnSelect(null);
                        GameObject.FindGameObjectWithTag("Select Sound").GetComponent<AudioSource>().Play();
                    }
                    _loadoutAxisTriggered[playerNumber] = true;
                }
            }
            else if (Input.GetAxisRaw("Vertical" + (playerNumber + 1)) < 0 || Input.GetAxisRaw("VerticalAlt" + (playerNumber + 1)) < 0)
            {
                if (!_loadoutAxisTriggered[playerNumber]
                && CharacterLoadoutControllers[playerNumber].LoadoutGUIAbilityIcons[CharacterLoadoutControllers[playerNumber].AbilityIndex].transform.parent.GetComponent<AbilityMutationButtonBehaviour>()
                && CharacterLoadoutControllers[playerNumber].LoadoutGUIAbilityIcons[CharacterLoadoutControllers[playerNumber].AbilityIndex].transform.parent.GetComponent<AbilityMutationButtonBehaviour>().MutationPossible())
                {
                    CharacterLoadoutControllers[playerNumber].LoadoutGUIAbilityIcons[CharacterLoadoutControllers[playerNumber].AbilityIndex].transform.parent.GetComponent<AbilityMutationButtonBehaviour>().OnClick();
                    _loadoutNavigationStates[playerNumber] = 4;
                    SwitchActiveLoadoutStatePanel(playerNumber, 4);
                    CharacterLoadoutControllers[playerNumber].MutationIndex = 0;
                    int index = CharacterLoadoutControllers[playerNumber].MutationIndex;
                    if (CharacterLoadoutControllers[playerNumber].AbilityIndex == 2)
                        index += 2;
                    for (int child = 0; child < 4; child++)
                        LoadoutStatePanels[playerNumber,4].transform.GetChild(child).gameObject.SetActive(false);
                    LoadoutStatePanels[playerNumber,4].transform.GetChild(index).gameObject.SetActive(true);
                    CharacterLoadoutControllers[playerNumber].LoadoutGUIMutatationIcons[index].transform.parent.GetComponent<AbilityDescriptionSwitcher>().OnSelect(null);
                    GameObject.FindGameObjectWithTag("Click Sound").GetComponent<AudioSource>().Play();
                    _loadoutAxisTriggered[playerNumber] = true;
                }
            }
            else if (Input.GetButtonDown("Submit" + (playerNumber + 1)) || Input.GetButtonDown("SubmitAlt" + (playerNumber + 1)))
            {
                if (CharacterLoadoutControllers[playerNumber].MutationsAvailable == 0 || CurrentRound >= 4)
                {
                    _loadoutNavigationStates[playerNumber] = 5;
                    SwitchActiveLoadoutStatePanel(playerNumber, 5);
                    GameObject.FindGameObjectWithTag("Click Sound").GetComponent<AudioSource>().Play();
                }
            }
        }
        // State 4: Ability Mutation
        else if (_loadoutNavigationStates[playerNumber] == 4)
        {
            if (Input.GetAxisRaw("Horizontal" + (playerNumber + 1)) == 0 && Input.GetAxisRaw("HorizontalAlt" + (playerNumber + 1)) == 0
            && Input.GetAxisRaw("Vertical" + (playerNumber + 1)) == 0 && Input.GetAxisRaw("VerticalAlt" + (playerNumber + 1)) == 0)
                _loadoutAxisTriggered[playerNumber] = false;
            if (Input.GetAxisRaw("Horizontal" + (playerNumber + 1)) < 0 || Input.GetAxisRaw("HorizontalAlt" + (playerNumber + 1)) < 0)
            {
                if (!_loadoutAxisTriggered[playerNumber])
                {
                    if (CharacterLoadoutControllers[playerNumber].MutationIndex > 0)
                    {
                        int index = CharacterLoadoutControllers[playerNumber].MutationIndex;
                        if (CharacterLoadoutControllers[playerNumber].AbilityIndex == 2)
                            index += 2;
                        for (int child = 0; child < 4; child++)
                            LoadoutStatePanels[playerNumber,4].transform.GetChild(child).gameObject.SetActive(false);
                        CharacterLoadoutControllers[playerNumber].MutationIndex--;
                        index--;
                        LoadoutStatePanels[playerNumber,4].transform.GetChild(index).gameObject.SetActive(true);
                        CharacterLoadoutControllers[playerNumber].LoadoutGUIMutatationIcons[index].transform.parent.GetComponent<AbilityDescriptionSwitcher>().OnSelect(null);
                        GameObject.FindGameObjectWithTag("Select Sound").GetComponent<AudioSource>().Play();
                    }
                    _loadoutAxisTriggered[playerNumber] = true;
                }
            }
            else if (Input.GetAxisRaw("Horizontal" + (playerNumber + 1)) > 0 || Input.GetAxisRaw("HorizontalAlt" + (playerNumber + 1)) > 0)
            {
                if (!_loadoutAxisTriggered[playerNumber])
                {
                    if (CharacterLoadoutControllers[playerNumber].MutationIndex < 1)
                    {
                        int index = CharacterLoadoutControllers[playerNumber].MutationIndex;
                        if (CharacterLoadoutControllers[playerNumber].AbilityIndex == 2)
                            index += 2;
                        for (int child = 0; child < 4; child++)
                            LoadoutStatePanels[playerNumber,4].transform.GetChild(child).gameObject.SetActive(false);
                        CharacterLoadoutControllers[playerNumber].MutationIndex++;
                        index++;
                        LoadoutStatePanels[playerNumber,4].transform.GetChild(index).gameObject.SetActive(true);
                        CharacterLoadoutControllers[playerNumber].LoadoutGUIMutatationIcons[index].transform.parent.GetComponent<AbilityDescriptionSwitcher>().OnSelect(null);
                        GameObject.FindGameObjectWithTag("Select Sound").GetComponent<AudioSource>().Play();
                    }
                    _loadoutAxisTriggered[playerNumber] = true;
                }
            }
            else if (Input.GetAxisRaw("Vertical" + (playerNumber + 1)) > 0 || Input.GetAxisRaw("VerticalAlt" + (playerNumber + 1)) > 0
            || Input.GetButtonDown("Cancel" + (playerNumber + 1)) || Input.GetButtonDown("CancelAlt" + (playerNumber + 1)))
            {
                if (!_loadoutAxisTriggered[playerNumber])
                {
                    CharacterLoadoutControllers[playerNumber].LoadoutGUIAbilityIcons[CharacterLoadoutControllers[playerNumber].AbilityIndex].transform.parent.GetComponent<AbilityMutationButtonBehaviour>().OnClick();
                    _loadoutNavigationStates[playerNumber] = 3;
                    SwitchActiveLoadoutStatePanel(playerNumber, 3);
                    CharacterLoadoutControllers[playerNumber].LoadoutGUIAbilityIcons[CharacterLoadoutControllers[playerNumber].AbilityIndex].transform.parent.GetComponent<AbilityDescriptionSwitcher>().OnSelect(null);
                    GameObject.FindGameObjectWithTag("Click Sound").GetComponent<AudioSource>().Play();
                    _loadoutAxisTriggered[playerNumber] = true;
                }
            }
            else if (Input.GetButtonDown("Submit" + (playerNumber + 1)) || Input.GetButtonDown("SubmitAlt" + (playerNumber + 1)))
            {
                int index = CharacterLoadoutControllers[playerNumber].MutationIndex;
                if (CharacterLoadoutControllers[playerNumber].AbilityIndex == 2)
                    index += 2;
                CharacterLoadoutControllers[playerNumber].LoadoutGUIMutatationIcons[index].transform.parent.GetComponent<Button>().onClick.Invoke();
                _loadoutNavigationStates[playerNumber] = 3;
                SwitchActiveLoadoutStatePanel(playerNumber, 3);
                GameObject.FindGameObjectWithTag("Click Sound").GetComponent<AudioSource>().Play();
            }
        }
        // State 5: Ready
        else if (_loadoutNavigationStates[playerNumber] == 5)
        {
            if (Input.GetButtonDown("Submit" + (playerNumber + 1)) || Input.GetButtonDown("SubmitAlt" + (playerNumber + 1)))
            {
                CharacterLoadoutControllers[playerNumber].ReadyToggle.isOn = true;
            }
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
                else if (CurrentRound > 1)
                {
                    _loadoutNavigationStates[playerNumber] = 3;
                    SwitchActiveLoadoutStatePanel(playerNumber, 3);
                }
            }
        }
    }

    void Start()
    {
        LoadoutStatePanels[0, 1] = RedStatePanels[0];
        LoadoutStatePanels[0, 2] = RedStatePanels[1];
        LoadoutStatePanels[0, 3] = RedStatePanels[2];
        LoadoutStatePanels[0, 4] = RedStatePanels[3];
        LoadoutStatePanels[0, 5] = RedStatePanels[4];
        LoadoutStatePanels[1, 1] = BlueStatePanels[0];
        LoadoutStatePanels[1, 2] = BlueStatePanels[1];
        LoadoutStatePanels[1, 3] = BlueStatePanels[2];
        LoadoutStatePanels[1, 4] = BlueStatePanels[3];
        LoadoutStatePanels[1, 5] = BlueStatePanels[4];
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
        if (RoomNumberText)
            RoomNumberText.text = "#" + new System.Random().Next(0, 1000000).ToString("D6");
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
                    _firstCountdownSoundPlayed = false;
                    RandomEnvironmentController.SpawnRandomEnvironmentObjects(CurrentGameMode);
                    SwitchGameModeObjects(_currentGameMode);
                    FadeMusic(0.05f);
                    BlueCharacterStatsController.RemoveShieldWall();
                    RedCharacterStatsController.RemoveShieldWall();
                }
            }
            // Start countdown after both players are ready
            else if (_countdown > 0.0f && !MainMenuCanvasGameObject.activeInHierarchy && !GameSetupCanvasGameObject.activeInHierarchy && !_transitionActive && !RandomEnvironmentController.SinkOrRaiseActive)
            {
                NotificationText.gameObject.SetActive(true);
                _countdown -= Time.deltaTime;
                if (NotificationText.text != ("" + (int)(_countdown + 1)) || !_firstCountdownSoundPlayed)
                {
                    _firstCountdownSoundPlayed = true;
                    NotificationText.text = "" + (int)(_countdown + 1);
                    if (_countdown <= 0.0f)
                    {
                        _firstCountdownSoundPlayed = false;
                        GameObject.FindGameObjectWithTag("Go Sound").GetComponent<AudioSource>().Play();
                        NotificationText.gameObject.SetActive(false);
                        RedCharacterAnimationController.enabled = true;
                        BlueCharacterAnimationController.enabled = true;
                        _currentRoundTime = _roundDuration;
                        foreach (CharacterLoadoutController loadoutController in CharacterLoadoutControllers)
                            loadoutController.SetAbilitiesActive(true);
                    }
                    else
                        GameObject.FindGameObjectWithTag("Countdown Sound").GetComponent<AudioSource>().Play();
                }
            }
            // Countdown finished, round started
            else if (_currentRoundTime > 0.0f && !NotificationText.gameObject.activeInHierarchy && !MainMenuCanvasGameObject.activeInHierarchy && !_transitionActive && !RandomEnvironmentController.SinkOrRaiseActive)
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
                            GameObject.FindGameObjectWithTag("Respawn Sound").GetComponent<AudioSource>().Play();
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
                            GameObject.FindGameObjectWithTag("Respawn Sound").GetComponent<AudioSource>().Play();
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
                    DestroyTraps();
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
                        for (int playerNumber = 0; playerNumber <= 1; playerNumber++)
                        {
                            _loadoutNavigationStates[playerNumber] = 3;
                            SwitchActiveLoadoutStatePanel(playerNumber, 3);
                            for (int child = 0; child < 3; child++)
                                LoadoutStatePanels[playerNumber,3].transform.GetChild(child).gameObject.SetActive(false);
                            LoadoutStatePanels[playerNumber,3].transform.GetChild(0).gameObject.SetActive(true);
                            CharacterLoadoutControllers[playerNumber].LoadoutGUIAbilityIcons[0].transform.parent.GetComponent<AbilityDescriptionSwitcher>().OnSelect(null);
                            CharacterLoadoutControllers[playerNumber].AbilityIndex = 0;
                            CharacterLoadoutControllers[playerNumber].MutationIndex = 0;
                        }
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
                        FadeMusic(0.2f);
                        BlueCharacterStatsController.RemoveShieldWall();
                        RedCharacterStatsController.RemoveShieldWall();
                        CharacterLoadoutControllers[0].ReadyToggle.isOn = false;
                        CharacterLoadoutControllers[1].ReadyToggle.isOn = false;
                        CharacterLoadoutControllers[0].ResetAbilityCooldowns();
                        CharacterLoadoutControllers[1].ResetAbilityCooldowns();
                        CharacterLoadoutControllers[0].MutationsAvailable++;
                        CharacterLoadoutControllers[1].MutationsAvailable++;
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
                if (!MainMenuCanvasGameObject.activeInHierarchy && !_transitionActive && !RandomEnvironmentController.SinkOrRaiseActive && !NotificationText.gameObject.activeInHierarchy)
                {
                    FadeMusic(0.2f);
                    ControlsPanelGameObject.SetActive(false);
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
                    FadeMusic(0.05f);
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
        if (ControlsPanelGameObject.activeInHierarchy && ((Input.GetButtonDown("Cancel1") || Input.GetButtonDown("CancelAlt1"))))
        {
            ControlsPanelGameObject.SetActive(false);
            _mainMenuPanelGameObject.GetComponent<CanvasGroup>().interactable = true;
            GameObject.FindGameObjectWithTag("Click Sound").GetComponent<AudioSource>().Play();
        }
        if (QuitToMainMenuPressed && !_transitionActive && !RandomEnvironmentController.SinkOrRaiseActive)
        {
            MainMenuCanvasGameObject.SetActive(true);
            _mainMenuPanelGameObject.GetComponent<CanvasGroup>().interactable = true;
            EventSystem.SetSelectedGameObject(_newGameButtonGameObject.gameObject);
            QuitToMainMenuPressed = false;
        }
        else if (NewGamePressed && !_transitionActive && !RandomEnvironmentController.SinkOrRaiseActive)
        {
            GameSetupCanvasGameObject.SetActive(true);
            EventSystem.SetSelectedGameObject(_roundsDropdown.gameObject);
            NewGamePressed = false;
        }
    }
}
