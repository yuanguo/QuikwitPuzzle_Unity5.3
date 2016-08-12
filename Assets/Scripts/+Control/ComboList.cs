using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public interface IComboListDelegate
{
	bool beginReloadComboList(ComboList _comboList);
	void endReloadComboList(ComboList _comboList);
	bool insertingCellInfo(ComboList _comboList, ListViewCell _cellInfo);

	void didSelectedHeaderCell(ComboList _comboList, HeadListViewCell _header);
	void didDeSelectedHeaderCell(ComboList _comboList, HeadListViewCell _header);

	void didSelectedCellInfo(ComboList _comboList, ListViewCell _cellInfo);
	void didDeSelectedCellInfo(ComboList _comboList, ListViewCell _cellInfo);
}

public interface IComboResizingDelegate
{
	void willBeginResizingComboListView(ComboList _comboList);
	void didStopResizingComboListView(ComboList _comboList);
}



[ExecuteInEditMode] 
[RequireComponent(typeof(TweenHeight))]
public class ComboList : CEBehaviour, IListViewDelegate
{
	#region const members
	protected const string ListViewName = "combo_listViewName";
	protected const string HeaderViewName = "combo_headerViewName";
	#endregion

	#region public members
	//[HideInInspector]
	public int m_cellCount = 0;
	//[HideInInspector]
	public int m_showingCellCount = 0;

	public bool m_fixingAutoCellSize = true;
	public bool m_resizingWithAnimationFlag = true;
	//[HideInInspector]
	public IComboListDelegate m_delegate = null;
	public IComboResizingDelegate m_resizingDelegate = null;

	public bool isAssignedListView
	{
		get
		{
			return (m_listView == null ? false : true);
		}
	}

	public bool isAssignedHeaderView
	{
		get
		{
			return (m_headerCell == null ? false : true);
		}
	}

	public bool isOpenComboList
	{
		get
		{
			return m_isOpenComboList;
		}
	}
// 	public bool scrolling
// 	{
// 		get
// 		{
// 			if (m_listView != null)
// 				return m_listView.scrolling;
// 
// 			return false;
// 		}
// 		set
// 		{
// 			if (m_listView != null)
// 				m_listView.scrolling = value;
// 
// 			m_isScrolling = value;
// 		}
// 	}

	public RectTransform headerCell
	{
		get
		{
			return m_headerCell;
		}
		set
		{
			m_headerCell = value;
		}
	}

	public HeadListViewCell headerInfo
	{
		get
		{
			if (m_headerInfo == null)
			{
				m_headerInfo = headerCell.GetComponent<HeadListViewCell>();

				if (m_headerInfo != null)
					return m_headerInfo;

				m_headerInfo = headerCell.gameObject.AddComponent<HeadListViewCell>();
			}

			return m_headerInfo;
		}
	}

	public List<RectTransform> cellTemplates
	{
		get
		{
			if (m_listView != null)
				return m_listView.templates;

			return null;
		}
		set
		{
			if (value != null &&
				value.Count >= 0)
			{
				m_cellTemplates.Clear();
				
				for (int idx = 0; idx < value.Count; idx++)
					m_cellTemplates.Add(value[idx]);

				if (m_listView != null)
					m_listView.setTemplates(m_cellTemplates);
			}
		}
	}
	#endregion

	#region UI members
	[SerializeField]
	protected RectTransform m_headerCell = null;	// 기정으로 현시되는 셀
	[SerializeField]
	protected ListView m_listView = null;

	[SerializeField]
	protected List<RectTransform> m_cellTemplates = null;

	[SerializeField]
	protected bool m_isScrolling = false;
	#endregion

	#region private members
	protected bool m_isOpenComboList = false;
	private TweenHeight m_tween = null;

	private HeadListViewCell m_headerInfo = null;

	protected TweenHeight tween
	{
		get
		{
			if (m_tween == null)
				m_tween = GetComponent<TweenHeight>();

			return m_tween;
		} 
	}


	protected int templateCellCount
	{
		get
		{
			if (m_listView != null)
				return m_listView.templateCellCount;
			return 0;
		}
	}
	#endregion

	#region public API methods
	public void init()
	{
		m_isOpenComboList = false;
	}

	virtual public void initComboList (IComboListDelegate _delegate = null)
	{
		if (isAssignedListView == false)
			createListView();

		if (isAssignedHeaderView == false)
			createHeaderView();

		if (m_listView != null)
		{
			m_listView.m_delegate = this;
            //m_listView.templates = m_cellTemplates;
            //m_listView.setTemplates(m_cellTemplates);
			m_listView.initListViewTemplates();
			m_listView.scrolling = m_isScrolling;
		}

		m_delegate = _delegate;
	}

	virtual public void reloadComboList()
	{
		if (m_listView == null)
			return;

		if (m_delegate != null)
		{
			// 셀들을 재로드 한다.
			if (m_delegate.beginReloadComboList(this) == true)
			{
				// 이전 셀들을 삭제
				m_listView.clearCells();

				insertCells();
				resizingCells(m_resizingWithAnimationFlag);

				m_isOpenComboList = false;
				m_delegate.endReloadComboList(this);
			}
		}
	}

	public void playOpenCloseAnimation()
	{
		_playOpenCloseAnimation();
	}

	public void createListView()
	{
		_createListView();
	}

	public void createHeaderView()
	{
		_createHeaderView();
	}

	public ListViewCell insertAtLastRow(int _templetID, object _userData = null)
	{
		ListViewCell cellInfo = null;
		if (m_listView != null)
			cellInfo = m_listView.insertAtLastRow(_templetID, _userData);

		resizingCells(m_resizingWithAnimationFlag);
		return cellInfo;
	}

	public bool removeCellAtRow(ListViewCell _cellInfo)
	{
		if (_cellInfo == null)
			return false;
		m_listView.removeCellAtRow(_cellInfo);
		return true;
	}

	public void clearCell()
	{
		if (m_listView != null)
			m_listView.clearCells();

		deselectHeaderCell();
		resizingCells(m_resizingWithAnimationFlag);
	}

	public void selectHeaderCell()
	{
		_selectHeaderCell();
	}

	public void deselectHeaderCell()
	{
		_deselectHeaderCell();
	}

	public virtual bool selectCellInfo(ListViewCell _cellInfo)
	{
		return m_listView.selectCellAtRow(_cellInfo);
	}

	public virtual ListViewCell selectCellAtRow(int _cellIdx)
	{
		return m_listView.selectCellAtRow(0);
	}

	public virtual ListViewCell selectCellByUserData(object _userData)
	{
		return m_listView.selectCellByUserData(_userData);
	}


	public void unselectCell()
	{
		m_listView.unselectCell();
	}


	public void resizeComboList()
	{
		resizingCells(m_resizingWithAnimationFlag);
	}
	
	public void forceResizeComboListWithoutAnimation()
	{
		resizingCells(false);
	}

	#endregion

	#region comboList protect methods
	protected virtual ListViewCell _insertCellWithData(int _rowIdx, object _userData)
	{
		if (m_listView == null)
			return null;

		if (_userData != null && (string)_userData != string.Empty)
			return m_listView.insertAtLastRow(_rowIdx % templateCellCount, _userData);

		return null;
	}


    protected virtual void insertCells()
	{
		// 셀들을 리스트뷰에 삽입
		for (int nIdx = 0; nIdx < m_cellCount; nIdx++)
		{
			ListViewCell cell = m_listView.insertAtLastRow(0);
			if (m_delegate.insertingCellInfo(this, cell) == false)
			{
				m_listView.removeCellAtRow(cell);
				continue;
			}
		}
	}

    protected virtual Vector2 settingDefaultCell()
	{
		// 한개셀의 크기 얻기 및 default의 위치지정
		if (m_headerCell != null && m_fixingAutoCellSize)
		{
			m_headerCell.pivot = new Vector2(0f, 1f);
			m_headerCell.anchorMin = new Vector2(0f, 1f);
			m_headerCell.anchorMax = new Vector2(0f, 1f);
			m_headerCell.anchoredPosition = Vector2.zero;

			return m_headerCell.sizeDelta;
		}
		else if (m_headerCell != null && m_fixingAutoCellSize == false)
		{
			return m_headerCell.sizeDelta;
		}

		return Vector2.zero;
	}

    protected virtual Vector2 settingListView(ListViewCell _cellInfo = null)
	{
		if (m_listView != null)
		{
			Vector2 cellItemSize = Vector2.zero;
			Vector2 sizeOfDefaultCell = Vector2.zero;

			if (_cellInfo != null)
				cellItemSize = _cellInfo.rectTransform.sizeDelta;

			if (m_headerCell != null)
				sizeOfDefaultCell = m_headerCell.sizeDelta;

			// 화면에 현시되게 되는 cell최소개수 정의
			float minCount = m_listView.cellCount > m_showingCellCount ? m_showingCellCount : m_listView.cellCount;

			if (m_fixingAutoCellSize == true)
			{
				m_listView.rectTransform.pivot = new Vector2(0f, 1f);
				m_listView.rectTransform.anchorMin = new Vector2(0f, 1f);
				m_listView.rectTransform.anchorMax = new Vector2(0f, 1f);
				m_listView.rectTransform.anchoredPosition = new Vector2(0f, Math.Abs(sizeOfDefaultCell.y) * (-1f));
				m_listView.rectTransform.sizeDelta = new Vector2(cellItemSize.x, Math.Abs(cellItemSize.y * minCount));
			}
			else
			{
				m_listView.rectTransform.sizeDelta = new Vector2(m_listView.rectTransform.sizeDelta.x, Math.Abs(cellItemSize.y * minCount));
			}

			return m_listView.rectTransform.sizeDelta;
		}

		return Vector2.zero;
	}

    protected virtual void resizingCells(bool _animation = true)
	{
		Vector2 defaultCellSize = Vector2.zero;

		// auto fixing all cells and views
		// 첫번째의 셀 찾기
		ListViewCell cellInfo = m_listView.findCell(0);

		// default의 위치지정
		defaultCellSize = settingDefaultCell();

		// 콤보리스트의 크기지정
		if (m_fixingAutoCellSize == true)
			rectTransform.sizeDelta = defaultCellSize;
		else
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, defaultCellSize.y);

		// listview의 크기 및 위치 지정	
		Vector2 sizeOfListView = settingListView(cellInfo);

		if (_animation == true)
		{
			// 트윈설정
			if (tween != null)
			{
				tween.From = defaultCellSize.y;
				tween.To = defaultCellSize.y + sizeOfListView.y;
				tween.enabled = false;
			}

			if (m_resizingDelegate != null)
				m_resizingDelegate.willBeginResizingComboListView(this);
		}
		else
		{
			if (m_resizingDelegate != null)
				m_resizingDelegate.willBeginResizingComboListView(this);

			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, defaultCellSize.y + sizeOfListView.y);

			if (m_resizingDelegate != null)
				m_resizingDelegate.didStopResizingComboListView(this);
		}
	}

	protected void _createListView()
	{
		if (m_listView == null)
		{
			GameObject obj = GameObject.Find(ListViewName);

			if (obj == null)
				obj = new GameObject();

			obj.name = ListViewName;

			RectTransform rectTrsf = obj.GetComponent<RectTransform>();

			if (rectTrsf == null)
				rectTrsf = obj.AddComponent<RectTransform>();

			if (rectTrsf == null)
				return;

			rectTrsf.SetParent(rectTransform);

			rectTrsf.localPosition = Vector3.zero;
			rectTrsf.localRotation = Quaternion.identity;
			rectTrsf.localScale = Vector3.one;

			rectTrsf.position = Vector3.zero;
			rectTrsf.rotation = Quaternion.identity;

			Vector2 posLeftTop = new Vector2(0f, 1f);

			rectTrsf.pivot = posLeftTop;
			rectTrsf.anchorMin = posLeftTop;
			rectTrsf.anchorMax = posLeftTop;

			Vector2 sizeOfParent = rectTransform.sizeDelta;
			rectTrsf.anchoredPosition = new Vector2(0f, - sizeOfParent.y);
			rectTrsf.sizeDelta = sizeOfParent;

			m_listView = obj.GetComponent<ListView>();

			if (m_listView == null)
				m_listView = obj.AddComponent<ListView>();
			m_listView.m_delegate = this;
		}
	}

	protected void _createHeaderView()
	{
		if (m_headerCell == null)
		{
			GameObject obj = GameObject.Find(HeaderViewName);

			if (obj == null)
				obj = new GameObject();

			obj.name = HeaderViewName;

			m_headerCell = obj.GetComponent<RectTransform>();

			if (m_headerCell == null)
				m_headerCell = obj.AddComponent<RectTransform>();

			if (m_headerCell == null)
				return;



			m_headerCell.SetParent(rectTransform);

			m_headerCell.localPosition = Vector3.zero;
			m_headerCell.localRotation = Quaternion.identity;
			m_headerCell.localScale = Vector3.one;

			m_headerCell.position = Vector3.zero;
			m_headerCell.rotation = Quaternion.identity;

			Vector2 posLeftTop = new Vector2(0f, 1f);

			m_headerCell.pivot = posLeftTop;
			m_headerCell.anchorMin = posLeftTop;
			m_headerCell.anchorMax = posLeftTop;

			Vector2 sizeOfParent = rectTransform.sizeDelta;
			m_headerCell.anchoredPosition = new Vector2(0f, 0f);
			m_headerCell.sizeDelta = sizeOfParent;
		}
	}

	protected void _selectHeaderCell()
	{
		if (headerInfo != null)
			headerInfo.selecteCell();

		if (m_delegate != null)
			m_delegate.didSelectedHeaderCell(this, headerInfo);

		//_playOpenCloseAnimation();
	}

	protected void _deselectHeaderCell() 
	{
		if (headerInfo != null)
			headerInfo.deselecteCell();

		if (m_delegate != null)
			m_delegate.didDeSelectedHeaderCell(this, headerInfo);

// 		if (m_isOpenComboList == true)
// 			_playOpenCloseAnimation();
	}

	protected void _playOpenCloseAnimation()
	{
		resizingCells(m_resizingWithAnimationFlag);

		if (tween != null)
		{
			tween.enabled = true;
			tween.AddOnFinished(new EventDelegate(didFinishTweenAnimation));

			if (m_isOpenComboList == false)
				tween.Play();
			else
				tween.PlayReverse();

			m_isOpenComboList = m_isOpenComboList ? false : true;
		}
	}

	protected void didFinishTweenAnimation()
	{
		if (m_resizingDelegate != null)
			m_resizingDelegate.didStopResizingComboListView(this);
	}
	#endregion

	#region event Methods
	// default 셀을 클릭하였을때 호출함수
	virtual public void OnDefaultCellItemClick()
	{
	}

	// ==========================================
	// IListViewDelegate methods
	virtual public void reloadListView(ListView _listView)
	{

	}

	virtual public void didSelectedItem(ListView _listView, ListViewCell _cell)
	{
		if (_listView != null && _cell != null)
		{
			if (m_delegate != null)
				m_delegate.didSelectedCellInfo(this, _cell);
		}
	}

	virtual public void didUnSelectedItem(ListView _listView, ListViewCell _cell)
	{
		if (_listView != null && _cell != null)
		{
			if (m_delegate != null)
				m_delegate.didDeSelectedCellInfo(this, _cell);
		}
	}
	#endregion
}
