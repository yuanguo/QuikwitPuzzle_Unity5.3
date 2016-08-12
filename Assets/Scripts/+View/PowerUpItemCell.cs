using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;

public interface iPoweupItemCellDelegate
{
	void didSelectedPowerUpItem(PowerUpItemCell _cell, ItemInfo _itemInfo);
}

public class PowerUpItemCell : CEBehaviour, IPointerClickHandler
{
	#region UI members
	[SerializeField] private Image m_imgItem = null;
	[SerializeField] private List<Sprite> m_sprItemKinds = new List<Sprite>();
	[SerializeField] private ScreenGame screenGame = null;
	[SerializeField] private Text m_txtNum = null;

	public iPoweupItemCellDelegate m_delegate = null;

	public ItemKinds kind = ItemKinds.blank;
	public ItemInfo itemInfo = null;
	public ScreenPowerUps m_powerUps = null;
	#endregion

	#region public methods
	public void setKind(ItemKinds _kind, ItemInfo _itemInfo = null)
	{
		kind = _kind;

		if (m_imgItem != null)
			m_imgItem.sprite = m_sprItemKinds[(int)_kind];

		if (_itemInfo != null)
		{
			_itemInfo.kind = _kind;

			if (m_txtNum != null)
			{
				m_txtNum.enabled = _itemInfo.countOfItem > 0 ? true : false;
				m_txtNum.text = "+" + _itemInfo.countOfItem.ToString();
			}

			itemInfo = _itemInfo;
			m_delegate = screenGame;
		}
		else if (_itemInfo == null || _kind == ItemKinds.blank)
		{
			if (m_txtNum != null)
			{
				m_txtNum.enabled = false;
				m_txtNum.text = "";
			}
		}
	}

	public void clearCell()
	{
		kind = ItemKinds.blank;
		if (m_txtNum != null)
		{
			m_txtNum.enabled = false;
			m_txtNum.text = "";
		}

		if (m_imgItem != null)
			m_imgItem.sprite = m_sprItemKinds[(int)kind];
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (kind >= ItemKinds.blank)
			return;

		if (m_powerUps != null)
		{
			if (m_powerUps.haveAction == false)
				return;

			m_powerUps.closeItemWnd();

			if (m_delegate != null)
				m_delegate.didSelectedPowerUpItem(this, itemInfo);
		}

	}
	#endregion
}
