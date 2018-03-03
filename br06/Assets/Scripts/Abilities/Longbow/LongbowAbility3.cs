using UnityEngine;

public class LongbowAbility3 : Ability
{
    public int WeaponDamage = 10;
    private GameObject _characterGameObject;
    public static string leftHandString = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand/Weapon";

    public override void Initialize(GameObject obj)
    {
        _enemyColour = obj.name.Contains("Red") ? "Blue" : "Red";
        _characterGameObject = obj;
    }

    public override void TriggerAbility()
    {
		_characterGameObject.GetComponent<CharacterAnimationController>().JumpBack();
		Invoke("ShootArrow", _characterGameObject.GetComponent<CharacterAnimationController>().knockbackForce);
    }

	private void ShootArrow()
	{
        GameObject arrow = (GameObject) Instantiate(Resources.Load("Big Arrow"));
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
		GameObject.FindGameObjectWithTag("Blaster Sound").GetComponent<AudioSource>().Play();
	}
}
