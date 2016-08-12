using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("UI/Tween/Alpha Tween")]
public class TweenAlpha : Tweener
{
    #region Public Member
    [HideInInspector]
    [Range(0f, 1f)] public float m_From = 0f;
    [HideInInspector]
    [Range(0f, 1f)] public float m_To = 0f;
    [HideInInspector]
    [Range(0f, 1f)]public float m_Offset = 0.0f;
    [HideInInspector]
    public bool m_EnableOffset = false;
    #endregion Public Member

    #region Private Member
    private CanvasGroup m_CanvasGroup = null;
    private Image m_Image = null;
    private RawImage m_RawImage = null;
    private Text m_Text = null;
    private float m_InitAlpha = 0.0f;
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
            from = m_InitAlpha;
            to = m_InitAlpha + m_Offset;
        }

        value = Mathf.Lerp(from, to, factor);

        base.onUpdate();
    }

    protected override void SetOffsetCurrentValue()
    {
        if (m_Offset == 0.0f && m_From == m_To)
            m_Offset = value;
    }
    protected override void SetStartToCurrentValue()
    {
        if (m_From == 0.0f && m_From == m_To)
            m_From = value;
    }

    protected override void SetEndToCurrentValue()
    {
        if (m_To == 0.0f && m_From == value)
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
    public CanvasGroup Canvas
    {
        get
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
            if (!m_CanvasGroup)
                m_CanvasGroup = GetComponentInChildren<CanvasGroup>();

            return m_CanvasGroup;
        }
    }

    public Image Img
    {
        get
        {

            m_Image = GetComponent<Image>();
            if (!m_Image)
                m_Image = GetComponentInChildren<Image>();

            return m_Image;
        }
    }
    public RawImage RawImg
    {
        get
        {
            m_RawImage = GetComponent<RawImage>();
            if (!m_RawImage)
                m_RawImage = GetComponentInChildren<RawImage>();

            return m_RawImage;

        }
    }
    public Text TextImg
    {
        get
        {
            m_Text = GetComponent<Text>();
            if (!m_Text)
                m_Text = GetComponentInChildren<Text>();

            return m_Text;
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
    public float alpha
    {
        get { return this.value; }
        set { this.value = value; }
    }

    public float value
    {
        get 
        {
            float alpha = 1.0f;
            if (Canvas)
                alpha = Canvas.alpha;
            else if (Img)
                alpha = Img.color[3];
            else if (RawImg)
                alpha = RawImg.color[3];
            else if (TextImg)
                alpha = TextImg.color[3];

            return alpha;
        }
        set 
        {
            if (Canvas)
            {
                Canvas.alpha = value;
            }
            else if (Img != null)
            {
                Color color = Img.color;
                color[3] = value;
                Img.color = color;
            }
            else if (RawImg != null)
            {
                Color color = RawImg.color;
                color[3] = value;
                RawImg.color = color;
            }
            else if (TextImg != null)
            {
                Color color = TextImg.color;
                color[3] = value;
                TextImg.color = color;
            }
        }
    }
    #endregion Property

    #region Public Function
    public override void init()
    {
        if (Canvas || Img)
            m_Offset = value;

        base.init();
    }
    #endregion Public Function

    #region Private Function
    #endregion Private Function
}
