using UnityEngine;

public class MeleeAbility : Ability
{
    public int WeaponDamage = 1;
    private HitboxTriggerController _hitboxTriggerController;
    public static string headString = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head/EyeLevel";

    public override void Initialize(GameObject obj)
    {
        if (this.aName.Contains("Spear"))
            _hitboxTriggerController = obj.transform.Find("HitboxLine").GetComponent<HitboxTriggerController>();
        else if (this.aName.Contains("Longbow"))
            _hitboxTriggerController = obj.transform.Find("HitboxArrow").GetComponent<HitboxTriggerController>();
        else
        _hitboxTriggerController = obj.transform.Find("HitboxSemiCircle").GetComponent<HitboxTriggerController>();
        _enemyColour = obj.name.Contains("Red") ? "Blue" : "Red";
    }

    public override void TriggerAbility()
    {
        for (int index = 0; index < _hitboxTriggerController.CollidingObjects.Count; index++)
        {
            if (_hitboxTriggerController.CollidingObjects[index].name.Contains(_enemyColour))
            {
                Transform origin = _hitboxTriggerController.transform.parent.gameObject.transform.Find(headString);
                Transform target = GameObject.FindGameObjectWithTag(_enemyColour).transform.Find(headString);
                RaycastHit hit;
                if (Physics.Raycast(origin.position, target.position - origin.position, out hit, 100, ~(1 << 8)))
                {
                    Debug.DrawRay(origin.position, target.position - origin.position, Color.red, 10);
                    if (hit.transform.name.Contains(_enemyColour))
                        _hitboxTriggerController.CollidingObjects[index].GetComponent<CharacterStatsController>().HitPoints -= WeaponDamage;
                }
            }
        }
    }

}
