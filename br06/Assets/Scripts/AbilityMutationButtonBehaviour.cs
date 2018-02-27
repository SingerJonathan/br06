using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityMutationButtonBehaviour : MonoBehaviour
{
	private Button _button;

	public int AbilityIndex;
	public GameObject OtherAbilityMutations;
	public CharacterLoadoutController CharacterLoadoutController;

    void Start()
    {
		if (GetComponent<Button>())
		{
        	_button = GetComponent<Button>();
        	_button.onClick.AddListener(OnClick);
		}
    }

	public void OnClick()
	{
		// Only allow mutation if there are mutations available and the ability isn't already mutated
		OtherAbilityMutations.SetActive(false);
		if (MutationPossible())
		{
			GetComponent<GameObjectActiveToggle>().ToggleGameObjectActive();
			CharacterLoadoutController.AbilityIndex = AbilityIndex;
			CharacterLoadoutController.MutateAbility(true);
		}
	}

	public bool MutationPossible()
	{
		return CharacterLoadoutController.MutationsAvailable > 0
		&& (CharacterLoadoutController.Abilities[AbilityIndex].Ability == CharacterLoadoutController.MainWeaponGameObject.GetComponent<Weapon>().Abilities[AbilityIndex]
		|| CharacterLoadoutController.Abilities[AbilityIndex].Ability == CharacterLoadoutController.OffhandWeaponGameObject.GetComponent<Weapon>().Abilities[AbilityIndex]);
	}
}
