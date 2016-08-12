using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

using UnityEngine.UI;

public class DebugBoard : UIBehaviour
{
    public Button m_btnDebug = null;

    public RectTransform m_rectDebugBoard = null;


    public RectTransform m_rectPrizeBoard = null;


    protected override void Awake()
    {
        base.Awake();

        if (Debug.isDebugBuild)
        {
            m_btnDebug.gameObject.SetActive(true);
        }
        else
        {
            m_btnDebug.gameObject.SetActive(false);
        }

    }
    
    protected override void OnEnable()
    {
        base.OnEnable();

        m_rectDebugBoard.gameObject.SetActive(false);
        m_rectPrizeBoard.gameObject.SetActive(false);
    }

    public void OnDebugBtnClick()
    {
        m_rectDebugBoard.gameObject.SetActive(m_rectDebugBoard.gameObject.activeSelf ? false : true);
    }


    public void OnPrizeBtnClick()
    {
        m_rectPrizeBoard.gameObject.SetActive(true);
    }
}
