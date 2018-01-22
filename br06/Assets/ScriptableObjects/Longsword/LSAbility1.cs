using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/LongswordAbility1")]
public class LSAbility1 : Ability {

    public int WeaponDamage = 3;

    public override void Initialize(GameObject obj)
    {
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
