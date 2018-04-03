using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public string aName = "New Ability";
    public string aTitle = "Ability Name";
    public string aDescription = "Ability Description";
    public Sprite aSprite;
    public AudioClip aSound;
    public AudioClip aHitSound;
    public AnimationClip aClip;
    public float aBaseCooldown = 1f;
    public float aTimeBeforeTrigger;
    protected string _enemyColour;

    public abstract void Initialize(GameObject obj);

    public abstract void TriggerAbility();
}
