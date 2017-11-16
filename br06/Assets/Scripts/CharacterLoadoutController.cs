using System;
using UnityEngine;

public class CharacterLoadoutController : MonoBehaviour
{
    public enum WeaponEnum
    {
        None, Longsword, Battleaxe, Greataxe, Spear, Longbow
    }

    private WeaponEnum  _mainWeapon;
    private WeaponEnum  _offhandWeapon;

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

        // Remove old game object for old weapon
        if (handTransform.childCount > 0)
            Destroy(handTransform.GetChild(0));

        if (weapon == WeaponEnum.None) return;
        
        // Create game object from prefab
        GameObject weaponGameObject;
        switch (weapon)
        {
            case WeaponEnum.Longsword:
                weaponGameObject = (GameObject) Instantiate(Resources.Load("Longsword"));
                break;
            case WeaponEnum.Battleaxe:
                weaponGameObject = (GameObject) Instantiate(Resources.Load("Battleaxe"));
                break;
            case WeaponEnum.Greataxe:
                weaponGameObject = (GameObject) Instantiate(Resources.Load("Greataxe"));
                break;
            case WeaponEnum.Spear:
                weaponGameObject = (GameObject) Instantiate(Resources.Load("Spear"));
                break;
            case WeaponEnum.Longbow:
                weaponGameObject = (GameObject) Instantiate(Resources.Load("Longbow"));
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

    void Start()
    {
        _characterRightHandTransform = transform.Find("mixamorig:Hips").Find("mixamorig:Spine").Find("mixamorig:Spine1").Find("mixamorig:Spine2").Find("mixamorig:RightShoulder").Find("mixamorig:RightArm").Find("mixamorig:RightForeArm").Find("mixamorig:RightHand");
        _characterLeftHandTransform = transform.Find("mixamorig:Hips").Find("mixamorig:Spine").Find("mixamorig:Spine1").Find("mixamorig:Spine2").Find("mixamorig:LeftShoulder").Find("mixamorig:LeftArm").Find("mixamorig:LeftForeArm").Find("mixamorig:LeftHand");
    }

    void Update()
    {
		
	}
}
