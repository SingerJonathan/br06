﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleaxeAbility1 : Ability
{

    private HitboxTriggerController _hitboxTriggerController;
    public int WeaponDamage = 1;

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
            {
                _hitboxTriggerController.CollidingObjects[index].GetComponent<CharacterStatsController>().HitPoints -= WeaponDamage;
                _hitboxTriggerController.CollidingObjects[index].GetComponent<CharacterAnimationController>().Slow();
            }
        }
    }
}
