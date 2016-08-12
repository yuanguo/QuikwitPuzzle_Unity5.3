using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScreenAllWorlds : CEBehaviour
{
	public List<RectTransform> m_worldCheckMark = new List<RectTransform>();

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
    public void OnHelp()
    {

    }

	protected override void OnEnable ()
	{
		base.OnEnable ();

		for (int idx = 0; idx < m_worldCheckMark.Count; idx++) {
			m_worldCheckMark [idx].gameObject.SetActive(false);
		}

		for (int curWorldIdx = 0; curWorldIdx < 8; curWorldIdx++) {
			WorldIdentifier curIdentifier = (WorldIdentifier)(curWorldIdx + 1);

			if (curIdentifier <= WorldInfo.PassedWorlds)
				m_worldCheckMark [curWorldIdx].gameObject.SetActive(true);
		}

	}

}
