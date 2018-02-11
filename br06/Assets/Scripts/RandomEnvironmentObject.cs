using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEnvironmentObject : MonoBehaviour
{
	private bool _rotate;
	private int _rotationSpeed;

    public bool Rotate
    {
        get
        {
            return _rotate;
        }

        set
        {
            _rotate = value;
        }
    }

    public int RotationSpeed
    {
        get
        {
            return _rotationSpeed;
        }

        set
        {
            _rotationSpeed = value;
        }
    }

    void Update()
	{
		if (_rotate)
		{
            Vector3 angles = transform.localEulerAngles;
            angles.y += _rotationSpeed * Time.deltaTime;
            transform.localEulerAngles = angles;
		}
	}
}
