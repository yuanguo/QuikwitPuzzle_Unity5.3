using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Reflection;

public class PropertyBinder : UIBehaviour
{
	[System.Serializable]
	public class PropInfo
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

	public PropInfo m_sourceInfo = new PropInfo();
	public PropInfo m_controlInfo = new PropInfo();

    protected override void Awake()
	{
		m_sourceInfo.UpdateInfo();
		m_controlInfo.UpdateInfo();
	}

    protected override void OnEnable()
	{
		m_controlInfo.Set(m_sourceInfo.Get());
	}

	public void OnValueChanged()
	{
		m_sourceInfo.Set(m_controlInfo.Get());
	}

	public void Setup(Object srcObj, string srcMember, Object ctrlObj, string ctrlMember)
	{
		m_sourceInfo.SetInfo(srcObj, srcMember);
		m_sourceInfo.UpdateInfo();

		m_controlInfo.SetInfo(ctrlObj, ctrlMember);
		m_controlInfo.UpdateInfo();

		FieldInfo eventField = ctrlObj.GetType().GetField("onValueChanged");
		MethodInfo eventMethod = eventField.FieldType.GetMethod("AddListener");
		UnityEngine.Events.UnityAction action = OnValueChanged;
		eventMethod.Invoke(ctrlObj, new object[] { action });
	}
}
