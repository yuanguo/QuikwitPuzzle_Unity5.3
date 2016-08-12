using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class UIEditorTools
{
    static public void SetLabelWidth(float _width)
    {
        EditorGUIUtility.labelWidth = _width;
    }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string _text) { return DrawHeader(_text, _text, false); }
    static public bool DrawNormalHeader(string _text) { return DrawNormalHeader(_text, _text, false); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string _text, string _key) { return DrawHeader(_text, _key, false); }
    static public bool DrawNormalHeader(string _text, string _key) { return DrawNormalHeader(_text, _key, false); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string _text, bool _forceOn) { return DrawHeader(_text, _text, _forceOn); }
    static public bool DrawNormalHeader(string _text, bool _forceOn) { return DrawNormalHeader(_text, _text, _forceOn); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string _text, string _key, bool _forceOn)
    {
        bool state = EditorPrefs.GetBool(_key, true);

        GUILayout.Space(3f);
        if (!_forceOn && !state) 
            GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);

        GUILayout.BeginHorizontal();
        GUILayout.Space(3f);

        GUI.changed = false;

        _text = "<b><size=11>" + _text + "</size></b>";

        if (state)
            _text = "\u25BC " + _text;
        else
            _text = "\u25BA " + _text;

        if (!state)
            _text = "<color=#B4B4B4>" + _text + "</color>";
        else
            _text = "<color=#4C7EFF>" + _text + "</color>";


        if (!GUILayout.Toggle(true, _text, "dragtab", GUILayout.MinWidth(20f))) 
            state = !state;

        if (GUI.changed) 
            EditorPrefs.SetBool(_key, state);

        GUILayout.Space(2f);
        GUILayout.EndHorizontal();

        GUI.backgroundColor = Color.white;

        if (!_forceOn && !state) 
            GUILayout.Space(3f);

        return state;
    }

    static public bool DrawNormalHeader(string _text, string _key, bool _forceOn)
    {
        bool state = EditorPrefs.GetBool(_key, true);

        GUILayout.Space(3f);
        if (!_forceOn && !state)
            GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);

        GUILayout.BeginHorizontal();
        GUILayout.Space(3f);

        GUI.changed = false;

        _text = "<b><size=11>" + _text + "</size></b>";

        //if (state)
        //    _text = "\u25B2 " + _text;
        //else
        //    _text = "\u25BC " + _text;

        if (state)
            _text = "\u25BC " + _text;
        else
            _text = "\u25BA " + _text;


        if (!state)
            _text = "<color=#B4B4B4>" + _text + "</color>";
        else
            _text = "<color=#4C7EFF>" + _text + "</color>";

        if (!GUILayout.Toggle(true, _text, "text", GUILayout.MinWidth(20f)))
            state = !state;
        //state = EditorGUILayout.Foldout(state, _text);
            

        if (GUI.changed)
            EditorPrefs.SetBool(_key, state);

        GUILayout.Space(2f);
        GUILayout.EndHorizontal();

        GUI.backgroundColor = Color.white;

        if (!_forceOn && !state)
            GUILayout.Space(3f);

        return state;
    }


    static public void BeginContents()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(4f);
        EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }

    /// <summary>
    /// End drawing the content area.
    /// </summary>

    static public void EndContents()
    {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(3f);
        GUILayout.EndHorizontal();
        GUILayout.Space(3f);
    }

    /// <summary>
    /// Create an undo point for the specified objects.
    /// </summary>

    static public void RegisterUndo(string name, params Object[] objects)
    {
        if (objects != null && objects.Length > 0)
        {
            UnityEditor.Undo.RecordObjects(objects, name);
            foreach (Object obj in objects)
            {
                if (obj == null) continue;
                EditorUtility.SetDirty(obj);
            }
        }
    }


    /// <summary>
    /// Draw a list of fields for the specified list of delegates.
    /// </summary>

    static public void DrawEvents(string text, Object undoObject, List<EventDelegate> list)
    {
        DrawEvents(text, text, undoObject, list, null, null);
    }
    static public void DrawEvents(string text, string key, Object undoObject, List<EventDelegate> list)
    {
        DrawEvents(text, key, undoObject, list, null, null);
    }

    /// <summary>
    /// Draw a list of fields for the specified list of delegates.
    /// </summary>

    static public void DrawEvents(string text, string key, Object undoObject, List<EventDelegate> list, string noTarget, string notValid)
    {
        if (!UIEditorTools.DrawHeader(text, key)) 
            return;

        UIEditorTools.BeginContents();
        EventDelegateEditor.Field(undoObject, list, notValid, notValid);
        UIEditorTools.EndContents();
    }

    static public void DrawActions(string text, Object undoObject, ref Collider collider, List<EventDelegate> list)
    {
        DrawActions(text, text, undoObject, ref collider, list, null, null);
    }
    static public void DrawActions(string text, string key, Object undoObject, ref Collider collider, List<EventDelegate> list)
    {
        DrawActions(text, key, undoObject, ref collider, list, null, null);
    }

    /// <summary>
    /// Draw a list of fields for the specified list of delegates.
    /// </summary>

    static public void DrawActions(string text, string key, Object undoObject, ref Collider collider, List<EventDelegate> list, string noTarget, string notValid)
    {
        if (!UIEditorTools.DrawHeader(text, key))
            return;

        UIEditorTools.BeginContents();
        GUILayout.Space(5);
        collider = EditorGUILayout.ObjectField("Trigger Rgn", collider, typeof(Collider), true) as Collider;
        GUILayout.Space(5);
        EventDelegateEditor.Field(undoObject, list, notValid, notValid);
        GUILayout.Space(5);
        UIEditorTools.EndContents();
    }
    static public void DrawActions(string text, Object undoObject, ref float distance, List<EventDelegate> list)
    {
        DrawActions(text, text, undoObject, ref distance, list, null, null);
    }
    static public void DrawActions(string text, string key, Object undoObject, ref float distance, List<EventDelegate> list)
    {
        DrawActions(text, key, undoObject, ref distance, list, null, null);
    }

    /// <summary>
    /// Draw a list of fields for the specified list of delegates.
    /// </summary>

    static public void DrawActions(string text, string key, Object undoObject, ref float distance, List<EventDelegate> list, string noTarget, string notValid)
    {
        if (!UIEditorTools.DrawHeader(text, key))
            return;

        UIEditorTools.BeginContents();
        GUILayout.Space(5);
        distance = EditorGUILayout.FloatField("Trigger Dist", distance);
        GUILayout.Space(5);
        EventDelegateEditor.Field(undoObject, list, notValid, notValid);
        GUILayout.Space(5);
        UIEditorTools.EndContents();
    }


    /// <summary>
    /// Convenience function that converts Class + Function combo into Class.Function representation.
    /// </summary>

    static public string GetFuncName(object obj, string method)
    {
        if (obj == null) 
            return "<null>";

        if (string.IsNullOrEmpty(method)) 
            return "<Choose>";

        string type = obj.GetType().ToString();

        int period = type.LastIndexOf('.');

        if (period > 0) 
            type = type.Substring(period + 1);

        //return type + "." + method;
        return type + "/" + method;
    }


}
