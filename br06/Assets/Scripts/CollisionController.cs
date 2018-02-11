using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _collidingObjects;

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

    void OnCollisionEnter(Collision other)
    {
        if (!_collidingObjects.Contains(other.gameObject))
            _collidingObjects.Add(other.gameObject);
        //Debug.Log(gameObject.name + " + " + other.gameObject.name + ": collision enter!");
    }

    void OnCollisionExit(Collision other)
    {
        if (_collidingObjects.Contains(other.gameObject))
            _collidingObjects.Remove(other.gameObject);
        //Debug.Log(gameObject.name + " + " + other.gameObject.name + ": collision exit!");
    }

    void Start()
    {
        _collidingObjects = new List<GameObject>();
        transform.parent.GetComponent<ParentCollisionController>().ChildrenReady++;
    }
}
