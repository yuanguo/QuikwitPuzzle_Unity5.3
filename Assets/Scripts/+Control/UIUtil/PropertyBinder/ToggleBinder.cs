using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleBinder : MonoBehaviour
{
	public BindingPropertyInfo m_target = new BindingPropertyInfo();
	public Toggle m_toggle;

	void Awake()
	{
		if (m_toggle == null)
			m_toggle = GetComponent<Toggle>();

		LinkEvent();

		m_target.UpdateInfo();
	}

	void OnEnable()
	{
		UpdateBinding();
	}

	public void OnValueChanged(bool value)
	{
		m_target.Set(value);
	}

	public void Setup(Object targetObj, string member, Toggle toggle)
	{
		m_toggle = toggle;

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
		if (m_toggle == null)
			return;

		m_toggle.onValueChanged.AddListener(OnValueChanged);
	}

	void UpdateBinding()
	{
		object value = m_target.Get();
		if (m_toggle != null && value != null)
			m_toggle.isOn = (bool)value;
	}
}
