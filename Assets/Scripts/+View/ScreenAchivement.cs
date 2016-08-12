using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScreenAchivement : CEBehaviour
{
	public GridLayoutGroup m_gridView = null;
	public RectTransform m_caption = null;

	public List<RectTransform> m_worldItems = new List<RectTransform>();


	protected override void OnEnable ()
	{
		base.OnEnable ();
		return;
		/*
		m_caption.gameObject.SetActive (true);
		for (int idx = 0; idx < m_worldItems.Count; idx++) {
			m_worldItems [idx].SetParent (null);
			m_worldItems [idx].gameObject.SetActive(false);
		}

		for (int curWorldIdx = 0; curWorldIdx < 8; curWorldIdx++) {
			WorldIdentifier idTemp = (WorldIdentifier)(0x01 << curWorldIdx);
			WorldIdentifier curIdentifier = WorldInfo.PassedWorlds & idTemp;

			if (idTemp == curIdentifier) {
				m_caption.gameObject.SetActive (false);
				m_worldItems [curWorldIdx].SetParent (m_gridView.transform);
				m_worldItems [curWorldIdx].gameObject.SetActive(true);
			}
		}
		*/
	}

    public void openScreen()
    {
        rectTransform.SetAsLastSibling();
        gameObject.SetActive(true);
    }

    public void closeScreen()
    {
        rectTransform.SetAsFirstSibling();
        gameObject.SetActive(false);
    }

    public void OnCloseBtnClick()
    {
        closeScreen();
    }
}
