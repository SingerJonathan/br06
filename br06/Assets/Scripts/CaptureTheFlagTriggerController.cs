using UnityEngine;

public class CaptureTheFlagTriggerController : MonoBehaviour
{
    public GameLoopController GameLoopController;

    private string _characterName;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == _characterName && other.transform.Find("mixamorig:Hips/mixamorig:Spine/Flagpole") != null)
        {
            Transform flag = other.transform.Find("mixamorig:Hips/mixamorig:Spine/Flagpole");
            flag.SetParent(GameLoopController.CaptureTheFlagObjects.transform);
            flag.SetPositionAndRotation(GameLoopController.InitialFlagPosition, GameLoopController.InitialFlagRotation);
            flag.GetComponent<FlagController>().ResetMaterial();
            flag.GetComponent<CapsuleCollider>().enabled = true;
            if (_characterName == "Red Character")
                GameLoopController.RedScore++;
            else
                GameLoopController.BlueScore++;
        }
    }

    void Start()
    {
        _characterName = name.Contains("Red") ? "Red Character" : "Blue Character";
    }
}
