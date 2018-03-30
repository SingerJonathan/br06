using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongswordPull : Ability{

    public int WeaponDamage = 1;
    public float PullForce = 1f;
    private HitboxTriggerController _hitboxTriggerController;
    private CharacterAnimationController animator;
    private string myColour;

    public override void Initialize(GameObject obj)
    {
        _hitboxTriggerController = obj.transform.Find("HitboxLine").GetComponent<HitboxTriggerController>();
        _enemyColour = obj.name.Contains("Red") ? "Blue" : "Red";
        myColour = obj.name.Contains("Blue") ? "Red" : "Blue";
        animator = GameObject.FindGameObjectWithTag(myColour).GetComponent<CharacterAnimationController>();
    }

    public override void TriggerAbility()
    {
        for (int index = 0; index < _hitboxTriggerController.CollidingObjects.Count; index++)
        {
            if (_hitboxTriggerController.CollidingObjects[index].name.Contains(_enemyColour))
            {
                _hitboxTriggerController.CollidingObjects[index].GetComponent<CharacterStatsController>().HitPoints -= WeaponDamage;
                animator.PullTowards(PullForce);
            }
        }
    }
}
