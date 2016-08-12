using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScreenLogin : CEBehaviour, IPuzzleNetLogicDelegates, IComboListDelegate
{
    // for login
    public InputField m_LoginUser = null;
    public InputField m_LoginPwd = null;
    public Toggle m_AutoLogin = null;

    // for register
    public ComboList m_RegCountry = null;
    public InputField m_RegEmail = null;
    public InputField m_RegGender = null;
    public InputField m_RegPwd = null;
    public InputField m_RegConfirmPwd = null;

	// Use this for initialization
    protected override void OnEnable()
    {
		base.OnEnable();

		string temp = string.Empty;
		if (PlayerPrefs.HasKey("autoLogin"))
		{
			temp = PlayerPrefs.GetString("autoLogin", temp);
			gameData.autoLogin = bool.Parse(temp);
		}

		updateData ();
    }
	public void parseForGameUserData(object _data)
	{
		Dictionary<string, object> data = (Dictionary<string, object>)_data;
		{
			UserInfo userInfo = UserInfo.defaultUser();
			WorldInfo.curWorlNum = 1;
			WorldInfo.curWorldInfo.curQuestionNum = 1;

			if (data.ContainsKey("level"))
			{
				int nWorldId = int.Parse(data["level"].ToString());
				WorldInfo.curWorlNum = nWorldId < 1 ? 1 : nWorldId;
			}

			if (data.ContainsKey("question"))
			{
				int nLevel = int.Parse(data["question"].ToString());
				WorldInfo.curWorldInfo.curQuestionNum = nLevel < 1 ? 1 : nLevel;
			}
			else
				WorldInfo.curWorldInfo.curQuestionNum = 1;

			if (data.ContainsKey("points"))
				userInfo.point = int.Parse(data["points"].ToString());

			UserInfo.wasLogined = true;
			userInfo.loadLifeTime();

			WorldInfo.LoadPassedWorldID();
			for (WorldIdentifier id = WorldIdentifier.Polomia; id <= WorldIdentifier.Cattica; id++ )
			{
				if ((WorldIdentifier)WorldInfo.curWorlNum > id)
					WorldInfo.UpdatePassedWorldId(id);
				else
					break;
			}
		}
	}

	public void updateData()
	{
		gameLogic.ChangeGameState(GameLogic.GameState.Login);
		
		if (gameData.autoLogin) {
			m_LoginUser.text = gameData.userInfo.email;
			m_LoginPwd.text = gameData.userInfo.password;
			m_AutoLogin.isOn = true;
		} else {
			gameData.userInfo.name = "";
			gameData.userInfo.password = "";

			m_AutoLogin.isOn = false;
			m_LoginUser.text = string.Empty;
			m_LoginPwd.text = string.Empty;
		}
		
		if (m_RegCountry != null)
		{
			m_RegCountry.initComboList(this);
			List<string> countries = gameData.allCountries();
			
			foreach(string coutry in countries)
			{
				ListViewCell cell = m_RegCountry.insertAtLastRow(0, coutry);
				
				Text captionText = cell.GetComponent<Text>();
				
				if (captionText != null)
					captionText.text = coutry;
			}
			
			if (m_RegCountry.headerCell != null)
			{
				Text headerText = m_RegCountry.headerCell.GetComponent<Text>();
				if (headerText != null)
				{
					headerText.text = countries.Count > 0 ? countries[0] : string.Empty;
					gameData.userInfo.country = headerText.text;
				}
			}
		}
	}
	
    public void OnLogin()
    {
		if (m_LoginUser.text != string.Empty)
			gameData.userInfo.email = m_LoginUser.text;

		if (m_LoginPwd.text != string.Empty)
			gameData.userInfo.password = m_LoginPwd.text;

		PuzzleNetLogic.requestLogin(this, gameData.userInfo);
    }
    public void OnRegister()
    {
        if (gameLogic.IsInGameState(GameLogic.GameState.Login))
            gameLogic.ChangeGameState(GameLogic.GameState.Register);
        else
        {
            // TODO:
            if (m_RegPwd.text != m_RegConfirmPwd.text)
            {
                Debug.Log("Password and ConfirmPassword is not according. ");
                return;
            }

			gameData.userInfo.email = m_RegEmail.text;
			gameData.userInfo.password = m_RegPwd.text;
			gameData.userInfo.id = string.Empty;

            gameLogic.ChangeGameState(GameLogic.GameState.SelCh);
        }
    }
    public void OnRemember()
    {
        gameData.autoLogin = m_AutoLogin.isOn;

		PlayerPrefs.SetString("autoLogin", gameData.autoLogin.ToString());
		PlayerPrefs.Save ();
    }

	public void OnForgotPassword()
	{
		if (m_LoginUser.text == string.Empty)
			gameLogic.ErrorBox(Message.MSG_INVALID_EMAIL);
		else
		{
			CELoadingBar.startLoadingAnimation("Loading...");
			PuzzleNetLogic.requestForgotenPassword(this, m_LoginUser.text);
		}
	}

    public void OnOk()
    {
        if (!gameLogic.IsInGameState(GameLogic.GameState.SelCh))
            return;

        gameLogic.ChangeGameState(GameLogic.GameState.Setting);
    }
    public void OnCancel()
    {
        if (gameLogic.IsInGameState(GameLogic.GameState.SelCh))
            return;

        gameLogic.ChangeGameState(GameLogic.GameState.Login);
	}

	#region INetManagerDelegate methods

	public void willStartRequest(RequestState _state) 
	{
		if (gameLogic.State == GameLogic.GameState.Login)
		{
			if (_state == RequestState.RequestLogin)
				CELoadingBar.startLoadingAnimation("Log in ...");
			else if (_state == RequestState.RequestUserInfo)
				CELoadingBar.startLoadingAnimation("Getting infos ...");
			else if (_state == RequestState.RequestGameInfo ||
				_state == RequestState.RequestGetPowerUp)
				CELoadingBar.startLoadingAnimation("Getting infos ...");
		}
	}
	public void parsingError(RequestState _state) 
	{
		// to do error message... 
		CELoadingBar.stopLoadingAnimation();
	}
	public void didRecievedSuccessResponseFromServer(object _data, RequestState _state)
	{
		Dictionary<string, object> data = (Dictionary<string, object>)_data;

        if (_state == RequestState.RequestLogin)
        {
			if (data.ContainsKey("userid_id"))
				gameData.userInfo.id = data["userid_id"].ToString();

			CELoadingBar.stopLoadingAnimation();
			PuzzleNetLogic.requestUserInfo(this, gameData.userInfo); 
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
		else if (_state  == RequestState.RequestGetPowerUp)
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
		else if (_state == RequestState.RequestForgotenPassword)
		{
			CELoadingBar.stopLoadingAnimation();
			gameLogic.ErrorBox(Message.MSG_EMAIL_SENT);
		}
	}
	public void didRecievedFailedResponseFromServer(string _message, RequestState _state) 
	{
		if (_state == RequestState.RequestLogin)
			gameLogic.ErrorBox(Message.MSG_INVALID_USERID_OR_PWD);

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

	#region combolist delegate methods
	public bool beginReloadComboList(ComboList _comboList)
	{
		return true;
	}

	public void endReloadComboList(ComboList _comboList)
	{

	}

	public bool insertingCellInfo(ComboList _comboList, ListViewCell _cellInfo)
	{

		return true;
	}

	public void didSelectedHeaderCell(ComboList _comboList, HeadListViewCell _header)
	{
		_comboList.playOpenCloseAnimation();
	}

	public void didDeSelectedHeaderCell(ComboList _comboList, HeadListViewCell _header)
	{
		_comboList.playOpenCloseAnimation();
	}

	public void didSelectedCellInfo(ComboList _comboList, ListViewCell _cellInfo)
	{
		Text headerText = _comboList.headerCell.GetComponent<Text>();
		if (headerText != null)
			headerText.text = _cellInfo.userData.ToString();

		gameData.userInfo.country = _cellInfo.userData.ToString();
		_comboList.playOpenCloseAnimation();
	}

	public void didDeSelectedCellInfo(ComboList _comboList, ListViewCell _cellInfo)
	{

	}
	#endregion
}
