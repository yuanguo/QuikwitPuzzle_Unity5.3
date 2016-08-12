using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;



public class ListViewCell : CEBehaviour, IListViewCell
{
	public const int s_TemplateIdxNone = -1;

	public int m_templateIdx = 0;
	public bool m_selected = false;


    [SerializeField]
    protected Sprite m_normalSprite = null;
    [SerializeField]
    protected Sprite m_highlitedSprite = null;
    [SerializeField]
    protected Sprite m_disableSprite = null;

    [SerializeField]
    protected Image m_imageForground = null;

	public object userData = null;
	public int index
	{
		get
		{
			if (cellManager != null)
				return cellManager.usingCells.IndexOf(this);

			return -1;
		}
	}

	protected ListViewCellManager cellManager
	{
		get
		{
			if (m_cellManager == null)
				m_cellManager = rectTransform.GetComponentInParent<ListViewCellManager>();

			return m_cellManager;
		}
		set
		{
			m_cellManager = value;
		}
	}
	private ListViewCellManager m_cellManager = null;

    protected ListView listView
    {
        get
        {
            if (m_listview == null)
                m_listview = rectTransform.GetComponentInParent<ListView>();
            if (m_listview == null)
            {
                if (transform.parent != null)
                    m_listview = transform.parent.GetComponentInParent<ListView>();
             }

            return m_listview;
        }
    }

    private ListView m_listview = null;

	virtual public void OnDeselected()
	{
		m_selected = false;

        if (m_imageForground != null &&
            m_normalSprite != null)
            m_imageForground.sprite = m_normalSprite;
    }
	virtual public void OnSelected()
	{
		m_selected = true;

        if (m_imageForground != null &&
            m_highlitedSprite != null)
            m_imageForground.sprite = m_highlitedSprite;
	}

	virtual public void InitCell()
	{

	}

    public virtual void UpdateCell()
    {
        
    }

    virtual public void ResetCell()
    {
        userData = null;
    }
}


