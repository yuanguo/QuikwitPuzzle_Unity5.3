using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

[CustomEditor(typeof(TweenPRS))]
public class TweenPRSEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Space(6f);
        UIEditorTools.SetLabelWidth(120f);

        TweenPRS tw = target as TweenPRS;

        DrawTweenContents(tw, TweenPRS.PRSType.Pos);

        DrawTweenContents(tw, TweenPRS.PRSType.Rot);

        DrawTweenContents(tw, TweenPRS.PRSType.Scale);
    }

    void DrawTweenContents(TweenPRS _tw, TweenPRS.PRSType _type)
    {
        if (DrawToggleContent(_tw, _type))
        {
            UIEditorTools.BeginContents();
            UIEditorTools.SetLabelWidth(110f);

            GUI.changed = false;

            bool enableOffset = DrawOffsetContent(_tw, _type);
            DrawTweenInfo(_tw, _type, enableOffset);

            UIEditorTools.BeginContents();
            UIEditorTools.SetLabelWidth(100f);
            DrawCommonProperties(_tw, _type);
            UIEditorTools.EndContents();

            UIEditorTools.EndContents();
        }
    }

    bool DrawToggleContent(TweenPRS _tw, TweenPRS.PRSType _type)
    {
        GUI.changed = false;

        string name = "";
        if (_type == TweenPRS.PRSType.Pos)
            name = " TweenPosition";
        else if (_type == TweenPRS.PRSType.Rot)
            name = " TweenRotation";
        else
            name = " TweenScale";

        bool enablePRS = _tw.isEnable(_type);
        enablePRS = EditorGUILayout.ToggleLeft(name, enablePRS);

        if (GUI.changed == true)
        {
            UIEditorTools.RegisterUndo("Tween Change", _tw);
            _tw.setEnable(_type, enablePRS);
            UnityEditor.EditorUtility.SetDirty(_tw);
        }

        return enablePRS;
    }

    bool DrawOffsetContent(TweenPRS _tw, TweenPRS.PRSType _type)
    {
        GUI.changed = false;
        bool enableOffset = _tw.isEnableOffset(_type);
        enableOffset = EditorGUILayout.ToggleLeft(" Enable Offset", enableOffset);
 
        if (GUI.changed == true)
        {
            UIEditorTools.RegisterUndo("Tween Change", _tw);
            _tw.setEnableOffset(_type, enableOffset);
            UnityEditor.EditorUtility.SetDirty(_tw);
        }

        return enableOffset;
    }

    bool DrawTweenInfo(TweenPRS _tw, TweenPRS.PRSType _type, bool _offset)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        GUILayout.BeginVertical();
        GUILayout.Space(2f);

        GUI.changed = false;


        Vector3 from = _tw.GetFrom(_type);
        Vector3 to = _tw.GetTo(_type);
        Vector3 offset = _tw.GetOffset(_type);

        if (!_offset)
        {
            if (_type == TweenPRS.PRSType.Pos)
            {
                Vector2 tmpFrom = new Vector2(from.x, from.y);
                Vector2 tmpTo = new Vector2(to.x, to.y);
                tmpFrom = EditorGUILayout.Vector2Field("From", tmpFrom);
                tmpTo = EditorGUILayout.Vector2Field("To", tmpTo);

                from = new Vector3(tmpFrom.x, tmpFrom.y, 0);
                to = new Vector3(tmpTo.x, tmpTo.y, 0);
            }
            else
            {
                from = EditorGUILayout.Vector3Field("From", from);
                to = EditorGUILayout.Vector3Field("To", to);
            }
        }
        else
        {
            if (_type == TweenPRS.PRSType.Pos)
            {
                Vector2 tmp = new Vector2(offset.x, offset.y);
                tmp = EditorGUILayout.Vector2Field("Offset", tmp);
                offset = new Vector3(tmp.x, tmp.y, 0);
            }
            else
                offset = EditorGUILayout.Vector3Field("Offset", offset);
        }

        if (GUI.changed == true)
        {
            UIEditorTools.RegisterUndo("Tween Change", _tw);

            if (!_offset)
            {
                _tw.SetFrom(_type, from);
                _tw.SetTo(_type, to);
            }
            else
            {
                _tw.SetOffset(_type, offset);
            }

            UnityEditor.EditorUtility.SetDirty(_tw);
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(20f);
        GUILayout.EndVertical();
        GUILayout.Space(2f);

        return true;
    }

    protected void DrawCommonProperties(TweenPRS _tw, TweenPRS.PRSType _type)
    {
        if (UIEditorTools.DrawHeader("Tweener", "Tweener_" + _type.ToString()))
        {
            UIEditorTools.BeginContents();
            UIEditorTools.SetLabelWidth(110f);

            GUI.changed = false;

            TweenPRS.Style style = _tw.GetStyle(_type);
            style = (TweenPRS.Style)EditorGUILayout.EnumPopup("Play Style", style);

            AnimationCurve curve = _tw.GetAnimationCurve(_type);
            curve = EditorGUILayout.CurveField("Animation Curve", curve,
                GUILayout.Width(170), GUILayout.Height(62));

            bool useSpeed = _tw.isSpeed(_type);
            if (style == TweenPRS.Style.Once && _type == TweenPRS.PRSType.Pos)
                useSpeed = EditorGUILayout.ToggleLeft("Use Speed", useSpeed);
            else
                useSpeed = false;

            GUILayout.BeginHorizontal();
            float dur = _tw.GetDuration(_type);

            string str = style == TweenPRS.Style.Once ? (useSpeed ? "Speed" : "Duration") : "Duration";
            dur = EditorGUILayout.FloatField(str, dur, GUILayout.Width(170f));
            str = style == TweenPRS.Style.Once ? (useSpeed ? "unit/s" : "seconds") : "seconds";
            GUILayout.Label(str);
            GUILayout.EndHorizontal();


            if (GUI.changed == true)
            {
                UIEditorTools.RegisterUndo("Tween Change", _tw);

                _tw.SetAnimationCurve(_type, curve);
                _tw.SetDuration(_type, dur);
                _tw.SetStyle(_type, style);
                _tw.setEnableSpeed(_type, useSpeed);

                UnityEditor.EditorUtility.SetDirty(_tw);
            }

            UIEditorTools.EndContents();
        }

        UIEditorTools.SetLabelWidth(80f);
        UIEditorTools.DrawEvents("On Finished", "On Finished_" + _type.ToString(), _tw, _tw.GetOnFinished(_type));
    }

}
