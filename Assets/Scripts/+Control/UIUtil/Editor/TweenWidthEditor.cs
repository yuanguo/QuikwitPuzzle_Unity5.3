using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TweenWidth))]
public class TweenWidthEditor : TweenerEditor 
{
    public override void OnInspectorGUI()
    {
        GUILayout.Space(6f);
        UIEditorTools.SetLabelWidth(120f);

        TweenWidth tw = target as TweenWidth;

        GUI.changed = false;
        bool enableOffset = EditorGUILayout.ToggleLeft("Enable Offset ", tw.EnableOffset);
        if (GUI.changed)
        {
            UIEditorTools.RegisterUndo("Tween Change", tw);
            tw.EnableOffset = enableOffset;
            UnityEditor.EditorUtility.SetDirty(tw);
        }

        GUI.changed = false;
        int from = 0, to = 0, offset = 0;

        GUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        GUILayout.BeginVertical();
        GUILayout.Space(2f);

        if (!enableOffset)
        {
            from = EditorGUILayout.IntField("From", (int)tw.From);
            to = EditorGUILayout.IntField("To", (int)tw.To);
        }
        else
            offset = EditorGUILayout.IntField("Offset", (int)tw.Offset);

        GUILayout.EndHorizontal();
        GUILayout.Space(20f);
        GUILayout.EndVertical();
        GUILayout.Space(2f);

        if (from < 0) from = 0;
        if (to < 0) to = 0;
        if (offset < 0) offset = 0;

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
