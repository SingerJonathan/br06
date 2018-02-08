using UnityEngine;

public class KingOfTheHillCollisionController : MonoBehaviour
{
    public enum TriggerMaterial
    {
        Initial, Red, Blue
    }

    private bool _redColliding;
    private bool _blueColliding;

    [SerializeField] private Material _redMaterial;
    [SerializeField] private Material _blueMaterial;
    [SerializeField] private Material _initialMaterial;

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

    public void SetMaterial(TriggerMaterial material)
    {
        Material selectedMaterial;
        switch (material)
        {
            case TriggerMaterial.Red:
                selectedMaterial = _redMaterial;
                break;
            case TriggerMaterial.Blue:
                selectedMaterial = _blueMaterial;
                break;
            default:
                selectedMaterial = _initialMaterial;
                break;
        }
        for (int index = 0; index < transform.parent.childCount; index++)
            transform.parent.GetChild(index).GetComponent<MeshRenderer>().material = selectedMaterial;
    }

    public void DetermineMaterial()
    {
        if (_redColliding && _blueColliding || !_redColliding && !_blueColliding)
            SetMaterial(TriggerMaterial.Initial);
        else if (_redColliding && !_blueColliding)
            SetMaterial(TriggerMaterial.Red);
        else if (!_redColliding && _blueColliding)
            SetMaterial(TriggerMaterial.Blue);
    }

    void OnTriggerEnter(Collider other)
    {
        _redColliding |= other.gameObject.name == "Red Character";
        _blueColliding |= other.gameObject.name == "Blue Character";
        DetermineMaterial();
    }

    void OnTriggerExit(Collider other)
    {
        _redColliding &= other.gameObject.name != "Red Character";
        _blueColliding &= other.gameObject.name != "Blue Character";
        DetermineMaterial();
    }
}
