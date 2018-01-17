﻿using System;
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

    public enum WeaponEnum
    {
        None, Battleaxe, Greataxe, Longbow, Longsword, Spear
    }

    public CanvasRenderer LoadoutPanel;

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
                ReadyToggle.transform.Find("Background").GetComponent<Image>().color = Color.green;
            else
                ReadyToggle.transform.Find("Background").GetComponent<Image>().color = Color.white;
        }
    }

    private void ChangeWeapon(WeaponEnum weapon, bool offhand = false)
    {
        // DEVNOTE: Remove this when Loadout GUI is implemented
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

        // Make sure the correct character running animation is used
        GetComponent<CharacterAnimationController>().MirrorRun = weaponGameObject.GetComponent<Weapon>().Bow;

        // Assign parent for new weapon and adjust position
        if (weaponGameObject.GetComponent<Weapon>().Bow)
            handTransform = _characterLeftHandTransform;
        weaponGameObject.transform.SetParent(handTransform);
        weaponGameObject.transform.localPosition = _weaponPositions[(int)weapon-1];
        weaponGameObject.transform.localEulerAngles = _weaponEulerAngles[(int)weapon-1];

        if (!offhand)
            _mainWeaponGameObject = weaponGameObject;
        else
            _offhandWeaponGameObject = weaponGameObject;
    }

    // DEVNOTE: Consider moving this into OnValueChanged in inspector for dropdown
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
                _offhandWeaponDropdown.enabled = false;
                _offhandWeaponDropdown.value = 0;
            }
            else
            {
                _offhandWeaponDropdown.enabled = true;
            }
        }
        else
            OffhandWeapon = (WeaponEnum)weapon+1;
    }

    void Start()
    {
        _characterRightHandTransform = transform.Find("mixamorig:Hips").Find("mixamorig:Spine").Find("mixamorig:Spine1").Find("mixamorig:Spine2").Find("mixamorig:RightShoulder").Find("mixamorig:RightArm").Find("mixamorig:RightForeArm").Find("mixamorig:RightHand");
        _characterLeftHandTransform = transform.Find("mixamorig:Hips").Find("mixamorig:Spine").Find("mixamorig:Spine1").Find("mixamorig:Spine2").Find("mixamorig:LeftShoulder").Find("mixamorig:LeftArm").Find("mixamorig:LeftForeArm").Find("mixamorig:LeftHand");

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
    }

    void OnDestroy()
    {
        _mainWeaponDropdown.onValueChanged.RemoveAllListeners();
        _offhandWeaponDropdown.onValueChanged.RemoveAllListeners();
    }

    void Update()
    {
		
	}
}
