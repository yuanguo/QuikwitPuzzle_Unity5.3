using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class TweenImage : MonoBehaviour
{
    public enum Style
    {
        Once,
        Loop,
    }

    //[HideInInspector]
    public float m_FlickerTime = 1.0f;
    //[HideInInspector]
    public Style m_Style = Style.Once;
    //[HideInInspector]
    public List<Sprite> m_Streams = new List<Sprite>();

    private int m_CurIndex = 0;
    private Sprite m_OldSprite = null;

    void Awake()
    {
        m_OldSprite = GetComponent<Image>().sprite;
    }

    void OnEnable()
    {
        StopCoroutine("ImageAni");
        StartCoroutine("ImageAni");
    }

    void OnDisable()
    {
        GetComponent<Image>().sprite = m_OldSprite;
        StopCoroutine("ImageAni");
    }
	void Start () 
    {
	}
	
	
	IEnumerator ImageAni() 
    {
        while (true)
        {
            if (m_Style == Style.Once && m_CurIndex == m_Streams.Count)
            {
                enabled = false;
                m_CurIndex = 0;
                yield break;
            }
            else if (m_Style == Style.Loop && m_CurIndex == m_Streams.Count)
                m_CurIndex = 0;

            m_CurIndex = Mathf.Clamp(m_CurIndex, 0, m_Streams.Count - 1);
            SpriteImg = m_Streams[m_CurIndex];

            yield return new WaitForSeconds(m_FlickerTime);
            m_CurIndex++;
        }
	}

    public Sprite SpriteImg
    {
        set
        {
            GetComponent<Image>().sprite = value;
        }
    }

    public Style style
    {
        get { return m_Style; }
        set { m_Style = value; }
    }

    public float FlickerTime
    {
        get { return m_FlickerTime; }
        set { m_FlickerTime = value; }
    }

    public List<Sprite> Streams
    {
        get { return m_Streams; }
        set { m_Streams = value; }
    }
}
