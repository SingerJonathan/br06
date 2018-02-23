using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectActiveToggle : MonoBehaviour
{
	public GameObject GameObject;

	public void ToggleGameObjectActive()
	{
		GameObject.SetActive(!GameObject.activeInHierarchy);
	}
}
