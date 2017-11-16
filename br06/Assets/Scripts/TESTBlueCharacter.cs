using UnityEngine;

// DEVNOTE: Remove this class when Loadout GUI is implemented
public class TESTBlueCharacter : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<CharacterLoadoutController>().MainWeapon = CharacterLoadoutController.WeaponEnum.Spear;
    }
}
