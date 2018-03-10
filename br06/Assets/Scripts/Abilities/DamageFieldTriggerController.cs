using UnityEngine;

public class DamageFieldTriggerController : MonoBehaviour
{
    [SerializeField] private string _enemyColour;
    [SerializeField] private string _damageOverTimeTag = "StaticField";
    [SerializeField] private string _damageAudioSourceTag = "Electric Burst Sound";
	private int _damagePerSecond;
	private int _damageOverTimeDuration;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains(_enemyColour))
		{
			other.GetComponent<CharacterStatsController>().AddDamageOverTime(_damagePerSecond, _damageOverTimeDuration, _damageOverTimeTag, _damageAudioSourceTag);
		}
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains(_enemyColour))
		{
			other.GetComponent<CharacterStatsController>().RemoveDamageOverTimeWithTag(_damageOverTimeTag);
		}
    }

    public int DamagePerSecond
    {
        get
        {
            return _damagePerSecond;
        }

        set
        {
            _damagePerSecond = value;
        }
    }

    public int DamageOverTimeDuration
    {
        get
        {
            return _damageOverTimeDuration;
        }

        set
        {
            _damageOverTimeDuration = value;
        }
    }
}
