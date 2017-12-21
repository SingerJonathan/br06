using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameLoopController : MonoBehaviour
{
    public GameObject MainMenuCanvasGameObject;
    public GameObject LoadoutCanvasGameObject;

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
    private int _round;
    private int _maxRounds;

    public void NewGame()
    {
        _round = 1;
        _maxRounds = _roundsDropdown.value + 1;
        MainMenuCanvasGameObject.SetActive(false);
        LoadoutCanvasGameObject.SetActive(true);
        _quitButton.interactable = true;
        RedCharacterLoadoutController.transform.SetPositionAndRotation(_initialRedCharacterPosition, _initialRedCharacterRotation);
        BlueCharacterLoadoutController.transform.SetPositionAndRotation(_initialBlueCharacterPosition, _initialBlueCharacterRotation);
        RedCharacterLoadoutController.ReadyToggle.isOn = false;
        BlueCharacterLoadoutController.ReadyToggle.isOn = false;
        RedCharacterAnimationController.enabled = false;
        BlueCharacterAnimationController.enabled = false;
        _confirmPanelGameObject.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        _round = 0;
        _quitButton.interactable = false;
        RedCharacterAnimationController.enabled = false;
        BlueCharacterAnimationController.enabled = false;
        MainMenuCanvasGameObject.SetActive(true);
        LoadoutCanvasGameObject.SetActive(false);
        _confirmPanelGameObject.SetActive(false);
        RedCharacterLoadoutController.transform.SetPositionAndRotation(_initialRedCharacterPosition, _initialRedCharacterRotation);
        BlueCharacterLoadoutController.transform.SetPositionAndRotation(_initialBlueCharacterPosition, _initialBlueCharacterRotation);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenConfirmDialogue(string context)
    {
        _confirmPanelGameObject.SetActive(true);
        UnityAction call;
        if (context == "New")
            call = NewGame;
        else if (context == "Quit")
            call = QuitToMainMenu;
        else if (context == "Exit")
            call = ExitGame;
        else
            call = delegate { };
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
        if (_round > 0)
        {
            // In Loadout menu
            if (LoadoutCanvasGameObject.activeInHierarchy)
            {
                if (RedCharacterLoadoutController.Ready && BlueCharacterLoadoutController.Ready)
                {
                    LoadoutCanvasGameObject.SetActive(false);
                    RedCharacterAnimationController.enabled = true;
                    BlueCharacterAnimationController.enabled = true;
                }
            }
            if (Input.GetButtonDown("Main Menu"))
            {
                if (!MainMenuCanvasGameObject.activeInHierarchy)
                {
                    MainMenuCanvasGameObject.SetActive(true);
                    RedCharacterAnimationController.enabled = false;
                    BlueCharacterAnimationController.enabled = false;
                    RedCharacterAnimationController._animator.SetInteger("walking", 0);
                    RedCharacterAnimationController._animator.SetBool("running", false);
                    BlueCharacterAnimationController._animator.SetInteger("walking", 0);
                    BlueCharacterAnimationController._animator.SetBool("running", false);
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
