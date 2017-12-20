using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameLoopController : MonoBehaviour
{
    public GameObject MainMenuCanvasGameObject;
    public GameObject LoadoutCanvasGameObject;
    public CharacterController RedCharacter;
    public CharacterController BlueCharacter;

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
        RedCharacter.GetComponent<CharacterLoadoutController>().Ready = false;
        BlueCharacter.GetComponent<CharacterLoadoutController>().Ready = false;
        RedCharacter.GetComponent<CharacterAnimationController>().enabled = false;
        BlueCharacter.GetComponent<CharacterAnimationController>().enabled = false;
    }

    public void QuitToMainMenu()
    {
        _round = 0;
        _quitButton.interactable = false;
        RedCharacter.GetComponent<CharacterAnimationController>().enabled = false;
        BlueCharacter.GetComponent<CharacterAnimationController>().enabled = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenConfirmDialogue(string context)
    {
        _confirmPanelGameObject.SetActive(true);
        UnityAction call;
        if (context == "Exit")
            call = ExitGame;
        else if (context == "New")
            call = NewGame;
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
    }

    void Update()
    {

    }
}
