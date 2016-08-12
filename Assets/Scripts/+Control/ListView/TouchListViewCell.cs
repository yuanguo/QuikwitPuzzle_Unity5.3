using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TouchListViewCell : ListViewCell, IPointerClickHandler
{
	virtual public void OnPointerClick(PointerEventData eventData)
	{
		if (listView != null)
		{
			listView.selectCellAtRow(this);
		}
	}
}
