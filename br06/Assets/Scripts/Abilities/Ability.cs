using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public string aName = "New Ability";
    public Sprite aSprite;
    public AudioClip aSound;
    public float aBaseCooldown = 1f;
    protected string _enemyColour;

    public abstract void Initialize(GameObject obj);

    public abstract void TriggerAbility();
}
