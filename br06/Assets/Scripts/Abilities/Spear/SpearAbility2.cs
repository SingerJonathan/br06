using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearAbility2 : Ability {
    private HitboxTriggerController _hitboxTriggerController;
    public override void Initialize(GameObject obj)
    {
        _hitboxTriggerController = obj.transform.Find("HitboxSemiCircle").GetComponent<HitboxTriggerController>();
        _enemyColour = obj.name.Contains("Red") ? "Blue" : "Red";
    }

    public override void TriggerAbility()
    {
        throw new System.NotImplementedException();
    }
}





