using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScreenSelGender : CEBehaviour, IPuzzleNetLogicDelegates
{
    #region Public Member
	public Transform m_chPos = null;

    public InputField m_ChName = null;
    public Toggle m_ManToggle = null;
    public Toggle m_WomenToggle = null;
    public Button m_Play = null;
    #endregion Public Member

    #region Private Member
    private bool m_bGender = true;

	private GameObject chObject = null;
    #endregion Private Member

	// Use this for initialization
    protected override void OnEnable()
    {
		if (gameLogic.Character != null)
		{
			chObject = gameLogic.Character;
			chObject.transform.SetParent(m_chPos);
			chObject.transform.localPosition = Vector3.zero;
			chObject.transform.localRotation = Quaternion.identity;
			chObject.transform.localScale = Vector3.one;


			chObject.SetActive(true);
		}

        if (m_ManToggle)
            m_ManToggle.isOn = true;
    }

    #region Public Member
    public void OnSelGender(bool bMan)
    {
        m_bGender = bMan;

		if (chObject != null)
		{
			chObject.SetActive(false);

			gameData.userInfo.isMale = m_bGender;
			chObject = gameLogic.Character;
			chObject.transform.SetParent(m_chPos);
			chObject.transform.localPosition = Vector3.zero;
			chObject.transform.localRotation = Quaternion.identity;
			chObject.transform.localScale = Vector3.one;

			chObject.SetActive(true);
		}
    }
    public void OnOk()
    {

        gameData.userInfo.gender = m_bGender ? GameData.Male : GameData.FeMale;
        gameData.userInfo.name = m_ChName.text;

		gameData.userInfo.iq = 0;
		gameData.userInfo.Lifes = 0;
		gameData.userInfo.city = string.Empty;
		gameData.userInfo.point = 0;
		gameData.userInfo.age = 0;

		PuzzleNetLogic.requestRegisterUser(this, gameData.userInfo);
    }
    public void OnChange(Text placeholder)
    {
        if (!m_ChName || !placeholder)
            return;

        if (m_ChName.text != placeholder.text)
        {
            if (m_ManToggle)
            {
                m_ManToggle.interactable = true;
                m_ManToggle.isOn = m_bGender;
            }

            if (m_WomenToggle)
            {
                m_WomenToggle.interactable = true;
                m_WomenToggle.isOn = !m_bGender;
            }

            if (m_Play)
                m_Play.interactable = true;
        }
        else
        {
            if (m_ManToggle)
            {
                m_ManToggle.interactable = false;
                m_ManToggle.isOn = false;
            }

            if (m_WomenToggle)
            {
                m_WomenToggle.interactable = false;
                m_WomenToggle.isOn = false;
            }

            if (m_Play)
                m_Play.interactable = false;
        }


    }
    #endregion Public Member

	#region INetManagerDelegate methods

	public void willStartRequest(RequestState _state)
	{
		if (_state == RequestState.RequestRegister)
			CELoadingBar.startLoadingAnimation("Sign up ...");
		else if (_state == RequestState.RequestGetPowerUp)
			CELoadingBar.startLoadingAnimation("Getting Info...");
		else if (_state == RequestState.RequestUserInfo)
			CELoadingBar.startLoadingAnimation("Getting infos ...");
		else if (_state == RequestState.RequestGameInfo)
			CELoadingBar.startLoadingAnimation("Getting infos ...");

	}
	public void parsingError(RequestState _state)
	{
		// to do error message... 
		CELoadingBar.stopLoadingAnimation();
	}
	public void didRecievedSuccessResponseFromServer(object _data, RequestState _state)
	{
		if (_data == null)
			return;

		Dictionary<string, object> data = (Dictionary<string, object>)_data;

		if (_state == RequestState.RequestRegister)
		{
			if (data.ContainsKey("userid_id"))
				gameData.userInfo.id = data["userid_id"].ToString();

			gameData.userInfo.Lifes = GameData.MaxLifeCount;

			WorldInfo.curWorldInfo.InitObject();
			WorldInfo.LoadPassedWorldID();

			PuzzleNetLogic.requestUserInfo(this, gameData.userInfo); 
			CELoadingBar.stopLoadingAnimation();
		}
		else if (_state == RequestState.RequestUserInfo)
		{
			CELoadingBar.stopLoadingAnimation();
			UserInfo userInfo = gameData.userInfo;
			if (data.ContainsKey("id"))
				userInfo.id = data["id"].ToString();

			if (data.ContainsKey("email"))
				userInfo.email = data["email"].ToString();

			if (data.ContainsKey("name"))
				userInfo.name = data["name"].ToString();

			if (data.ContainsKey("gender"))
			{
				string bufferGender = data["gender"].ToString().ToLower();

				if (bufferGender == "male" ||
					bufferGender == "m" ||
					bufferGender == "true")
					userInfo.gender = true;
				else
					userInfo.gender = false;
			}

			if (data.ContainsKey("age"))
				userInfo.age = int.Parse(data["age"].ToString());

			if (data.ContainsKey("country"))
				userInfo.country = data["country"].ToString();

			if (data.ContainsKey("city"))
				userInfo.city = data["city"].ToString();

			UserInfo.wasLogined = true;
			userInfo.loadLifeTime();

			WorldInfo.LoadPassedWorldID();

			PuzzleNetLogic.requestGetUserPoints(this);
		}
		else if (_state == RequestState.RequestGameInfo)
		{
			// RequestGameInfo(user point data)
			parseForGameUserData(_data);

			CELoadingBar.stopLoadingAnimation();
			PuzzleNetLogic.requestGetPowerUp(this);
		}
		else if (_state == RequestState.RequestGetPowerUp)
		{
			CELoadingBar.stopLoadingAnimation();

			ItemKinds kind = ItemKinds.blank;
			int counts = 0;

			if (data.ContainsKey("skipping_rope"))
			{
				kind = ItemKinds.rope;
				counts = int.Parse(data["skipping_rope"].ToString());

				counts = counts < 0 ? 0 : counts;
				if (counts > 0)
					ItemInfo.InsertItem(kind, counts);
			}

			if (data.ContainsKey("question_bomb"))
			{
				kind = ItemKinds.bomb;
				counts = int.Parse(data["skipping_rope"].ToString());
				counts = counts < 0 ? 0 : counts;
				if (counts > 0)
					ItemInfo.InsertItem(kind, counts);
			}
			if (data.ContainsKey("rack_in_time"))
			{
				kind = ItemKinds.tornado;
				counts = int.Parse(data["rack_in_time"].ToString());
				counts = counts < 0 ? 0 : counts;
				if (counts > 0)
					ItemInfo.InsertItem(kind, counts);
			}
			if (data.ContainsKey("hour_glass"))
			{
				kind = ItemKinds.timer;
				counts = int.Parse(data["hour_glass"].ToString());
				counts = counts < 0 ? 0 : counts;
				if (counts > 0)
					ItemInfo.InsertItem(kind, counts);
			}
			if (data.ContainsKey("ask_the_audience"))
			{
				kind = ItemKinds.audience;
				counts = int.Parse(data["ask_the_audience"].ToString());
				counts = counts < 0 ? 0 : counts;
				if (counts > 0)
					ItemInfo.InsertItem(kind, counts);
			}

			gameLogic.ChangeGameState(GameLogic.GameState.SetUp);

			GameData.Singleton.curWorldInfo.loadPrizeInfo();
		}
	}

	public void didRecievedFailedResponseFromServer(string _message, RequestState _state)
	{
		if (_state == RequestState.RequestRegister)
		{
			gameLogic.ChangeGameState(GameLogic.GameState.Register);
			gameLogic.ErrorBox(Message.MSG_SING_UP_EMAIL_ERROR);
		}


		CELoadingBar.stopLoadingAnimation();
	}
	public void didReceivingError(string _error, RequestState _state)
	{
		gameLogic.ErrorBox(Message.MSG_RECEIVE_FAILED);
		CELoadingBar.stopLoadingAnimation();
	}
	public void didReceivingTimeOut(RequestState _state)
	{
		gameLogic.ErrorBox(Message.MSG_TIME_OUT);
		CELoadingBar.stopLoadingAnimation();
	}
	#endregion

	public void parseForGameUserData(object _data)
	{
		Dictionary<string, object> data = (Dictionary<string, object>)_data;
		{
			UserInfo userInfo = UserInfo.defaultUser();

			userInfo.Lifes = GameData.MaxLifeCount;
			userInfo.m_Achievement = 0;
			userInfo.iq = 0;
			userInfo.rightCount = 0;
			userInfo.point = 0;

			WorldInfo.curWorlNum = 1;
			WorldInfo.curWorldInfo.curQuestionNum = 1;
			if (data.ContainsKey("points"))
				userInfo.point = int.Parse(data["points"].ToString());
		}
	}

}
