using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HeadListViewCell : CEBehaviour, IPointerClickHandler 
{

	private ComboList m_cmbListview = null;
	protected bool m_selectedFlag = false;

	public Text caption
	{
		get
		{
			return m_txtCaption;
		}
	}

	[SerializeField]
	protected Text m_txtCaption = null;

	[SerializeField]
	protected Image m_imgBackGround = null;
	[SerializeField]
	protected Sprite m_normalSprite = null;
	[SerializeField]
	protected Sprite m_hightLightedSprite = null;
	[SerializeField]
	protected Sprite m_disableSprite = null;

	protected virtual void InitCell()
	{
	}

	protected ComboList comboListView
	{
		get
		{
			if (m_cmbListview == null)
				m_cmbListview = rectTransform.GetComponentInParent<ComboList>();
			if (m_cmbListview == null)
			{
				if (transform.parent != null)
					m_cmbListview = transform.parent.GetComponentInParent<ComboList>();
			}

			return m_cmbListview;
		}
	}

	virtual public void selecteCell()
	{
		m_selectedFlag = true;

		if (m_imgBackGround != null &&
			m_hightLightedSprite != null)
			m_imgBackGround.sprite = m_hightLightedSprite;
	}

	virtual public void deselecteCell()
	{
		m_selectedFlag = false;

		if (m_imgBackGround != null &&
			m_normalSprite != null)
			m_imgBackGround.sprite = m_normalSprite;
	}


	virtual public void OnPointerClick(PointerEventData eventData)
	{
		if (comboListView != null)
			comboListView.selectHeaderCell();
	}

	virtual public void setCaption(string _szCaption)
	{
		if (m_txtCaption != null)
			m_txtCaption.text = _szCaption;
	}
}
