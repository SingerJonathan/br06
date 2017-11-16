using UnityEngine;

// DEVNOTE: Remove this class when Loadout GUI is implemented
public class TESTRedCharacter : MonoBehaviour
{
	void Start()
	{
	    gameObject.GetComponent<CharacterLoadoutController>().MainWeapon = CharacterLoadoutController.WeaponEnum.Longsword;
	}
}
