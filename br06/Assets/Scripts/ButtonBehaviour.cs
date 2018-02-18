using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour, ISelectHandler , IPointerEnterHandler
{
	private Button _button;
	private Toggle _toggle;
	private Dropdown _dropdown;
	[SerializeField] private AudioSource SelectSound;
	[SerializeField] private AudioSource ClickSound;

    void Start()
    {
		if (GetComponent<Button>())
		{
        	_button = GetComponent<Button>();
        	_button.onClick.AddListener(OnClick);
		}
		else if (GetComponent<Toggle>())
		{
			_toggle = GetComponent<Toggle>();
			_toggle.onValueChanged.AddListener((x) => Invoke("OnClick", 0f));
		}
		else if (GetComponent<Dropdown>())
		{
			_dropdown = GetComponent<Dropdown>();
			_dropdown.onValueChanged.AddListener((x) => Invoke("OnClick", 0f));
		}
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
		if (SelectSound == null)
			SelectSound = GameObject.FindGameObjectWithTag("Select Sound").GetComponent<AudioSource>();
		SelectSound.Play();
    }

    public void OnSelect(BaseEventData eventData)
    {
		if (SelectSound == null)
			SelectSound = GameObject.FindGameObjectWithTag("Select Sound").GetComponent<AudioSource>();
		SelectSound.Play();
    }

	public void OnClick()
	{
		if (ClickSound == null)
			ClickSound = GameObject.FindGameObjectWithTag("Click Sound").GetComponent<AudioSource>();
		ClickSound.Play();
	}
}
