using UnityEngine;

public class KingOfTheHillCollisionController : MonoBehaviour
{
    public bool _redColliding;
    public bool _blueColliding;

    public bool RedColliding
    {
        get
        {
            return _redColliding;
        }

        set
        {
            _redColliding = value;
        }
    }

    public bool BlueColliding
    {
        get
        {
            return _blueColliding;
        }

        set
        {
            _blueColliding = value;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        _redColliding |= other.gameObject.name == "Red Character";
        _blueColliding |= other.gameObject.name == "Blue Character";
    }

    void OnTriggerExit(Collider other)
    {
        _redColliding &= other.gameObject.name != "Red Character";
        _blueColliding &= other.gameObject.name != "Blue Character";
    }
}
