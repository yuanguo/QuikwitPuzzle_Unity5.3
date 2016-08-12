using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Ucss;
using MiniJSON;


public interface IPuzzleNetLogicDelegates
{
	void willStartRequest(RequestState _state);
	void parsingError(RequestState _state);
	void didRecievedSuccessResponseFromServer(object _data, RequestState _state);
	void didRecievedFailedResponseFromServer(string _message, RequestState _state);
	void didReceivingError(string _error, RequestState _state);
	void didReceivingTimeOut(RequestState _state);
}

// 
// public class NetDelegates : Dictionary<RequestState, List<IPuzzleNetLogicDelegates>> { }
// public class NetDelegateSets : Dictionary<string, NetDelegates> 
// {
// 	public static NetDelegateSets defaultSets = new NetDelegateSets();
// 
// 	public static void add (string _transaction, RequestState _state, IPuzzleNetLogicDelegates _delegate = null)
// 	{
// 		if (defaultSets.ContainsKey(_transaction))
// 		{
// 			NetDelegates item = defaultSets[_transaction];
// 			if (item == null)
// 				item = new NetDelegates();
// 
// 			if (item.ContainsKey(_state))
// 			{
// 				List<IPuzzleNetLogicDelegates> delegatesItem = item[_state];
// 
// 				if (delegatesItem == null)
// 				{
// 					delegatesItem = new List<IPuzzleNetLogicDelegates>();
// 					item[_state] = delegatesItem;
// 				}
// 
// 				if (_delegate != null)
// 					delegatesItem.Add(_delegate);
// 				return;
// 			}
// 			else
// 			{
// 				List<IPuzzleNetLogicDelegates> delegats = new List<IPuzzleNetLogicDelegates>();
// 
// 				if (_delegate != null)
// 					delegats.Add(_delegate);
// 
// 				item.Add(_state, delegats);
// 				return;
// 			}
// 		}
// 		else
// 		{
// 			foreach (string key in defaultSets.Keys)
// 			{
// 				NetDelegates item = defaultSets[key];
// 				if (item != null)
// 				{
// 					if (item.ContainsKey(_state))
// 					{
// 						List<IPuzzleNetLogicDelegates> delegates = item[_state];
// 
// 						if (delegates == null)
// 						{
// 							delegates = new List<IPuzzleNetLogicDelegates>();
// 							item[_state] = delegates;
// 						}
// 
// 						if (_delegate != null)
// 							delegates.Add(_delegate);
// 						return;
// 					}
// 				}
// 			}
// 
// 			List<IPuzzleNetLogicDelegates> delegats = new List<IPuzzleNetLogicDelegates>();
// 			if (_delegate != null)
// 				delegats.Add(_delegate);
// 			NetDelegates delegateSet = new NetDelegates();
// 			delegateSet.Add(_state, delegats);
// 
// 			defaultSets.Add(_transaction, delegateSet);
// 			return;
// 		}
// 	}
// 
// 	public static void remove (string _transactionID, RequestState _state, IPuzzleNetLogicDelegates _delegate = null)
// 	{
// 		if (defaultSets.ContainsKey(_transactionID))
// 		{
// 			NetDelegates item = defaultSets[_transactionID];
// 			if (item == null)
// 				defaultSets.Remove(_transactionID);
// 			else
// 			{
// 				if (item.ContainsKey(_state))
// 				{
// 					List<IPuzzleNetLogicDelegates> delegatesItem = item[_state];
// 
// 					if (delegatesItem == null)
// 						item.Remove(_state);
// 					else
// 					{
// 						if (_delegate == null)
// 						{
// 							delegatesItem.Clear();
// 							item.Remove(_state);
// 						}
// 						else
// 							delegatesItem.Remove(_delegate);
// 					}
// 				}
// 			}
// 		}
// 		else
// 		{
// 			foreach (string key in defaultSets.Keys)
// 			{
// 				NetDelegates item = defaultSets[key];
// 				if (item != null)
// 				{
// 					if (item.ContainsKey(_state))
// 					{
// 						List<IPuzzleNetLogicDelegates> delegates = item[_state];
// 
// 						if (delegates == null)
// 							item.Remove(_state);
// 						else
// 						{
// 							if (_delegate == null)
// 							{
// 								delegates.Clear();
// 								item.Remove(_state);
// 							}
// 							else
// 								delegates.Remove(_delegate);
// 						}
// 					}
// 				}
// 			}
// 		}
// 	}
// 
// 	public static void clearAll()
// 	{
// 		foreach (string key in defaultSets.Keys)
// 		{
// 			foreach (RequestState it in defaultSets[key].Keys)
// 			{
// 				List<IPuzzleNetLogicDelegates> itDelegats = defaultSets[key][it];
// 
// 				if (itDelegats != null)
// 					itDelegats.Clear();
// 
// 				defaultSets[key].Remove(it);
// 			}
// 
// 			defaultSets[key].Clear();
// 			defaultSets.Remove(key);
// 		}
// 
// 		defaultSets.Clear();
// 	}
// 
// 	public static NetDelegates findDelegates(string _transactionID)
// 	{
// 		if (defaultSets.ContainsKey(_transactionID))
// 		{
// 			NetDelegates item = defaultSets[_transactionID];
// 
// 			return item;
// 		}
// 
// 		return null;
// 	}
// }

public class PuzzleNetLogic : CEObject
{
	//private const string _HTTP_API_URI = "http://teslavault.com/quitwit/services/";
	private const string _HTTP_API_URI = "http://quikwitapp.com/services/services/";
	private const string _LOGIN_API = "index.php/welcome/login";		// [key], [email], [password]
	private const string _SIGNUP_API = "index.php/welcome/signup";		// [key], [email], [name], [password], [gender], [age], [country], [city]
	private const string _FORGOTEN_PASSWORD_API = "index.php/welcome/forgetpassword";		// [key], [email]
	private const string _USERLIST_API = "index.php/welcome/list_users";					// [key], [page]
	private const string _GETUSER_API = "index.php/welcome/get_user";				// [key], [email] / [userID]
	private const string _GET_WORLD_API = "index.php/welcome/getLevel";				// [level], [userid]
	private const string _CHECK_DATE_API = "index.php/welcome/checkDate";			// [key], [userid]
	private const string _UPDATE_POINTS_API = "index.php/welcome/updatepoints";		// [key], [userid], [level], [question], [points]
	private const string _GET_USER_POINTS_API = "index.php/welcome/getuserpoints";	// [key] [userid]
	private const string _PRIZE_WIN_API = "index.php/welcome/prizewin";				// [key] [userid], [questionid], [answer], [level]
	private const string _POWERUP_API = "index.php/welcome/powerups";				// [key] [userid], [powerups], [token], [type(plus/minus)], [lifename(skipping_rope / question_bomb / rack_in_time / hour_glass / ask_the_audience)]
	//private const string _POWERUP_API = "http://asifphpdev.com/projects/quitwit/services/index.php/welcome/powerups";				// [key] [userid], [powerups], [token], [type(plus/minus)], [lifename(skipping_rope / question_bomb / rack_in_time / hour_glass / ask_the_audience)]
	private const string _GET_POWERUP_API = "index.php/welcome/getpowerups";		// [key] [userid]


	static Dictionary<string, RequestState> requestStates = new Dictionary<string, RequestState>();
	static Dictionary<string, IPuzzleNetLogicDelegates> s_delegates = new Dictionary<string, IPuzzleNetLogicDelegates>();

	static void registerDelegate(string _transactionID, IPuzzleNetLogicDelegates _delegate)
	{
		if (!s_delegates.ContainsKey(_transactionID))
			s_delegates.Add(_transactionID, _delegate);
	}

	static void unregisterDelegate(string _transactionID)
	{
		if (s_delegates.ContainsKey(_transactionID))
			s_delegates.Remove(_transactionID);
	}

	public void clearAllDelegates()
	{
		s_delegates.Clear();
	}
	private static string _PuzzleApiURI(string _api)
	{
		return (_HTTP_API_URI + _api);
	}

	// [key], [email], [password]
	// return : transactionID[string]
	public static string requestLogin(IPuzzleNetLogicDelegates _delegate, UserInfo _userInfo)
	{
		WWWForm data = new WWWForm();
		data.AddField("key", _userInfo.sessionID);
		data.AddField("email", _userInfo.email);
		data.AddField("password", _userInfo.password);

		string transactyionID = UCSS.HTTP.PostForm(_PuzzleApiURI(_LOGIN_API), data,
			new EventHandlerHTTPBytes(OnReceivedResponse), 
			new EventHandlerServiceError(OnHTTPError), 
			new EventHandlerServiceTimeOut(OnHTTPTimeOut));

		requestStates.Add(transactyionID, RequestState.RequestLogin);
		registerDelegate(transactyionID, _delegate);
		willStartRequest(transactyionID);
		return transactyionID;
	}

	// [key], [email], [name], [password], [gender], [age], [country], [city]
	// return : transactionID[string]
	public static string requestRegisterUser(IPuzzleNetLogicDelegates _delegate, UserInfo _userInfo)
	{
		WWWForm data = new WWWForm();
		data.AddField("key", _userInfo.sessionID);
		data.AddField("email", _userInfo.email);
		data.AddField("name", _userInfo.name);
		data.AddField("password", _userInfo.password);
		data.AddField("gender", (_userInfo.gender ? "male" : "female"));
		data.AddField("age", _userInfo.age.ToString());
		data.AddField("country",	_userInfo.country);
		data.AddField("city",		_userInfo.city);

		string transactyionID = UCSS.HTTP.PostForm(_PuzzleApiURI(_SIGNUP_API), data,
			new EventHandlerHTTPBytes(OnReceivedResponse),
			new EventHandlerServiceError(OnHTTPError),
			new EventHandlerServiceTimeOut(OnHTTPTimeOut));

		requestStates.Add(transactyionID, RequestState.RequestRegister);
		registerDelegate(transactyionID, _delegate);
		willStartRequest(transactyionID);
		return transactyionID;
	}

	// [key], [email] / [userID]
	public static string requestUserInfo(IPuzzleNetLogicDelegates _delegate, UserInfo _userInfo)
	{
		WWWForm data = new WWWForm();
		data.AddField("key", _userInfo.sessionID);

		if (_userInfo.email == null ||
			_userInfo.email.Equals(string.Empty))
			data.AddField("id", _userInfo.id);
		else
			data.AddField("email", _userInfo.email);

		string transactyionID = UCSS.HTTP.PostForm(_PuzzleApiURI(_GETUSER_API), data,
			new EventHandlerHTTPBytes(OnReceivedResponse),
			new EventHandlerServiceError(OnHTTPError),
			new EventHandlerServiceTimeOut(OnHTTPTimeOut));

		requestStates.Add(transactyionID, RequestState.RequestUserInfo);
		registerDelegate(transactyionID, _delegate);
		willStartRequest(transactyionID);
		return transactyionID;
	}


	// [level, userid]
	public static string requestWorldInfo(IPuzzleNetLogicDelegates _delegate, int _worldNum, int _userID)
	{
		WWWForm data = new WWWForm();
		data.AddField("level", _worldNum);
		data.AddField("userid", _userID);

		string transactyionID = UCSS.HTTP.PostForm(_PuzzleApiURI(_GET_WORLD_API), data,
			new EventHandlerHTTPBytes(OnReceivedResponse),
			new EventHandlerServiceError(OnHTTPError),
			new EventHandlerServiceTimeOut(OnHTTPTimeOut));

		requestStates.Add(transactyionID, RequestState.RequestWorldInfo);
		registerDelegate(transactyionID, _delegate);
		willStartRequest(transactyionID);
		return transactyionID;
	}


	// [key], [userid]
	public static string requestCheckDate(IPuzzleNetLogicDelegates _delegate, UserInfo _userInfo)
	{
		WWWForm data = new WWWForm();
		data.AddField("key", _userInfo.sessionID);
		data.AddField("userid", _userInfo.id);

		string transactyionID = UCSS.HTTP.PostForm(_PuzzleApiURI(_CHECK_DATE_API), data,
			new EventHandlerHTTPBytes(OnReceivedResponse),
			new EventHandlerServiceError(OnHTTPError),
			new EventHandlerServiceTimeOut(OnHTTPTimeOut));

		requestStates.Add(transactyionID, RequestState.RequestCheckDate);
		registerDelegate(transactyionID, _delegate);
		willStartRequest(transactyionID);
		return transactyionID;
	}

	// [key], [userid], [level], [question], [points]
	public static string requestUpdatePoints(IPuzzleNetLogicDelegates _delegate)
	{
		GameData gameData = GameData.Singleton;

		WWWForm data = new WWWForm();
		data.AddField("key", gameData.userInfo.sessionID);
		data.AddField("userid", gameData.userInfo.id);
		data.AddField("level", gameData.curWorldInfo.worldIDX);
		data.AddField("question", (gameData.curWorldInfo.curQuestionNum + 1));
		data.AddField("points", (int)gameData.userInfo.point);

		string transactyionID = UCSS.HTTP.PostForm(_PuzzleApiURI(_UPDATE_POINTS_API), data,
			new EventHandlerHTTPBytes(OnReceivedResponse),
			new EventHandlerServiceError(OnHTTPError),
			new EventHandlerServiceTimeOut(OnHTTPTimeOut));

		requestStates.Add(transactyionID, RequestState.RequestUpdatePoints);
		registerDelegate(transactyionID, _delegate);
		willStartRequest(transactyionID);
		return transactyionID;
	}

	// [key] [userid]
	public static string requestGetUserPoints(IPuzzleNetLogicDelegates _delegate)
	{
		GameData gameData = GameData.Singleton;

		WWWForm data = new WWWForm();
		data.AddField("key", gameData.userInfo.sessionID);
		data.AddField("userid", gameData.userInfo.id);

		string transactyionID = UCSS.HTTP.PostForm(_PuzzleApiURI(_GET_USER_POINTS_API), data,
			new EventHandlerHTTPBytes(OnReceivedResponse),
			new EventHandlerServiceError(OnHTTPError),
			new EventHandlerServiceTimeOut(OnHTTPTimeOut));

		requestStates.Add(transactyionID, RequestState.RequestGameInfo);
		registerDelegate(transactyionID, _delegate);
		willStartRequest(transactyionID);
		return transactyionID;
	}

	// [key] [userid], [questionid], [answer], [level]
	public static string requestPrizeWin(IPuzzleNetLogicDelegates _delegate, int _answerID)
	{
		GameData gameData = GameData.Singleton;

		WWWForm data = new WWWForm();
		data.AddField("key", gameData.userInfo.sessionID);
		data.AddField("userid", gameData.userInfo.id);
		data.AddField("questionid", (gameData.curWorldInfo.curQuestionNum));
		data.AddField("answer", _answerID);
		data.AddField("level", (int)(gameData.curWorldInfo.worldIDX + 1));

		string transactyionID = UCSS.HTTP.PostForm(_PuzzleApiURI(_PRIZE_WIN_API), data,
			new EventHandlerHTTPBytes(OnReceivedResponse),
			new EventHandlerServiceError(OnHTTPError),
			new EventHandlerServiceTimeOut(OnHTTPTimeOut));

		requestStates.Add(transactyionID, RequestState.RequestPrizeWin);
		registerDelegate(transactyionID, _delegate);
		willStartRequest(transactyionID);
		return transactyionID;
	}

	// [key] [userid], [powerups], [token], [type(plus/minus)], 
	// [lifename(skipping_rope / question_bomb / rack_in_time / hour_glass / ask_the_audience)]
	public static string requestUpdatePowerUp(IPuzzleNetLogicDelegates _delegate, ItemKinds _kind, int _count, bool _plus, int _purchaseID)
	{
		GameData gameData = GameData.Singleton;

		string strPowerUpName = "";
		switch(_kind)
		{
			case ItemKinds.audience:
				strPowerUpName = "ask_the_audience";
				break;
			case ItemKinds.bomb:
				strPowerUpName = "question_bomb";
				break;
			case ItemKinds.rope:
				strPowerUpName = "skipping_rope";
				break;
			case ItemKinds.timer:
				strPowerUpName = "hour_glass";
				break;
			case ItemKinds.tornado:
				strPowerUpName = "rack_in_time";
				break;
		}

		WWWForm data = new WWWForm();
		data.AddField("key", gameData.userInfo.sessionID);
		data.AddField("userid", gameData.userInfo.id);
		data.AddField("powerups", _count);
		data.AddField("token", "1");
		data.AddField("type", (_plus ? "plus" : "minus"));
		data.AddField("lifename", strPowerUpName);

		string transactyionID = UCSS.HTTP.PostForm(_PuzzleApiURI(_POWERUP_API), data,
//		string transactyionID = UCSS.HTTP.PostForm(_POWERUP_API, data,
			new EventHandlerHTTPBytes(OnReceivedResponse),
			new EventHandlerServiceError(OnHTTPError),
			new EventHandlerServiceTimeOut(OnHTTPTimeOut));

		requestStates.Add(transactyionID, RequestState.RequestUpdatePowerUp);
		registerDelegate(transactyionID, _delegate);
		willStartRequest(transactyionID);
		return transactyionID;
	}

	// [key] [userid]
	public static string requestGetPowerUp(IPuzzleNetLogicDelegates _delegate)
	{
		GameData gameData = GameData.Singleton;

		WWWForm data = new WWWForm();
		data.AddField("key", gameData.userInfo.sessionID);
		data.AddField("userid", gameData.userInfo.id);

		string transactyionID = UCSS.HTTP.PostForm(_PuzzleApiURI(_GET_POWERUP_API), data,
			new EventHandlerHTTPBytes(OnReceivedResponse),
			new EventHandlerServiceError(OnHTTPError),
			new EventHandlerServiceTimeOut(OnHTTPTimeOut));

		requestStates.Add(transactyionID, RequestState.RequestGetPowerUp);
		registerDelegate(transactyionID, _delegate);
		willStartRequest(transactyionID);
		return transactyionID;
	}


	// [key], [email]
	public static string requestForgotenPassword(IPuzzleNetLogicDelegates _delegate, string _email)
	{
		GameData gameData = GameData.Singleton;

		WWWForm data = new WWWForm();
		data.AddField("key", gameData.userInfo.sessionID);
		data.AddField("email", _email);

		string transactyionID = UCSS.HTTP.PostForm(_PuzzleApiURI(_FORGOTEN_PASSWORD_API), data,
			new EventHandlerHTTPBytes(OnReceivedResponse),
			new EventHandlerServiceError(OnHTTPError),
			new EventHandlerServiceTimeOut(OnHTTPTimeOut));

		requestStates.Add(transactyionID, RequestState.RequestForgotenPassword);
		registerDelegate(transactyionID, _delegate);
		willStartRequest(transactyionID);
		return transactyionID;
	}

	static void willStartRequest(string _transactionID)
	{
		if (s_delegates != null &&
			requestStates.ContainsKey(_transactionID) &&
			s_delegates.ContainsKey(_transactionID))
		{
			if (s_delegates[_transactionID] != null)
				s_delegates[_transactionID].willStartRequest(requestStates[_transactionID]);
		}
	}

	static void OnReceivedResponse(byte[] _data, string _transactionID)
	{
		string strReceivedData = System.Text.Encoding.UTF8.GetString(_data);
		if (strReceivedData.StartsWith("<pre>"))
			strReceivedData = strReceivedData.Substring(5);

		if (strReceivedData.IndexOf("{\"status\"") > 0)
			strReceivedData = strReceivedData.Remove(0, strReceivedData.IndexOf("{\"status\""));

		Debug.Log(strReceivedData);

		Dictionary<string, object> dataTemp = (Dictionary<string, object>)Json.Deserialize(strReceivedData);

		if (s_delegates.Count <= 0)
		{
			Debug.LogError("Not has this transactionID : " + _transactionID + ", Plz check again");
			if (requestStates.ContainsKey(_transactionID) &&
				s_delegates.ContainsKey(_transactionID))
			{
				if (s_delegates[_transactionID] != null)
				{
					s_delegates[_transactionID].parsingError(requestStates[_transactionID]);
					unregisterDelegate(_transactionID);
				}
			}

			return;
		}

		if (dataTemp != null)
		{
			if (dataTemp.ContainsKey("status"))
			{
				string status = (string)dataTemp["status"];

				if (status != "SUCCESS")
				{
					if (requestStates.ContainsKey(_transactionID) &&
						s_delegates.ContainsKey(_transactionID))
					{
						if (s_delegates[_transactionID] != null)
						{
							s_delegates[_transactionID].didRecievedFailedResponseFromServer(status, requestStates[_transactionID]);
							unregisterDelegate(_transactionID);
						}
					}
					return;
				}
				else
				{
					// success
					if (dataTemp.ContainsKey("data"))
					{
						Dictionary<string, object> data = (Dictionary<string, object>)dataTemp["data"];

						if (requestStates.ContainsKey(_transactionID) &&
							s_delegates.ContainsKey(_transactionID))
						{
							if (s_delegates[_transactionID] != null)
							{
								s_delegates[_transactionID].didRecievedSuccessResponseFromServer(data, requestStates[_transactionID]);
								unregisterDelegate(_transactionID);
							}
						}

						return;
					}
				}
			}
		}


		// parsing error
		if (requestStates.ContainsKey(_transactionID) &&
			s_delegates.ContainsKey(_transactionID))
		{
			if (s_delegates[_transactionID] != null)
			{
				s_delegates[_transactionID].parsingError(requestStates[_transactionID]);
				unregisterDelegate(_transactionID);
			}
		}

		UCSS.HTTP.RemoveTransaction(_transactionID);
	}

	static void OnHTTPError(string error, string transactionId)
	{
		if (requestStates.ContainsKey(transactionId) &&
			s_delegates.ContainsKey(transactionId))
		{
			if (s_delegates[transactionId] != null)
			{
				s_delegates[transactionId].didReceivingError(error, requestStates[transactionId]);
				unregisterDelegate(transactionId);
			}
		}

		UCSS.HTTP.RemoveTransaction(transactionId);
	}

	static void OnHTTPTimeOut(string transactionId)
	{
		if (requestStates.ContainsKey(transactionId) &&
			s_delegates.ContainsKey(transactionId))
		{
			if (s_delegates[transactionId] != null)
			{
				s_delegates[transactionId].didReceivingTimeOut(requestStates[transactionId]);
				unregisterDelegate(transactionId);
			}
		}

		UCSS.HTTP.RemoveTransaction(transactionId);
	}
}
