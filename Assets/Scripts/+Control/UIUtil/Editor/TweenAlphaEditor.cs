using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TweenAlpha))]
public class TweenAlphaEditor : TweenerEditor 
{
    public override void OnInspectorGUI()
    {
        GUILayout.Space(6f);
        UIEditorTools.SetLabelWidth(120f);

        TweenAlpha tw = target as TweenAlpha;

        GUI.changed = false;

        bool enableOffset = EditorGUILayout.ToggleLeft("Enable Offset ", tw.EnableOffset);
        if (GUI.changed)
        {
            UIEditorTools.RegisterUndo("Tween Change", tw);
            tw.EnableOffset = enableOffset;
            UnityEditor.EditorUtility.SetDirty(tw);
        }

        GUI.changed = false;
        float from = 0f, to = 0f, offset = 0f;

        GUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        GUILayout.BeginVertical();
        GUILayout.Space(2f);

        if (!enableOffset)
        {
            from = EditorGUILayout.Slider("From", tw.From, 0f, 1f);
            to = EditorGUILayout.Slider("To", tw.To, 0f, 1f);
        }
        else
            offset = EditorGUILayout.Slider("Offset", tw.Offset, 0f, 1f);

        GUILayout.EndHorizontal();
        GUILayout.Space(20f);
        GUILayout.EndVertical();
        GUILayout.Space(2f);

        if (from < 0) from = 0f;
        if (to < 0) to = 0f;
        if (offset < 0) offset = 0f;

        if (GUI.changed)
        {
            UIEditorTools.RegisterUndo("Tween Change", tw);
            if (!enableOffset)
            {
                tw.From = from;
                tw.To = to;
            }
            else
                tw.Offset = offset;

            UnityEditor.EditorUtility.SetDirty(tw);
        }

        DrawCommonProperties();
    }
}
