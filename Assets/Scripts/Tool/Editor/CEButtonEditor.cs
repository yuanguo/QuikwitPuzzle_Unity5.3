using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
using System.Collections;

[CustomEditor(typeof(CEButton), true)]
[CanEditMultipleObjects]
public class CEButtonEditor : ButtonEditor
{
	SerializedProperty m_textTargetProperty;
	SerializedProperty m_colorsForTextProperty;

	protected override void OnEnable()
	{
		base.OnEnable();

		m_textTargetProperty = serializedObject.FindProperty("m_textTarget");
		m_colorsForTextProperty = serializedObject.FindProperty("m_colorsForText");

		Debug.Log("OnEnable");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorGUILayout.Space();
		serializedObject.Update();
		EditorGUILayout.PropertyField(m_textTargetProperty);
		EditorGUILayout.PropertyField(m_colorsForTextProperty);
		serializedObject.ApplyModifiedProperties();

		//Debug.Log("OnInspectorGUI");
	}
}
