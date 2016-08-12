using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SliderBinder : MonoBehaviour
{
	public BindingPropertyInfo m_target = new BindingPropertyInfo();
	public Slider m_slider;

	void Awake()
	{
		if (m_slider == null)
			m_slider = GetComponent<Slider>();

		LinkEvent();

		m_target.UpdateInfo();
	}

	void OnEnable()
	{
		UpdateBinding();
	}

	public void OnValueChanged(float value)
	{
		m_target.Set(value);
	}

	public void Setup(Object targetObj, string member, Slider slider)
	{
		m_slider = slider;

		LinkEvent();
		Setup(targetObj, member);
	}

	public void Setup(Object targetObj, string member)
	{
		m_target.SetInfo(targetObj, member);
		m_target.UpdateInfo();
		UpdateBinding();
	}

	void LinkEvent()
	{
		if (m_slider == null)
			return;

		m_slider.onValueChanged.AddListener(OnValueChanged);
	}

	void UpdateBinding()
	{
		object value = m_target.Get();
		if (m_slider != null && value != null)
			m_slider.value = (float)value;
	}
}
