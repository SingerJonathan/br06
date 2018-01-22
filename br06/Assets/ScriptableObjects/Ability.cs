using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string aName = "New Ability";
    public Sprite aSprite;
    public AudioClip aSound;
    public float aBaseCooldown = 1f;

    protected HitboxTriggerController _hitboxTriggerController;
    protected string _enemyColour;

    public abstract void Initialize(GameObject obj);

    public abstract void TriggerAbility();
}
