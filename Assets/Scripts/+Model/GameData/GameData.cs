using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public class GameData : CEObject, IPuzzleNetLogicDelegates
{
	public bool autoLogin = false;

    public static bool Male = true;
    public static bool FeMale = false;

	public static int MAX_ITEM_COUNT = 9;

	public static float MAX_LIMIT_IQ = 999.75f;

	public static int gameWorldCount {
		get { return WorldInfo.MAX_WORLD_COUNT; }
	}

	public static int curTime = 0;
	public static int totalTimeForQuestion = 15;
    public static int MaxLifeCount = 5;
    public static int MaxLifeTime = 1200;   // 19mintues 59second = 1199seconds

    public static bool DebugModeShowAnswer = true;

	public static int prizePodState	{ 
		get {return Singleton.curWorldInfo.prizePodState; } 
		set { Singleton.curWorldInfo.prizePodState = value; }
	}

	private List<string> m_countries = new List<string>();

    
	public List<string> allCountries()
	{
		if (m_countries == null ||
			m_countries.Count == 0)
		{
			if (m_countries == null)
				m_countries = new List<string>();

			string[] buffers = Country.Countries.Split(',');

			m_countries.Clear();
			m_countries.AddRange(buffers);
		}

		return m_countries;
	}

    public static GameData own = null;
    public static GameData Singleton
    {
        get
        {
            if (own == null)
                own = new GameData();

            return own;
        }
    }
	#region public members and properties
	public UserInfo userInfo
	{
		get
		{
			if (m_userInfo == null)
				m_userInfo = UserInfo.defaultUser();

            // Hwang Test Code
            //m_userInfo.testValue();
			return m_userInfo;
		}
	}

	public WorldInfo curWorldInfo {
		get { return WorldInfo.curWorldInfo; }
	}

	public WorldInfo nextWorldInfo	{
		get { return WorldInfo.nextWorldInfo; }
	}

	public WorldInfo prevWorldInfo {
		get { return WorldInfo.prevWorldInfo; }
	}


    public bool MusicOn
    {
        get { return m_MusicOn; }
        set { m_MusicOn = value; }
    }
    public bool VibrationOn
    {
        get { return m_VibrationOn; }
        set { m_VibrationOn = value; }
    }

	public Dictionary<ItemKinds, ItemInfo> powerUpsItems
	{
		get { return m_powerUpItems; }
	}
    public bool AnswerStatus
    {
        get { return m_AnswerState; }
        set { m_AnswerState = value; }
    }
	#endregion

	#region private and protected members
	protected static UserInfo m_userInfo = null;
	protected static WorldInfo m_worldInfo = null;

	protected Dictionary<ItemKinds, ItemInfo> m_powerUpItems = new Dictionary<ItemKinds, ItemInfo>();
    private bool m_MusicOn = true;
    private bool m_VibrationOn = true;
    private bool m_AnswerState = false;

	#endregion
	public void shuffleAnswers ()
	{

	}

    public override void InitObject()
    {
    }

    #region INetManagerDelegate Methods
    public void willStartRequest(RequestState _state) { }
	public void parsingError(RequestState _state) { }
	public void didRecievedSuccessResponseFromServer(object _data, RequestState _state) { }
	public void didRecievedFailedResponseFromServer(string _message, RequestState _state) { }
	public void didReceivingError(string _error, RequestState _state) { }
	public void didReceivingTimeOut(RequestState _state) { }


	#endregion

}
