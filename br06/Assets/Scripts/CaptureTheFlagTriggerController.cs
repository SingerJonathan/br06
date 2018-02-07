using UnityEngine;

public class CaptureTheFlagTriggerController : MonoBehaviour
{
    public GameLoopController GameLoopController;

    private string _characterName;
    private float _respawnCountdown;
    private Transform _flag;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == _characterName && other.transform.Find("mixamorig:Hips/mixamorig:Spine/Flagpole") != null)
        {
            _flag = other.transform.Find("mixamorig:Hips/mixamorig:Spine/Flagpole");
            _flag.SetParent(GameLoopController.CaptureTheFlagObjects.transform);
            _respawnCountdown = 3f;
            GameLoopController.FlagRespawnText.gameObject.SetActive(true);
            _flag.gameObject.SetActive(false);
            _flag.GetComponent<FlagController>().ResetMaterial();
            _flag.GetComponent<CapsuleCollider>().enabled = true;
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

    void Update()
    {
        if (_respawnCountdown > 0.0f && !GameLoopController.MainMenuCanvasGameObject.activeInHierarchy)
        {
            _respawnCountdown -= Time.deltaTime;
            GameLoopController.FlagRespawnText.text = ""+(int)(_respawnCountdown+1);
            if (_respawnCountdown <= 0.0f)
            {
                _flag.SetPositionAndRotation(GameLoopController.InitialFlagPosition, GameLoopController.InitialFlagRotation);
                _flag.gameObject.SetActive(true);
                GameLoopController.FlagRespawnText.gameObject.SetActive(false);
            }
        }
    }
}
