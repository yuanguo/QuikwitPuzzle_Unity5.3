using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Tweener), true)]
public class TweenerEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        GUILayout.Space(6f);
        UIEditorTools.SetLabelWidth(110f);
          
 	    base.OnInspectorGUI();

        DrawCommonProperties();

    }

    protected void DrawCommonProperties()
    {
        Tweener tw = target as Tweener;

        if (UIEditorTools.DrawHeader("Tweener"))
        {
            UIEditorTools.BeginContents();
            UIEditorTools.SetLabelWidth(110f);

            GUI.changed = false;

            Tweener.Style style = (Tweener.Style)EditorGUILayout.EnumPopup("Play Style", tw.m_Style);
            AnimationCurve curve = EditorGUILayout.CurveField("Animation Curve", tw.m_AnimationCurve,
                GUILayout.Width(170), GUILayout.Height(62));

            GUILayout.BeginHorizontal();
            float dur = EditorGUILayout.FloatField("Duration", tw.Duration, GUILayout.Width(170f));
            GUILayout.Label("seconds");
            GUILayout.EndHorizontal();


            if (GUI.changed == true)
            {
                UIEditorTools.RegisterUndo("Tween Change", tw);

                tw.m_AnimationCurve = curve;
                tw.m_Style = style;
                tw.Duration = dur;

                UnityEditor.EditorUtility.SetDirty(tw);
            }

            UIEditorTools.EndContents();
        }

        UIEditorTools.SetLabelWidth(80f);
        UIEditorTools.DrawEvents("On Finished", tw, tw.onFinished);
    }
}
