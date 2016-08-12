using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

public class ScreenMoreLife : CEBehaviour 
{
    public GameObject m_NoLifeObj = null;
    public GameObject m_LifeObj = null;
    public Text m_RemainTime = null;
    public List<Image> m_HeartLifes = new List<Image>();

    public ScreenShop m_screenShop = null;

	public Transform m_chPos = null;

	private GameObject chObject = null;
	private Player m_player = null;
	// Use this for initialization

    protected override void OnEnable()
    {
		base.OnEnable();

		InitCtrls();

		if (gameLogic.Character != null)
		{
			chObject = gameLogic.Character;
			chObject.transform.SetParent(m_chPos);
			chObject.transform.localPosition = Vector3.zero;
			chObject.transform.localRotation = Quaternion.identity;
			chObject.transform.localScale = Vector3.one;

			chObject.SetActive(true);
		}

		if (chObject != null)
		{
			m_player = chObject.GetComponent<Player>();
			if (m_player != null)
			{
				m_player.m_delegate = null;
				m_player.PlayIdleAnimation();
			}

			if (IsInvoking("_updateAnimation") != true)
				InvokeRepeating("_updateAnimation", 1f, 8f);
		}
    }

	protected override void OnDisable()
	{
		if (IsInvoking("_updateAnimation") == true)
			CancelInvoke("_updateAnimation");

		base.OnDisable();
	}

    private void InitCtrls()
    {
        m_NoLifeObj.SetActive(gameData.userInfo.Lifes == 0);
        m_LifeObj.SetActive(gameData.userInfo.Lifes > 0);
        for (int i = 0; i < m_HeartLifes.Count; ++i)
            m_HeartLifes[i].gameObject.SetActive(i < gameData.userInfo.Lifes ? true : false);

    }

	private void _updateAnimation()
	{
		m_player = chObject.GetComponent<Player>();
		if (m_player != null)
		{
			m_player.m_delegate = null;
			m_player.PlayShrugAnimation();
		}
	}

    public void Buy()
    {
        if (gameLogic.IsInGameState(GameLogic.GameState.MoreLife))
            gameLogic.ChangeGameState(GameLogic.GameState.Shop);
    }

	public void OnBackBtnClick()
	{
		gameLogic.ChangeGameState (GameLogic.GameState.SetUp);
	}

	void Update()
	{
		m_NoLifeObj.SetActive(gameData.userInfo.Lifes == 0);
		m_LifeObj.SetActive(gameData.userInfo.Lifes > 0);
		for (int i = 0; i < m_HeartLifes.Count; ++i)
			m_HeartLifes[i].gameObject.SetActive(i < gameData.userInfo.Lifes ? true : false);

		// update time
		UserInfo userinfo = UserInfo.defaultUser();

		DateTime nextLifeTime = userinfo.m_lifeStartedTime.AddSeconds(GameData.MaxLifeTime);
		TimeSpan tempTimeSpan = nextLifeTime - DateTime.Now;
		m_RemainTime.text = string.Format("{0} : {1}", tempTimeSpan.Minutes.ToString("00"), tempTimeSpan.Seconds.ToString("00"));
	}

}
