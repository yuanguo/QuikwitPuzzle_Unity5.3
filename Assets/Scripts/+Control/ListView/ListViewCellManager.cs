using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Events;
using UnityEngine.EventSystems;

[Serializable]
public class ListViewCellManager : MonoBehaviour
{

	protected const string s_IdentifierForCell		= "identifierForCell";
	protected const string s_IndentifierForUnusedCell = "unusedCell";

/*
	public List<RectTransform> templates
	{
		get
		{
			return getTemplates();
		}
		set
		{
			if (m_templates != null)
			{
				if (value == null)
				{
					m_templates.Clear();
					return;
				}

				if (value.Count == 0)
					return;

				m_templates.Clear();

				//foreach(RectTransform template in value)
				for (int nIndex = 0; nIndex < value.Count; nIndex++)
				{
					if (value[nIndex] != null)
					{
						m_templates.Add(value[nIndex]);
						value[nIndex].gameObject.SetActive(false);
					}
				}
			}
		}
	}
*/
	public List<ListViewCell> unusedCells
	{
		get
		{
			if (m_unusedCells == null)
				m_unusedCells = new List<ListViewCell>();

			return m_unusedCells;
		}
	}
	public List<ListViewCell> usingCells
	{
		get
		{
			if (m_usingCells == null)
				m_usingCells = new List<ListViewCell>();

			return m_usingCells;
		}
	}

	public ListViewCell lastSelectedCell
	{
		get
		{
			return m_lastSelectedCell;
		}

		set
		{
			m_lastSelectedCell = value;
		}
	}

	#region private members
	[SerializeField]
	protected List<ListViewCell> m_unusedCells = null;
	[SerializeField]
	protected List<ListViewCell> m_usingCells = null;

	[SerializeField]
	private List<RectTransform> m_templates = new List<RectTransform>();

	[SerializeField]
	private ListViewCell m_lastSelectedCell = null;
	#endregion private members

	#region identifier
	public string identifierForCell()
	{
		return (s_IdentifierForCell);
	}

	#endregion identifier

	#region get and set cell
	public List<RectTransform> getTemplates()
	{
		if (m_templates == null)
			m_templates = new List<RectTransform>();

		return new List<RectTransform>(m_templates);
	}

	public void setTemplates(List<RectTransform> _templates)
	{
		if (m_templates == null)
			m_templates = new List<RectTransform>();

		if (_templates == null)
		{
			m_templates.Clear();
			return;
		}

		if (_templates.Count == 0)
			return;

		m_templates.Clear();

		//foreach(RectTransform template in value)
		for (int nIndex = 0; nIndex < _templates.Count; nIndex++)
		{
			if (_templates[nIndex] != null)
			{
				m_templates.Add(_templates[nIndex]);
				_templates[nIndex].gameObject.SetActive(false);
			}
		}
	}

	public bool isExistingCell(ListViewCell _cellInfo)
	{
		if (usingCells.Find(cell => cell.m_templateIdx == _cellInfo.m_templateIdx &&
			cell.Equals(_cellInfo)) != null)
		{
			if (usingCells.IndexOf(_cellInfo) != -1)
				return true;
		}

		return false;

	}

	/**
	 * get a cell that is not using from unused cell's table
	 * */
	public ListViewCell getCellFromUnusedCellsTable(int _templateIdx)
	{
		if (unusedCells == null)
			return null;

		ListViewCell cellInfo = null;
		cellInfo = unusedCells.Find(cell => cell.m_templateIdx == _templateIdx);

		return cellInfo;
	}

	/**
	 * get a cell that is same as _row or templateId
	 * */
	public ListViewCell getCellFromUsingCellsTable(int _row, int _templateIdx = ListViewCell.s_TemplateIdxNone)
	{
		if (usingCells == null ||
			_row >= usingCells.Count ||
			_row < 0 ||
			usingCells.Count == 0)
		{
			return null;
		}

		ListViewCell cellInfo = null;
		cellInfo = usingCells[_row];
		
		if (_templateIdx <= ListViewCell.s_TemplateIdxNone)
			return cellInfo;
		else
		{
			if (cellInfo.m_templateIdx == _templateIdx)
				return cellInfo;
		}

		return null;

	}

	public ListViewCell setActiveCellFromUnusedTable(int _row, int _templateIdx = ListViewCell.s_TemplateIdxNone)
	{
		if (_row > usingCells.Count ||
			_row < 0)
			return null;

		ListViewCell cellInfo = getCellFromUnusedCellsTable(_templateIdx);

		if (cellInfo == null)
			return null;

		string nameKey = identifierForCell();

		cellInfo.name = nameKey;
		cellInfo.gameObject.SetActive(true);

		usingCells.Insert(_row, cellInfo);
		unusedCells.Remove(cellInfo);

		if (cellInfo == null)
			return null;

		return cellInfo;
	}

	public ListViewCell addCellIntoUnusedTable(int _row)
	{
		ListViewCell cellInfo = getCellFromUsingCellsTable(_row);

		if (cellInfo == null)
			return null;

		cellInfo.name = s_IndentifierForUnusedCell;
		cellInfo.gameObject.SetActive(false);
		cellInfo.rectTransform.SetAsLastSibling();
        cellInfo.ResetCell(); // destroy cell used data

		unusedCells.Add(cellInfo);
		usingCells.Remove(cellInfo);

		return cellInfo;
	}

	public bool addCellIntoUnusedTable(ListViewCell _cellInfo)
	{
		if (isExistingCell(_cellInfo))
		{
			_cellInfo.name = s_IndentifierForUnusedCell;
			_cellInfo.gameObject.SetActive(false);
			_cellInfo.rectTransform.SetAsLastSibling();
            _cellInfo.ResetCell(); // destroy cell used data

			unusedCells.Add(_cellInfo);
			usingCells.Remove(_cellInfo);
			return true;
		}

		return false;
	}

    public void destroyAllCells()
    {
        for (int nIndex = 0; nIndex < usingCells.Count; nIndex++)
        {
            usingCells[nIndex].ResetCell(); // destroy cell used data
        }

        usingCells.Clear();
    }
	public void addAllCellIntoUnusedTable()
	{
		//foreach(ListViewCell cellInfo in usingCells)
		for (int nIndex = 0; nIndex < usingCells.Count; nIndex++)
		{
			usingCells[nIndex].name = s_IndentifierForUnusedCell;
			usingCells[nIndex].gameObject.SetActive(false);
			usingCells[nIndex].rectTransform.SetAsLastSibling();
            usingCells[nIndex].ResetCell(); // destroy cell used data

			unusedCells.Add(usingCells[nIndex]);
		}

		usingCells.Clear();
	}



	#endregion get and set cell

	/**
	 * create a cell by using the object in template tables
	 * param1 (_row) : game object's index
	 * param2 [_templateIdx] : template id number
	 *	create a cell and add the ListViewCell component,
	 */
	public ListViewCell createCellFromTemplateTable(int _row, int _templateIdx)
	{
		if (_row < 0 ||
			m_templates == null)
		{
			Debug.LogError("parent or name is null!!!");
			return null;
		}

		if (m_templates.Count < _templateIdx ||
			_templateIdx < 0)
		{
			Debug.LogError("_templateIdx is over than templates table number");
			return null;
		}


		GameObject newCell = null;

		if (m_templates != null &&
			m_templates.Count > _templateIdx)
			newCell = Instantiate(m_templates[_templateIdx].gameObject) as GameObject;

		if (newCell == null)
			return null;

		RectTransform trsf = newCell.GetComponent<RectTransform>();

		if (trsf == null)
			trsf = newCell.AddComponent<RectTransform>();

		if (trsf == null)
			return null;

		ListViewCell cellInfo = trsf.gameObject.GetComponent<ListViewCell>();

		if (cellInfo == null)
			cellInfo = trsf.gameObject.AddComponent<ListViewCell>();

		//cellTemp.m_cellDelegate = this;
		cellInfo.m_selected = false;
		cellInfo.m_templateIdx = _templateIdx;

		insertCellInfoToUsingTable(_row, cellInfo);
		cellInfo.rectTransform.gameObject.SetActive(true);
		return cellInfo;
	}
	public ListViewCell createCell(int _row, int _templateIdx, bool _usingUnusedTable = true, bool _usingAlreadyExistingCell = false)
	{
		ListViewCell createdCellInfo = null;

		if (_usingAlreadyExistingCell == true)
		{
			createdCellInfo = getCellFromUsingCellsTable(_row, _templateIdx);

			if (createdCellInfo != null)
				return createdCellInfo;
		}

		if (_usingUnusedTable == true)
		{
			createdCellInfo = setActiveCellFromUnusedTable(_row, _templateIdx);

			if (createdCellInfo != null)
				return createdCellInfo;
		}

		createdCellInfo = createCellFromTemplateTable(_row, _templateIdx);

        if (createdCellInfo == null)
            return null;

		createdCellInfo.rectTransform.gameObject.SetActive(true);
		return createdCellInfo;
	}

	public bool insertCellInfoToUsingTable(int _row, ListViewCell _cellInfo)
	{
		if (_cellInfo == false ||
			_cellInfo.transform == null)
			return false;
		if (_cellInfo.rectTransform == null)
			return false;

		// 새로 창조된 오브젝트의 속성값설정
		_cellInfo.rectTransform.name = identifierForCell();
		_cellInfo.rectTransform.SetParent(transform);
		_cellInfo.rectTransform.localPosition = Vector3.zero;
		_cellInfo.rectTransform.localRotation = Quaternion.identity;
		_cellInfo.rectTransform.localScale = Vector3.one;

		Vector2 value = new Vector2(0.0f, 1.0f);
		_cellInfo.rectTransform.pivot = value;
		_cellInfo.rectTransform.anchorMin = value;
		_cellInfo.rectTransform.anchorMax = value;

		usingCells.Insert(_row, _cellInfo);
		return true;
	}

	public bool insertCellInfoToUsingTable(ListViewCell _cellInfo)
	{
		if (_cellInfo == false ||
			_cellInfo.transform == null)
			return false;
		if (_cellInfo.rectTransform == null)
			return false;

		// 새로 창조된 오브젝트의 속성값설정
		_cellInfo.rectTransform.name = identifierForCell();
		_cellInfo.rectTransform.SetParent(transform);
		_cellInfo.rectTransform.localPosition = Vector3.zero;
		_cellInfo.rectTransform.localRotation = Quaternion.identity;
		_cellInfo.rectTransform.localScale = Vector3.one;

		Vector2 value = new Vector2(0.0f, 1.0f);
		_cellInfo.rectTransform.pivot = value;
		_cellInfo.rectTransform.anchorMin = value;
		_cellInfo.rectTransform.anchorMax = value;

		usingCells.Add(_cellInfo);
		return true;
	}
}
