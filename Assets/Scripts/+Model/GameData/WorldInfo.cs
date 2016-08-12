using UnityEngine;


using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public enum WorldIdentifier
{
	None = 0x00,
	Polomia = 0x01,
	Alonis = 0x02,
	Octiva = 0x03,
	Winipex = 0x04,
	Semilan = 0x05,
	Galeo = 0x06,
	Zonis = 0x07,
	Cattica = 0x08,
};

public class QuestionsManager
{
	private CollectQuestions m_allQuestions = new CollectQuestions();
	private QuestionInfo m_curQuestion = null;

	public QuestionInfo getCurQuestion()
	{
		return m_curQuestion;
	}

	public void setQuestions(CollectQuestions _questions)
	{
		m_allQuestions.Clear();
		m_curQuestion = null;

		if (_questions != null && _questions.Count > 0)
		{
			m_allQuestions.AddRange(_questions);
			nextQuestion();
		}
	}

	/// <summary>
	/// if m_allQuestion's table has any questions, return first question
	/// else return null
	/// </summary>
	/// <returns>QuestionInfo / null</returns>
	public QuestionInfo nextQuestion()
	{
		// get the resultQuestion
		QuestionInfo resultQuestion = null;
		if (m_allQuestions.Count >= 1)
			resultQuestion = m_allQuestions[0];

		m_curQuestion = resultQuestion;

		if (m_curQuestion != null)
		{
			if (m_allQuestions.Contains(m_curQuestion))
				m_allQuestions.Remove(m_curQuestion);
		}

		return m_curQuestion;
	}

	public int getQuestionCount()
	{
		return m_allQuestions == null ? 0 : m_allQuestions.Count;
	}

	public void init()
	{
		if (m_allQuestions == null)
			m_allQuestions = new CollectQuestions();

		m_allQuestions.Clear();
		m_curQuestion = null;
	}
}

public class WorldInfo : CEObject
{
	public static int MAX_WORLD_COUNT = 8;
    public static int MAX_QUESTION_COUNT = 100;

	public static WorldIdentifier PassedWorlds = WorldIdentifier.None;

	#region public members and properties
	protected static string[] strWorlName = {
												"Polomia", 
												"Allonis",  
												"Octiva",  
												"Winipex",  
												"Semilan",  
												"Galeo",  
												"Zonis",  
												"Cattica",  
											};

	protected static string[] strWordDescription = {
													   "An underwater adventure awaits you. Are you prepared for the challenge beneath the sea?",
													   "The questions get a bit harder, are you ready for the wilderness? Enter the dark forest of Allonis and find out.",
													   "Throw on your jacket, grab your boots and prepare for the snowy world of Octiva. Can you handle the cold? ",
													   "Come enjoy the sun and go for a ride. Bring your sun screen because the question heat up and get a bit more.",
													   "The falls behind the mist provides a very exotic scenery. However don’t be fooled these questions will test you like never before.",
													   "When the sun sets the moon rises. Let the moonlight guide you thru the world of Galeo. You’ve made it this far, is this the end  of the road?",
													   "Bright lights, big city.  Are you prepared for the fast pace environment of Zonis?  Don’t get caught up and left behind.",
													   "Is this a mirage?  You’ve made it to Cattica the hardest world yet.  The end is near or is it?",
												   };

	protected static float[] worldDifficulties = { 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f };

	/*<-- members */
	public static int worldInstanceID = 0;

	public int worldIDX = 0;

	public int prizePodState{
		get{ 
			if ((prizeQuestionPosition - 1) >= (curQuestionNum - 1))
				return m_prizePodState;

			return 0;
		}
		set{ m_prizePodState = value; }
	}

	protected int m_prizePodState = 1;

	public string worldName = string.Empty;

	public int curQuestionNum = 1;


	public int prizeQuestionPosition = -1;

	public bool showPrizeQuestion = false;
	public DateTime m_prizeFinishTime;

	public ScreenLevel m_screenLevel = null;

	private QuestionsManager m_regularQuestions = new QuestionsManager();
	private QuestionsManager m_bonusQuestions = new QuestionsManager();
	private QuestionsManager m_prizeQuestions = new QuestionsManager();

	protected static int m_curWorldNum = 1;
	private QuestionInfo m_curQuestion = null;


	/*<-- properties */
	public static int curWorlNum {
		get { return m_curWorldNum; }
		set { m_curWorldNum = value; }
	}

	public static WorldInfo curWorldInfo {
		get	{ return allWorldInfos[curWorlNum]; }
	}

	public static WorldInfo nextWorldInfo {
		get {
			curWorlNum++;
			if (curWorlNum > allWorldInfos.Count)
				curWorlNum = 1;
			return allWorldInfos[curWorlNum - 1];
		}
	}

	public static WorldInfo prevWorldInfo
	{
		get 
		{ 
			curWorlNum--;
			if (curWorlNum < 1)
				curWorlNum = allWorldInfos.Count;
			return allWorldInfos[curWorlNum - 1];
		}
	}

	public static string getWorldName(int _worldID)
	{
		if (_worldID < strWorlName.Length &&
			_worldID >= 0)
			return strWorlName[_worldID];

		return string.Empty;
	}

	public static string getWorlDescription(int _worldID)
	{
		if (_worldID < strWordDescription.Length &&
			_worldID >= 0)
			return strWordDescription[_worldID];

		return string.Empty;
	}

	public static float getWorldDifficult(int _worldID)
	{
		if (_worldID < worldDifficulties.Length &&
			_worldID >= 0)
			return worldDifficulties[_worldID];

		return worldDifficulties[0];
	}

	public static CollectWorlds allWorldInfos {
		get	{
			if (s_allWorldInfos == null)
                s_allWorldInfos = new CollectWorlds();
            
			return s_allWorldInfos;
		}
	}

	#endregion


	#region protected and private members
	protected static CollectWorlds s_allWorldInfos = null;
	#endregion

	public WorldInfo()
	{
		worldInstanceID++;
	}

	public WorldInfo(int _worldNum)
	{
		worldInstanceID++;
		worldIDX = _worldNum;
	}

	~WorldInfo()
	{
		worldInstanceID--;

		if (m_regularQuestions != null)
			m_regularQuestions.init();

		if (m_bonusQuestions != null)
			m_bonusQuestions.init();

		if (m_prizeQuestions != null)
			m_prizeQuestions.init();

		m_curQuestion = null;
	}

	public override void InitObject()
	{
		base.InitObject();

		if (m_regularQuestions == null)
			m_regularQuestions = new QuestionsManager();
		m_regularQuestions.init();

		if (m_bonusQuestions == null)
			m_bonusQuestions = new QuestionsManager();
		m_bonusQuestions.init();



		if (m_prizeQuestions == null)
			m_prizeQuestions = new QuestionsManager();
		m_prizeQuestions.init();


		worldName = string.Empty;
		worldIDX = 0;

        prizeQuestionPosition = -1;

		m_prizeFinishTime = new DateTime();

		m_screenLevel = null;
	}


	public QuestionInfo curQuestionInfo()
	{
		if (m_curQuestion == null)
			m_curQuestion = nextQuestion();

		return m_curQuestion;
	}

	public QuestionInfo nextQuestion(bool _changeCurQuestion = true)
	{
		QuestionType _type = QuestionType.none;

		QuestionInfo resultQuestion = null;

		if (m_curQuestion != null)
			_type = m_curQuestion.questionType;

		// prize question
		if (m_curQuestion != null)
		{
			if (_type != QuestionType.prize_question)
			{
				if (m_curQuestion.isPassed == true)
				{
					// getting a prize question (prize_question_position == curQuestionNum)
					if ((prizeQuestionPosition - 1) >= 0 &&
						(prizeQuestionPosition - 1) == curQuestionNum)
					{
						resultQuestion = m_prizeQuestions.nextQuestion();
						if (resultQuestion == null)
						{
							if (m_screenLevel != null)
								m_screenLevel.requestWorldInfosAgain();

							return null;
						}

						if (m_curQuestion != null)
						{
							if (_changeCurQuestion == true)
								m_curQuestion = resultQuestion;
							return m_curQuestion;
						}
					}
				}
			}
		}

		// bonus question
		// if last question(curquestion)'s type is bonus,
		// if cur question's position is the position of the bonus's pod
		// if next pod's position(curQuestionNum + 2) == 10 is the position of the bonus's pod
		{
			if ((m_curQuestion == null  || _type == QuestionType.none) && (curQuestionNum % 10) == 0)
			{
				resultQuestion = m_bonusQuestions.nextQuestion();
				if (resultQuestion == null)
				{
					if (m_screenLevel != null)
						m_screenLevel.requestWorldInfosAgain();

					return null;
				}

				if (resultQuestion != null)
				{
					if (_changeCurQuestion == true)
						m_curQuestion = resultQuestion;
					return resultQuestion;
				}
			}

			if (m_curQuestion != null)
			{
				if (((_type == QuestionType.bonus_question) && m_curQuestion.isPassed == false) ||
					((_type != QuestionType.bonus_question) && (m_curQuestion.isPassed == true) && (curQuestionNum % 10) == 0))
				{
					resultQuestion = m_bonusQuestions.nextQuestion();
					if (resultQuestion == null)
					{
						if (m_screenLevel != null)
							m_screenLevel.requestWorldInfosAgain();

						return null;
					}

					if (resultQuestion != null)
					{
						if (_changeCurQuestion == true)
							m_curQuestion = resultQuestion;
						return resultQuestion;
					}
				}
			}
		}

		// regular question
		resultQuestion = m_regularQuestions.nextQuestion();
		if (resultQuestion == null)
		{
			if (m_screenLevel != null)
				m_screenLevel.requestWorldInfosAgain();

			return null;
		}

		if (_changeCurQuestion == true)
			m_curQuestion = resultQuestion;
		return resultQuestion;
	}

    private const string WorldInfoIentifier = "World_";
    public static void UpdatePassedWorldId(WorldIdentifier _id)
    {
		if (_id > PassedWorlds)
		{
			PassedWorlds = _id;
			PlayerPrefs.SetInt(WorldInfoIentifier + UserInfo.defaultUser().id + "PassedWorlds", (int)WorldInfo.PassedWorlds);
			PlayerPrefs.Save();
		}
    }

    public static void LoadPassedWorldID()
    {
		WorldInfo.PassedWorlds = (WorldIdentifier)(PlayerPrefs.GetInt(WorldInfoIentifier + UserInfo.defaultUser().id + "PassedWorlds", 0));
    }

	#region private methods

	private bool newAnswerFromDict(Dictionary<string, object> _dict, QuestionInfo _questionInfo)
	{
		if (_dict == null || _questionInfo == null)
			return false;

		if (_dict.ContainsKey("id"))
			_questionInfo.id = _dict["id"].ToString();

		if (_dict.ContainsKey("question"))
			_questionInfo.question = _dict["question"].ToString();

		if (_dict.ContainsKey("type"))
			_questionInfo.type = _dict["type"].ToString();

		if (_dict.ContainsKey("cat"))
			_questionInfo.category = _dict["cat"].ToString();

		if (_dict.ContainsKey("choices"))
		{
			List<object> answerTemps = (List<object>)_dict["choices"];

			_questionInfo.answers.Clear();

			foreach (object answerInfo in answerTemps)
			{
				Dictionary<string, object> answerTemp = (Dictionary<string, object>)answerInfo;
				Answer answer = new Answer();

				if (answerTemp.ContainsKey("answer"))
					answer.answer = answerTemp["answer"].ToString();

				if (answerTemp.ContainsKey("iscorrect"))
					answer.isCorrect = bool.Parse(answerTemp["iscorrect"].ToString());

				_questionInfo.answers.Add(answer);
			}

		}

		return true;
	}



	private void getQuestions(Dictionary<string, object> _data, string _questionType, CollectQuestions _questions)
	{
		if (_questions == null ||
			_data == null ||
			_questionType == null)
			return;

		_questions.Clear();
		// bonus_question
		if (_data.ContainsKey(_questionType))
		{
			List<object> questions = (List<object>)_data[_questionType];

			if (questions != null && questions.Count > 0)
			{
				foreach (object questionA in questions)
				{
                    QuestionInfo question = new QuestionInfo();
                    newAnswerFromDict((Dictionary<string, object>)questionA, question);
                    _questions.Add(question);

                    if (_questionType == "regular_question")
                        question.type = "regular_question";
                    else if (_questionType == "bonus_question")
                        question.type = "bonus_question";
                    else if (_questionType == "prize_question")
                        question.type = "prize_question";
                    else
                        question.type = "regular_question";
				}
			}
		}

	}

	public bool parsingData(object _data)
	{
		Dictionary<string, object> data = (Dictionary<string, object>)_data;
		{
			if (data != null &&
				data.Count > 0)
			{
				// regular_questions
				CollectQuestions tempQuestions = new CollectQuestions();
				getQuestions(data, "regular_question", tempQuestions);
				m_regularQuestions.setQuestions(tempQuestions);

				// bonus_question
				getQuestions(data, "bonus_question", tempQuestions);
				m_bonusQuestions.setQuestions(tempQuestions);

				// prize_questions
				getQuestions(data, "prize_question", tempQuestions);
				m_prizeQuestions.setQuestions(tempQuestions);


				Debug.Log("worldinfo finish parsing question data");

				if (data.ContainsKey("prize_question_position"))
				{
					object objPrizeQuestionPositions = data["prize_question_position"];

					if (objPrizeQuestionPositions != null &&
						objPrizeQuestionPositions.ToString() != string.Empty)
					{
						prizeQuestionPosition = int.Parse(data["prize_question_position"].ToString());
					}
					else
						Debug.LogError("prizeQuestionPosition don't have any val");
				}

				if (data.ContainsKey("prize_question_show"))
				{
					string strPrizeQuestionShow = data["prize_question_show"].ToString();
					if (strPrizeQuestionShow != string.Empty)
						showPrizeQuestion = strPrizeQuestionShow == "false" ? false : true;
				}


				if (showPrizeQuestion == true ||
					prizePodState == 1)
				{
					prizePodState = 1;
					// it's the time that prize item was started
					string szPrizeStartedTimerIDKey = UserInfo.UserInfoIentifier + worldIDX.ToString() + "prizeStartedTimer";
					string szPrizeStaredTime = string.Empty;
					DateTime curTime = DateTime.Now.AddSeconds(1800);

					// if prize time was not started, the time register on player prefab.
					if (PlayerPrefs.HasKey(szPrizeStartedTimerIDKey) == false)
					{
						szPrizeStaredTime = curTime.ToString();
						PlayerPrefs.SetString(szPrizeStartedTimerIDKey, szPrizeStaredTime);
						PlayerPrefs.Save();
					}
					else
						szPrizeStaredTime = PlayerPrefs.GetString(szPrizeStartedTimerIDKey, curTime.ToString());

					if (DateTime.TryParse(szPrizeStaredTime, out m_prizeFinishTime) == false)
					{
						m_prizeFinishTime = new DateTime();
						Debug.LogError("Couldn't parsing the DateTime from string : " + szPrizeStaredTime);
					}
					else
						Debug.LogWarning("Success to parsing the DateTime");
				}


				return true;
			}
		}
		return false;
	}

	#endregion


	public void saveData()
	{
		savePrizePodStat ();
	}

	public void loadPrizeInfo()
	{
		string prizePodStateKey = UserInfo.UserInfoIentifier + worldIDX.ToString() + "prizePodStateKey";
		if (PlayerPrefs.HasKey(prizePodStateKey))
			GameData.prizePodState = PlayerPrefs.GetInt(prizePodStateKey, 1);
		else
			GameData.prizePodState = 1;
	}


	public void savePrizePodStat()
	{
		string prizePodStateKey = UserInfo.UserInfoIentifier + worldIDX.ToString () + "prizePodStateKey";

		PlayerPrefs.SetInt(prizePodStateKey, GameData.prizePodState);
		PlayerPrefs.Save();
	}
}

public class CollectWorlds : List<WorldInfo> 
{
	public CollectWorlds()
	{
		for (int nIdx = 0; nIdx < WorldInfo.MAX_WORLD_COUNT; nIdx++)
			Add(new WorldInfo(nIdx));
	}
}

