using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScreenSetting : CEBehaviour
{
    public Toggle m_MusicOn = null;
    public Toggle m_SfxOn = null;
    public Toggle m_Vibration = null;
    public List<Image> m_HeartLifes = new List<Image>();

	public ScreenPowerUps m_scrPowerUps = null;
    public ScreenAllWorlds m_allWorldsScreen = null;
    public ScreenAchivement m_achivementScreen = null;
	public ScreenWitBank m_witBank = null;

    //private Color selColor = new Color(0.02f, 0.25f, 0.4f);
    protected override void OnEnable()
    {
        for (int i = 0; i < m_HeartLifes.Count; ++i)
            m_HeartLifes[i].gameObject.SetActive(i < gameData.userInfo.Lifes ? true : false);

        m_Vibration.isOn = false; 
        gameLogic.Vibration = false;
    }

    public void OnShop()
    {
        if (gameLogic.IsInGameState(GameLogic.GameState.Setting))
            gameLogic.ChangeGameState(GameLogic.GameState.Shop);
		
    }

    public void OnArchievement()
    {
        if (m_scrPowerUps != null)
            m_scrPowerUps.closeItemWnd();

        if (m_allWorldsScreen != null)
            m_allWorldsScreen.closeScreen();

        if (m_achivementScreen != null)
            m_achivementScreen.openScreen();
    }

    public void OnWorlds()
    {
        if (m_scrPowerUps != null)
            m_scrPowerUps.closeItemWnd();

        if (m_achivementScreen != null)
            m_achivementScreen.closeScreen();

        if (m_allWorldsScreen != null)
            m_allWorldsScreen.openScreen();
    }

    public void OnMusicOn()
    {
        gameLogic.MusicOn = m_MusicOn.isOn;
    }

    public void OnSFX()
    {
        gameLogic.SfxOn = m_SfxOn.isOn;
    }

    public void OnVibration()
    {
        gameLogic.Vibration = m_Vibration.isOn;
    }

	public void OnGotoHome()
	{
		gameLogic.ChangeGameState(GameLogic.GameState.SetUp);
	}
    public void OnPowerUps()
    {
		if (m_scrPowerUps != null)
		{
		    m_scrPowerUps.haveAction = false;
		    m_scrPowerUps.openItemWnd();
		}
    }

    public void OnHelp()
    {

    }

    public void OnWitbank()
    {
		if (m_witBank != null)
			m_witBank.openScreen ();
    }

    public void OnInstargram()
    {
		Application.OpenURL("https://www.instagram.com/quikwit_trivia/");
	}

    public void OnTwitter()
    {
		Application.OpenURL("https://twitter.com/quikwit_trivia?s=02");
	}

    public void OnFacebook()
    {
		Application.OpenURL("http://www.facebook.com/quikwitapp");
    }

	public void OnLogOffBtnClick()
	{
		gameLogic.ChangeGameState(GameLogic.GameState.Login);
		UserInfo.wasLogined = false;
	}
}
