using UnityEngine;

public class LongbowAbility2 : Ability
{
    public int WeaponDamage = 0;
	public float PrisonDuration = 5;
    private GameObject _characterGameObject;
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
		arrowComponent.CreatePrison = true;
        arrowComponent.EnemyColour = _enemyColour;
		arrowComponent.PrisonDuration = PrisonDuration;
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
