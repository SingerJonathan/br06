using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prison : MonoBehaviour
{
	[SerializeField] private float _duration = 5;
	private float _lifetime;
	private bool _finished;

    void Update()
	{
		_lifetime += Time.deltaTime;
		if (_lifetime >= _duration && !_finished)
		{
			_finished = true;
			GetComponent<Animator>().SetTrigger("sink");
			transform.GetChild(0).GetComponent<AudioSource>().Play();
			Destroy(transform.parent.gameObject, 1);
		}
	}

    public float Duration
    {
        get
        {
            return _duration;
        }

        set
        {
            _duration = value;
        }
    }
}
