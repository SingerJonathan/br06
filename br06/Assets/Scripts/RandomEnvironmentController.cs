using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomEnvironmentController : MonoBehaviour
{
    public Toggle RandomEnvironmentToggle;

    private static int SmallObjects = 4;
    private List<int> usedObjects;

    public void SpawnRandomEnvironmentObjects(bool spawnMiddleObject = false)
    {
        usedObjects = new List<int>();
        if (RandomEnvironmentToggle.isOn)
        {
            DeleteRandomEnvironmentObjects();
            int index = !spawnMiddleObject ? 1 : 0;
            while (index < transform.childCount)
            {
                int objectNumber = Random.Range(0, SmallObjects);
                while (usedObjects.Contains(objectNumber) && usedObjects.Count < SmallObjects)
                    objectNumber = Random.Range(0, SmallObjects);
                GameObject environmentObject = (GameObject)Instantiate(Resources.Load("Random Environment/Small" + objectNumber));
                usedObjects.Add(objectNumber);
                environmentObject.transform.SetParent(transform.GetChild(index));
                environmentObject.transform.localPosition = Vector3.zero;
                environmentObject.transform.localEulerAngles = new Vector3(0, Random.Range(0, 359), 0);
                float scaleRandom = index == 0 ? Random.Range(2f, 2.5f) : Random.Range(0.75f, 1.25f);
                environmentObject.transform.localScale = new Vector3(scaleRandom, scaleRandom, scaleRandom);
                index++;
            }
        }
    }

    public void DeleteRandomEnvironmentObjects()
    {
        for (int index = 0; index < transform.childCount; index++)
        {
            if (transform.GetChild(index).childCount > 0)
                Destroy(transform.GetChild(index).GetChild(0).gameObject);
        }
    }
}
