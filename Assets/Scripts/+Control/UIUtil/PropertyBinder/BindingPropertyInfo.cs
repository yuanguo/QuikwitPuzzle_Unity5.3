using UnityEngine;
using System.Reflection;

[System.Serializable]
public class BindingPropertyInfo
{
	public Object m_object = null;
	public string m_member = "";

	[HideInInspector]
	public System.Type type = null;

	private PropertyInfo pi = null;
	private FieldInfo fi = null;

	public void SetInfo(Object obj, string member)
	{
		m_object = obj;
		m_member = member;
	}

	public void UpdateInfo()
	{
		type = null;
		if (m_object == null)
			return;

		pi = m_object.GetType().GetProperty(m_member);
		fi = m_object.GetType().GetField(m_member);

		if (pi != null)
			type = pi.PropertyType;
		else if (fi != null)
			type = fi.FieldType;
	}

	public object Get()
	{
		if (m_object == null)
			return null;

		if (pi != null)
			return pi.GetValue(m_object, null);
		else if (fi != null)
			return fi.GetValue(m_object);

		return null;
	}

	public void Set(object value)
	{
		if (m_object == null)
			return;

		if (pi != null)
			pi.SetValue(m_object, value, null);
		else if (fi != null)
			fi.SetValue(m_object, value);
	}
}