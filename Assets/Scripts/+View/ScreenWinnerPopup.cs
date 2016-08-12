using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;

public enum ScreenWinnerPopupType
{
	ForPrizeMessage = 0x01,
	ForBonusMessage = 0x01 << 1,
	ForWinnerMessage = 0x01 << 2,
}

public delegate void screenWinnerPopupCloseDelegate();

public class ScreenWinnerPopup : CEBehaviour, IPuzzleNetLogicDelegates
{
	public screenWinnerPopupCloseDelegate m_delegate;

	public string m_szForPrize = "You are listed in our Lucky Draw!";

	public ScreenWinnerPopupType powerUpType
	{
		get { return m_type; }
		set { ChangePopupType(value); }
	}

	[SerializeField] RectTransform m_rectForPrizePopup = null;
	[SerializeField] RectTransform m_rectForWinnerPopup = null;

	[SerializeField] RectTransform m_rectPrizeStringBar = null;

	// winner images
	[SerializeField] List<Sprite> m_sprtWinners = new List<Sprite>();

	// m_rectForPrizePopup/m_imgTitle
	[SerializeField] Image m_imgTitle = null;
	[SerializeField] Sprite m_spriteForPrize = null;
	[SerializeField] Sprite m_spriteForBonus = null;

	[SerializeField] Image m_imgBgIcon = null;

	[SerializeField] Image m_imgPowerUpItem = null;
	[SerializeField] Image m_imgPowerUpBox = null;

	[SerializeField] List<Sprite> m_spritsPowerUpIcns = null;


	// private members
	private ScreenWinnerPopupType m_type = ScreenWinnerPopupType.ForPrizeMessage;

    private bool isPressedDissmisBtn = true;
    private ItemKinds m_itemKind = ItemKinds.blank;

	public void OpenPopup(ScreenWinnerPopupType _type, ItemKinds _kind = ItemKinds.tornado)
	{
		rectTransform.SetAsLastSibling();
		powerUpType = _type;
        m_itemKind = _kind;

		this.gameObject.SetActive(true);

		m_imgPowerUpItem.sprite = m_spritsPowerUpIcns[(int)_kind];

        isPressedDissmisBtn = true;

		if (m_type == ScreenWinnerPopupType.ForWinnerMessage)
			gameLogic.playWinnerSound ();
	}

	public void ClosePopup()
	{
        if (isPressedDissmisBtn == false) {
		}

        isPressedDissmisBtn = true;

		this.gameObject.SetActive (false);
		rectTransform.SetAsFirstSibling();
	}

	IEnumerator CloseAndShowCongratulationImg()
	{
		m_imgPowerUpBox.gameObject.SetActive(false);

		yield return new WaitForSeconds (2);


		if (m_type == ScreenWinnerPopupType.ForWinnerMessage) 
		{
			gameLogic.ChangeGameState (GameLogic.GameState.World);
			this.gameObject.SetActive (false);
			rectTransform.SetAsFirstSibling();
		}
	}

    public void OnDissmisBtnClick()
    {
		if (m_delegate != null &&
			WorldInfo.PassedWorlds < (WorldIdentifier)(WorldInfo.curWorlNum))
			m_delegate();
        isPressedDissmisBtn = true;

		if (m_type == ScreenWinnerPopupType.ForBonusMessage ||
			m_type == ScreenWinnerPopupType.ForPrizeMessage)
		{
			ItemInfo.InsertItem(m_itemKind, 1, true);
			PuzzleNetLogic.requestUpdatePowerUp(this, m_itemKind, 1, true, 0);
		}


		if (WorldInfo.PassedWorlds < (WorldIdentifier)(WorldInfo.curWorlNum)) 
		{
			ClosePopup();
		} 
		else if (m_type != ScreenWinnerPopupType.ForWinnerMessage) 
		{
			gameLogic.ChangeGameState (GameLogic.GameState.Level);
			OpenPopup (ScreenWinnerPopupType.ForWinnerMessage);
		} 			
		else 
		{
			if (m_type == ScreenWinnerPopupType.ForWinnerMessage)
				gameLogic.ChangeGameState(GameLogic.GameState.World);
		}
    }

	public void ChangePopupType(ScreenWinnerPopupType _type)
	{
		m_type = _type;

		m_rectForPrizePopup.gameObject.SetActive (false);
		m_rectPrizeStringBar.gameObject.SetActive (false);

		m_rectForWinnerPopup.gameObject.SetActive (false);

		m_imgPowerUpBox.gameObject.SetActive (false);

		m_imgBgIcon.gameObject.SetActive (false);

		if (m_type == ScreenWinnerPopupType.ForPrizeMessage) 
		{
			m_rectForPrizePopup.gameObject.SetActive (true);
			m_rectPrizeStringBar.gameObject.SetActive (true);

			m_imgTitle.sprite = m_spriteForPrize;
			m_imgPowerUpBox.gameObject.SetActive (true);

		} 
		else if (m_type == ScreenWinnerPopupType.ForBonusMessage) 
		{
			m_rectForPrizePopup.gameObject.SetActive (true);
			m_imgTitle.sprite = m_spriteForBonus;

			m_imgBgIcon.gameObject.SetActive (true);
		}
		else if (m_type == ScreenWinnerPopupType.ForWinnerMessage) 
		{
			m_rectForWinnerPopup.gameObject.SetActive (true);
			Image imgWinner = m_rectForWinnerPopup.GetComponent<Image>();
			imgWinner.sprite = m_sprtWinners[WorldInfo.curWorlNum - 1];
		}
	}

	public void OnPointerClick()
	{
		isPressedDissmisBtn = false;

		if (m_delegate != null &&
		    WorldInfo.PassedWorlds < (WorldIdentifier)WorldInfo.curWorlNum) 
		{
			m_delegate ();
		}

		if (m_type == ScreenWinnerPopupType.ForBonusMessage ||
		    m_type == ScreenWinnerPopupType.ForPrizeMessage) 
		{
			ItemInfo.InsertItem (m_itemKind, 1, true);
			PuzzleNetLogic.requestUpdatePowerUp (this, m_itemKind, 1, true, 0);
		}

		if (WorldInfo.PassedWorlds < (WorldIdentifier)WorldInfo.curWorlNum) 
		{
			WorldInfo.UpdatePassedWorldId((WorldIdentifier)WorldInfo.curWorlNum);
			ClosePopup();
		} 
		else if (m_type != ScreenWinnerPopupType.ForWinnerMessage) 
		{
			gameLogic.ChangeGameState (GameLogic.GameState.Level);
			OpenPopup (ScreenWinnerPopupType.ForWinnerMessage);
		}
		else 
		{
			if (m_type == ScreenWinnerPopupType.ForWinnerMessage)
			{
				gameLogic.ChangeGameState (GameLogic.GameState.World);
			}
		}
	}

	public void willStartRequest(RequestState _state)
	{
	}
	public void parsingError(RequestState _state)
	{
	}
	public void didRecievedSuccessResponseFromServer(object _data, RequestState _state)
	{
	}
	public void didRecievedFailedResponseFromServer(string _message, RequestState _state)
	{
	}
	public void didReceivingError(string _error, RequestState _state)
	{
	}
	public void didReceivingTimeOut(RequestState _state)
	{
	}

}
