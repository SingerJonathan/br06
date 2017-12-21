using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameLoopController : MonoBehaviour
{
    public GameObject MainMenuCanvasGameObject;
    public GameObject LoadoutCanvasGameObject;
    public GameObject HUDCanvasGameObject;

    public Text RoundCounterText;
    public Text RoundTimeText;
    public Text RoundCountdownText;

    public CharacterLoadoutController RedCharacterLoadoutController;
    public CharacterAnimationController RedCharacterAnimationController;
    public CharacterLoadoutController BlueCharacterLoadoutController;
    public CharacterAnimationController BlueCharacterAnimationController;

    private Vector3 _initialRedCharacterPosition;
    private Vector3 _initialBlueCharacterPosition;
    private Quaternion _initialRedCharacterRotation;
    private Quaternion _initialBlueCharacterRotation;

    private GameObject _confirmPanelGameObject;
    private Button _confirmPanelYesButton;
    private Button _quitButton;
    private Dropdown _roundsDropdown;
    private Dropdown _roundDurationDropdown;
    private int _maxRounds;
    private float _roundDuration;
    private int _currentRound;
    private float _currentRoundTime;
    private float _countdown;

    private static string _timeFormat = "{0:D1}:{1:D2}";

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

    private void NewGame()
    {
        _currentRound = 1;
        _maxRounds = Int32.Parse(_roundsDropdown.options[_roundsDropdown.value].text);
        _roundDuration = 60*(_roundDurationDropdown.value + 1);
        MainMenuCanvasGameObject.SetActive(false);
        LoadoutCanvasGameObject.SetActive(true);
        HUDCanvasGameObject.SetActive(true);
        RoundCounterText.text = "" + _currentRound;
        TimeSpan time = TimeSpan.FromSeconds(_roundDuration);
        RoundTimeText.text = string.Format(_timeFormat, time.Minutes, time.Seconds);
        _quitButton.interactable = true;
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
        _currentRound = 0;
        _quitButton.interactable = false;
        RedCharacterAnimationController.enabled = false;
        BlueCharacterAnimationController.enabled = false;
        MainMenuCanvasGameObject.SetActive(true);
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

    public void OpenConfirmDialogue(string context)
    {
        _confirmPanelGameObject.SetActive(true);
        UnityAction call;
        switch (context)
        {
            case "New":
                call = NewGame;
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

    void Start()
    {
        _quitButton = MainMenuCanvasGameObject.transform.Find("Main Panel").Find("Quit Button").GetComponent<Button>();
        _roundsDropdown = MainMenuCanvasGameObject.transform.Find("Main Panel").Find("Rounds Dropdown").GetComponent<Dropdown>();
        _roundDurationDropdown = MainMenuCanvasGameObject.transform.Find("Main Panel").Find("Minutes Dropdown").GetComponent<Dropdown>();
        _confirmPanelGameObject = MainMenuCanvasGameObject.transform.Find("Confirm Panel").gameObject;
        _confirmPanelYesButton = _confirmPanelGameObject.transform.Find("Yes Button").GetComponent<Button>();
        _initialRedCharacterPosition = RedCharacterLoadoutController.transform.position;
        _initialBlueCharacterPosition = BlueCharacterLoadoutController.transform.position;
        _initialRedCharacterRotation = RedCharacterLoadoutController.transform.rotation;
        _initialBlueCharacterRotation = BlueCharacterLoadoutController.transform.rotation;
    }

    void Update()
    {
        // Game is in progress
        if (_currentRound > 0)
        {
            // In Loadout menu
            if (LoadoutCanvasGameObject.activeInHierarchy)
            {
                if (RedCharacterLoadoutController.Ready && BlueCharacterLoadoutController.Ready)
                {
                    RoundCountdownText.gameObject.SetActive(true);
                    _countdown = 3;
                    RoundCountdownText.text = "" + 3;
                    LoadoutCanvasGameObject.SetActive(false);
                }
            }
            // Start countdown after both players are ready
            else if (_countdown > 0.0f && !MainMenuCanvasGameObject.activeInHierarchy)
            {
                _countdown -= Time.deltaTime;
                RoundCountdownText.text = "" + (int)(_countdown + 1);
                if (_countdown <= 0.0f)
                {
                    RoundCountdownText.gameObject.SetActive(false);
                    RedCharacterAnimationController.enabled = true;
                    BlueCharacterAnimationController.enabled = true;
                    _currentRoundTime = _roundDuration;
                }
            }
            // Countdown finished, round started
            else if (_currentRoundTime >= 0.0f && !MainMenuCanvasGameObject.activeInHierarchy)
            {
                _currentRoundTime -= Time.deltaTime;
                TimeSpan time = TimeSpan.FromSeconds(_currentRoundTime);
                RoundTimeText.text = string.Format(_timeFormat, time.Minutes, time.Seconds);
                if (_currentRoundTime <= 0.0f)
                {
                    if (_currentRound == _maxRounds)
                        QuitToMainMenu();
                    else
                    {
                        _currentRound++;
                        RoundCounterText.text = "" + _currentRound;
                        TimeSpan newTime = TimeSpan.FromSeconds(_roundDuration);
                        RoundTimeText.text = string.Format(_timeFormat, newTime.Minutes, newTime.Seconds);
                        LoadoutCanvasGameObject.SetActive(true);
                        RedCharacterLoadoutController.ReadyToggle.isOn = false;
                        BlueCharacterLoadoutController.ReadyToggle.isOn = false;
                        RedCharacterLoadoutController.transform.SetPositionAndRotation(_initialRedCharacterPosition, _initialRedCharacterRotation);
                        BlueCharacterLoadoutController.transform.SetPositionAndRotation(_initialBlueCharacterPosition, _initialBlueCharacterRotation);
                        DisableCharacterAnimations();
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
