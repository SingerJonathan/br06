using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongswordPull : Ability
{
    public int WeaponDamage = 1;
    public float PullForce = 1f;
    private HitboxTriggerController _hitboxTriggerController;
    private CharacterAnimationController animator;
    private string myColour;
    public Material RedLineMaterial;
    public Material BlueLineMaterial;

    private LineRenderer _line;
    private GameObject _enemy;

    public override void Initialize(GameObject obj)
    {
        _hitboxTriggerController = obj.transform.Find("HitboxLineLarge").GetComponent<HitboxTriggerController>();
        _enemyColour = obj.name.Contains("Red") ? "Blue" : "Red";
        myColour = obj.name.Contains("Blue") ? "Red" : "Blue";
        animator = GameObject.FindGameObjectWithTag(myColour).GetComponent<CharacterAnimationController>();
    }

    public override bool TriggerAbility()
    {
        bool abilityHit = false;
        for (int index = 0; index < _hitboxTriggerController.CollidingObjects.Count; index++)
        {
            if (_hitboxTriggerController.CollidingObjects[index].name.Contains(_enemyColour))
            {
                Transform origin = _hitboxTriggerController.transform.parent.gameObject.transform.Find(MeleeAbility.headString);
                Transform target = GameObject.FindGameObjectWithTag(_enemyColour).transform.Find(MeleeAbility.headString);
                RaycastHit hit;
                if (Physics.Raycast(origin.position, target.position - origin.position, out hit, 100, ~(1 << 8)))
                {
                    Debug.DrawRay(origin.position, target.position - origin.position, Color.red, 10);
                    if (hit.transform.name.Contains(_enemyColour))
                    {
                        _enemy = hit.transform.gameObject;
                        abilityHit = true;
                        _hitboxTriggerController.CollidingObjects[index].GetComponent<CharacterStatsController>().DoDamage(WeaponDamage);
                        animator.PullTowards(PullForce);
                        _line = gameObject.AddComponent<LineRenderer>();
                        if (_line)
                            _line.startWidth = 0.3f;
                        if (_line)
                            _line.endWidth = 0.3f;
                        if (_line)
                            _line.positionCount = 2;
                        if (_line && _enemyColour.Contains("Red"))
                            _line.material = BlueLineMaterial;
                        else if (_line)
                            _line.material = RedLineMaterial;
                    }
                }
            }
        }
        return abilityHit;
    }

    void Update ()
    {
        if (_line && !animator.PullTowardsValue)
        {
            Destroy(_line);
        }
        else if (_line && _enemy != null)
        {
            _line.SetPosition(0, gameObject.transform.position);
            Vector3 enemyPosition = _enemy.transform.position;
            enemyPosition.y += 5;
            _line.SetPosition(1, enemyPosition);
        }
    }
}

