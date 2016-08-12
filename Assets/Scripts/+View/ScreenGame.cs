using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class ScreenGame : CEBehaviour, 
iPoweupItemCellDelegate, 
IPlayerAnimationDelegate,
ISpriteAnimationPlayer,
IPuzzleNetLogicDelegates
{
	public bool DebugModeIsPrizeCurQuestion = false;
	public bool DebugModeShowAnswer= false;
	
	#region Public Member
	public Sprite m_TrueImg = null;
	public Sprite m_FalseImg = null;
	public Image m_TimeBar = null;
	public Image m_IQLevelBar = null;
	public Text m_txtCategory = null;
	public Text m_Time = null;
	public Text m_IQLevels = null;
	public Text m_Question = null;
	public Image m_BgImg = null;
	public List<Sprite> m_BgImgs = new List<Sprite>();
	public List<Text> m_Answers = new List<Text>();
	public List<Text> m_txtProbabilities = new List<Text>();
	public List<Image> m_QuestionBtnImgs = new List<Image>();
	public List<Sprite> m_QuestionImgs = new List<Sprite>();
	public List<Image> m_HeartLifes = new List<Image>();
	
	public ScreenPowerUps m_powerUps = null;
	public Image m_imgPowerUps = null;
	public SpriteAnimationPlayer m_powerUpAnimator = null;
	
	public List<Color> m_colorsOfProbability = new List<Color>();
	
	public Transform m_chPos = null;
	
	public ScreenWinnerPopup m_winnerPopup = null;
	
	public bool m_correctAnswerOfCurQuestion = false;
	public QuestionInfo m_curQuestionInfo = null;
	
	#endregion Public Member
	
	#region Private Member
	private int m_BtnNum = 0;
	private bool m_PlayAni = false;
	
	private GameObject chObject = null;
	private Player m_player = null;
	
	private int m_chanceOneMore = 0;
	
	private bool m_timeOutProcess = false;
	
	private bool m_isPrizeQuestionWin = false;
	private bool m_isBonusQuestionWin = false;
	
	public bool isAlreadyGiveAnswered = false;
	
	#endregion Private Member
	protected override void OnEnable()
	{
		base.OnEnable();
		m_BtnNum = 0;
		m_PlayAni = false;
		m_timeOutProcess = false;
		
		m_isPrizeQuestionWin = false;
		m_isBonusQuestionWin = false;

        DebugModeShowAnswer = GameData.DebugModeShowAnswer;
		
		UpdateQuestion(gameData.curWorldInfo.curQuestionInfo());
		m_Time.text = GameData.curTime.ToString();
		m_IQLevels.text = ((int)(gameData.userInfo.iq)).ToString();
		
		if (m_BgImg && gameData.curWorldInfo.worldIDX < m_BgImgs.Count)
			m_BgImg.sprite = m_BgImgs[gameData.curWorldInfo.worldIDX];
		
		int i = 0;
		for (i = 0; i < m_HeartLifes.Count; ++i)
			m_HeartLifes[i].gameObject.SetActive(i < gameData.userInfo.Lifes ? true : false);
		
		gameLogic.didStopTimer = EndTime;
		
		for (i = 0; i < m_QuestionBtnImgs.Count; ++i)
			m_QuestionBtnImgs[i].sprite = m_QuestionImgs[2];
		
		for (i = 0; i < m_txtProbabilities.Count; i++)
			m_txtProbabilities[i].enabled = DebugModeShowAnswer;
		
		if (m_imgPowerUps != null)
			m_imgPowerUps.enabled = false;
		
		if (m_powerUps != null)
			m_powerUps.closeItemWnd();
		
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
				m_player.PlayIdleAnimation(this);
		}
		
		m_correctAnswerOfCurQuestion = false;
		
		if (DebugModeShowAnswer)
			powerupAudience(gameData.curWorldInfo.curQuestionInfo());

		m_chanceOneMore = 0;
	}
	
	protected override void OnDisable()
	{
		base.OnDisable();
		
		isAlreadyGiveAnswered = true;
		m_powerUps.closeItemWnd ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_Time.text = GameData.curTime.ToString();
		m_IQLevels.text = ((int)(gameData.userInfo.iq)).ToString();
		for (int i = 0; i < m_HeartLifes.Count; ++i)
		{
			if (i < gameData.userInfo.Lifes)
				m_HeartLifes[i].gameObject.SetActive(true);
			else
				m_HeartLifes[i].gameObject.SetActive(false);
		}
		
		if (m_TimeBar)
			m_TimeBar.fillAmount = (float)GameData.curTime / (float)GameData.totalTimeForQuestion;
		
		if (m_IQLevelBar)
		{
			if (gameData.userInfo.iq <= 0)
				gameData.userInfo.iq = 0;
			
			float iqVal = gameData.userInfo.iq - Mathf.Floor(gameData.userInfo.iq);
			if (iqVal >= 0 && iqVal < 0.25)
				iqVal = 0;
			else if (iqVal >= 0.25 && iqVal < 0.5)
				iqVal = 0.25f;
			else if (iqVal >= 0.5 && iqVal < 0.75)
				iqVal = 0.5f;
			else if (iqVal >= 0.75 && iqVal < 0.99)
				iqVal = 0.75f;
			else
				iqVal = 0;
			m_IQLevelBar.fillAmount = iqVal;
		}
	}
	
	public override void OnEscape()
	{
		base.OnEscape();
		
		if (m_powerUps != null)
		{
			m_powerUps.haveAction = true;
			m_powerUps.closeItemWnd();
		}
	}
	#region Public Function
	public void OnMoreLife()
	{
		if (m_powerUps != null)
		{
			m_powerUps.haveAction = true;
			m_powerUps.openItemWnd(true);
		}
	}

	public void OnBackBtnClick()
	{
		OnEscape ();
	}
	
	public void OnSelAnswer(int index)
	{
		if (m_PlayAni)
			return;
		
		//m_isPrizeQuestionWin = debug ? true : false;
		
		if (isAlreadyGiveAnswered == true && m_chanceOneMore <= 0)
			return;
		
		isAlreadyGiveAnswered = true;
		
		if (gameData.curWorldInfo.curQuestionNum >= 1 && gameData.curWorldInfo.curQuestionInfo() != null)
		{
			QuestionInfo info = gameData.curWorldInfo.curQuestionInfo();
			UserInfo userInfo = gameData.userInfo;
			
			if (info != null && info.answers.Count > index)
			{
				if (m_QuestionBtnImgs[index])
				{
					m_QuestionBtnImgs[index].sprite = info.answers[index].isCorrect ? m_QuestionImgs[0] : m_QuestionImgs[1];
					m_BtnNum = index;
				}

				gameLogic.PlaySound(gameData.userInfo.isMale, info.answers[index].isCorrect);
				if (info.answers[index].isCorrect)
				{
					m_correctAnswerOfCurQuestion = true;

					if (info.questionType == QuestionType.regular_question)
						m_player.PlayFistPumpAnimation(this);
					else
						m_player.PlayHappDanceAnimation(this);
					
					m_PlayAni = true;
					gameData.AnswerStatus = true;

					PuzzleNetLogic.requestUpdatePoints(this);
					
					info.pass(true);
					
					// calculate iq and point
					userInfo.point += 3;
					float stepIQValue = 0f;
					
					if (userInfo.rightCount < 5)
						stepIQValue = 0.5f;
					else
						stepIQValue = 0.75f;
					
					if (info.questionType == QuestionType.bonus_question ||
					    info.questionType == QuestionType.prize_question)
                    {
                        stepIQValue = 1f;
                    }

					// calc user iq
					userInfo.iq += stepIQValue;
					if (userInfo.iq >= GameData.MAX_LIMIT_IQ)
						userInfo.iq = GameData.MAX_LIMIT_IQ;
					
					userInfo.rightCount++;
					
					// if current question is prizeQuestion, request a prize win webservice
					if ((((gameData.curWorldInfo.prizeQuestionPosition == gameData.curWorldInfo.curQuestionNum) &&
						info.questionType == QuestionType.prize_question) && GameData.prizePodState != 0) ||
					    DebugModeIsPrizeCurQuestion )
					{
						Debug.Log("prize Question is correct");
						m_isPrizeQuestionWin = true;

						PuzzleNetLogic.requestPrizeWin(this, index);
					}
					else if (info.questionType == QuestionType.bonus_question)
					{
						Debug.Log("prize Question is correct");
						m_isBonusQuestionWin = true;
					}

					if (gameData.curWorldInfo.curQuestionNum >= 100) 
					{
						Debug.Log("You passed in the world : " + WorldInfo.curWorlNum.ToString());
						WorldInfo.UpdatePassedWorldId((WorldIdentifier)WorldInfo.curWorlNum);
					}
				}
				else
				{
					m_correctAnswerOfCurQuestion = false;
					
					if (info.questionType == QuestionType.regular_question)
						m_player.PlaySadCrossHandAnimation(this);
					else
						m_player.PlayShrugAnimation(this);
					
					
					if (m_chanceOneMore > 1)
					{
						m_chanceOneMore --;
						
						gameLogic.resetUpdateTimer();
						gameLogic.beginUpdateTimer();
						
						return;
					}
					
					m_PlayAni = true;
					gameData.AnswerStatus = false;
					info.pass(false);
					
					userInfo.Lifes--;
					
					// calculate iq and point
					userInfo.rightCount = 0;
					userInfo.iq -= 0.25f;

					if (userInfo.iq <= 0)
						userInfo.iq = 0;
				}
			}

            if ((info.questionType == QuestionType.prize_question) || DebugModeIsPrizeCurQuestion)
            {
                GameData.prizePodState = 0;
                gameData.curWorldInfo.savePrizePodStat();
            }
        }


		gameData.curWorldInfo.nextQuestion();
		//UpdateQuestion(m_CurIndex++);
		StartCoroutine(NextQuestion());
	}
	
	#endregion Public Function
	
	#region Private Function
	private bool OpenWinnerPopup()
	{
		if (m_isPrizeQuestionWin == false && m_isBonusQuestionWin == false)
			return false;
		
		if (m_powerUps != null)
			m_powerUps.closeItemWnd();
		
		ItemKinds kind = ItemInfo.getRandomItemKind();
		m_winnerPopup.m_delegate = callBackCloseWinnerPopup;

		if (m_isBonusQuestionWin) 
			m_winnerPopup.OpenPopup (ScreenWinnerPopupType.ForBonusMessage, kind);
		else if (m_isPrizeQuestionWin) 
			m_winnerPopup.OpenPopup (ScreenWinnerPopupType.ForPrizeMessage, kind);

		m_isPrizeQuestionWin = false;
		m_isBonusQuestionWin = false;

		gameLogic.stopUpdateTimer ();

		return true;
	}
	
	private void UpdateQuestion(QuestionInfo _question)
	{
		gameLogic.resetUpdateTimer();
		gameLogic.beginUpdateTimer();
		
		m_curQuestionInfo = _question;
		m_timeOutProcess = false;
		
		if (_question == null)
			return;
		
		if (m_Question)
			m_Question.text = _question.question;
		
		int i = 0;
		for (i = 0; i < m_Answers.Count; ++i)
		{
			if (!m_Answers[i])
				continue;
			
			if (i < _question.answers.Count)
			{
				m_Answers[i].transform.parent.gameObject.SetActive(true);
				m_Answers[i].text = _question.answers[i].answer;
			}
			else
				m_Answers[i].transform.parent.gameObject.SetActive(false);
		}
		
		if (m_QuestionBtnImgs[m_BtnNum])
			m_QuestionBtnImgs[m_BtnNum].sprite = m_QuestionImgs[2];
		
		if (m_txtProbabilities != null)
		{
			foreach (Text txt in m_txtProbabilities)
				txt.enabled = DebugModeShowAnswer;
		}
		
		m_txtCategory.text = _question.category;
		
		if (m_powerUps != null)
			m_powerUps.closeItemWnd();
	}

	void callBackCloseWinnerPopup()
	{
		if (gameData.userInfo.Lifes > 0) {
			if (WorldInfo.PassedWorlds < (WorldIdentifier)WorldInfo.curWorlNum) {
				gameLogic.ChangeGameState (GameLogic.GameState.Level);
			} else {
				// hhh
			}
		}
		else
		{
			if (gameLogic.IsInGameState(GameLogic.GameState.Game))
				gameLogic.ChangeGameState(GameLogic.GameState.MoreLife);
		}

		m_PlayAni = false;
		m_timeOutProcess = false;
		m_correctAnswerOfCurQuestion = false;
	}

	void goLevelSceneOrMoreLifeScene()
	{
		if (gameData.userInfo.Lifes > 0)
		{
			if (gameLogic.IsInGameState(GameLogic.GameState.Game))
				gameLogic.ChangeGameState(GameLogic.GameState.Level);
		}
		else
		{
			if (gameLogic.IsInGameState(GameLogic.GameState.Game))
				gameLogic.ChangeGameState(GameLogic.GameState.MoreLife);
		}
	}
	
	IEnumerator NextQuestion()
	{
		yield return new WaitForSeconds(2f);
		
		if (m_curQuestionInfo != null)
		{
			if (m_curQuestionInfo.questionType == QuestionType.regular_question &&
			    DebugModeIsPrizeCurQuestion == false)
			{
				goLevelSceneOrMoreLifeScene ();
			}
			else
			{
				if (m_correctAnswerOfCurQuestion == false ||
					(OpenWinnerPopup () == false) )
				{
					yield return new WaitForSeconds (2f);
				
					goLevelSceneOrMoreLifeScene ();
				}
			}
		}
		
		m_PlayAni = false;
		m_timeOutProcess = false;
		m_correctAnswerOfCurQuestion = false;
		
		yield break;
	}
	private void EndTime()
	{
		if (gameObject.activeSelf == false)
			return;
		
		if (m_timeOutProcess != true)
		{
			if (m_player.IsPlayingAnimation(m_player.m_aniType) == false)
			{
				m_player.PlaySadCrossHandAnimation(this);
				gameLogic.PlaySound(gameData.userInfo.isMale, false);
				
				if (m_winnerPopup != null)
					m_winnerPopup.ClosePopup();
				
				if (m_powerUps != null)
					m_powerUps.closeItemWnd();
			}
			
			
			StartCoroutine(GotoLevel());
			gameData.curWorldInfo.nextQuestion();
			StartCoroutine(NextQuestion());
			
			m_timeOutProcess = true;
		}
	}
	
	private IEnumerator GotoLevel()
	{
		yield return new WaitForSeconds(2f);
		
		if (gameLogic.IsInGameState(GameLogic.GameState.Game))
		{
			gameLogic.ChangeGameState(GameLogic.GameState.Level);
			gameData.userInfo.Lifes--;
		}
		
		m_PlayAni = false;
		
		yield break;
	}
	
	public void powerupAudience(QuestionInfo _questinInfo)
	{
		if (_questinInfo == null)
			return;
		
		int correctAnswerIdx = _questinInfo.answers.FindIndex(answer => answer.isCorrect == true);
		
		List<int> nAudiences = GameLogic.Singleton.makeProbabilities(0, 3, 3);
		int nIdx = 0;
		
		for (int idx = 0; idx < 4; idx++)
		{
			m_txtProbabilities[idx].enabled = true;
			
			if (idx == correctAnswerIdx)
			{
				m_txtProbabilities[correctAnswerIdx].text = GameLogic.m_audienceProbailities[3].ToString() + "%";
				m_txtProbabilities[correctAnswerIdx].color = m_colorsOfProbability[3];
			}
			else
			{
				m_txtProbabilities[idx].text = GameLogic.m_audienceProbailities[nAudiences[nIdx]].ToString() + "%";
				m_txtProbabilities[idx].color = m_colorsOfProbability[nAudiences[nIdx]];
				
				nIdx++;
			}
		}
	}
	
	public void powerUpBomb(QuestionInfo _question)
	{
		int correctAnswerIdx = _question.answers.FindIndex(answer => answer.isCorrect == true);
		
		List<int> nAudiences = GameLogic.Singleton.makeProbabilities(0, 3, correctAnswerIdx);
		
		m_txtProbabilities[nAudiences[0]].enabled = true;
		m_txtProbabilities[nAudiences[0]].text = "X";
		m_txtProbabilities[nAudiences[0]].color = m_colorsOfProbability[0];
	}
	
	public void powerUpTimer(QuestionInfo _question)
	{
		gameLogic.resetUpdateTimer();
		gameLogic.beginUpdateTimer();
		//GameLogic.Singleton.resetUpdateDeltaTime(2f);
	}
	
	public void powerUpRope(QuestionInfo _question)
	{
		if (gameData.curWorldInfo.curQuestionInfo() != null)
		{
			UpdateQuestion(gameData.curWorldInfo.nextQuestion());
		}
	}
	
	public void powerUpTornado(QuestionInfo _questionInfo)
	{
		m_chanceOneMore = 2;
		m_timeOutProcess = false;
	}
	
	
	private ItemInfo m_info = null;
	public void didSelectedPowerUpItem(PowerUpItemCell _cell, ItemInfo _itemInfo)
	{
		if (_itemInfo.kind == ItemKinds.tornado) {
			powerUpTornado(gameData.curWorldInfo.curQuestionInfo());
		}
		m_powerUpAnimator.m_delegate = this;
		m_powerUpAnimator.PlayAnimationType (_itemInfo.kind);
		m_info = _itemInfo;
	}
	
	
	#endregion Private Function
	
	public void stopAnimation(Player _player, PlayAnimationType _type)
	{
		Debug.Log("_player stopAnimation");
	}

	public void startAnimation(Player _player, PlayAnimationType _type)
	{
		Debug.Log("_player startAnimation");
	}
	
	
	public bool willStartAnimation(SpriteAnimationPlayer _aniPlayer, ItemKinds _kind)
	{
		// stop game timer
		gameLogic.stopUpdateTimer();

		m_imgPowerUps.enabled = true;
		gameLogic.PlayPowerUpSound(_kind);
		
		return true;
	}
	public bool willFinishAnimation(SpriteAnimationPlayer _aniPlayer, ItemKinds _kind){
		m_imgPowerUps.enabled = false;

		m_info.countOfItem --;
		if (m_info.countOfItem >= 0)
		{
			if (m_info.countOfItem == 0)
			{
				if (gameData.powerUpsItems.ContainsKey(m_info.kind))
					gameData.powerUpsItems.Remove(m_info.kind);
			}
			
			if (m_info.kind == ItemKinds.audience)
				powerupAudience(gameData.curWorldInfo.curQuestionInfo());
			else if (m_info.kind == ItemKinds.tornado)
			{
				//powerUpTornado(gameData.curWorldInfo.curQuestionInfo());
			}
			else if (m_info.kind == ItemKinds.bomb)
				powerUpBomb(gameData.curWorldInfo.curQuestionInfo());
			else if (m_info.kind == ItemKinds.timer)
				powerUpTimer(gameData.curWorldInfo.curQuestionInfo());
			else if (m_info.kind == ItemKinds.rope)
				powerUpRope(gameData.curWorldInfo.curQuestionInfo());
			
			PuzzleNetLogic.requestUpdatePowerUp(this, m_info.kind, 1, false, 0);
		}
		else if (m_info.countOfItem < 0)
		{
			if (gameData.powerUpsItems.ContainsKey(m_info.kind))
				gameData.powerUpsItems.Remove(m_info.kind);
			m_info.countOfItem = 0;
		}

		// stop game timer
		gameLogic.beginUpdateTimer();

		return true;
	}
	
	public bool willEndAnimation(SpriteAnimationPlayer _aniPlayer, ItemKinds _kind){
		m_imgPowerUps.enabled = false;

		return true;
	}


	public void willStartRequest(RequestState _state)
	{
		Debug.Log("willStartRequest" + _state.ToString());
	}

	public void parsingError(RequestState _state)
	{
		Debug.Log("parsingError" + _state.ToString());
	}

	public void didRecievedSuccessResponseFromServer(object _data, RequestState _state)
	{
		Debug.Log("didRecievedSuccessResponseFromServer" + _state.ToString());
	}

	public void didRecievedFailedResponseFromServer(string _message, RequestState _state)
	{
		Debug.Log("didRecievedFailedResponseFromServer" + _state.ToString());
	}

	public void didReceivingError(string _error, RequestState _state)
	{
		Debug.Log("didReceivingError" + _state.ToString());
	}

	public void didReceivingTimeOut(RequestState _state)
	{
		Debug.Log("didReceivingTimeOut" + _state.ToString());
	}

}
