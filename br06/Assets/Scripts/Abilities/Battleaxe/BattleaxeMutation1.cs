﻿using UnityEngine;

public class BattleaxeMutation1 : Ability
{

    private HitboxTriggerController _hitboxTriggerController;
    public float SlowSpeed = 1f;
    public float SlowDuration = 1f;
    public int WeaponDamage = 1;
    public int BleedDamage = 1;
    public float BleedDuration = 1f;
    public float BleedInterval = 1f;

    public override void Initialize(GameObject obj)
    {
        _hitboxTriggerController = obj.transform.Find("HitboxSemiCircle").GetComponent<HitboxTriggerController>();
        _enemyColour = obj.name.Contains("Red") ? "Blue" : "Red";
    }

    public override bool TriggerAbility()
    {
        bool abilityHit = false;
        for (int index = 0; index < _hitboxTriggerController.CollidingObjects.Count; index++)
        {
            if (_hitboxTriggerController.CollidingObjects[index].name.Contains(_enemyColour))
            {
                Transform origin = _hitboxTriggerController.transform.parent.gameObject.transform.Find(MeleeAbility.headString);
                Transform target = GameObject.FindGameObjectWithTag(_enemyColour).transform.Find(MeleeAbility.headString);
                RaycastHit hit;
                if (Physics.Raycast(origin.position, target.position - origin.position, out hit, 100, ~(1 << 8)))
                {
                    Debug.DrawRay(origin.position, target.position - origin.position, Color.red, 10);
                    if (hit.transform.name.Contains(_enemyColour))
                    {
                        abilityHit = true;
                        _hitboxTriggerController.CollidingObjects[index].GetComponent<CharacterStatsController>().DoDamage(WeaponDamage);
                        _hitboxTriggerController.CollidingObjects[index].GetComponent<CharacterAnimationController>().Slow(SlowSpeed, SlowDuration);
                        _hitboxTriggerController.CollidingObjects[index].GetComponent<CharacterStatsController>().setBleed(BleedDamage,BleedDuration,BleedInterval);
                    }
                }
            }
        }
        return abilityHit;
    }
}