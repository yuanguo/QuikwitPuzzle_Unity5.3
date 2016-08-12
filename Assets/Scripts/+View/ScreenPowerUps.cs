using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public class ScreenPowerUps : CEBehaviour
{
	public GridLayoutGroup m_contentGridView = null;
	public PowerUpItemCell m_templateCell = null;

	public bool haveAction = false;

	public Text m_txtDescription = null;

	private string m_constDescriptionString = "This Dialog Will Be Closed After {0} Seconds.";

	List<PowerUpItemCell> childs = new List<PowerUpItemCell>();
	List<PowerUpItemCell> childsForRandomPowerup = new List<PowerUpItemCell>();
	List<PowerUpItemCell> unusedCells = new List<PowerUpItemCell>();

	private int CONST_MAX_TIMER = 5;
	private int m_timer = 0;

	private bool m_autoCloseFalg = false;

	//List<ItemInfo> m_itemCellInfos = new List<ItemInfo>();

	const int MAX_POWERUP_ITEM_NUM = 9;
	#region public methods

	public void openItemWnd(bool _autoClose = false)
	{
		m_autoCloseFalg = _autoClose;
		rectTransform.SetAsLastSibling();
		gameObject.SetActive(true);
		gameData.testValue();

		_loadItems();

		// stop game timer
		gameLogic.stopUpdateTimer();
	}

	public void closeItemWnd()
	{
		rectTransform.SetAsFirstSibling();
		gameObject.SetActive(false);

		// stop game timer
		gameLogic.beginUpdateTimer();
	}


	#endregion

	#region private methods
	void LateUpdate()
	{
		m_txtDescription.text = string.Format(m_constDescriptionString, m_timer);
	}

	protected override void Start()
	{
		base.Start ();

		if (m_templateCell != null)
			m_templateCell.gameObject.SetActive(false);
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_txtDescription.gameObject.SetActive(m_autoCloseFalg);

		if (m_autoCloseFalg == true)
		{
			m_timer = CONST_MAX_TIMER;
			m_txtDescription.text = string.Format(m_constDescriptionString, m_timer);
			InvokeRepeating("callCountDownTimer", 1, 1.0f);
		}
	}

	protected override void OnDisable()
	{
 		 base.OnDisable();
		 if (IsInvoking("callCountDownTimer") == true)
			 CancelInvoke("callCountDownTimer");
	}

	protected void callCountDownTimer()
	{
		m_timer--;

		if (m_timer <= 0)
		{
			if (IsInvoking("callCountDownTimer") == true)
				CancelInvoke("callCountDownTimer");

			closeItemWnd();
			/*
			int randItem = Random.Range(0, (childsForRandomPowerup.Count * Random.Range(1, 12345)));
			randItem = randItem % childsForRandomPowerup.Count;

			PowerUpItemCell cell = childsForRandomPowerup[randItem];
			if (cell != null)
				cell.OnPointerClick(null);
			*/
		}
	}


	private void _clearAllCells()
	{
		foreach (PowerUpItemCell cell in childs)
		{
			cell.transform.SetParent(null);
			cell.clearCell();
		}

		foreach (PowerUpItemCell cell1 in childsForRandomPowerup)
		{
			cell1.transform.SetParent(null);
			cell1.clearCell();
		}

		unusedCells.AddRange(childs);
		unusedCells.AddRange(childsForRandomPowerup);
		childs.Clear();
		childsForRandomPowerup.Clear();
	}

	private void _loadItems()
	{
		_clearAllCells();

		int nCellCount = 0;
		// power up item cell
		foreach (ItemKinds kind in gameData.powerUpsItems.Keys)
		{
			PowerUpItemCell cell = unusedCells.Find(itemCell => itemCell.kind == kind);

			// create a cell
			if (cell == null)
			{
				// create a new cell
				cell = Instantiate(m_templateCell) as PowerUpItemCell;
			}
			else
			{
				// using a cell that had been made already
				unusedCells.Remove(cell);
			}

			childsForRandomPowerup.Add(cell);

			// set a data into cell
			if (cell != null &&
				m_contentGridView != null)
			{
				if (kind != ItemKinds.blank)
				{
					gameData.powerUpsItems[kind].kind = kind;
					cell.setKind(kind, gameData.powerUpsItems[kind]);
					cell.gameObject.SetActive(true);
					cell.transform.SetParent(m_contentGridView.transform);
					cell.rectTransform.localScale = Vector3.one;
					cell.rectTransform.localPosition = Vector3.zero;
					cell.rectTransform.localRotation = Quaternion.identity;

					nCellCount++;
				}
			}
		}

		// blank item cell
		for (int nIdx = 0; nIdx < GameData.MAX_ITEM_COUNT - nCellCount; nIdx++)
		{
			PowerUpItemCell cell = unusedCells.Find(itemCell => itemCell.kind == ItemKinds.blank);

			// create a cell
			if (cell == null)
			{
				// create a new cell
				cell = Instantiate(m_templateCell) as PowerUpItemCell;
				childs.Add(cell);
			}
			else
			{
				// using a cell that had been made already
				unusedCells.Remove(cell);
				childs.Add(cell);
			}

			// set a data into cell
			if (cell != null &&
				m_contentGridView != null)
			{
				cell.setKind(ItemKinds.blank);
				cell.gameObject.SetActive(true);
				cell.transform.SetParent(m_contentGridView.transform);
				cell.rectTransform.localScale = Vector3.one;
				cell.rectTransform.localPosition = Vector3.zero;
				cell.rectTransform.localRotation = Quaternion.identity;
			}
		}

		if (m_contentGridView != null)
		{
			m_contentGridView.CalculateLayoutInputVertical();
			m_contentGridView.CalculateLayoutInputHorizontal();
			m_contentGridView.SetLayoutVertical();
			m_contentGridView.SetLayoutHorizontal();
			PowerUpItemCell maxItemCell = childs[0];
			foreach (PowerUpItemCell itemCell in childs)
			{
				if (maxItemCell.rectTransform.anchoredPosition.y < Mathf.Abs(itemCell.rectTransform.anchoredPosition.y))
					maxItemCell = itemCell;
			}
			RectTransform scrollContentView = (RectTransform)m_contentGridView.GetComponent<RectTransform>().parent;

			if (maxItemCell != null)
			{
				scrollContentView.sizeDelta = new Vector2(scrollContentView.sizeDelta.x,
					Mathf.Abs(maxItemCell.rectTransform.anchoredPosition.y) + Mathf.Abs(maxItemCell.rectTransform.sizeDelta.y));
				m_contentGridView.GetComponent<RectTransform>().sizeDelta = scrollContentView.sizeDelta;
			}
		}
	}
	#endregion
}
