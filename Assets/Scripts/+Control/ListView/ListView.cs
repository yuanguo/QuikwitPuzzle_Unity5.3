using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//using UnityEditor;

using System;
using System.Collections.Generic;

[Serializable]
public enum ListViewAlignType
{
	none = -1,
	Left = 0,
	Center = 1,
	Right = 2
}


[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(ScrollRect))]
[RequireComponent(typeof(Image))]
//[RequireComponent(typeof(Mask))]

public class ListView : MonoBehaviour, ListViewCellDelegate, IEventSystemHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	protected const string s_ContentViewName = "contentView";

	#region public members

    /// <summary>
    /// true : cell크기를 view alignment parameter(padding, spacing, view_count)에 의하여 결정한다.
    /// false : template의 크기를 그대로 리용한다.
    /// </summary>
	public bool m_fixingFullWidth = true; 

	public float durationOfSwipe
	{
		get
		{
			return m_durationOfSwipe;
		}
		set
		{
			m_durationOfSwipe = value;
		}
	}

	public float m_leftPadding = 0.0f;
	public float m_rightPadding = 0.0f;
	public float m_topPadding = 0.0f;
	public float m_bottomPadding = 0.0f;
	public float m_cellSpacing = 0.0f;
    public float m_cellHeight = 0.0f;
    /// <summary>
    /// false: 이미 선택된 row를 다시 누를때 원래 상태 유지, true: 상태를 반전시킨다.
    /// </summary>
	public bool m_selectedRowReverse = true; 
    /// <summary>
    /// true : list view 의 slide  기능을 disable한다.
    /// false : slide기능을 enable한다.
    /// </summary>
    public bool  m_noMoveFlag = false;

    /// <summary>
    ///  When one cell is selected ,if you need select event and no need deselect event, you make this true.
    ///  for using this m_selectedRowRe must be true;
    ///  true :  같은 cell을 반복 터치 할때 deselect사건을 보내지 않는다.
    /// </summary>
    public bool m_noNeedDeselect = false;

	public ListViewCell lastSelectedCell
	{
		get
		{
			return cellManager.lastSelectedCell;
		}
		set
		{
			cellManager.lastSelectedCell = value;
		}
	}

	public ListViewAlignType alignType
	{
		get
		{
			return m_alignType;
		}
		set
		{
			m_alignType = value;
		}
	}

	public List<RectTransform> templates
	{
		get
		{
            if (cellManager.getTemplates().Count == 0)
                cellManager.setTemplates(m_templates);

			m_templates = cellManager.getTemplates();
			return m_templates;
		}
/*
		set
		{
			if (templates != null)
			{
				if (value.Count == 0)
					return;

				cellManager.setTemplates(value);
				m_templates = cellManager.getTemplates();
			}
		}
*/
	}

	public IListViewDelegate m_delegate = null;

	public RectTransform rectOfTip = null;
	public bool showReloadTip
	{
		get
		{
			return m_showReloadTip;
		}
		set
		{
			if (rectOfTip != null)
				rectOfTip.gameObject.SetActive(value);
			m_showReloadTip = value;
		}
	}

	public RectTransform rectTransform
	{
		get
		{
			return scrollViewRect;
		}
	}

	public bool scrolling
	{
		get
		{
			return m_isScrolling;
		}
		set
		{
			if (value == false)
				stopScrolling();
			else
				resumeScrolling();
		}
	}

	public void SetSelectedRowReverse(bool value)
	{				
		m_selectedRowReverse = value;
	}


	public int cellCount
	{
		get
		{
			return cellManager.usingCells.Count;
		}
	}

	public int templateCellCount
	{
		get { return templates.Count; }
	}

	public bool isResizing	{
		get { return m_resizeFlag; }
		set { m_resizeFlag = value; }
	}

	#endregion public members

	#region private members
	[SerializeField]
	private float m_durationOfSwipe = 0.1f;
	private float m_timer = 0.0f;
	private bool m_isSwiping = false;
	private bool m_isSendReloadMessage = false;

	private bool m_isScrolling = true;

	[SerializeField]
	private List<RectTransform> m_templates = null;

	[SerializeField]
	protected ListViewAlignType m_alignType = ListViewAlignType.Left;

	[SerializeField]
	protected Scrollbar m_scrollBar = null;

	[SerializeField]
	protected bool m_isScrollBarShowControl = false;

    /// <summary>
    /// list view sliding 방향
    /// </summary>
	[SerializeField]
	protected bool m_isHorizontal = false;

	protected bool m_resizeFlag = false;
	protected Vector2 m_alignVecotr = Vector2.zero;

	protected ScrollRect m_scrollView = null;
	protected RectTransform m_scrollViewRect = null;
	protected RectTransform m_contentViewRect = null;
	protected ListViewCell m_lastSelectedCell = null;

	private ListViewCellManager m_cellManager = null;

	public ListViewCellManager cellManager
	{
		get
		{
			if (m_cellManager == null)
			{
				m_cellManager = contentViewRect.gameObject.GetComponent<ListViewCellManager>();

				if (m_cellManager == null)
					m_cellManager = contentViewRect.gameObject.AddComponent<ListViewCellManager>();

                m_cellManager.setTemplates(m_templates);
			}

			return m_cellManager;
		}
	}
	protected virtual RectTransform contentViewRect
	{
		get
		{
			if (m_contentViewRect == null)
			{
				Transform contentTransform = transform.FindChild(s_ContentViewName);
				GameObject content = null;

				if (contentTransform == null)
					content = new GameObject(s_ContentViewName);
				else
					content = contentTransform.gameObject;

				m_contentViewRect = content.GetComponent<RectTransform>();

				if (m_contentViewRect == null)
					m_contentViewRect = content.AddComponent<RectTransform>();

				if (m_contentViewRect == null)
				{
					Debug.LogError("don't have RectTransform");
					return null;
				}

				m_contentViewRect.SetParent(scrollViewRect.transform);
				m_contentViewRect.localPosition = Vector3.zero;
				m_contentViewRect.localRotation = Quaternion.identity;
				m_contentViewRect.localScale = Vector3.one;

				Vector2 value = new Vector2(0.0f, 1.0f);
				m_contentViewRect.pivot = value;
				m_contentViewRect.anchorMin = value;
				m_contentViewRect.anchorMax = value;
				m_contentViewRect.anchoredPosition = Vector2.zero;

				scrollView.content = m_contentViewRect;
			}
			return m_contentViewRect;
		}
        set
        {
            m_contentViewRect = value;
        }
	}
	protected RectTransform scrollViewRect
	{
		get
		{
			if (m_scrollViewRect == null)
			{
				m_scrollViewRect = gameObject.GetComponent<RectTransform>();

				if (m_scrollViewRect == null)
					m_scrollViewRect = gameObject.AddComponent<RectTransform>();

				if (m_scrollViewRect == null)
				{
					Debug.LogError("don't have RectTransform");
					return null;
				}
			}

			return m_scrollViewRect;
		}
	}

	protected ScrollRect scrollView
	{
		get
		{
			if (m_scrollView == null)
			{
				m_scrollView = gameObject.GetComponent<ScrollRect>();

				if (m_scrollView == null)
					m_scrollView = gameObject.AddComponent<ScrollRect>();

				if (m_scrollView == null)
				{
					Debug.LogError("don't have ScrollRect component");
					return null;
				}

				if (m_isHorizontal == true)
				{
					m_scrollView.vertical = false;
					m_scrollView.horizontal = true;
				}
				else
				{
					m_scrollView.vertical = true;
					m_scrollView.horizontal = false;
				}
				
				m_scrollView.elasticity = 0.1f;
				m_scrollView.movementType = ScrollRect.MovementType.Elastic;
				m_scrollView.inertia = true;
				m_scrollView.scrollSensitivity = 1.0f;
				//m_scrollView.onValueChanged.AddListener(new UnityAction<Vector2>)
			}

			return m_scrollView;
		}
	}

	protected Scrollbar scrollBar
	{
		get
		{
			if (m_scrollBar == null)
				return null;
			else
				return m_scrollBar;
		}
	}

	protected float heightOfScrollRect
	{
		get
		{
			return scrollViewRect.rect.height;
		}
	}
	protected float widthOfScrollRect
	{
		get
		{
			if (m_widthOfScrollRect == 0.0f && scrollViewRect.rect.width!= 0.0f)
				m_widthOfScrollRect = scrollViewRect.rect.width;

			return (scrollViewRect.rect.width == 0.0f ? m_widthOfScrollRect : scrollViewRect.rect.width);
		}
	}

	protected int m_cellCount = 0;

	private float m_widthOfScrollRect = 0.0f;

	private bool m_showReloadTip = false;
	

	protected virtual Vector2 alignVector
	{
		get
		{
			if (alignType == ListViewAlignType.Left)
				m_alignVecotr = new Vector2(0.0f, 1.0f);
			else if (alignType == ListViewAlignType.Center)
				m_alignVecotr = new Vector2(0.5f, 1.0f);
			else
				m_alignVecotr = new Vector2(1.0f, 1.0f);

			return m_alignVecotr;
		}
	}

	#endregion private members

	#region public create and get a cell, select

	virtual public void stopScrolling()
	{
		m_isScrolling = false;

		if (scrollView != null)
			scrollView.enabled = false;

		this.enabled = false;
	}

	virtual public void resumeScrolling()
	{
		m_isScrolling = true;

		if (scrollView != null)
			scrollView.enabled = true;

		this.enabled = true;
	}

    public void initListViewTemplates ()
    {
        cellManager.setTemplates(m_templates);

		Mask maskTemp = GetComponent<Mask>();
		
		if (maskTemp != null)
			maskTemp.showMaskGraphic = false;
    }

	public ListViewCell getCellAtRow(int _row, int _templateIdx = ListViewCell.s_TemplateIdxNone)
	{
		return findCell(_row, _templateIdx);
	}

	public virtual ListViewCell selectCellAtRow(int _row, int _templateIdx = ListViewCell.s_TemplateIdxNone)
	{
		ListViewCell cellInfo = findCell(_row, _templateIdx);

        
		selectCellInfo(cellInfo);

		return cellInfo;
	}

	public virtual bool selectCellAtRow(ListViewCell _cellInfo)
	{
        
		return selectCellInfo(_cellInfo);
	}

	public virtual ListViewCell selectCellByUserData(object _userData)
	{
		ListViewCell _cellInfo = findCellByUserData(_userData);

		if (_cellInfo != null)
			selectCellInfo(_cellInfo);

		return lastSelectedCell;
	}

	protected virtual bool selectCellInfo(ListViewCell _cellInfo)
	{
		if (_cellInfo == null)
			return false;

		if (findCell(_cellInfo) == false)
			return false;

		if (!m_selectedRowReverse &&
			lastSelectedCell == _cellInfo)
			return true;

		if (lastSelectedCell != null)
		{
			lastSelectedCell.OnDeselected();

			if (m_delegate != null)
				m_delegate.didUnSelectedItem(this, lastSelectedCell);
		}

        if (m_noNeedDeselect || lastSelectedCell != _cellInfo)
		{
			_cellInfo.OnSelected();
			lastSelectedCell = _cellInfo;
			if (m_delegate != null)
				m_delegate.didSelectedItem(this, lastSelectedCell);
		}
		else
			lastSelectedCell = null;
		return true;
	}

	public void unselectCell()
	{
		if (lastSelectedCell != null)
		{
			lastSelectedCell.m_selected = false;
			lastSelectedCell.OnDeselected();

			if (m_delegate != null)
				m_delegate.didUnSelectedItem(this, lastSelectedCell);

			lastSelectedCell = null;
		}
	}

	public void setTemplates(List<RectTransform> _templates)
	{
		cellManager.setTemplates(_templates);
	}

	public virtual void gotoCellInfo(int _row, int _templateIdx = ListViewCell.s_TemplateIdxNone)
	{
		ListViewCell cellInfo = findCell(_row, _templateIdx);

		if (cellInfo == null)
			return;
	}

	public virtual void gotoCellInfo(ListViewCell _cellInfo)
	{

	}

    public virtual void updateCells()
    {
        
    }
	#endregion public create....

	#region private find, create
	public ListViewCell findCell(int _row, int _templateIdx = ListViewCell.s_TemplateIdxNone)
	{
		ListViewCell cellInfo = cellManager.getCellFromUsingCellsTable(_row, _templateIdx);

		return cellInfo;
	}

	bool findCell(ListViewCell _cellInfo)
	{
		return cellManager.isExistingCell(_cellInfo);
	}

	public virtual ListViewCell findCellByUserData(object _userData)
	{
		foreach (ListViewCell cellInfo in cellManager.usingCells)
		{
			if ((cellInfo.userData == _userData ||
				cellInfo.userData.ToString().Equals(_userData.ToString())) &&
				cellInfo.userData.GetHashCode() == _userData.GetHashCode())
				return cellInfo;
		}
		return null;
	}


	protected ListViewCell createCell(int _row, int _templateIdx = ListViewCell.s_TemplateIdxNone)
	{
		ListViewCell cellInfo = cellManager.createCell(_row, _templateIdx, true);

		if (cellInfo == null)
			return null;

		cellInfo.gameObject.SetActive(true);

        return cellInfo;
	}
	#endregion find create


	#region public  insert and remove cell (manage cells)
	/**
	 * insertCellAtRow :
	 *		insert a cell at row by using template idx
	 *	param1 (_row) : cell of position
	 *	param2 (_templateIdx) : template's index
	 *	param3 [_insertFlag] : insert flag (true : replace false : add a cell at last position)
	 * */
	public ListViewCell insertCellAtRow(int _row, int _templateIdx, bool _replace = false)
	{
		if (_row < 0)
			return null;

		if (_templateIdx <= ListViewCell.s_TemplateIdxNone)
			return null;

		ListViewCell cellInfo = null;

		if (_replace == true)
			cellInfo = findCell(_row, _templateIdx);

		if (cellInfo == null)
			cellInfo = createCell(_row, _templateIdx);

		cellInfo.gameObject.SetActive(true);

		resizingCells();
		return cellInfo;
	}

    public virtual ListViewCell insertAtLastRow(int _templateIdx, object _userData = null)
    {
        if (_templateIdx <= ListViewCell.s_TemplateIdxNone)
            return null;

        ListViewCell cellInfo = null;

        cellInfo = createCell(cellManager.usingCells.Count, _templateIdx);

		if (cellInfo == null)
			return null;

		cellInfo.userData = _userData;
		cellInfo.InitCell();

		resizingCells();
        return cellInfo;
    }

	public bool insertCellAtLastRow(ListViewCell _cellInfo)
	{
		int templateIdx = ListViewCell.s_TemplateIdxNone;

		templateIdx = _cellInfo.m_templateIdx;

		if (templateIdx <= ListViewCell.s_TemplateIdxNone)
			return false;

		resizingCells();
		return cellManager.insertCellInfoToUsingTable(_cellInfo);
	}

	public int GetCount()
	{
		return cellManager.usingCells.Count;
	}

	public ListViewCell removeCellAtRow(int _row)
	{
		// 우선 섹션을 삭제하고 섹션 밑의 row들을 모두 삭제한다.
		// 현섹션에 row가 하나이면서 row = 0일때
		if (_row < 0)
			return null;

		ListViewCell cellInfo = cellManager.addCellIntoUnusedTable(_row);

		resizingCells();
		return cellInfo;
	}

	public bool removeCellAtRow(ListViewCell _cellInfo)
	{
		if (_cellInfo == null)
			return false;

		cellManager.addCellIntoUnusedTable(_cellInfo);
		resizingCells();
		return true;
	}

	public virtual void clearCells()
	{
		cellManager.addAllCellIntoUnusedTable();
		unselectCell();
		resizingCells();

		lastSelectedCell = null;
	}

	public void reloadData()
	{
		// call table data source delegate methods
        changeSizeOfTransform(ref m_contentViewRect, new Vector2(widthOfScrollRect, 0.0f), false);

		resizingCells();
	}
    bool m_Value = true;
	virtual public void resizingCells()
	{
		float heightOfContentView = -m_topPadding;

		foreach (ListViewCell cellInfo in cellManager.usingCells)
		{
			RectTransform rectOfCell = cellInfo.rectTransform;
			float heightOfCell = rectOfCell.rect.height;

            if (m_Value == true)
            {
                m_cellHeight = heightOfCell;
                m_Value = false;
            }
			
            float widthOfCell = 0.0f;

			if (m_fixingFullWidth == true)
				widthOfCell = Math.Abs(widthOfScrollRect) - m_leftPadding - m_rightPadding;
			else
				widthOfCell = Math.Abs(rectOfCell.rect.width);

			if (alignType == ListViewAlignType.Center)
				changePositionOfTransform(ref rectOfCell, new Vector2((m_leftPadding - m_rightPadding) / 2.0f, heightOfContentView));
			else if (alignType == ListViewAlignType.Left)
				changePositionOfTransform(ref rectOfCell, new Vector2(m_leftPadding, heightOfContentView));
			else if (alignType == ListViewAlignType.Right)
				changePositionOfTransform(ref rectOfCell, new Vector2(- m_rightPadding, heightOfContentView));
			else
				changePositionOfTransform(ref rectOfCell, new Vector2(m_leftPadding, heightOfContentView));

			changeSizeOfTransform(ref rectOfCell,
				new Vector2(widthOfCell, Math.Abs(heightOfCell)));

			heightOfContentView -= (heightOfCell + m_cellSpacing);
		}
		heightOfContentView -= m_bottomPadding;

		changeSizeOfTransform(ref m_contentViewRect, new Vector2(Math.Abs(widthOfScrollRect), Math.Abs(heightOfContentView + m_cellSpacing)), false);

		// scrollbar showing control
		ControlScrollBar();
    }

	public bool isSelectedCell(ListViewCell _cell)
	{
		if (_cell != null)
			return (_cell.m_selected);

		return false;
	}

	protected void ControlScrollBar()
	{
		if (m_isScrollBarShowControl == true)
		{
			if (m_scrollBar != null)
			{
				if (scrollView.verticalScrollbar != null)
				{
					if (contentViewRect.rect.height > heightOfScrollRect)
						m_scrollBar.gameObject.SetActive(true);
					else
						m_scrollBar.gameObject.SetActive(false);
				}

				if (scrollView.horizontalScrollbar != null)
				{
					if (contentViewRect.rect.width > widthOfScrollRect)
						m_scrollBar.gameObject.SetActive(true);
					else
						m_scrollBar.gameObject.SetActive(false);
				}
			}
		}
	}
	#endregion (manage cells)

	#region override methods
// #if UNITY_EDITOR
// 	void OnValidate()
// 	{
// 		if (scrollView == null ||
// 			scrollViewRect == null ||
// 			contentViewRect == null)
// 			return;
// 
// 		cellManager.templates = m_templates;
// 		clearCells();
// 	}
// #endif


	void Start()
	{
        _initialize();
	}
	void OnEnable()
	{
        _initialize();
	}

	void Update()
	{
		
	}

	void LateUpdate()
	{
		if (m_resizeFlag == true)
            resizingCells();
		if (lastSelectedCell != null && m_resizeFlag == true)
        {
            if (lastSelectedCell.m_selected == true)
                MoveContentView(lastSelectedCell);
        }
	}

	void FixedUpdate()
	{
		if (m_isSwiping == true)
		{
			m_timer += Time.fixedDeltaTime;

			if (m_timer >= durationOfSwipe)
			{
				if (m_delegate != null && m_isSendReloadMessage == false)
				{
					m_delegate.reloadListView(this);
					m_isSendReloadMessage = true; ;
				}

				m_isSwiping = false;
				m_timer = 0;
			}
		}

	}

    void MoveContentView(ListViewCell _cellInfo)
    {
        if (m_noMoveFlag == true)
            return;
        
        if (_cellInfo == null)
            return;

        TweenHeight tween = _cellInfo.GetComponent<TweenHeight>();

        if (tween == null)
            return;
		float heightSum = Math.Abs(contentViewRect.anchoredPosition.y) + heightOfScrollRect;
        float heightDisplay = (m_cellHeight + m_cellSpacing) * _cellInfo.index;
       
        if (heightSum < (heightDisplay + tween.m_To))
        {
            //if (contentViewRect.childCount == (_cellInfo.index + 1))
            //    scrollView.verticalNormalizedPosition = 1f;
            //else if (heightOfScrollRect < (contentViewRect.rect.height - Math.Abs(contentViewRect.anchoredPosition.y)))
				contentViewRect.anchoredPosition = new Vector2(contentViewRect.anchoredPosition.x, Math.Abs(contentViewRect.anchoredPosition.y) + (tween.m_To - (heightSum - heightDisplay)));
        }
    }
	#endregion override methods

	#region methods to change transform
	protected void changePositionOfTransform(ref RectTransform _trsfParam, Vector2 _pos, bool _changePivot = true)
	{
		if (_changePivot == true)
		{
			_trsfParam.pivot = alignVector;
			_trsfParam.anchorMax = alignVector;
			_trsfParam.anchorMin = alignVector;
		}
		_trsfParam.anchoredPosition = _pos;
	}

	protected void changeOffsetPositionOfTransform(ref RectTransform _trsfParam, Vector2 _offsetPos, bool _changePivot = true)
	{
		if (_changePivot == true)
		{
			_trsfParam.pivot = alignVector;
			_trsfParam.anchorMax = alignVector;
			_trsfParam.anchorMin = alignVector;
		}
		_trsfParam.anchoredPosition += _offsetPos;
	}

	protected void changeSizeOfTransform(ref RectTransform _trsfParam, Vector2 _offsetSize, bool _changePivot = true)
	{
		if (_changePivot == true)
		{
			_trsfParam.pivot = alignVector;
			_trsfParam.anchorMax = alignVector;
			_trsfParam.anchorMin = alignVector;
		}
		_trsfParam.sizeDelta = _offsetSize;
	}

	protected void changeOffsetSizeOfTransform(ref RectTransform _trsfParam, Vector2 _offsetSize, bool _changePivot = true)
	{
		if (_changePivot == true)
		{
			_trsfParam.pivot = alignVector;
			_trsfParam.anchorMax = alignVector;
			_trsfParam.anchorMin = alignVector;
		}
		_trsfParam.sizeDelta += _offsetSize;
	}

    protected virtual void _initialize()
    {
        if (rectOfTip != null)
            rectOfTip.gameObject.SetActive(false);

		m_resizeFlag = false;

        initListViewTemplates();
    }
	#endregion

	#region ListViewCellDelegate methods
	public void didSelectedCell(ListViewCell _cellInfo)
	{
       
	}
	public void beginResizing(ListViewCell _cellInfo)
	{
		m_resizeFlag = true;
	}
	public void endResizing(ListViewCell _cellInfo)
	{
		m_resizeFlag = false;
	}
	#endregion

	#region DragInterface methods
	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		m_timer = 0;
		m_isSwiping = false;

		m_isSendReloadMessage = false;
	}

	public virtual void OnDrag(PointerEventData eventData)
	{
		if (scrollView.verticalNormalizedPosition <= 0)
			m_isSwiping = true;
		else
			m_isSwiping = false;
	}

	public virtual void OnEndDrag(PointerEventData eventData)
	{
		m_timer = 0;
		m_isSwiping = false;

		m_isSendReloadMessage = false;
	}

	#endregion

	#region show / hide reload tip methods
	protected virtual void showReloadTipe()
	{

	}
	#endregion
}



