using UnityEngine;

public class LongbowAbility1 : Ability
{
    public int WeaponDamage = 10;
    private HitboxTriggerController _hitboxTriggerController;
    private GameObject _characterGameObject;
    public static string headString = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head/EyeLevel";
    public static string leftHandString = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand/Weapon";

    public override void Initialize(GameObject obj)
    {
        _enemyColour = obj.name.Contains("Red") ? "Blue" : "Red";
        _characterGameObject = obj;
    }

    public override void TriggerAbility()
    {
        GameObject arrow = (GameObject) Instantiate(Resources.Load("Arrow"));
        Arrow arrowComponent = arrow.transform.GetChild(0).GetComponent<Arrow>();
        arrowComponent.EnemyColour = _enemyColour;
        arrowComponent.Damage = WeaponDamage;
        arrow.transform.position = _characterGameObject.transform.Find(leftHandString).position;
        arrow.transform.rotation = _characterGameObject.transform.rotation;
        if (_characterGameObject.name.Contains("Red"))
        {
            arrowComponent.GetComponent<Renderer>().material = arrowComponent.RedMaterial;
            arrowComponent.HitParticleSystem = arrowComponent.RedParticleSystem;
        }
        else
        {
            arrowComponent.GetComponent<Renderer>().material = arrowComponent.BlueMaterial;
            arrowComponent.HitParticleSystem = arrowComponent.BlueParticleSystem;
        }
    }
}
