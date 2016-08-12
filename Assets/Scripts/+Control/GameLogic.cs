using UnityEngine;
using System.Collections;
using System;

using System.Collections.Generic;

public delegate void didStopQuestionTime();
public delegate void NotifyLifeRemainTime(int second);
public delegate void IncreaseLife();

public class GameLogic : CEBehaviour 
{
    public enum GameState { None, Login, Register, SelCh, SetUp, Setting, World, Level, Game, PowerUp, MoreLife, Shop, }

	public static int[] m_audienceProbailities = { 10, 20, 30, 40 };

    public GameObject m_Login = null;
    public GameObject m_Register = null;
    public GameObject m_SelectCh = null;
    public GameObject m_Setup = null;
    public GameObject m_Setting = null;
    public GameObject m_GamePlay = null;
    public GameObject m_PowerUp = null;
    public GameObject m_World = null;
    public GameObject m_MoreLife = null;
    public GameObject m_Level = null;
    public GameObject m_Shop = null;
    public MessageBox m_MsgBox = null;

	public ScreenLogin m_loginScreen = null;
	public ScreenWinnerPopup m_winnerPopup = null;
	public ScreenAllWorlds m_allWorlds = null;
	public ScreenAchivement m_archivement = null;
	public ScreenWitBank m_witBank = null;

    public GameObject m_ManPrefabe = null;
    public GameObject m_WomenPrefabe = null;

    public AudioClip m_MainBgClip = null;
	public List<AudioClip> m_audioClips = new List<AudioClip>();
    public List<AudioClip> m_chAudioClips = new List<AudioClip>();
    public List<AudioClip> m_PowerUpClips = new List<AudioClip>();
    public AudioClip m_WalkingClip = null;
    public AudioClip m_BonusClip = null;

	public AudioSource m_audioSourceForBG = null;
    public AudioSource m_chSource = null;

	public didStopQuestionTime didStopTimer = null;

	public float updateDeltaTime = 1.0f;

	public AudioClip m_winnerSoundClip = null;

    private bool m_MusicOn = true;
    private bool m_SfxOn = true;
    private bool m_Vibration = false;

    private bool m_Escape = false;
    private GameObject m_ManCharacter = null;
    private GameObject m_WomenCharacter = null;

	// FOR Hour Glass Power item.
	private const float m_defaultDeltaTime = 1f;

	//Dictionary<string, Sprite> m_loadedObjectSpites = new Dictionary<string, Sprite>();


	private System.Random m_rndSystem = new System.Random();
    public bool EscapeKey
    {
        get { return m_Escape; }
        set { m_Escape = value; }
    }

    public GameObject Character {
        get { /*return gameData.userInfo.isMale ? m_ManCharacter : m_WomenCharacter;*/ 
			if (gameData.userInfo.isMale)
			{
				m_WomenCharacter.SetActive(false);
				return m_ManCharacter;
			}

			m_ManCharacter.SetActive(false);
			return m_WomenCharacter;
		}
    }

    private static GameLogic m_Instance = null;
    public static GameLogic Singleton
    {
        get
        {
            if (m_Instance == null)
                m_Instance = GameObject.FindObjectOfType(typeof(GameLogic)) as GameLogic;

            return m_Instance;
        }
    }

	public void resetUpdateTimer ()
	{
		GameData.curTime = GameData.totalTimeForQuestion;
		updateDeltaTime = m_defaultDeltaTime;
        stopUpdateTimer();
	}

	public void resetUpdateDeltaTime(float _deltatTime)
	{
		stopUpdateTimer();
		updateDeltaTime = _deltatTime;
		beginUpdateTimer();
	}

	public void beginUpdateTimer()
	{
		if (IsInvoking("updateTimer") == false)
			InvokeRepeating("updateTimer", 0.1f, updateDeltaTime);
	}

	public void stopUpdateTimer()
	{
		if (IsInvoking("updateTimer") == true)
			CancelInvoke("updateTimer");
	}

	private void updateTimer()
	{
		GameData.curTime --;

		if (GameData.curTime <= 0)
		{
			GameData.curTime = 0;
			updateDeltaTime = m_defaultDeltaTime;
			stopUpdateTimer();

			if (didStopTimer != null)
				didStopTimer();
		}
	}


    private GameState m_State = GameState.None;
    #region Override MonoBehaviour
    protected override void OnEnable()
    {
		base.OnEnable();

        DisableUI();
        if (m_Login)
            m_Login.transform.parent.gameObject.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            Escape();		

		// update user's life
		if (UserInfo.wasLogined == true)
			UserInfo.defaultUser().updateUserLife();
	}
    #endregion Override MonoBehaviour
    #region Public Function
    public void ChangeGameState(GameState _state)
    {
        DisableUI();

		if (m_archivement != null)
			m_archivement.closeScreen();

		if (m_witBank != null)
			m_witBank.closeScreen ();

		if (m_allWorlds != null)
			m_allWorlds.closeScreen();

		if (m_winnerPopup != null)
			m_winnerPopup.ClosePopup();

        if (_state == GameState.Login && m_Login)
            m_Login.SetActive(true);
        else if (_state == GameState.Register && m_Register)
            m_Register.SetActive(true);
        else if (_state == GameState.SelCh && m_SelectCh)
            m_SelectCh.SetActive(true);
        else if (_state == GameState.SetUp && m_Setup)
        {
            m_Setup.SetActive(true);
            m_audioSourceForBG.clip = m_MainBgClip;
            m_audioSourceForBG.Play();
        }
        else if (_state == GameState.Setting && m_Setting)
            m_Setting.SetActive(true);
        else if (_state == GameState.Game && m_GamePlay)
        {
			ScreenGame gamePlay = m_GamePlay.GetComponent<ScreenGame>();
			gamePlay.isAlreadyGiveAnswered = false;
            m_GamePlay.SetActive(true);
        }
        else if (_state == GameState.PowerUp && m_PowerUp)
            m_PowerUp.SetActive(true);
        else if (_state == GameState.World && m_World)
        {
            m_World.SetActive(true);
            m_audioSourceForBG.Stop();
            m_audioSourceForBG.clip = m_audioClips[gameData.curWorldInfo.worldIDX];
            m_audioSourceForBG.Play();
        }
        else if (_state == GameState.Level && m_Level)
            m_Level.SetActive(true);
        else if (_state == GameState.Shop && m_Shop)
            m_Shop.SetActive(true);
        else if (_state == GameState.MoreLife && m_MoreLife)
        {
            m_MoreLife.SetActive(true);
            m_audioSourceForBG.Stop();
            m_audioSourceForBG.clip = m_MainBgClip;
            m_audioSourceForBG.Play();
        }

        State = _state;
    }
    public bool IsInGameState(GameState _state)
    {
        return State == _state;
    }
    public void ErrorBox(string msg)
    {
        if (!m_MsgBox)
            return;

        m_MsgBox.Msg = msg;
		m_MsgBox.rectTransform.SetAsLastSibling();
        m_MsgBox.gameObject.SetActive(true);
    }

	public List<int> makeProbabilities(int _startIdx, int _maxIdx, int _exceptionIdx = -1, bool _canInclude = false)
	{
		List<int> nLists = new List<int>();

		// make array 
		for (int nIdx = _startIdx; nIdx < _maxIdx; nIdx++)
			nLists.Add(nIdx);

		List<int> nResultList = new List<int>();

		if (_exceptionIdx != -1 && _canInclude == true)
			nResultList.Add(_exceptionIdx);

		while (nLists.Count != 0)
		{
			int nRandom = m_rndSystem.Next(_maxIdx);

			if (nRandom >=_startIdx &&
				nRandom <= _maxIdx)
			{
				if (nRandom == _exceptionIdx)
				{
					if (nResultList.Contains(_exceptionIdx) == false)
						nLists.Remove(_exceptionIdx);
				}
				else
				{
					if (nResultList.Contains(nRandom) == false)
					{
						nResultList.Add(nRandom);
						nLists.Remove(nRandom);
					}
				}
			}
		}

		return nResultList;
	}

	System.Random randomSystem = new System.Random();
	public int rand(int _idxStart, int _idxEnd)
	{
		int val = randomSystem.Next(_idxStart, _idxEnd);

		if (val <= _idxStart || val >= _idxEnd)
			val = rand(_idxStart, _idxEnd);

		return val;
	}

	public void playWinnerSound ()
	{
		m_chSource.clip = m_winnerSoundClip;
		m_chSource.loop = false;
		m_chSource.Play();
	}
    public void SetWorldSound(int worldNum)
    {
        if (IsInGameState(GameState.World) && worldNum >= 0 && worldNum < 8)
        {
            m_audioSourceForBG.Stop();
            m_audioSourceForBG.clip = m_audioClips[worldNum];
            m_audioSourceForBG.Play();
        }
    }
    public void PlaySound(bool _male, bool _yes)
    {
        if (!m_chSource)
            return;

        int index = (int)(_male ? 0 : 2) + (int)(_yes ? 0 : 1);
        m_chSource.clip = m_chAudioClips[index];
        m_chSource.Play();
        if (m_Vibration && _yes)
            Handheld.Vibrate();
    }
    public void PlayPowerUpSound(ItemKinds _kind)
    {
        if (!m_chSource)
            return;

        m_chSource.clip = m_PowerUpClips[(int)_kind];
        m_chSource.Play();
        if (m_Vibration)
            Handheld.Vibrate();
    }
    public void PlayWalkingSound(bool _play)
    {
        if (!m_chSource)
            return;

        if (_play)
        {
            m_chSource.clip = m_WalkingClip;
            m_chSource.loop = true;
            m_chSource.Play();
            //if (m_Vibration)
            //    Handheld.Vibrate();
        }
        else
        {
            m_chSource.loop = false;
            m_chSource.Stop();
        }
    }
    public void PlayBonusSound()
    {
        if (!m_chSource)
            return;

        m_chSource.clip = m_BonusClip;
        m_chSource.Play();
        if (m_Vibration)
            Handheld.Vibrate();
    }
	public void IncreaseLife()
	{
	}
    #endregion Public Function
    #region Private Function
    private void DisableUI()
    {
        if (m_Login)
            m_Login.SetActive(false);

        if (m_Register)
            m_Register.SetActive(false);

        if (m_SelectCh)
            m_SelectCh.SetActive(false);

        if (m_Setup)
            m_Setup.SetActive(false);

        if (m_Setting)
            m_Setting.SetActive(false);

        if (m_GamePlay)
            m_GamePlay.SetActive(false);

        if (m_PowerUp)
            m_PowerUp.SetActive(false);

        if (m_World)
            m_World.SetActive(false);

        if (m_Level)
            m_Level.SetActive(false);

        if (m_MoreLife)
            m_MoreLife.SetActive(false);

        if (m_Shop)
            m_Shop.SetActive(false);
    }
    private void Escape()
    {
        if (m_MsgBox.gameObject.activeSelf)
            return;

		if (CELoadingBar.isPlayingLoadingAni == true)
		{
			CELoadingBar.stopLoadingAnimation();
			return;
		}

		if (m_allWorlds != null)
			m_allWorlds.closeScreen();

		if (m_archivement != null)
			m_archivement.closeScreen();

        if (State == GameState.Login)
            Application.Quit();
        if (State == GameState.Register || State == GameState.SetUp)
            ChangeGameState(GameState.Login);
        else if (State == GameState.Setting || State == GameState.MoreLife
            || State == GameState.World)
            ChangeGameState(GameState.SetUp);
        else if (State == GameState.Level)
            ChangeGameState(GameState.World);
        else if (State == GameState.PowerUp)
            ChangeGameState(GameState.Game);
        else if (State == GameState.Game)
        {
			ScreenGame screenGame = m_GamePlay.GetComponent<ScreenGame>();
			screenGame.OnEscape();
            ChangeGameState(GameState.Level);
            EscapeKey = true;
            gameData.userInfo.Lifes--;

			gameData.curWorldInfo.nextQuestion();

            if (gameData.userInfo.Lifes < 0)
                gameData.userInfo.Lifes = 0;


        }
        else if (State == GameState.Shop)
            ChangeGameState(GameState.Setting);
    }

    protected override void Awake()
    {
		base.Awake();
		Handheld.PlayFullScreenMovie("LOGO_720.mp4", Color.black, FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.AspectFit);

        m_ManCharacter = Instantiate(m_ManPrefabe) as GameObject;
        m_WomenCharacter = Instantiate(m_WomenPrefabe) as GameObject;
        ChangeGameState(GameState.Login);
    }
    protected override void OnDestroy()
    {
        gameData.userInfo.saveUserInfo();
    }
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus == true)
        {
            gameData.userInfo.saveUserInfo();
			gameData.curWorldInfo.saveData();
            WorldInfo.UpdatePassedWorldId((WorldIdentifier)WorldInfo.curWorlNum);
        }
        else
        {
            gameData.userInfo.loadUserInfo();
		
			if (m_loginScreen != null)
				m_loginScreen.updateData();


		}
    }
    #endregion Private Function
    #region Property
    public GameState State
    {
        get { return m_State; }
        set { m_State = value; }
    }
    public bool MusicOn
    {
        get { return m_MusicOn; }
        set
        {
            m_MusicOn = value;
            if (m_audioSourceForBG)
            {
                m_audioSourceForBG.volume = m_MusicOn ? 1.0f : 0.0f;
                //m_audioSourceForBG.Play();

            }
        }
    }
    public bool SfxOn
    {
        get { return m_SfxOn; }
        set
        {
            m_SfxOn = value;
            if (m_chSource)
            {
                m_chSource.volume = m_SfxOn ? 1.0f : 0.0f;
                //m_chSource.Play();
            }
        }
    }
    public bool Vibration
    {
        get { return m_Vibration; }
        set 
        { 
            m_Vibration = value;
            if (m_Vibration)
                Handheld.Vibrate();
        }
    }
    #endregion Property
}


