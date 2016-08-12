using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("UI/Tween/PRS Tween")]
public class TweenPRS : MonoBehaviour
{
    public enum Style { Once, Loop, PingPong, }
    public enum PRSType { None = 0x0000, Pos = 0x0001, Rot = 0x0010, Scale = 0x0100, }

    [System.Serializable]
    public class PRSInfo
    {
        public PRSType m_Type = PRSType.Pos;
        public bool m_Enable = false;
        public bool m_EnableOffset = false;
        public Vector3 m_From = Vector3.zero;
        public Vector3 m_To = Vector3.zero;
        public Vector3 m_Offset = Vector3.zero;

        public bool m_IsSpeed = false;
        public float m_Speed = 1f;
        public Vector3 m_Dir = Vector3.one;
        public float m_Duration = 1f;
        public Style m_Style = Style.Once;
        public AnimationCurve m_AnimationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
        public List<EventDelegate> onFinished = new List<EventDelegate>();
        public float m_AmountPerDelta = 1f;
        public float m_Factor = 0f;

        //public PRSInfo()
        //{

        //}
        public PRSInfo(PRSType _type)
        {
            m_Type = _type;
        }
    }

    
    #region Public Member
    public PRSInfo[] m_TweenList = 
    {
        new PRSInfo(PRSType.Pos),
        new PRSInfo(PRSType.Rot),
        new PRSInfo(PRSType.Scale)
    };

    #endregion Public Member

    #region Private Member
    private RectTransform m_Trans;
    public bool m_init = false;
    private Vector3 m_InitPos = Vector3.zero;
    private Vector3 m_InitRot = Vector3.zero;
    private Vector3 m_initScale = Vector3.zero;
    protected List<EventDelegate> m_Temp = null;

    #endregion Private Member
    // Use this for initialization
    void Awake()
    {
        //base.Awake();

        if (isSetCurrentValue())
        {
            SetStartToCurrentValue();
            SetEndToCurrentValue();
            SetOffsetCurrentValue();
        }

        if (!m_init)
            init();

     }
    void Start()
    {
        FixedUpdate();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_init)
            init();

        float delta = Time.deltaTime;

        //foreach (PRSInfo info in m_TweenList)
		for (int nIndex = 0; nIndex < m_TweenList.Length; nIndex++)
        {
			PRSInfo info = m_TweenList[nIndex];

            if (!info.m_Enable)
                continue;

            UpdateFactor(info, delta);
            UpdatePRS(info, delta);
        }
    }

    void Reset()
    {
        SetStartToCurrentValue();
        SetEndToCurrentValue();
        SetOffsetCurrentValue();
    }
    void SetOffsetCurrentValue()
    {
        //foreach (PRSInfo info in m_TweenList)
		for (int nIndex = 0; nIndex < m_TweenList.Length; nIndex++)
        {
			PRSInfo info = m_TweenList[nIndex];
        
            //if (!info.m_Enable)
            //    continue;

            if (info.m_Type == PRSType.Pos)
                info.m_Offset = pos;
            else if (info.m_Type == PRSType.Rot)
                info.m_Offset = rotation.eulerAngles;
            else
                info.m_Offset = scale;
        }
    }

    void SetStartToCurrentValue()
    {
        //foreach (PRSInfo info in m_TweenList)
		for (int nIndex = 0; nIndex < m_TweenList.Length; nIndex++)
        {
			PRSInfo info = m_TweenList[nIndex];
        
            //if (!info.m_Enable)
            //    continue;

            if (info.m_Type == PRSType.Pos)
                info.m_From = pos;
            else if (info.m_Type == PRSType.Rot)
                info.m_From = rotation.eulerAngles;
            else
                info.m_From = scale;
        }
    }
    void SetEndToCurrentValue()
    {
        //foreach (PRSInfo info in m_TweenList)
		for (int nIndex = 0; nIndex < m_TweenList.Length; nIndex++)
        {
			PRSInfo info = m_TweenList[nIndex];
            //if (!info.m_Enable)
            //    continue;

            if (info.m_Type == PRSType.Pos)
                info.m_To = pos;
            else if (info.m_Type == PRSType.Rot)
                info.m_To = rotation.eulerAngles;
            else
                info.m_To = scale;
        }
    }

    bool isSetCurrentValue()
    {
        //foreach (PRSInfo info in m_TweenList)
		for (int nIndex = 0; nIndex < m_TweenList.Length; nIndex++)
        {
			PRSInfo info = m_TweenList[nIndex];
            if (!info.m_Enable)
                continue;

            if (info.m_EnableOffset == false && (info.m_From == Vector3.zero || info.m_To == Vector3.zero))
                return true;
            else if (info.m_EnableOffset == true && info.m_Offset == Vector3.zero)
                return true;
        }

        return false;
    }
    public bool isEnable(PRSType type)
    {
        PRSInfo info = null;
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
            info = (PRSInfo)m_TweenList[index];

        return info != null ? info.m_Enable : false;
    }
    public void init(PRSType _type, Vector3 _from, Vector3 _to, float _duration = 1.0f, List<EventDelegate> _finished = null,
        Style _style = Style.Once, AnimationCurve _curve = null)
    {
        setEnable(_type, true);
        SetFrom(_type, _from);
        SetTo(_type, _to);
        SetDuration(_type, _duration);
        SetOnFinished(_type, _finished);
        SetStyle(_type, _style);
        SetAnimationCurve(_type, _curve);
    }

    public void init(PRSType _type, Vector3 _offset, float _duration = 1.0f, List<EventDelegate> _finished = null,
    Style _style = Style.Once, AnimationCurve _curve = null)
    {
        setEnableOffset(_type, true);
        SetOffset(_type, _offset);
        SetDuration(_type, _duration);
        SetOnFinished(_type, _finished);
        SetStyle(_type, _style);
        SetAnimationCurve(_type, _curve);
    }
    public void setEnable(uint _type, bool _enablePos, bool _enableRot = false, bool _enableScale = false)
    {
        if ((_type & 0x01) == (uint)PRSType.Pos)
        {
            setEnable(PRSType.Pos, _enablePos);
            if ((_type & 0x10) == (uint)PRSType.Rot)
            {
                setEnable(PRSType.Rot, _enableRot);

                if ((_type & 0x100) == (uint)PRSType.Scale)
                    setEnable(PRSType.Scale, _enableScale);
            }
            else if ((_type & 0x100) == (uint)PRSType.Scale)
                setEnable(PRSType.Scale, _enableRot);
        }
        else if ((_type & 0x10) == (uint)PRSType.Rot)
        {
            setEnable(PRSType.Rot, _enablePos);
            if ((_type & 0x100) == (uint)PRSType.Scale)
                setEnable(PRSType.Scale, _enableRot);
        }
        else
            setEnable(PRSType.Scale, _enablePos);
    }
    public void setEnable(PRSType type, bool enable)
    {
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
        {
            PRSInfo info = (PRSInfo)m_TweenList[index];
            info.m_Enable = enable;
        }
    }

    public bool isEnableOffset(PRSType type)
    {
        PRSInfo info = null;
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
            info = (PRSInfo)m_TweenList[index];

        return info != null ? info.m_EnableOffset : false;
    }

    public void setEnableOffset(uint _type, bool _enablePos, bool _enableRot = false, bool _enableScale = false)
    {
        if ((_type & 0x01) == (uint)PRSType.Pos)
        {
            setEnableOffset(PRSType.Pos, _enablePos);
            if ((_type & 0x10) == (uint)PRSType.Rot)
            {
                setEnableOffset(PRSType.Rot, _enableRot);

                if ((_type & 0x100) == (uint)PRSType.Scale)
                    setEnableOffset(PRSType.Scale, _enableScale);
            }
            else if ((_type & 0x100) == (uint)PRSType.Scale)
                setEnableOffset(PRSType.Scale, _enableRot);
        }
        else if ((_type & 0x10) == (uint)PRSType.Rot)
        {
            setEnableOffset(PRSType.Rot, _enablePos);
            if ((_type & 0x100) == (uint)PRSType.Scale)
                setEnableOffset(PRSType.Scale, _enableRot);
        }
        else
            setEnableOffset(PRSType.Scale, _enablePos);
    }

    public void setEnableOffset(PRSType type, bool enable)
    {
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
        {
            PRSInfo info = (PRSInfo)m_TweenList[index];
            info.m_EnableOffset = enable;
        }
    }

    public Vector3 GetFrom(PRSType type)
    {
        PRSInfo info = null;
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
            info = (PRSInfo)m_TweenList[index];

        return info != null ? info.m_From : Vector3.zero;

    }

    public void SetFrom(uint _type, Vector3 _pos)
    {
        SetFrom(_type, _pos, Vector3.one, Vector3.one);
    }
    public void SetFrom(uint _type, Vector3 _pos, Vector3 _rot)
    {
        SetFrom(_type, _pos, _rot, Vector3.one);
    }
    public void SetFrom(uint _type, Vector3 _pos, Vector3 _rot, Vector3 _scale)
    {
        if ((_type & 0x01) == (uint)PRSType.Pos)
        {
            SetFrom(PRSType.Pos, _pos);
            if ((_type & 0x10) == (uint)PRSType.Rot)
            {
                SetFrom(PRSType.Rot, _rot);

                if ((_type & 0x100) == (uint)PRSType.Scale)
                    SetFrom(PRSType.Scale, _scale);
            }
            else if ((_type & 0x100) == (uint)PRSType.Scale)
                SetFrom(PRSType.Scale, _rot);
        }
        else if ((_type & 0x10) == (uint)PRSType.Rot)
        {
            SetFrom(PRSType.Rot, _pos);
            if ((_type & 0x100) == (uint)PRSType.Scale)
                SetFrom(PRSType.Scale, _rot);
        }
        else
            SetFrom(PRSType.Scale, _pos);
    }
    
    public void SetFrom(PRSType type, Vector3 from)
    {
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
        {
            PRSInfo info = (PRSInfo)m_TweenList[index];
            info.m_From = from;

            if (type == PRSType.Pos && info.m_IsSpeed)
            {
                pos = info.m_From;
                info.m_Dir = info.m_To - info.m_From;
                info.m_Dir.Normalize();
            }
        }
    }

    public Vector3 GetTo(PRSType type)
    {
        PRSInfo info = null;
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
            info = (PRSInfo)m_TweenList[index];

        return info != null ? info.m_To : Vector3.zero;

    }

    public void SetTo(uint _type, Vector3 _pos)
    {
        SetTo(_type, _pos, Vector3.one, Vector3.one);
    }
    public void SetTo(uint _type, Vector3 _pos, Vector3 _rot)
    {
        SetTo(_type, _pos, _rot, Vector3.one);
    }
    public void SetTo(uint _type, Vector3 _pos, Vector3 _rot, Vector3 _scale)
    {
        if ((_type & 0x01) == (uint)PRSType.Pos)
        {
            SetTo(PRSType.Pos, _pos);
            if ((_type & 0x10) == (uint)PRSType.Rot)
            {
                SetTo(PRSType.Rot, _rot);

                if ((_type & 0x100) == (uint)PRSType.Scale)
                    SetTo(PRSType.Scale, _scale);
            }
            else if ((_type & 0x100) == (uint)PRSType.Scale)
                SetTo(PRSType.Scale, _rot);
        }
        else if ((_type & 0x10) == (uint)PRSType.Rot)
        {
            SetTo(PRSType.Rot, _pos);
            if ((_type & 0x100) == (uint)PRSType.Scale)
                SetTo(PRSType.Scale, _rot);
        }
        else
            SetTo(PRSType.Scale, _pos);
    }

    public void SetTo(PRSType type, Vector3 to)
    {
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
        {
            PRSInfo info = (PRSInfo)m_TweenList[index];
            info.m_To = to;

            if (type == PRSType.Pos && info.m_IsSpeed)
            {
                info.m_Dir = info.m_To - info.m_From;
                info.m_Dir.Normalize();
            }
        }
    }

    public Vector3 GetOffset(PRSType type)
    {
        PRSInfo info = null;
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
            info = (PRSInfo)m_TweenList[index];

        return info != null ? info.m_Offset : Vector3.zero;

    }
    public void SetOffset(uint _type, Vector3 _pos)
    {
        SetOffset(_type, _pos, Vector3.one, Vector3.one);
    }
    public void SetOffset(uint _type, Vector3 _pos, Vector3 _rot)
    {
        SetOffset(_type, _pos, _rot, Vector3.one);
    }
    public void SetOffset(uint _type, Vector3 _pos, Vector3 _rot, Vector3 _scale)
    {
        if ((_type & 0x01) == (uint)PRSType.Pos)
        {
            SetOffset(PRSType.Pos, _pos);
            if ((_type & 0x10) == (uint)PRSType.Rot)
            {
                SetOffset(PRSType.Rot, _rot);

                if ((_type & 0x100) == (uint)PRSType.Scale)
                    SetOffset(PRSType.Scale, _scale);
            }
            else if ((_type & 0x100) == (uint)PRSType.Scale)
                SetOffset(PRSType.Scale, _rot);
        }
        else if ((_type & 0x10) == (uint)PRSType.Rot)
        {
            SetOffset(PRSType.Rot, _pos);
            if ((_type & 0x100) == (uint)PRSType.Scale)
                SetOffset(PRSType.Scale, _rot);
        }
        else
            SetOffset(PRSType.Scale, _pos);
    }
    
    public void SetOffset(PRSType type, Vector3 to)
    {
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
        {
            PRSInfo info = (PRSInfo)m_TweenList[index];
            info.m_Offset = to;
        }
    }

    public Style GetStyle(PRSType type)
    {
        PRSInfo info = null;
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
            info = (PRSInfo)m_TweenList[index];

        return info != null ? info.m_Style : Style.Once;

    }

    public void SetStyle(uint _type, Style _stylePos, Style _styleRot = Style.Once, Style _styleScale = Style.Once)
    {
        if ((_type & 0x01) == (uint)PRSType.Pos)
        {
            SetStyle(PRSType.Pos, _stylePos);
            if ((_type & 0x10) == (uint)PRSType.Rot)
            {
                SetStyle(PRSType.Rot, _styleRot);

                if ((_type & 0x100) == (uint)PRSType.Scale)
                    SetStyle(PRSType.Scale, _styleScale);
            }
            else if ((_type & 0x100) == (uint)PRSType.Scale)
                SetStyle(PRSType.Scale, _styleRot);
        }
        else if ((_type & 0x10) == (uint)PRSType.Rot)
        {
            SetStyle(PRSType.Rot, _stylePos);
            if ((_type & 0x100) == (uint)PRSType.Scale)
                SetStyle(PRSType.Scale, _styleRot);
        }
        else
            SetStyle(PRSType.Scale, _stylePos);
    }
    
    public void SetStyle(PRSType type, Style style)
    {
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
            m_TweenList[index].m_Style = style;
    }

    public bool isSpeed(PRSType type)
    {
        PRSInfo info = null;
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
            info = (PRSInfo)m_TweenList[index];

        return info != null ? info.m_IsSpeed : false;
    }

    public void setEnableSpeed(PRSType type, bool enable)
    {
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
        {
            PRSInfo info = (PRSInfo)m_TweenList[index];
            info.m_IsSpeed = enable;
        }
    }

    public void setEnableSpeed(uint _type, bool _enablePos, bool _enableRot = false, bool _enableScale = false)
    {
        if ((_type & 0x01) == (uint)PRSType.Pos)
        {
            setEnableSpeed(PRSType.Pos, _enablePos);
            if ((_type & 0x10) == (uint)PRSType.Rot)
            {
                setEnableSpeed(PRSType.Rot, _enableRot);

                if ((_type & 0x100) == (uint)PRSType.Scale)
                    setEnableSpeed(PRSType.Scale, _enableScale);
            }
            else if ((_type & 0x100) == (uint)PRSType.Scale)
                setEnableSpeed(PRSType.Scale, _enableRot);
        }
        else if ((_type & 0x10) == (uint)PRSType.Rot)
        {
            setEnableSpeed(PRSType.Rot, _enablePos);
            if ((_type & 0x100) == (uint)PRSType.Scale)
                setEnableSpeed(PRSType.Scale, _enableRot);
        }
        else
            setEnableSpeed(PRSType.Scale, _enablePos);
    }

    public float GetDuration(PRSType type)
    {
        PRSInfo info = null;
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
            info = (PRSInfo)m_TweenList[index];

        return info != null ? (info.m_IsSpeed ? info.m_Speed : info.m_Duration) : 0f;

    }

    public void SetDuration(uint _type, float _pos, float _rot = 1f, float _scale = 1f)
    {
        if ((_type & 0x01) == (uint)PRSType.Pos)
        {
            SetDuration(PRSType.Pos, _pos);
            if ((_type & 0x10) == (uint)PRSType.Rot)
            {
                SetDuration(PRSType.Rot, _rot);

                if ((_type & 0x100) == (uint)PRSType.Scale)
                    SetDuration(PRSType.Scale, _scale);
            }
            else if ((_type & 0x100) == (uint)PRSType.Scale)
                SetDuration(PRSType.Scale, _rot);
        }
        else if ((_type & 0x10) == (uint)PRSType.Rot)
        {
            SetDuration(PRSType.Rot, _pos);
            if ((_type & 0x100) == (uint)PRSType.Scale)
                SetDuration(PRSType.Scale, _rot);
        }
        else
            SetDuration(PRSType.Scale, _pos);
    }

    public void SetDuration(PRSType type, float duration)
    {
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
        {
            if (!m_TweenList[index].m_IsSpeed)
            {
                m_TweenList[index].m_AmountPerDelta = Mathf.Abs((duration > 0f) ? 1f / duration : 1000f);
                m_TweenList[index].m_Duration = duration;
            }
            else
            {
                m_TweenList[index].m_Speed = duration;
                m_TweenList[index].m_Dir = m_TweenList[index].m_To - m_TweenList[index].m_From;
                m_TweenList[index].m_Dir.Normalize();
            }
        }
    }

    public AnimationCurve GetAnimationCurve(PRSType type)
    {
        PRSInfo info = null;
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
            info = (PRSInfo)m_TweenList[index];

        return info != null ? info.m_AnimationCurve : null;

    }
    public List<EventDelegate> GetOnFinished(PRSType type)
    {
        PRSInfo info = null;
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
            info = (PRSInfo)m_TweenList[index];

        return info != null ? info.onFinished : null;

    }

    public void SetOnFinished(uint _type, List<EventDelegate> _pos)
    {
        SetOnFinished(_type, _pos, null, null);
    }
    public void SetOnFinished(uint _type, List<EventDelegate> _pos, List<EventDelegate> _rot)
    {
        SetOnFinished(_type, _pos, _rot, null);
    }
    public void SetOnFinished(uint _type, List<EventDelegate> _pos, List<EventDelegate> _rot, List<EventDelegate> _scale)
    {
        if ((_type & 0x01) == (uint)PRSType.Pos)
        {
            SetOnFinished(PRSType.Pos, _pos);
            if ((_type & 0x10) == (uint)PRSType.Rot)
            {
                SetOnFinished(PRSType.Rot, _rot);

                if ((_type & 0x100) == (uint)PRSType.Scale)
                    SetOnFinished(PRSType.Scale, _scale);
            }
            else if ((_type & 0x100) == (uint)PRSType.Scale)
                SetOnFinished(PRSType.Scale, _rot);
        }
        else if ((_type & 0x10) == (uint)PRSType.Rot)
        {
            SetOnFinished(PRSType.Rot, _pos);
            if ((_type & 0x100) == (uint)PRSType.Scale)
                SetOnFinished(PRSType.Scale, _rot);
        }
        else
            SetOnFinished(PRSType.Scale, _pos);
    }
    
    public void SetOnFinished(PRSType type, List<EventDelegate> finished)
    {
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
            m_TweenList[index].onFinished = finished;
    }

    public void SetOnFinished(PRSType type, EventDelegate.Callback del) 
    {
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
            EventDelegate.Set(m_TweenList[index].onFinished, del);
    }

    public void SetOnFinished(PRSType type, EventDelegate del) 
    {
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
            EventDelegate.Set(m_TweenList[index].onFinished, del);
    }

    public void AddOnFinished(PRSType type, EventDelegate.Callback del) 
    {
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
            EventDelegate.Add(m_TweenList[index].onFinished, del);
    }

    public void AddOnFinished(PRSType type, EventDelegate del) 
    {
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
            EventDelegate.Add(m_TweenList[index].onFinished, del);
    }

    public void RemoveOnFinished(PRSType type, EventDelegate del)
    {
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
        {
            if (m_TweenList[index].onFinished != null)
                m_TweenList[index].onFinished.Remove(del);

            if (m_Temp != null)
                m_Temp.Remove(del);
        }
    }

    public void RemoveOnFinished(PRSType type, EventDelegate.Callback del)
    {
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
        {
            if (m_TweenList[index].onFinished != null)
                EventDelegate.Remove(m_TweenList[index].onFinished, del);

            if (m_Temp != null)
                EventDelegate.Remove(m_Temp, del);
        }
    }

    public void SetAnimationCurve(uint _type, AnimationCurve _pos)
    {
        SetAnimationCurve(_type, _pos, null, null);
    }
    public void SetFrom(uint _type, AnimationCurve _pos, AnimationCurve _rot)
    {
        SetAnimationCurve(_type, _pos, _rot, null);
    }
    public void SetAnimationCurve(uint _type, AnimationCurve _pos, AnimationCurve _rot, AnimationCurve _scale)
    {
        if ((_type & 0x01) == (uint)PRSType.Pos)
        {
            SetAnimationCurve(PRSType.Pos, _pos);
            if ((_type & 0x10) == (uint)PRSType.Rot)
            {
                SetAnimationCurve(PRSType.Rot, _rot);

                if ((_type & 0x100) == (uint)PRSType.Scale)
                    SetAnimationCurve(PRSType.Scale, _scale);
            }
            else if ((_type & 0x100) == (uint)PRSType.Scale)
                SetAnimationCurve(PRSType.Scale, _rot);
        }
        else if ((_type & 0x10) == (uint)PRSType.Rot)
        {
            SetAnimationCurve(PRSType.Rot, _pos);
            if ((_type & 0x100) == (uint)PRSType.Scale)
                SetAnimationCurve(PRSType.Scale, _rot);
        }
        else
            SetAnimationCurve(PRSType.Scale, _pos);
    }
    
    public void SetAnimationCurve(PRSType type, AnimationCurve curve)
    {
        int index = getIndex(type);
        if (index > -1 && index < m_TweenList.Length)
            m_TweenList[index].m_AnimationCurve = curve;
    }

    public void Play()
    {
        Play(true);
    }

    public void PlayForward()
    {
        Play(true);
    }

    public void PlayReverse()
    {
        Play(false);
    }

    public void Play(bool _forward)
    {
        //foreach (PRSInfo info in m_TweenList)
		for (int nIndex = 0; nIndex < m_TweenList.Length; nIndex++)
        {
			PRSInfo info = m_TweenList[nIndex];
            if (!info.m_Enable)
                continue;

            info.m_AmountPerDelta = Mathf.Abs(info.m_AmountPerDelta);
            if (!_forward)
            {
                info.m_Factor = info.m_Factor <= 0f ? 1.0f : info.m_Factor;
                info.m_AmountPerDelta = -info.m_AmountPerDelta;
            }
            else
                info.m_Factor = info.m_Factor >= 1f ? 0.0f : info.m_Factor;
        }

        enabled = true;
        FixedUpdate();
    }

    public void ResetToBeginning()
    {
        //foreach (PRSInfo info in m_TweenList)
		for (int nIndex = 0; nIndex < m_TweenList.Length; nIndex++)
        {
			if (!m_TweenList[nIndex].m_Enable)
                continue;

			m_TweenList[nIndex].m_Factor = (m_TweenList[nIndex].m_AmountPerDelta < 0f) ? 1f : 0f;
        }

        FixedUpdate();
    }

    public void Toggle()
    {
        //foreach (PRSInfo info in m_TweenList)
		for (int nIndex = 0; nIndex < m_TweenList.Length; nIndex++)
        {
			if (!m_TweenList[nIndex].m_Enable)
                continue;

			m_TweenList[nIndex].m_AmountPerDelta = m_TweenList[nIndex].m_Factor > 0f ? -m_TweenList[nIndex].m_AmountPerDelta : Mathf.Abs(m_TweenList[nIndex].m_AmountPerDelta);
        }

        enabled = true;
    }

    #region Property
    public RectTransform Trans
    {
        get
        {
            m_Trans = GetComponent<RectTransform>();
            if (!m_Trans)
                m_Trans = GetComponentInChildren<RectTransform>();

            return m_Trans;
        }
    }
    public Vector3 pos
    {
        get { return Trans != null ? Trans.anchoredPosition : Vector2.zero; }
        set
        {
            if (Trans != null)
            {
                Vector3 pos = value;
                Trans.anchoredPosition = new Vector2(pos.x, pos.y);
            }
        }
    }

    public Quaternion rotation
    {
        get { return Trans ? Trans.localRotation : Quaternion.identity; }
        set { if (Trans) Trans.localRotation = value; }
    }

    public Vector3 scale
    {
        get { return Trans ? Trans.localScale : Vector3.one; }
        set { if (Trans) Trans.localScale = value; }
    }


    #endregion Property

    #region Private Function
    void UpdateFactor(PRSInfo info, float delta)
    {
        info.m_Factor += info.m_AmountPerDelta * delta;

        if (info.m_Style == Style.Loop)
        {
            if (info.m_Factor > 1f)
                info.m_Factor -= Mathf.Floor(info.m_Factor);
        }
        else if (info.m_Style == Style.PingPong)
        {
            if (info.m_Factor > 1f)
            {
                info.m_Factor = 1f - (info.m_Factor - Mathf.Floor(info.m_Factor));
                info.m_AmountPerDelta = -info.m_AmountPerDelta;
            }
            else if (info.m_Factor < 0f)
            {
                info.m_Factor = -info.m_Factor;
                info.m_Factor -= Mathf.Floor(info.m_Factor);
                info.m_AmountPerDelta = -info.m_AmountPerDelta;
            }
        }

        if ((info.m_Style == Style.Once) && isFinish(info))
        {
            info.m_Factor = Mathf.Clamp01(info.m_Factor);
            if (!info.m_IsSpeed && (info.m_Factor == 1f && info.m_AmountPerDelta > 0f ||
                info.m_Factor == 0f && info.m_AmountPerDelta < 0f))
                enabled = false;
            else if (info.m_IsSpeed)
                enabled = false;

            if (info.onFinished != null)
            {
                m_Temp = info.onFinished;
                info.onFinished = new List<EventDelegate>();

                EventDelegate.Execute(m_Temp);

                for (int i = 0; i < m_Temp.Count; ++i)
                {
                    EventDelegate ed = m_Temp[i];
                    if (ed != null)
                        EventDelegate.Add(info.onFinished, ed, ed.oneShot);
                }

                m_Temp = null;
            }
        }
        else
            info.m_Factor = Mathf.Clamp01(info.m_Factor);

        //info.m_Factor = (info.m_AnimationCurve != null) ? info.m_AnimationCurve.Evaluate(info.m_Factor) : info.m_Factor;
    }

    void UpdatePRS(PRSInfo info, float delta)
    {
        if (!enabled)
            return;

        float factor = (info.m_AnimationCurve != null) ? info.m_AnimationCurve.Evaluate(info.m_Factor) : info.m_Factor;

        Vector3 from = info.m_From, to = info.m_To;
        if (info.m_IsSpeed)
            from = pos;
        
        if (info.m_EnableOffset)
        {
            if (info.m_Type == PRSType.Pos)
                from = m_InitPos;
            else if (info.m_Type == PRSType.Rot)
                from = m_InitRot;
            else if (info.m_Type == PRSType.Scale)
                from = m_initScale;

            to = from + info.m_Offset;
        }

        if (info.m_Type == PRSType.Pos)
            pos = info.m_IsSpeed ? (from + info.m_Speed * delta * info.m_Dir) : (from * (1f - factor) + to * factor);
        else if (info.m_Type == PRSType.Scale)
            scale = (from * (1f - factor) + to * factor);
        else
        {
            rotation = Quaternion.Euler(new Vector3(
                Mathf.Lerp(from.x, to.x, factor),
                Mathf.Lerp(from.y, to.y, factor),
                Mathf.Lerp(from.z, to.z, factor)));
        }
    }

    void init()
    {
        //foreach (PRSInfo info in m_TweenList)
		for (int nIndex = 0; nIndex < m_TweenList.Length; nIndex++)
		{
			if (m_TweenList[nIndex].m_IsSpeed)
				m_TweenList[nIndex].m_AmountPerDelta = m_TweenList[nIndex].m_Duration;
			else
				m_TweenList[nIndex].m_AmountPerDelta = Mathf.Abs((m_TweenList[nIndex].m_Duration > 0f) ? 1f / m_TweenList[nIndex].m_Duration : 1000f);

            if (m_TweenList[nIndex].m_IsSpeed && nIndex == 0)
            {
                pos = m_TweenList[nIndex].m_From;
                m_TweenList[nIndex].m_Dir = m_TweenList[nIndex].m_To - m_TweenList[nIndex].m_From;
                m_TweenList[nIndex].m_Dir.Normalize();
            }
            else if (m_TweenList[nIndex].m_IsSpeed && nIndex == 1)
                rotation = Quaternion.Euler(m_TweenList[nIndex].m_From);
            else if (m_TweenList[nIndex].m_IsSpeed && nIndex == 2)
                scale = m_TweenList[nIndex].m_From;
		}

        if (Trans)
        {
            m_InitPos = pos;
            m_InitRot = rotation.eulerAngles;
            m_initScale = scale;
        }

        m_init = true;
    }

    int getIndex(PRSType _type)
    {
        if (_type == PRSType.Pos)
            return 0;

        if (_type == PRSType.Rot)
            return 1;

        if (_type == PRSType.Scale)
            return 2;

        return 0;
    }
    bool isFinish(PRSInfo info)
    {
        bool finish = false;
        if (info.m_IsSpeed)
        {
            finish = (info.m_From.x > info.m_To.x && pos.x <= info.m_To.x
                && info.m_From.y > info.m_To.y && pos.y <= info.m_To.y) ||
                (info.m_From.x > info.m_To.x && pos.x <= info.m_To.x
                && info.m_From.y <= info.m_To.y && pos.y >= info.m_To.y) ||
                (info.m_From.x <= info.m_To.x && pos.x >= info.m_To.x
                && info.m_From.y > info.m_To.y && pos.y <= info.m_To.y) ||
                (info.m_From.x <= info.m_To.x && pos.x >= info.m_To.x
                && info.m_From.y <= info.m_To.y && pos.y >= info.m_To.y);
        }
        else
            finish = (info.m_Duration == 0f || info.m_Factor > 1f || info.m_Factor < 0f);

        return finish;

    }
    #endregion Private Function

}
