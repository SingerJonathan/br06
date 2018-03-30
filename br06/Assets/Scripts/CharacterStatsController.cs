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

    public int BleedDamge = 1;
    public float BleedDuration = 1f;
    public float BleedInterval = 1f;
    private bool bleed = false;

    public int ShieldWallReduction = 1;
    private bool ShieldWall = false;
    private int ShieldWallCharge = 1;

    public Text PotionText;
    public ParticleSystem HealParticleSystem;

    private int _potions = 3;
    private int _maxHitPoints;
    private List<int> _damageOverTimeDurations;
    private List<string> _damageOverTimeTags;
    private List<string> _damageOverTimeAudioSourceTags;


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

    public void DoDamage(int DamageValue)
    {
        if(!ShieldWall)
            HitPoints -= DamageValue;
        else
        {
            DamageValue = DamageValue - ShieldWallReduction ;
            ShieldWallCharge -= 1;
            HitPoints -= DamageValue;
            if (ShieldWallCharge == 0)
                ShieldWall = false;
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
        _damageOverTimeDurations = new List<int>();
        _damageOverTimeTags = new List<string>();
        _damageOverTimeAudioSourceTags = new List<string>();
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

    public void AddDamageOverTime(int damage, int duration, string tag = "", string audioSourceTag = "")
    {
        _damageOverTimeDurations.Add(duration);
        _damageOverTimeTags.Add(tag);
        _damageOverTimeAudioSourceTags.Add(audioSourceTag);
        StartCoroutine(DamageOverTimeCoroutine(damage, _damageOverTimeDurations.Count-1));
    }

    public void RemoveDamageOverTimeWithTag(string tag)
    {
        for (int dotIndex = 0; dotIndex < _damageOverTimeTags.Count; dotIndex++)
        {
            if (_damageOverTimeTags[dotIndex] == tag)
            {
                _damageOverTimeDurations[dotIndex] = 0;
            }
        }
    }

    public void ClearDamageOverTime()
    {
        StopAllCoroutines();
        _damageOverTimeDurations.Clear();
        _damageOverTimeTags.Clear();
        _damageOverTimeAudioSourceTags.Clear();
    }

    public void setBleed(int _BleedDamage, float _BleedDuration, float _BleedInterval)
    {

        if (!bleed)
        {
            this.BleedDamge = _BleedDamage;
            this.BleedDuration = _BleedDuration;
            this.BleedInterval = _BleedInterval;
            bleed = true;
        }
    }

    public void EnableShieldWall(int _shieldWallCharge, int _shieldWallReduction)
    {
        if (!ShieldWall)
        {
            ShieldWallCharge = _shieldWallCharge;
            ShieldWallReduction = _shieldWallReduction;
            ShieldWall = true;
        }
        else
            ShieldWallCharge = _shieldWallCharge;
    }

    IEnumerator DamageOverTimeCoroutine(int damage, int durationIndex)
    {
        while (_damageOverTimeDurations[durationIndex] > 0)
        {
            yield return new WaitForSeconds(1);
            HitPoints -= damage;
            _damageOverTimeDurations[durationIndex] -= 1;
            if (_damageOverTimeAudioSourceTags[durationIndex] != "")
                GameObject.FindGameObjectWithTag(_damageOverTimeAudioSourceTags[durationIndex]).GetComponent<AudioSource>().Play();
        }
    }

    IEnumerator BleedRoutine()
    {
        bleed = false;
        float BleedTicks = 1f;
        if (BleedDuration % BleedInterval == 0)
            BleedTicks = BleedDuration / BleedInterval;
        for(int i = 0; i <= BleedTicks; i++)
        {
            yield return new WaitForSeconds(BleedInterval);
            HitPoints -= BleedDamge;
        }
    }
}
