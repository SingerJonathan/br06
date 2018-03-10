using UnityEngine;

public class TrapTriggerController : MonoBehaviour
{
    [SerializeField] private string _enemyColour;
	[SerializeField] private int _damage;
	[SerializeField] private AudioSource _triggerSound;
	[SerializeField] private ParticleSystem _triggerParticleSystem;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains(_enemyColour))
		{
			other.GetComponent<CharacterStatsController>().HitPoints -= _damage;
			if (_triggerSound)
				_triggerSound.Play();
			if (_triggerParticleSystem)
			{
				_triggerParticleSystem.Play();
				Destroy(transform.parent.gameObject, _triggerParticleSystem.main.duration);
			}
			else
				Destroy(transform.parent.gameObject);
		}
    }
}
