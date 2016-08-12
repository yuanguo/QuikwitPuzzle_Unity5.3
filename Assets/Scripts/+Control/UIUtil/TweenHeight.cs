using UnityEngine;
using System.Collections;

[AddComponentMenu("UI/Tween/Height Tween")]
public class TweenHeight : Tweener
{
	#region Public Member
	[HideInInspector]
	public float m_From;
	[HideInInspector]
	public float m_To;
	[HideInInspector]
	public float m_Offset;
	[HideInInspector]
	public bool m_EnableOffset = false; 
	#endregion Public Member

	#region Private Member
	RectTransform m_Trans = null;
	float m_InitHeight = 0;
	#endregion Private Member

	#region Override MonoBehaviour
	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void onUpdate()
	{
		float factor = (m_AnimationCurve != null) ? m_AnimationCurve.Evaluate(m_Factor) : m_Factor;

		float from = m_From, to = m_To;
		if (m_EnableOffset)
		{
			from = m_InitHeight;
			to = m_InitHeight + m_Offset;
		}

		value = Mathf.Round(from * (1f - factor) + to * factor);

		base.onUpdate();
	}

	protected override void SetOffsetCurrentValue()
	{
		m_Offset = value;
	}
	protected override void SetStartToCurrentValue()
	{
		m_From = value;
	}

	protected override void SetEndToCurrentValue()
	{
		m_To = value;
	}
	protected override bool IsPlaying()
	{
        bool result = true;
        result = m_EnableOffset ? m_Offset != 0 : From != To;
        if (result)
            result = IsReverse() ? value != From : value != To;

        return result;
	}
	#endregion Override MonoBehaviour

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
	public float Offset
	{
		get { return m_Offset; }
		set { m_Offset = value; m_EnableOffset = true; }
	}

	public bool EnableOffset
	{
		get { return m_EnableOffset; }
		set { m_EnableOffset = value; }

	}
	public float From
	{
		get { return m_From; }
		set { m_From = value; }
	}

	public float To
	{
		get { return m_To; }
		set { m_To = value; }
	}

	public float value
	{
		get { return Trans ? Trans.sizeDelta.y : 1; }
		set { if (Trans) Trans.sizeDelta = new Vector2(Trans.sizeDelta.x, value); }
	}

	public float Height
	{
		get { return this.value; }
		set { this.value = value; }
	}
	#endregion Property

	#region Public Function
    public override void init()
    {
        if (Trans)
            m_InitHeight = value;

        base.init();
    }
	#endregion Public Function
}
