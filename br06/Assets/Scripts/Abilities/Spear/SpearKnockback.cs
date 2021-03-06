﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearKnockback : Ability {

    public int WeaponDamage = 1;
    public float KnockbackForce = 1f;
    private HitboxTriggerController _hitboxTriggerController;

    public override void Initialize(GameObject obj)
    {
        _hitboxTriggerController = obj.transform.Find("HitboxLine").GetComponent<HitboxTriggerController>();
        _enemyColour = obj.name.Contains("Red") ? "Blue" : "Red";
    }
    // doing some commenting here
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
                        _hitboxTriggerController.CollidingObjects[index].GetComponent<CharacterStatsController>().HitPoints -= WeaponDamage;
                        _hitboxTriggerController.CollidingObjects[index].GetComponent<CharacterAnimationController>().KnockedBack(KnockbackForce);
                    }
                }
            }
        }
        return abilityHit;
    }
}
