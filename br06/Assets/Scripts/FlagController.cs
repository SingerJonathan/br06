using UnityEngine;

public class FlagController : MonoBehaviour
{
    public GameObject RedCharacterSpine;
    public GameObject BlueCharacterSpine;

    [SerializeField] private Material _redMaterial;
    [SerializeField] private Material _blueMaterial;
    [SerializeField] private Material _initialMaterial;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Red Character")
        {
            transform.SetParent(RedCharacterSpine.transform);
            transform.localPosition = new Vector3(0, 0.02f, -0.007f);
            transform.localEulerAngles = new Vector3(0, -45, 0);
            GetComponent<CapsuleCollider>().enabled = false;
            transform.GetChild(0).GetComponent<MeshRenderer>().material = _redMaterial;
        }
        else if (other.gameObject.name == "Blue Character")
        {
            transform.SetParent(BlueCharacterSpine.transform);
            transform.localPosition = new Vector3(0, 0.02f, -0.007f);
            transform.localEulerAngles = new Vector3(0, -45, 0);
            GetComponent<CapsuleCollider>().enabled = false;
            transform.GetChild(0).GetComponent<MeshRenderer>().material = _blueMaterial;
        }
    }

    public void ResetMaterial()
    {
        transform.GetChild(0).GetComponent<MeshRenderer>().material = _initialMaterial;
    }
}
