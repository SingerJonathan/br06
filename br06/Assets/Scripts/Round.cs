using UnityEngine;
using UnityEngine.UI;

public class Round : MonoBehaviour
{
    public Dropdown RoundModeDropdown;
    public Text RoundNumberText;

    public GameLoopController.GameMode GameMode
    {
        get
        {
            return (GameLoopController.GameMode)RoundModeDropdown.value;
        }

        set
        {
            RoundModeDropdown.value = (int)value;
        }
    }

    void Start()
    {
        if (RoundNumberText)
            RoundNumberText.text = "" + (transform.GetSiblingIndex() + 1);
    }
}
