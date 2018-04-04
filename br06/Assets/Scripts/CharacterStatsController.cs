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

    public int BleedDamage = 1;
    public float BleedDuration = 1f;
    public float BleedInterval = 1f;
    private bool bleed = false;

    private bool Vulnerable = false;
    private float VulnerableCooldown = 1f;
    private int VulnerableDamage = 1;

    public int ShieldWallReduction = 1;
    private bool ShieldWall = false;
    private int ShieldWallCharge = 1;

    private int DamageChargeLimit = 1;
    private int DamageChargeDamage = 1;
    private int DamageCharge = 0;

    public Text PotionText;
    public ParticleSystem HealParticleSystem;

    private int _potions = 3;
    private int _maxHitPoints;
    private List<int> _damageOverTimeDurations;
    private List<string> _damageOverTimeTags;
    private List<string> _damageOverTimeAudioSourceTags;

    private float ShieldWallDuration;
    private bool ImprovedShieldWall;

    private bool MortalWound;
    private float MortalWoundDuration;
    private int MortalWoundSeverity;

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
        if(Vulnerable)
        {
            DamageValue = DamageValue + VulnerableDamage;
            Vulnerable = false;
        }
        if(ShieldWall || ImprovedShieldWall)
        {
            DamageValue = DamageValue - ShieldWallReduction;
            ShieldWallCharge -= 1;
            if (ShieldWallCharge <= 0)
            {
                ShieldWall = false;
                transform.Find("Shield Wall/Top Ring").GetComponent<AudioSource>().Play();
                transform.Find("Shield Wall").gameObject.SetActive(false);
            }
        }
        int hitPointsBeforeDamage = HitPoints;
        HitPoints -= DamageValue;
        if (HitPoints <= 0 && hitPointsBeforeDamage >= 1)
            GameObject.FindGameObjectWithTag("Death Sound").GetComponent<AudioSource>().Play();
    }

    public void DrinkPotion()
    {
        if (Potions > 0 && HitPoints < MaxHitPoints)
        {
            if(MortalWound)
                HitPoints += PotionHealValue - MortalWoundSeverity;
            else
                HitPoints += PotionHealValue;
            if (HitPoints > MaxHitPoints)
                HitPoints = MaxHitPoints;
            Potions -= 1;
            HealParticleSystem.Play();
            GameObject.FindGameObjectWithTag("Heal Sound").GetComponent<AudioSource>().Play();
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

    public void VulnerableAttack(float _vulnerableCooldown, int _vulnerableDamage)
    {
        Vulnerable = true;
        VulnerableCooldown = _vulnerableCooldown;
        VulnerableDamage = _vulnerableDamage;
        StartCoroutine(VulnerableWindow());
    }

    public void setBleed(int _BleedDamage, float _BleedDuration, float _BleedInterval)
    {
        if (!bleed)
        {
            BleedDamage = _BleedDamage;
            BleedDuration = _BleedDuration;
            BleedInterval = _BleedInterval;
            bleed = true;
        }
    }

    public void EnableShieldWall(int _shieldWallCharge, int _shieldWallReduction)
    {
        transform.Find("Shield Wall").gameObject.SetActive(true);
        if (!ShieldWall)
        {
            ShieldWallCharge = _shieldWallCharge;
            ShieldWallReduction = _shieldWallReduction;
            ShieldWall = true;
        }
        else
            ShieldWallCharge = _shieldWallCharge;
    }

    public void EnableTimedShieldWall(float _shieldWallDuration, int _shieldWallReduction)
    {
        transform.Find("Shield Wall").gameObject.SetActive(true);
        if (!ImprovedShieldWall)
        {
            ShieldWallDuration = _shieldWallDuration;
            ShieldWallReduction = _shieldWallReduction;
            ImprovedShieldWall = true;
            StartCoroutine(ImprovedShieldClock());
        }
        else if (ShieldWallDuration == _shieldWallDuration && ShieldWallReduction == _shieldWallReduction)
        {
            StopCoroutine(ImprovedShieldClock());
            StartCoroutine(ImprovedShieldClock());
        }
        else
        {
            ShieldWallDuration = _shieldWallDuration;
            ShieldWallReduction = _shieldWallReduction;
            StopCoroutine(ImprovedShieldClock());
            StartCoroutine(ImprovedShieldClock());
        }
    }

    public void AddDamageCharge(int _damageChargeDamage, int _damageChargeLimit)
    {
        if(DamageChargeDamage != _damageChargeDamage && DamageChargeLimit != _damageChargeLimit)
        {
            DamageChargeDamage = _damageChargeDamage;
            DamageChargeLimit = _damageChargeLimit;
        }
        DamageCharge += 1;
        if (DamageCharge == DamageChargeLimit)
        {
            DamageCharge = 0;
            DoDamage(DamageChargeDamage);
        }
    }

    public void EnableMortalWound(float _mortalWoundDuration, int _mortalWoundSeverity)
    {
        if(!MortalWound)
        {
            MortalWoundDuration = _mortalWoundDuration;
            MortalWoundSeverity = _mortalWoundSeverity;
            MortalWound = true;
            StartCoroutine(MortalWoundTimer());
        }
        else if (MortalWoundDuration == _mortalWoundDuration && MortalWoundSeverity == _mortalWoundSeverity)
        {
            StopCoroutine(MortalWoundTimer());
            StartCoroutine(MortalWoundTimer());
        }
        else
        {
            MortalWoundDuration = _mortalWoundDuration;
            MortalWoundSeverity = _mortalWoundSeverity;
            StopCoroutine(MortalWoundTimer());
            StartCoroutine(MortalWoundTimer());
        }
    }

    IEnumerator DamageOverTimeCoroutine(int damage, int durationIndex)
    {
        while (_damageOverTimeDurations[durationIndex] > 0)
        {
            yield return new WaitForSeconds(1);
            DoDamage(damage);
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
            DoDamage(BleedDamage);
        }
    }

    IEnumerator VulnerableWindow()
    {
        yield return new WaitForSeconds(VulnerableCooldown);
        Vulnerable = false;
    }

    IEnumerator ImprovedShieldClock()
    {
        yield return new WaitForSeconds(ShieldWallDuration);
        ImprovedShieldWall = false;
        transform.Find("Shield Wall/Top Ring").GetComponent<AudioSource>().Play();
        transform.Find("Shield Wall").gameObject.SetActive(false);
    }

    IEnumerator MortalWoundTimer()
    {
        yield return new WaitForSeconds(MortalWoundDuration);
        MortalWound = false;
    }
}
