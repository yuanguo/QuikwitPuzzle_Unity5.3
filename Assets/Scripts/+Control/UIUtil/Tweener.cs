using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tweener : MonoBehaviour 
{
    const string NotifyFinished = "NotifyOnFinished";
    public enum Style
    {
        Once,
        Loop,
        PingPong,
    }

    #region Public Member
    [HideInInspector]
    public float m_Duration = 1f;
    [HideInInspector]
    public Style m_Style = Style.Once;
    [HideInInspector]
    public AnimationCurve m_AnimationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
    [HideInInspector]
    public List<EventDelegate> onFinished = new List<EventDelegate>();
    #endregion Public Member

    #region Protected Member
    protected float m_AmountPerDelta = 1000f;
    protected float m_Factor = 0f;
    protected List<EventDelegate> m_Temp = null;
    #endregion Protected Member


    // Use this for initialization
    #region Override MonoBehaviour 
    protected virtual void Awake()
    {
        m_AmountPerDelta = Mathf.Abs((m_Duration > 0f) ? 1f / m_Duration : 1000f);
#if !UNITY_EDITOR
		//Reset();
#endif
        init();
    }
    protected virtual void OnEnable()
    {
        init();
    }
	protected virtual void Start () 
    {
        FixedUpdate();
	}
	// Update is called once per frame
	protected virtual void FixedUpdate () 
    {
        float delta = Time.fixedDeltaTime;

        m_Factor += m_AmountPerDelta * delta;

        if (m_Style == Style.Loop)
        {
            if (m_Factor > 1f)
                m_Factor -= Mathf.Floor(m_Factor);
        }
        else if (m_Style == Style.PingPong)
        {
            if (m_Factor > 1f)
            {
                m_Factor = 1f - (m_Factor - Mathf.Floor(m_Factor));
                m_AmountPerDelta = -m_AmountPerDelta;
            }
            else if (m_Factor < 0f)
            {
                m_Factor = -m_Factor;
                m_Factor -= Mathf.Floor(m_Factor);
                m_AmountPerDelta = -m_AmountPerDelta;
            }
        }

        if ((m_Style == Style.Once) && (!IsPlaying() && 
            (m_Duration == 0f || m_Factor > 1f || m_Factor < 0f)))
        {
            m_Factor = Mathf.Clamp01(m_Factor);
            onUpdate();
            if (m_Factor == 1f && m_AmountPerDelta > 0f || m_Factor == 0f && m_AmountPerDelta < 0f)
                enabled = false;

            if (onFinished != null)
                StartCoroutine(NotifyFinished);

            if (IsSequenceAni())
                PlaySequenceAni();
        }
        else
        {
            m_Factor = Mathf.Clamp01(m_Factor);
            onUpdate();
        }
    }

    void OnDisable()
    {
    }

    void Reset()
    {
        SetStartToCurrentValue();
        SetEndToCurrentValue();
        SetOffsetCurrentValue();
    }

    #endregion Override MonoBehaviour

    #region Public Function
    public virtual void init()
    {
        m_Factor = 0.0f;
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
        m_AmountPerDelta = Mathf.Abs(m_AmountPerDelta);
        if (!_forward)
        {
            m_Factor = m_Factor <= 0f ? 1.0f : m_Factor;
            m_AmountPerDelta = -m_AmountPerDelta;
        }
        else
            m_Factor = m_Factor >= 1f ? 0.0f : m_Factor;

        enabled = true;
        FixedUpdate();
    }

    public void ResetToBeginning()
    {
        m_Factor = (m_AmountPerDelta < 0f) ? 1f : 0f;
        FixedUpdate();
    }

    public void Toggle()
    {
        m_AmountPerDelta = m_Factor > 0f ? -m_AmountPerDelta : Mathf.Abs(m_AmountPerDelta);
        enabled = true;
    }

    public void SetOnFinished(EventDelegate.Callback del) { EventDelegate.Set(onFinished, del); }

    public void SetOnFinished(EventDelegate del) { EventDelegate.Set(onFinished, del); }

    public void AddOnFinished(EventDelegate.Callback del) { EventDelegate.Add(onFinished, del); }

    public void AddOnFinished(EventDelegate del) { EventDelegate.Add(onFinished, del); }

    public void RemoveOnFinished(EventDelegate del)
    {
        if (onFinished != null)
            onFinished.Remove(del);

        if (m_Temp != null)
            m_Temp.Remove(del);
    }

    public void RemoveOnFinished(EventDelegate.Callback del)
    {
        if (onFinished != null)
            EventDelegate.Remove(onFinished, del);

        if (m_Temp != null)
            EventDelegate.Remove(m_Temp, del);
    }
    #endregion Public Function

    #region Protected Function
    protected virtual bool IsSequenceAni()
    {
        return false;
    }

    protected virtual void PlaySequenceAni()
    {

    }
    protected virtual void SetStartToCurrentValue()
    {

    }

    protected virtual void SetEndToCurrentValue()
    {

    }
    protected virtual void SetOffsetCurrentValue()
    {

    }
    protected virtual void onUpdate()
    {

    }
    protected virtual bool IsReverse()
    {
        return m_AmountPerDelta < 0;
    }
    protected virtual bool IsPlaying()
    {
        return true;
    }
    
    #endregion Protected Function

    #region Property
    public float Duration
    {
        get { return m_Duration; }
        set { m_Duration = value; }
    }
    public float TweenFactor
    {
        get { return m_Factor; }
        set { m_Factor = Mathf.Clamp01(value); }
    }
    public float AmountDelta
    {
        get { return m_AmountPerDelta; }
    }
    #endregion Property
    #region Private
    private IEnumerator NotifyOnFinished()
    {
        yield return null;

        m_Temp = onFinished;
        onFinished = new List<EventDelegate>();

        EventDelegate.Execute(m_Temp);
        for (int i = 0; i < m_Temp.Count; ++i)
        {
            EventDelegate ed = m_Temp[i];
            if (ed != null)
                EventDelegate.Add(onFinished, ed, ed.oneShot);
        }

        m_Temp = null;

    }
    #endregion Private
}
