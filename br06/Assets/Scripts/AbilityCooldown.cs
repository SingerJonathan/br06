using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AbilityCooldown : MonoBehaviour
{

    public string abilityButtonAxisName = "Fire1";
    public Image darkMask;
    public Text CooldownTextDisplay;

    [SerializeField] private Ability ability;
    [SerializeField] private GameObject weaponHolder;
    private Image myButtonImage;
    private AudioSource abilitySource;
    private float CooldownDuration;
    private float nextReadyTime;
    private float CooldownTimeLeft;


    void Start()
    {
        Initialize(ability, weaponHolder);
    }

    public void Initialize(Ability selectedAbility, GameObject weaponHolder)
    {
        ability = selectedAbility;
        myButtonImage = GetComponent<Image>();
        abilitySource = GetComponent<AudioSource>();
        myButtonImage.sprite = ability.aSprite;
        darkMask.sprite = ability.aSprite;
        CooldownDuration = ability.aBaseCooldown;
        ability.Initialise(weaponHolder);
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

    private void AbilityReady()
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
        nextReadyTime = CooldownDuration + Time.time;
        CooldownTimeLeft = CooldownDuration;
        darkMask.enabled = true;
        CooldownTextDisplay.enabled = true;

        abilitySource.clip = ability.aSound;
        abilitySource.Play();
        ability.TriggerAbility();
    }
}