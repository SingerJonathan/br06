using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RandomEnvironmentController : MonoBehaviour
{
    public Toggle RandomEnvironmentToggle;

    private static int SmallObjects = 4;
    private static int LargeObjects = 1;
    private List<int> _usedObjects;
    private List<GameObject> _childGameObjects;
    private int _rotatingObjectsCount;

    private GameObject _environmentObject;
    private int _collidersCount;

    public void SpawnRandomEnvironmentObjects(GameLoopController.GameMode gameMode)
    {
        _usedObjects = new List<int>();
        _childGameObjects = new List<GameObject>();
        _rotatingObjectsCount = 0;
        DeleteRandomEnvironmentObjects();
        if (RandomEnvironmentToggle.isOn)
        {
            int index = gameMode == GameLoopController.GameMode.KingOfTheHill ? 0 : gameMode == GameLoopController.GameMode.Standard ? 1 : 2;
            while (index < transform.childCount)
            {
                if (gameMode != GameLoopController.GameMode.KingOfTheHill || (gameMode == GameLoopController.GameMode.KingOfTheHill && index != 1))
                {
                    int objectNumber = index == 0 || index == 1 ? Random.Range(0, SmallObjects - 1) : Random.Range(0, SmallObjects);
                    while (_usedObjects.Contains(objectNumber) && _usedObjects.Count < SmallObjects)
                        objectNumber = Random.Range(0, SmallObjects);
                    GameObject environmentObject = (GameObject)Instantiate(Resources.Load("Random Environment/Small" + objectNumber));
                    _usedObjects.Add(objectNumber);
                    for (int childIndex = 0; childIndex < environmentObject.transform.childCount; childIndex++)
                        _childGameObjects.Add(environmentObject.transform.GetChild(childIndex).gameObject);
                    environmentObject.transform.SetParent(transform.GetChild(index));
                    environmentObject.transform.localPosition = Vector3.zero;
                    environmentObject.transform.localEulerAngles = new Vector3(0, Random.Range(0, 359), 0);
                    float scale = Random.Range(0.8f, 1.2f);
                    environmentObject.transform.localScale = index == 0 ? new Vector3(0.6f, 1, 0.6f) : new Vector3(scale, scale, scale);
                    /*if (_rotatingObjectsCount < 2 && Random.Range(1, 101) > 50)
                    {
                        RandomEnvironmentObject environmentObjectComponent = environmentObject.GetComponent<RandomEnvironmentObject>();
                        environmentObjectComponent.Rotate = true;
                        environmentObjectComponent.RotationSpeed = Random.Range(5, 11);
                        _rotatingObjectsCount++;
                    }*/
                }
                index++;
            }
            if (gameMode == GameLoopController.GameMode.Standard)
            {
                _environmentObject = (GameObject)Instantiate(Resources.Load("Random Environment/Large" + Random.Range(0, LargeObjects)));
                _environmentObject.transform.SetParent(transform.GetChild(1));
                _environmentObject.transform.localPosition = Vector3.zero;
            }
        }
    }

    public void RotateWithoutCollisions()
    {
        StartCoroutine(RotateToNoCollisionsPosition());
    }
    
    private IEnumerator RotateToNoCollisionsPosition()
    {
        _collidersCount = 1;
        _environmentObject.transform.localEulerAngles = new Vector3(0, Random.Range(0, 359), 0);
        while (_collidersCount > 0)
        {
            Vector3 angles = _environmentObject.transform.localEulerAngles;
            angles.y += 1;
            _environmentObject.transform.localEulerAngles = angles;
            yield return null;
            _collidersCount = 0;
            for (int childIndex = 0; childIndex < _environmentObject.transform.childCount; childIndex++)
                for (int childObjectIndex = 0; childObjectIndex < _childGameObjects.Count; childObjectIndex++)
                    if (_environmentObject.transform.GetChild(childIndex).GetComponent<CollisionController>().CollidingObjects.Contains(_childGameObjects[childObjectIndex]))
                        _collidersCount++;
        }
    }

    public void DeleteRandomEnvironmentObjects()
    {
        for (int index = 0; index < transform.childCount; index++)
        {
            for (int subIndex = 0; subIndex < transform.GetChild(index).childCount; subIndex++)
                Destroy(transform.GetChild(index).GetChild(subIndex).gameObject);
        }
    }
}
