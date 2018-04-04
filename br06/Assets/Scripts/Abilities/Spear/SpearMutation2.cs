using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearMutation2 : Ability
{

    private CharacterStatsController statsController;
    public float ShieldWallDuration = 1f;
    public int ShieldWallReduction = 1;


    public override void Initialize(GameObject obj)
    {
        statsController = obj.GetComponent<CharacterStatsController>();
    }

    public override bool TriggerAbility()
    {
        statsController.EnableTimedShieldWall(ShieldWallDuration, ShieldWallReduction);
        return true;
    }
}
