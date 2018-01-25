using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/MeleeAbility")]
public class MeleeAbility : Ability {

    public int WeaponDamage = 1;
    private HitboxTriggerController _hitboxTriggerController;

    public override void Initialize(GameObject obj)
    {
        if (this.aName.Contains("Spear"))
            _hitboxTriggerController = obj.transform.Find("HitboxLine").GetComponent<HitboxTriggerController>();
        else
            _hitboxTriggerController = obj.transform.Find("HitboxSemiCircle").GetComponent<HitboxTriggerController>();
        _enemyColour = obj.name.Contains("Red") ? "Blue" : "Red";
    }

    public override void TriggerAbility()
    {
        for (int index = 0; index < _hitboxTriggerController.CollidingObjects.Count; index++)
        {
            if (_hitboxTriggerController.CollidingObjects[index].name.Contains(_enemyColour))
                _hitboxTriggerController.CollidingObjects[index].GetComponent<CharacterStatsController>().HitPoints -= WeaponDamage;
        }
    }
}
