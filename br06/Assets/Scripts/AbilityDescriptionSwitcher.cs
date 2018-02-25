using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityDescriptionSwitcher : MonoBehaviour, ISelectHandler , IPointerEnterHandler
{
	public GameObject DescriptionObject;
	public string Colour;

    public void OnPointerEnter(PointerEventData eventData)
    {
		foreach (GameObject descriptionObject in GameObject.FindGameObjectsWithTag(Colour + " Description"))
			descriptionObject.SetActive(false);
			DescriptionObject.SetActive(true);
    }

    public void OnSelect(BaseEventData eventData)
    {
		foreach (GameObject descriptionObject in GameObject.FindGameObjectsWithTag(Colour + " Description"))
			descriptionObject.SetActive(false);
			DescriptionObject.SetActive(true);
    }
}
