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
