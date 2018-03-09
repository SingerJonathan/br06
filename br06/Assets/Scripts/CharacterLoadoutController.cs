using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLoadoutController : MonoBehaviour
{
    public static Weapon[] Weapons;

    private static List<Vector3> _weaponPositions = new List<Vector3>
    {
        new Vector3(0.00434f, 0.00542f, 0.00539f), // Battleaxe
        new Vector3(0.00453f, 0.00752f, -0.00454f), // Greataxe
        new Vector3(0.03305f, 0.00186f, 0.0084f), // Longbow
        new Vector3(0.00146f, 0.00637f, 0.00214f), // Longsword
        new Vector3(0.00453f, 0.00752f, -0.00454f) // Spear
    };

    private static List<Vector3> _weaponEulerAngles = new List<Vector3>
    {
        new Vector3(-178, 45, -90), // Battleaxe
        new Vector3(-188, 143, -75), // Greataxe
        new Vector3(-186, 78, 180), // Longbow
        new Vector3(-178, 45, -90), // Longsword
        new Vector3(-188, 143, -75) // Spear
    };

    private static List<Vector3> _weaponPositionsOffhand = new List<Vector3>
    {
        new Vector3(-0.007f, 0.006f, 0.0005f), // Battleaxe
        new Vector3(-0.007f, 0.006f, 0.0005f), // None
        new Vector3(-0.007f, 0.006f, 0.0005f), // None
        new Vector3(-0.003f, 0.006f, -0.0005f), // Longsword
    };

    private static List<Vector3> _weaponEulerAnglesOffhand = new List<Vector3>
    {
        new Vector3(-178, -76, -90), // Battleaxe
        new Vector3(-178, -76, -90), // None
        new Vector3(-178, -76, -90), // None
        new Vector3(-178, -76, -90), // Longsword
    };

    public enum WeaponEnum
    {
        None, Battleaxe, Greataxe, Longbow, Longsword, Spear
    }

    public CanvasRenderer LoadoutPanel;

    public AbilityCooldown[] Abilities;

    public Image[] LoadoutGUIAbilityIcons;
    public Image[] LoadoutGUIMutatationIcons;
    public Image[] LoadoutGUIDescriptionIcons;
    private int _abilityIndex;
    private int _mutationIndex;

    private int _mutationsAvailable;
    public Text MutationsAvailableText;


    [HideInInspector]
    public Toggle ReadyToggle;
    private bool _ready;

    private WeaponEnum _mainWeapon;
    private WeaponEnum _offhandWeapon;

    private Dropdown _mainWeaponDropdown;
    private Dropdown _offhandWeaponDropdown;

    private GameObject _mainWeaponGameObject;
    private GameObject _offhandWeaponGameObject;

    private Transform _characterRightHandTransform;
    private Transform _characterLeftHandTransform;

    public Color ReadyColor;
    public Color UnreadyColor;


    public WeaponEnum MainWeapon
    {
        get
        {
            return _mainWeapon;
        }

        set
        {
            _mainWeapon = value;
            ChangeWeapon(value);
        }
    }
    
    public WeaponEnum OffhandWeapon
    {
        get
        {
            return _offhandWeapon;
        }

        set
        {
            _offhandWeapon = value;
            ChangeWeapon(value, true);
        }
    }

    public bool Ready
    {
        get
        {
            return _ready;
        }

        set
        {
            _ready = value;
            if (_ready)
                ReadyToggle.transform.Find("Background").GetComponent<Image>().color = ReadyColor;
            else
                ReadyToggle.transform.Find("Background").GetComponent<Image>().color = UnreadyColor;
        }
    }

    public int AbilityIndex
    {
        get
        {
            return _abilityIndex;
        }

        set
        {
            _abilityIndex = value;
        }
    }

    public int MutationIndex
    {
        get
        {
            return _mutationIndex;
        }

        set
        {
            _mutationIndex = value;
        }
    }

    public int MutationsAvailable
    {
        get
        {
            return _mutationsAvailable;
        }

        set
        {
            _mutationsAvailable = value;
            MutationsAvailableText.text = "" + value;
        }
    }

    public GameObject MainWeaponGameObject
    {
        get
        {
            return _mainWeaponGameObject;
        }

        set
        {
            _mainWeaponGameObject = value;
        }
    }

    public GameObject OffhandWeaponGameObject
    {
        get
        {
            return _offhandWeaponGameObject;
        }

        set
        {
            _offhandWeaponGameObject = value;
        }
    }

    public void DecrementMutationsAvailable()
    {
        MutationsAvailable--;
    }

    public void SetAbilitiesActive(bool active)
    {
        foreach (AbilityCooldown ability in Abilities)
            ability.enabled = active;
    }

    public void ResetAbilityCooldowns()
    {
        foreach (AbilityCooldown ability in Abilities)
            ability.AbilityReady();
    }

    private void ChangeWeapon(WeaponEnum weapon, bool offhand = false)
    {
        if (_characterRightHandTransform == null)
            Start();

        Transform handTransform = !offhand ? _characterRightHandTransform : _characterLeftHandTransform;

        // Remove game object for old weapon
        if (handTransform.childCount > 0)
            Destroy(handTransform.GetChild(0).gameObject);
        if (_characterLeftHandTransform.childCount > 0 && _characterLeftHandTransform.GetChild(0).GetComponent<Weapon>().Bow)
            Destroy(_characterLeftHandTransform.GetChild(0).gameObject);

        if (weapon == WeaponEnum.None) return;
        
        // Create game object from prefab
        GameObject weaponGameObject;
        switch (weapon)
        {
            case WeaponEnum.Longsword:
                weaponGameObject = (GameObject) Instantiate(Resources.Load("Weapons/Longsword"));
                break;
            case WeaponEnum.Battleaxe:
                weaponGameObject = (GameObject) Instantiate(Resources.Load("Weapons/Battleaxe"));
                break;
            case WeaponEnum.Greataxe:
                weaponGameObject = (GameObject) Instantiate(Resources.Load("Weapons/Greataxe"));
                break;
            case WeaponEnum.Spear:
                weaponGameObject = (GameObject) Instantiate(Resources.Load("Weapons/Spear"));
                break;
            case WeaponEnum.Longbow:
                weaponGameObject = (GameObject) Instantiate(Resources.Load("Weapons/Longbow"));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Weapon weaponComponent = weaponGameObject.GetComponent<Weapon>();

        // Make sure the correct character running animation is used
        GetComponent<CharacterAnimationController>().TwoHandedRun = weaponComponent.TwoHanded;
        GetComponent<CharacterAnimationController>().MirrorRun = weaponComponent.Bow;

        // Assign parent for new weapon and adjust position
        if (weaponComponent.Bow)
            handTransform = _characterLeftHandTransform;
        weaponGameObject.transform.SetParent(handTransform);
        if (offhand)
        {
            weaponGameObject.transform.localPosition = _weaponPositionsOffhand[(int)weapon - 1];
            weaponGameObject.transform.localEulerAngles = _weaponEulerAnglesOffhand[(int)weapon - 1];
        }
        else
        {
            weaponGameObject.transform.localPosition = _weaponPositions[(int)weapon - 1];
            weaponGameObject.transform.localEulerAngles = _weaponEulerAngles[(int)weapon - 1];
        }

        if (!offhand)
            _mainWeaponGameObject = weaponGameObject;
        else
            _offhandWeaponGameObject = weaponGameObject;

        // Assign weapon abilities to buttons and HUD icons
        for (int index = 0; index < weaponComponent.Abilities.Length; index++)
            if ((!offhand && index < weaponComponent.Abilities.Length - 1) || (offhand && index == weaponComponent.Abilities.Length - 1) || weaponComponent.TwoHanded)
            {
                Abilities[index].Ability = weaponComponent.Abilities[index];
                Abilities[index].Initialize(weaponComponent.Abilities[index], gameObject);
                LoadoutGUIAbilityIcons[index].sprite = weaponComponent.Abilities[index].aSprite;
                LoadoutGUIAbilityIcons[index].transform.parent.Find("Description").GetComponent<Text>().text = weaponComponent.Abilities[index].aDescription;
                LoadoutGUIAbilityIcons[index].transform.parent.Find("Description/Name").GetComponent<Text>().text = weaponComponent.Abilities[index].aTitle;
                LoadoutGUIDescriptionIcons[index].sprite = weaponComponent.Abilities[index].aSprite;
                LoadoutGUIDescriptionIcons[index].transform.parent.Find("Description").GetComponent<Text>().text = weaponComponent.Abilities[index].aDescription;
                LoadoutGUIDescriptionIcons[index].transform.parent.Find("Description/Name").GetComponent<Text>().text = weaponComponent.Abilities[index].aTitle;
            }
    }

    private void OnWeaponDropdownValueChanged(Dropdown dropdown)
    {
        int weapon = -1;
        if (dropdown.options[dropdown.value].text != "None")
            for (int i = 0; i < Weapons.Length && weapon == -1; i++)
                if (dropdown.options[dropdown.value].text == Weapons[i].name)
                    weapon = i;

        if (dropdown.gameObject.name.Contains("Main"))
        {
            MainWeapon = (WeaponEnum)weapon+1;
            if (weapon != -1 && Weapons[weapon].TwoHanded)
            {
                _offhandWeaponDropdown.interactable = false;
                _offhandWeaponDropdown.value = 0;
            }
            else
            {
                _offhandWeaponDropdown.interactable = true;
            }
        }
        else
            OffhandWeapon = (WeaponEnum)weapon+1;
        if (!_mainWeaponGameObject.GetComponent<Weapon>().TwoHanded && _offhandWeaponDropdown.value == 0)
            _offhandWeaponDropdown.value = 1;
    }

    public void MutateAbility(bool resetMutation)
    {
        Weapon mainWeaponComponent = _mainWeaponGameObject.GetComponent<Weapon>();
        Weapon offhandWeaponComponent;
        if (_offhandWeaponGameObject)
            offhandWeaponComponent = _offhandWeaponGameObject.GetComponent<Weapon>();
        if (resetMutation)
        {
            if (_abilityIndex == mainWeaponComponent.Abilities.Length - 1)
            {
                if (_offhandWeaponDropdown.value != 0 && _offhandWeaponGameObject)
                {
                    offhandWeaponComponent = _offhandWeaponGameObject.GetComponent<Weapon>();
                    Abilities[_abilityIndex].Ability = offhandWeaponComponent.Abilities[_abilityIndex];
                    Abilities[_abilityIndex].Initialize(offhandWeaponComponent.Abilities[_abilityIndex], gameObject);
                    LoadoutGUIAbilityIcons[_abilityIndex].sprite = offhandWeaponComponent.Abilities[_abilityIndex].aSprite;
                    LoadoutGUIMutatationIcons[2].sprite = offhandWeaponComponent.MutatedAbilities[2].aSprite;
                    LoadoutGUIMutatationIcons[3].sprite = offhandWeaponComponent.MutatedAbilities[3].aSprite;
                    LoadoutGUIAbilityIcons[_abilityIndex].transform.parent.Find("Description").GetComponent<Text>().text = offhandWeaponComponent.Abilities[_abilityIndex].aDescription;
                    LoadoutGUIAbilityIcons[_abilityIndex].transform.parent.Find("Description/Name").GetComponent<Text>().text = offhandWeaponComponent.Abilities[_abilityIndex].aTitle;
                    LoadoutGUIMutatationIcons[2].transform.parent.Find("Description").GetComponent<Text>().text = offhandWeaponComponent.MutatedAbilities[2].aDescription;
                    LoadoutGUIMutatationIcons[2].transform.parent.Find("Description/Name").GetComponent<Text>().text = offhandWeaponComponent.MutatedAbilities[2].aTitle;
                    LoadoutGUIMutatationIcons[3].transform.parent.Find("Description").GetComponent<Text>().text = offhandWeaponComponent.MutatedAbilities[3].aDescription;
                    LoadoutGUIMutatationIcons[3].transform.parent.Find("Description/Name").GetComponent<Text>().text = offhandWeaponComponent.MutatedAbilities[3].aTitle;
                }
                else// if (mainWeaponComponent.TwoHanded)
                {
                    Abilities[_abilityIndex].Ability = mainWeaponComponent.Abilities[_abilityIndex];
                    Abilities[_abilityIndex].Initialize(mainWeaponComponent.Abilities[_abilityIndex], gameObject);
                    LoadoutGUIAbilityIcons[_abilityIndex].sprite = mainWeaponComponent.Abilities[_abilityIndex].aSprite;
                    LoadoutGUIMutatationIcons[2].sprite = mainWeaponComponent.MutatedAbilities[2].aSprite;
                    LoadoutGUIMutatationIcons[3].sprite = mainWeaponComponent.MutatedAbilities[3].aSprite;
                    LoadoutGUIAbilityIcons[_abilityIndex].transform.parent.Find("Description").GetComponent<Text>().text = mainWeaponComponent.Abilities[_abilityIndex].aDescription;
                    LoadoutGUIAbilityIcons[_abilityIndex].transform.parent.Find("Description/Name").GetComponent<Text>().text = mainWeaponComponent.Abilities[_abilityIndex].aTitle;
                    LoadoutGUIMutatationIcons[2].transform.parent.Find("Description").GetComponent<Text>().text = mainWeaponComponent.MutatedAbilities[2].aDescription;
                    LoadoutGUIMutatationIcons[2].transform.parent.Find("Description/Name").GetComponent<Text>().text = mainWeaponComponent.MutatedAbilities[2].aTitle;
                    LoadoutGUIMutatationIcons[3].transform.parent.Find("Description").GetComponent<Text>().text = mainWeaponComponent.MutatedAbilities[3].aDescription;
                    LoadoutGUIMutatationIcons[3].transform.parent.Find("Description/Name").GetComponent<Text>().text = mainWeaponComponent.MutatedAbilities[3].aTitle;
                }
            }
            else
            {
                Abilities[_abilityIndex].Ability = mainWeaponComponent.Abilities[_abilityIndex];
                Abilities[_abilityIndex].Initialize(mainWeaponComponent.Abilities[_abilityIndex], gameObject);
                LoadoutGUIAbilityIcons[_abilityIndex].sprite = mainWeaponComponent.Abilities[_abilityIndex].aSprite;
                LoadoutGUIMutatationIcons[0].sprite = mainWeaponComponent.MutatedAbilities[0].aSprite;
                LoadoutGUIMutatationIcons[1].sprite = mainWeaponComponent.MutatedAbilities[1].aSprite;
                LoadoutGUIAbilityIcons[_abilityIndex].transform.parent.Find("Description").GetComponent<Text>().text = mainWeaponComponent.Abilities[_abilityIndex].aDescription;
                LoadoutGUIAbilityIcons[_abilityIndex].transform.parent.Find("Description/Name").GetComponent<Text>().text = mainWeaponComponent.Abilities[_abilityIndex].aTitle;
                LoadoutGUIMutatationIcons[0].transform.parent.Find("Description").GetComponent<Text>().text = mainWeaponComponent.MutatedAbilities[0].aDescription;
                LoadoutGUIMutatationIcons[0].transform.parent.Find("Description/Name").GetComponent<Text>().text = mainWeaponComponent.MutatedAbilities[0].aTitle;
                LoadoutGUIMutatationIcons[1].transform.parent.Find("Description").GetComponent<Text>().text = mainWeaponComponent.MutatedAbilities[1].aDescription;
                LoadoutGUIMutatationIcons[1].transform.parent.Find("Description/Name").GetComponent<Text>().text = mainWeaponComponent.MutatedAbilities[1].aTitle;
            }
        }
        else if (_abilityIndex == mainWeaponComponent.Abilities.Length - 1)
        {
            if (_offhandWeaponDropdown.value != 0)
            {
                offhandWeaponComponent = _offhandWeaponGameObject.GetComponent<Weapon>();
                Abilities[_abilityIndex].Ability = offhandWeaponComponent.MutatedAbilities[_mutationIndex + 2];
                Abilities[_abilityIndex].Initialize(offhandWeaponComponent.MutatedAbilities[_mutationIndex + 2], gameObject);
                LoadoutGUIMutatationIcons[_mutationIndex + 2].sprite = LoadoutGUIAbilityIcons[_abilityIndex].sprite;
                LoadoutGUIAbilityIcons[_abilityIndex].sprite = offhandWeaponComponent.MutatedAbilities[_mutationIndex + 2].aSprite;
                LoadoutGUIMutatationIcons[_mutationIndex + 2].transform.parent.Find("Description").GetComponent<Text>().text = LoadoutGUIAbilityIcons[_abilityIndex].transform.parent.Find("Description").GetComponent<Text>().text;
                LoadoutGUIMutatationIcons[_mutationIndex + 2].transform.parent.Find("Description/Name").GetComponent<Text>().text = LoadoutGUIAbilityIcons[_abilityIndex].transform.parent.Find("Description/Name").GetComponent<Text>().text;
                LoadoutGUIAbilityIcons[_abilityIndex].transform.parent.Find("Description").GetComponent<Text>().text = offhandWeaponComponent.MutatedAbilities[_mutationIndex + 2].aDescription;
                LoadoutGUIAbilityIcons[_abilityIndex].transform.parent.Find("Description/Name").GetComponent<Text>().text = offhandWeaponComponent.MutatedAbilities[_mutationIndex + 2].aTitle;
            }
            else// if (mainWeaponComponent.TwoHanded)
            {
                Abilities[_abilityIndex].Ability = mainWeaponComponent.MutatedAbilities[_mutationIndex + 2];
                Abilities[_abilityIndex].Initialize(mainWeaponComponent.MutatedAbilities[_mutationIndex + 2], gameObject);
                LoadoutGUIMutatationIcons[_mutationIndex + 2].sprite = LoadoutGUIAbilityIcons[_abilityIndex].sprite;
                LoadoutGUIAbilityIcons[_abilityIndex].sprite = mainWeaponComponent.MutatedAbilities[_mutationIndex + 2].aSprite;
                LoadoutGUIMutatationIcons[_mutationIndex + 2].transform.parent.Find("Description").GetComponent<Text>().text = LoadoutGUIAbilityIcons[_abilityIndex].transform.parent.Find("Description").GetComponent<Text>().text;
                LoadoutGUIMutatationIcons[_mutationIndex + 2].transform.parent.Find("Description/Name").GetComponent<Text>().text = LoadoutGUIAbilityIcons[_abilityIndex].transform.parent.Find("Description/Name").GetComponent<Text>().text;
                LoadoutGUIAbilityIcons[_abilityIndex].transform.parent.Find("Description").GetComponent<Text>().text = mainWeaponComponent.MutatedAbilities[_mutationIndex + 2].aDescription;
                LoadoutGUIAbilityIcons[_abilityIndex].transform.parent.Find("Description/Name").GetComponent<Text>().text = mainWeaponComponent.MutatedAbilities[_mutationIndex + 2].aTitle;
            }
        }
        else
        {
            Abilities[_abilityIndex].Ability = mainWeaponComponent.MutatedAbilities[_mutationIndex];
            Abilities[_abilityIndex].Initialize(mainWeaponComponent.MutatedAbilities[_mutationIndex], gameObject);
            LoadoutGUIMutatationIcons[_mutationIndex].sprite = LoadoutGUIAbilityIcons[_abilityIndex].sprite;
            LoadoutGUIAbilityIcons[_abilityIndex].sprite = mainWeaponComponent.MutatedAbilities[_mutationIndex].aSprite;
            LoadoutGUIMutatationIcons[_mutationIndex].transform.parent.Find("Description").GetComponent<Text>().text = LoadoutGUIAbilityIcons[_abilityIndex].transform.parent.Find("Description").GetComponent<Text>().text;
            LoadoutGUIMutatationIcons[_mutationIndex].transform.parent.Find("Description/Name").GetComponent<Text>().text = LoadoutGUIAbilityIcons[_abilityIndex].transform.parent.Find("Description/Name").GetComponent<Text>().text;
            LoadoutGUIAbilityIcons[_abilityIndex].transform.parent.Find("Description").GetComponent<Text>().text = mainWeaponComponent.MutatedAbilities[_mutationIndex].aDescription;
            LoadoutGUIAbilityIcons[_abilityIndex].transform.parent.Find("Description/Name").GetComponent<Text>().text = mainWeaponComponent.MutatedAbilities[_mutationIndex].aTitle;
        }
    }

    void Start()
    {
        _characterRightHandTransform = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/Weapon");
        _characterLeftHandTransform = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand/Weapon");

        _mainWeaponDropdown = LoadoutPanel.transform.Find("Main Weapon Dropdown").GetComponent<Dropdown>();
        _offhandWeaponDropdown = LoadoutPanel.transform.Find("Offhand Weapon Dropdown").GetComponent<Dropdown>();

        _mainWeaponDropdown.onValueChanged.AddListener(delegate
        {
            OnWeaponDropdownValueChanged(_mainWeaponDropdown);
        });
        _offhandWeaponDropdown.onValueChanged.AddListener(delegate
        {
            OnWeaponDropdownValueChanged(_offhandWeaponDropdown);
        });

        ReadyToggle = LoadoutPanel.transform.Find("Ready Toggle").GetComponent<Toggle>();

        Weapons = Resources.LoadAll<Weapon>("Weapons");
        ChangeWeapon(WeaponEnum.Longsword, false);
        ChangeWeapon(WeaponEnum.Longsword, true);
    }

    void OnDestroy()
    {
        _mainWeaponDropdown.onValueChanged.RemoveAllListeners();
        _offhandWeaponDropdown.onValueChanged.RemoveAllListeners();
    }
}
