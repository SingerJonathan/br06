using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Material _redMaterial;
    [SerializeField] private Material _blueMaterial;
    [SerializeField] private ParticleSystem _redParticleSystem;
    [SerializeField] private ParticleSystem _blueParticleSystem;
	private ParticleSystem _hitParticleSystem;
	private string _enemyColour;
	private int _damage;
	private float _speed = 100f;
	private bool _hit;
	private float _lifetime;
	
	void Update()
	{
		if (!_hit)
			transform.parent.position += transform.parent.forward * Time.deltaTime * _speed;
		_lifetime += Time.deltaTime;
		if (_lifetime > 3)
			DestroyArrow();
	}

    void OnTriggerEnter(Collider other)
    {
		if (!other.gameObject.name.Contains("Character") || other.gameObject.name.Contains(_enemyColour))
		{
			_hit = true;
			if (other.gameObject.name.Contains(_enemyColour))
				other.GetComponent<CharacterStatsController>().HitPoints -= _damage;
			_hitParticleSystem.Play();
			GameObject.FindGameObjectWithTag("Arrow Sound").GetComponent<AudioSource>().Play();
			Invoke("DestroyArrow", 0.2f);
		}
    }

	private void DestroyArrow()
	{
		Destroy(transform.parent.gameObject);
	}

    public Material RedMaterial
    {
        get
        {
            return _redMaterial;
        }
    }

    public Material BlueMaterial
    {
        get
        {
            return _blueMaterial;
        }
    }

    public string EnemyColour
    {
        get
        {
            return _enemyColour;
        }

        set
        {
            _enemyColour = value;
        }
    }

    public int Damage
    {
        get
        {
            return _damage;
        }

        set
        {
            _damage = value;
        }
    }

    public float Speed
    {
        get
        {
            return _speed;
        }

        set
        {
            _speed = value;
        }
    }

    public ParticleSystem RedParticleSystem
    {
        get
        {
            return _redParticleSystem;
        }
    }

    public ParticleSystem BlueParticleSystem
    {
        get
        {
            return _blueParticleSystem;
        }
    }

    public ParticleSystem HitParticleSystem
    {
        get
        {
            return _hitParticleSystem;
        }

        set
        {
            _hitParticleSystem = value;
        }
    }
}
