using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class ComboHeaderCell : TouchListViewCell
{
	#region UI members
	[SerializeField] private ListView m_listView = null;

	[SerializeField] Image m_imgArrow = null;

	#endregion

	#region override Methods
	public override void OnSelected()
	{
		base.OnSelected();
		listView.beginResizing(this);

		if (m_imgArrow != null)
			m_imgArrow.rectTransform.Rotate(0, 0, -90);

		if (m_listView != null &&
			m_listView.lastSelectedCell != null)
		{
		}
	}

	public override void OnDeselected()
	{
		base.OnDeselected();
		listView.beginResizing(this);
		
		if (m_imgArrow != null)
			m_imgArrow.rectTransform.Rotate(0, 0, 90);
		
		if (m_listView != null &&
			m_listView.lastSelectedCell != null)
		{
		}
	}
	#endregion

	#region Tween Animation Delegate Methods
	public void didFinishAnimation()
	{
		listView.endResizing(this);
	}
	#endregion
}
