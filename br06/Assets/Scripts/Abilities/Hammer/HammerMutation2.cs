﻿using UnityEngine;

public class HammerMutation2 : Ability
{
    public int WeaponDamage = 1;
    public float KnockbackForce = 1f;
    public int DamageChargeDamage = 1;
    public int DamageChargeLimit = 1;
    private HitboxTriggerController _hitboxTriggerController;

    public override void Initialize(GameObject obj)
    {
        _hitboxTriggerController = obj.transform.Find("HitboxSemiCircleLarge").GetComponent<HitboxTriggerController>();
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
                        _hitboxTriggerController.CollidingObjects[index].GetComponent<CharacterAnimationController>().KnockedBack(KnockbackForce);
                        _hitboxTriggerController.CollidingObjects[index].GetComponent<CharacterStatsController>().AddDamageCharge(DamageChargeDamage, DamageChargeLimit);
                    }
                }
            }
        }
        return abilityHit;
    }
}