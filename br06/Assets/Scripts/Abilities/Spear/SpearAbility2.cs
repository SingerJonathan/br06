using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearAbility2 : Ability {

    private CharacterStatsController statsController;
    public int ShieldWallCharge = 1;
    public int ShieldWallReduction = 1;


    public override void Initialize(GameObject obj)
    {
        statsController = obj.GetComponent<CharacterStatsController>();
    }

    public override bool TriggerAbility()
    {
        statsController.EnableShieldWall(ShieldWallCharge, ShieldWallReduction);
        return true;
    }
}





