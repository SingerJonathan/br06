using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLoadoutController : MonoBehaviour
{
    public static Weapon[] Weapons;

    public enum WeaponEnum
    {
        None, Battleaxe, Greataxe, Longbow, Longsword, Spear
    }

    public CanvasRenderer LoadoutPanel;

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

    private void ChangeWeapon(WeaponEnum weapon, bool offhand = false)
    {
        // DEVNOTE: Remove this when Loadout GUI is implemented
        if (_characterRightHandTransform == null)
            Start();

        Transform handTransform = !offhand ? _characterRightHandTransform : _characterLeftHandTransform;

        // Remove game object for old weapon
        if (handTransform.childCount > 0)
            Destroy(handTransform.GetChild(0).gameObject);

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

        // Assign parent for new weapon and adjust position
        weaponGameObject.transform.SetParent(handTransform);
        weaponGameObject.transform.localPosition = new Vector3(0.00146f, 0.00637f, 0.00214f);
        weaponGameObject.transform.localEulerAngles = new Vector3(-178, 45, -90);

        // DEVNOTE: Remove this if / when better solution is found
        // Scale weapon to correct size
        weaponGameObject.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

        if (!offhand)
            _mainWeaponGameObject = weaponGameObject;
        else
            _offhandWeaponGameObject = weaponGameObject;
    }

    // DEVNOTE: Consider moving this into OnValueChanged in inspector for dropdown
    private void OnWeaponDropdownValueChanged(Dropdown dropdown)
    {
        WeaponEnum weapon = (WeaponEnum) dropdown.value;
        if (dropdown.gameObject.name.Contains("Main"))
        {
            MainWeapon = weapon;
            if (dropdown.value != 0 && Weapons[dropdown.value - 1].TwoHanded)
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
            OffhandWeapon = weapon;
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
        
        Weapons = Resources.LoadAll<Weapon>("Weapons");
        int test = Weapons.Length;
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
