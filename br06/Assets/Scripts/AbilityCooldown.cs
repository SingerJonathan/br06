using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class AbilityCooldown : MonoBehaviour
{
    public string abilityButtonAxisName = "Fire1";
    public Image darkMask;
    public Text CooldownTextDisplay;

    [SerializeField] private Ability ability;
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private Image myButtonImage;
    private Animator weaponHolderAnimator;
    private AudioSource abilitySource;
    private AudioSource abilityHitSource;
    private float CooldownDuration;
    private float nextReadyTime;
    private float CooldownTimeLeft;

    public Ability Ability
    {
        get
        {
            return ability;
        }

        set
        {
            ability = value;
        }
    }

    void Start()
    {
        Initialize(ability, weaponHolder);
        weaponHolderAnimator = weaponHolder.GetComponent<Animator>();
    }

    public void Initialize(Ability selectedAbility, GameObject weaponHolder)
    {
        ability = selectedAbility;
        //myButtonImage = GetComponent<Image>();
        abilitySource = GetComponent<AudioSource>();
        if (transform.Find("Hit Sound").GetComponent<AudioSource>())
            abilityHitSource = transform.Find("Hit Sound").GetComponent<AudioSource>();
        myButtonImage.sprite = ability.aSprite;
        //darkMask.sprite = ability.aSprite;
        CooldownDuration = ability.aBaseCooldown;
        ability.Initialize(weaponHolder);
        AbilityReady();
    }

    // Update is called once per frame
    void Update()
    {
        bool CooldownComplete = (Time.time > nextReadyTime);
        if (CooldownComplete)
        {
            AbilityReady();
            if (Input.GetButtonDown(abilityButtonAxisName))
            {
                ButtonTriggered();
            }
        }
        else
        {
            Cooldown();
        }
    }

    public void AbilityReady()
    {
        CooldownTextDisplay.enabled = false;
        darkMask.enabled = false;
    }

    private void Cooldown()
    {
        CooldownTimeLeft -= Time.deltaTime;
        float roundedCd = Mathf.Round(CooldownTimeLeft);
        CooldownTextDisplay.text = roundedCd.ToString();
        darkMask.fillAmount = (CooldownTimeLeft / CooldownDuration);
    }

    private void ButtonTriggered()
    {
        if (!weaponHolderAnimator.GetCurrentAnimatorStateInfo(0).IsName("Dodge") && !weaponHolderAnimator.GetCurrentAnimatorStateInfo(0).IsName("Ability"))
        {
            if(ability.aClip)
            {
                AnimatorOverrideController overrideController = new AnimatorOverrideController(weaponHolderAnimator.runtimeAnimatorController);
                List<KeyValuePair<AnimationClip, AnimationClip>> clips = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                foreach (AnimationClip clip in overrideController.animationClips)
                    if (clip.name.Contains("Ability"))
                        clips.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, ability.aClip));
                overrideController.ApplyOverrides(clips);
                weaponHolderAnimator.runtimeAnimatorController = overrideController;
            }
            weaponHolderAnimator.SetBool("ability", true);
            StartCoroutine(CompleteAnimation());
            nextReadyTime = CooldownDuration + Time.time;
            CooldownTimeLeft = CooldownDuration;
            darkMask.enabled = true;
            CooldownTextDisplay.enabled = true;
            if (ability.aSound)
            {
                abilitySource.clip = ability.aSound;
                abilitySource.Play();
            }
        }
    }

    private IEnumerator CompleteAnimation()
    {
        yield return new WaitForSeconds(ability.aTimeBeforeTrigger);
        if (ability.aHitSound)
            abilityHitSource.Play();
        ability.TriggerAbility();
        yield return new WaitForSeconds(ability.aClip.length - ability.aTimeBeforeTrigger);
        weaponHolderAnimator.SetBool("ability", false);
    }
}