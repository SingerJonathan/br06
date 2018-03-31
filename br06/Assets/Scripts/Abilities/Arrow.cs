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
    private bool _createPrison;
    private bool _createElectricPrison;
    private float _prisonDuration;
    private int _prisonFieldDamagePerSecond;
    private Vector3 _hitPosition;
	private bool _hit;
	private float _lifetime;
    private bool _axe;
	
	void Update()
	{
		if (!_hit)
			transform.parent.position += transform.parent.forward * Time.deltaTime * _speed;
		_lifetime += Time.deltaTime;
		if (_lifetime > 3)
		    Destroy(transform.parent.gameObject, 0.2f);
        if (_lifetime > 1 && _axe)
            Destroy(transform.parent.gameObject, 0.2f);
	}

    void OnTriggerEnter(Collider other)
    {
		if ((!other.gameObject.name.Contains("Character") || other.gameObject.name.Contains(_enemyColour)) && !_hit)
		{
			_hit = true;
            _hitPosition = transform.parent.position;
			if (other.gameObject.name.Contains(_enemyColour))
				other.GetComponent<CharacterStatsController>().HitPoints -= _damage;
			_hitParticleSystem.Play();
			GameObject.FindGameObjectWithTag("Arrow Sound").GetComponent<AudioSource>().Play();
            if (_createPrison)
            {
                GameObject dummyParent = new GameObject("Prison");
                string friendlyColour = _enemyColour.Contains("Red") ? "Blue" : "Red";
                GameObject prison = (GameObject) Instantiate(Resources.Load(friendlyColour + " Prison"));
                prison.transform.SetParent(dummyParent.transform);
                Vector3 newPosition = _hitPosition;
                newPosition.y = 0;
                dummyParent.transform.position = newPosition;
                prison.transform.localPosition = Vector3.zero;
                if (_prisonDuration != 0)
                    prison.GetComponent<Prison>().Duration = _prisonDuration;
            }
            else if (_createElectricPrison)
            {
                GameObject dummyParent = new GameObject("Prison");
                string friendlyColour = _enemyColour.Contains("Red") ? "Blue" : "Red";
                GameObject prison = (GameObject) Instantiate(Resources.Load(friendlyColour + " Electric Prison"));
                prison.transform.SetParent(dummyParent.transform);
                Vector3 newPosition = _hitPosition;
                newPosition.y = 0;
                dummyParent.transform.position = newPosition;
                prison.transform.localPosition = Vector3.zero;
                if (_prisonDuration != 0)
                    prison.GetComponent<Prison>().Duration = _prisonDuration;
                if (_prisonFieldDamagePerSecond != 0)
                {
                    prison.GetComponentInChildren<DamageFieldTriggerController>().DamagePerSecond = _prisonFieldDamagePerSecond;
                    prison.GetComponentInChildren<DamageFieldTriggerController>().DamageOverTimeDuration = (int)_prisonDuration;
                }
            }
		    Destroy(transform.parent.gameObject, 0.2f);
		}
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

    public bool CreatePrison
    {
        get
        {
            return _createPrison;
        }

        set
        {
            _createPrison = value;
        }
    }

    public float PrisonDuration
    {
        get
        {
            return _prisonDuration;
        }

        set
        {
            _prisonDuration = value;
        }
    }

    public bool CreateElectricPrison
    {
        get
        {
            return _createElectricPrison;
        }

        set
        {
            _createElectricPrison = value;
        }
    }

    public int PrisonFieldDamagePerSecond
    {
        get
        {
            return _prisonFieldDamagePerSecond;
        }

        set
        {
            _prisonFieldDamagePerSecond = value;
        }
    }

    public bool Axe
    {
        get
        {
            return _axe;
        }

        set
        {
            _axe = value;
        }
    }
}
