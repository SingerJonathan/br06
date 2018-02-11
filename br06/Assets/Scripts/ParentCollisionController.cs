using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentCollisionController : MonoBehaviour
{
	public int ChildCount;
    private RandomEnvironmentController _randomEnvironmentController;
	private int _childrenReady;
	private bool _done;

    public int ChildrenReady
    {
        get
        {
            return _childrenReady;
        }

        set
        {
            _childrenReady = value;
        }
    }

    public RandomEnvironmentController RandomEnvironmentController
    {
        get
        {
            return _randomEnvironmentController;
        }

        set
        {
            _randomEnvironmentController = value;
        }
    }

	void Start()
	{
		_randomEnvironmentController = FindObjectOfType<RandomEnvironmentController>();
	}

    void Update()
	{
		if (!_done && _childrenReady == ChildCount)
		{
			_done = true;
			_randomEnvironmentController.RotateWithoutCollisions();
		}
	}
}
