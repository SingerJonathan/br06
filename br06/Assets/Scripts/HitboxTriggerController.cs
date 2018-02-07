using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxTriggerController : MonoBehaviour
{
    private List<GameObject> _collidingObjects;

    public List<GameObject> CollidingObjects
    {
        get
        {
            return _collidingObjects;
        }

        set
        {
            _collidingObjects = value;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        _collidingObjects.Add(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        _collidingObjects.Remove(other.gameObject);
    }

    void Start()
    {
        _collidingObjects = new List<GameObject>();
    }
}
