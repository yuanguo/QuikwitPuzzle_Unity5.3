using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Ucss;

/*
 * UserInfo
 * 
 *	name
 *	id
 *	password
 *	gender
 *	sessionID
 */


public class UserInfo : CEObject
{
	private static UserInfo s_defaultUser = null;

	public static bool wasLogined = false;

	public static string UserInfoIentifier
	{
		get { return "UserInfoIentifier" + s_defaultUser.id; }
	}
	#region public members and properties

	public string name = string.Empty;
	public string id = "3";
	public string password = string.Empty;
	public bool gender = false; // false : female, true : male
	public string sessionID = string.Empty;
	public string email = string.Empty;
	public string country = string.Empty;
	public string city = string.Empty;
	public int age = 0;

    // user's item info
    public List<ItemInfo> m_Items = new List<ItemInfo>();
    public int m_Achievement = 0;

	public DateTime m_lifeStartedTime;
	private int lifes = 0;
    public int Lifes
    {
        set { lifes = Mathf.Min(value, GameData.MaxLifeCount); }
        get { return lifes; }
    }

	public float iq = 0f;
	public int rightCount = 0;
	public float point = 0f;


	public bool isMale
	{
		get { return gender; }
		set	{ gender = value; }
	}
	#endregion

	#region private members
	#endregion

	#region public API Methods
	public static UserInfo defaultUser()
	{
		if (s_defaultUser == null)
		{
			s_defaultUser = new UserInfo();
		}
		return s_defaultUser;
	}


	public override void InitObject()
	{
		this.name = string.Empty;
		this.id = string.Empty;
		this.password = string.Empty;
		this.gender = true;
		this.sessionID = string.Empty;
		this.email = string.Empty;
		this.country = string.Empty;
		this.city = string.Empty;
		this.age = 0;

		this.lifes = 0;
		this.iq = 0f;

		this.point = 0f;
		this.rightCount = 0;

		this.sessionID = "AS12!AS12!";

        base.InitObject();
	}

	public bool saveUserInfo()
	{
		try
		{

			PlayerPrefs.SetString("LastUserID", id);

			PlayerPrefs.SetString(UserInfoIentifier + "name", name);
			PlayerPrefs.SetString(UserInfoIentifier + "id", id);
			PlayerPrefs.SetString(UserInfoIentifier + "password", password);
			PlayerPrefs.SetInt(UserInfoIentifier + "gender", (gender == true ? 1 : 0));
			PlayerPrefs.SetString(UserInfoIentifier + "email", email);
			PlayerPrefs.SetString(UserInfoIentifier + "country", country);
			PlayerPrefs.SetString(UserInfoIentifier + "city", city);
			PlayerPrefs.SetInt(UserInfoIentifier + "age", age);
			PlayerPrefs.SetInt(UserInfoIentifier + "lifes", Mathf.Max(0, Mathf.Min(lifes, GameData.MaxLifeCount)));

			if (iq > GameData.MAX_LIMIT_IQ)
				iq = GameData.MAX_LIMIT_IQ;
			if (iq < 0)
				iq = 0;

			PlayerPrefs.SetFloat(UserInfoIentifier + "iq", iq);

			PlayerPrefs.Save();

			return true;
		}
		catch (System.Exception err)
		{
			Debug.Log("UserInfo : saveUserInfo : Error " + err.Message);
		}
		return false;
	}

	public bool loadUserInfo()
	{
		try
		{
            this.lifes = GameData.MaxLifeCount;
            //this.iq = 1f;

			if (PlayerPrefs.HasKey("LastUserID"))
				id = PlayerPrefs.GetString("LastUserID", "1");
			else
				id = "1";


			if (PlayerPrefs.HasKey(UserInfoIentifier + "name"))
				name = PlayerPrefs.GetString(UserInfoIentifier + "name", string.Empty);

			if (PlayerPrefs.HasKey(UserInfoIentifier + "id"))
				id = PlayerPrefs.GetString(UserInfoIentifier + "id", string.Empty);

			if (PlayerPrefs.HasKey(UserInfoIentifier + "password"))
				password = PlayerPrefs.GetString(UserInfoIentifier + "password", string.Empty);

			if (PlayerPrefs.HasKey(UserInfoIentifier + "gender"))
			{
				int buffer = 0;
				buffer = PlayerPrefs.GetInt(UserInfoIentifier + "gender", 1);
				gender = buffer == 1 ? true : false;
			}

			if (PlayerPrefs.HasKey(UserInfoIentifier + "email"))
				email = PlayerPrefs.GetString(UserInfoIentifier + "email", string.Empty);

			if (PlayerPrefs.HasKey(UserInfoIentifier + "country"))
				country = PlayerPrefs.GetString(UserInfoIentifier + "country", string.Empty);

			if (PlayerPrefs.HasKey(UserInfoIentifier + "city"))
				city = PlayerPrefs.GetString(UserInfoIentifier + "city", string.Empty);

			if (PlayerPrefs.HasKey(UserInfoIentifier + "age"))
				age = PlayerPrefs.GetInt(UserInfoIentifier + "age", 0);

			if (PlayerPrefs.HasKey(UserInfoIentifier + "lifes"))
				lifes = PlayerPrefs.GetInt(UserInfoIentifier + "lifes", 5);

			if (PlayerPrefs.HasKey(UserInfoIentifier + "iq"))
				iq = PlayerPrefs.GetFloat(UserInfoIentifier + "iq", 0f);

			if (iq > GameData.MAX_LIMIT_IQ)
				iq = GameData.MAX_LIMIT_IQ;
			if (iq < 0)
				iq = 0;


			//sessionID = UCSS.GenerateTransactionId(Common.Md5Sum(name + password + gender + country));
			sessionID = "AS12!AS12!";

			return true;
		}
		catch (System.Exception err)
		{
			Debug.Log("UserInfo : loadUserInfo : Error " + err.Message);
		}

		return false;
	}

	public void loadLifeTime()
	{
		string szLifeTimeKey = UserInfo.UserInfoIentifier + "LifeTime";
		string szLifeTime = string.Empty;
		DateTime curTime = DateTime.Now;

		if (PlayerPrefs.HasKey(szLifeTimeKey) == false)
		{
			szLifeTime = curTime.ToString();
			PlayerPrefs.SetString(szLifeTimeKey, szLifeTime);
			PlayerPrefs.Save();
		}
		else
			szLifeTime = PlayerPrefs.GetString(szLifeTimeKey, curTime.ToString());

		if (DateTime.TryParse(szLifeTime, out m_lifeStartedTime) == false)
		{
			m_lifeStartedTime = new DateTime();
			Debug.LogError("Couldn't parsing the LifeTime DateTime from string : " + szLifeTime);
		}
		else
			Debug.LogWarning("Success to parsing the DateTime (LifeTime) : " + szLifeTime);

		if (m_lifeStartedTime.ToString() == (new DateTime().ToString()))
			m_lifeStartedTime = DateTime.Now;
	}

	public void saveLifeTime()
	{
		string szLifeTimeKey = UserInfo.UserInfoIentifier + "LifeTime";
		string szLifeTime = string.Empty;
		DateTime curTime = DateTime.Now;

		if (PlayerPrefs.HasKey(szLifeTimeKey) == false)
		{
			szLifeTime = curTime.ToString();
			PlayerPrefs.SetString(szLifeTimeKey, szLifeTime);
		}
		else
			PlayerPrefs.SetString(szLifeTimeKey, m_lifeStartedTime.ToString());

		PlayerPrefs.Save();
	}


	public void updateUserLife()
	{
		TimeSpan tempTimeSpan = DateTime.Now - m_lifeStartedTime;

		if (tempTimeSpan.TotalSeconds > GameData.MaxLifeTime)
		{
			float lifeCountTemp = Mathf.Floor((float)tempTimeSpan.TotalSeconds / (float)GameData.MaxLifeTime);

			if (lifeCountTemp > 0)
			{
				m_lifeStartedTime = m_lifeStartedTime.AddSeconds(GameData.MaxLifeTime * lifeCountTemp);
				saveLifeTime();
			}

			Lifes = (Lifes + (int)lifeCountTemp);
		}
	}


	public override void testValue()
	{
 		base.testValue();

		this.sessionID = "AS12!AS12!";
	}

	#endregion
}
