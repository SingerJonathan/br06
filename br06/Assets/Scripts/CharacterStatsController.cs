using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatsController : MonoBehaviour
{
    public int HitPoints = 100;
    public float DodgeCooldown = 5;
    public int PotionHealValue = 50;
    public static int InitialPotions = 3;
    public static int MaxPotions = 4;
    public float BleedInterval = 1f;
    public float BleedTime = 1f;
    private bool bleed = false;

    public Text PotionText;
    public ParticleSystem HealParticleSystem;

    private int _potions = 3;
    private int _maxHitPoints;

    public int MaxHitPoints
    {
        get
        {
            return _maxHitPoints;
        }

        set
        {
            _maxHitPoints = value;
        }
    }

    public int Potions
    {
        get
        {
            return _potions;
        }

        set
        {
            _potions = value;
            if (_potions > MaxPotions)
                _potions = MaxPotions;
            PotionText.text = "" + _potions;
        }
    }

    public void DrinkPotion()
    {
        if (Potions > 0 && HitPoints < MaxHitPoints)
        {
            HitPoints += PotionHealValue;
            if (HitPoints > MaxHitPoints)
                HitPoints = MaxHitPoints;
            Potions -= 1;
            HealParticleSystem.Play();
        }
    }

    void Start()
    {
        MaxHitPoints = HitPoints;
	}
	
	void Update()
    {
        if (Input.GetButtonDown("Potion" + GetComponent<CharacterAnimationController>().PlayerNumber))
        {
            DrinkPotion();
        }
        if (bleed)
            StartCoroutine(BleedRoutine());
    }

    public void setBleed()
    {
        if (!bleed)
            bleed = true;
    }

    IEnumerator BleedRoutine()
    {
        bleed = false;
        for(int i = 0; i <= 4; i++)
        {
            yield return new WaitForSeconds(1f);
            HitPoints -= 3;
        }
    }
}
