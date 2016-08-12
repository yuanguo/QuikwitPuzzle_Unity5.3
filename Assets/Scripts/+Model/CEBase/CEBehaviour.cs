using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using System.Collections;


public class CEBehaviour : UIBehaviour 
{
	private static GameObject s_defaultParent = null;

	public static GameObject defaultParent
	{
		get
		{
			if (s_defaultParent == null)
			{
				s_defaultParent = GameObject.Find("Canvas");
			}

			return s_defaultParent;
		}
	}


	public static GameData gameData
	{
		get
		{
			if (s_gameData == null)
				s_gameData = GameData.Singleton;

			return s_gameData;
		}
	}

    public static GameLogic gameLogic
    {
        get
        {
            if (s_gameLogic == null)
                s_gameLogic = GameLogic.Singleton;

            return s_gameLogic;
        }
    }


    private static GameLogic s_gameLogic = null;
	private static GameData s_gameData = null;

	private RectTransform m_rectTransform = null;

	public RectTransform rectTransform
	{
		get
		{
			if (m_rectTransform == null)
			{
				m_rectTransform = gameObject.GetComponent<RectTransform>();
			}

			return m_rectTransform;
		}
	}

	public virtual void OnEscape() { }

}
