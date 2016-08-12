using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System;
using System.Collections;
using System.Collections.Generic;

public class Levels : List<LevelWnd> { }
//public class LevelSprites : List<Sprite> { }
public class Worlds : Dictionary<int, Levels> { }
//public class WorldSprite : Dictionary<int, LevelSprites> { }

public class LevelWnd
{
    public Button button = null;
    public Sprite srcSprite = null;
    public Text text = null;
}
public class Vec2List : List<Vector2> { }
[SerializeField]
public class Vec3List : List<Vector3> { }
public class ScreenLevel : CEBehaviour, IPointerClickHandler, IPuzzleNetLogicDelegates
{
    public Canvas m_Canvas = null;
    public ScrollRect m_ScrollRect = null;
    public List<Sprite> m_SrcSprites = new List<Sprite>();
    public List<Sprite> m_PrizeSprites = new List<Sprite>();
    private List<Transform> m_WorldObjs = new List<Transform>();
    private Worlds m_Worlds = new Worlds();
    //private WorldSprite m_WorldSprites = new WorldSprite();
    private Vec2List m_WorldImgSizes = new Vec2List();
    private Vec3List m_WorldImgPoses = new Vec3List();
    private List<BezierSpline> m_BezierSplines = new List<BezierSpline>();
    private Dictionary<Sprite, Sprite> m_LevelSprites = new Dictionary<Sprite, Sprite>();       // srcSprite:prizeSprite

	[SerializeField]
	private Text m_txtPrizeTimeMins = null;
	[SerializeField]
	private Text m_txtPrizeTimeIndicator = null;
	[SerializeField]
	private Text m_txtPrizeTimeSeconds = null;

	[SerializeField]
	private ScreenWinnerPopup m_popupWinner = null;

	public int gotoLevel = 0;

	//private int m_LevelIndex = 0;

	// Use this for initialization
	protected override void Awake()
	{
		base.Awake();

		string prefix = "ScrollRect/World{0}/World{1}";
        RectTransform world = null;
        RectTransform trns = null;
        Button btn = null;
        Text txt = null;
        int i = 0;
        for (i = 0; i < 8; ++i)
        {
			world = transform.Find(string.Format(prefix, i, i)) as RectTransform;
            if (!world)
                continue;

            m_WorldObjs.Add(world);
            m_BezierSplines.Add(world.GetComponent<BezierSpline>());
            m_WorldImgSizes.Add(world.sizeDelta);
            m_WorldImgPoses.Add(world.localPosition);
            m_Worlds.Add(i, new Levels());
            //m_WorldSprites.Add(i, new LevelSprites());
            for (int j = 0; j < 100; ++j)
            {
                trns = world.Find(string.Format("{0}", j)) as RectTransform;
                if (trns)
                {
                    btn = trns.gameObject.GetComponent<Button>();
                    txt = trns.Find("Text").gameObject.GetComponent<Text>();
                    LevelWnd lvlWnd = new LevelWnd();
                    lvlWnd.button = btn;
                    lvlWnd.text = txt;
                    lvlWnd.srcSprite = btn.image.sprite;
                    m_Worlds[i].Add(lvlWnd);
                }
            }
        }

		m_ScrollRect.normalizedPosition = new Vector2(0.5f, 0f);

        for (i = 0; i < m_SrcSprites.Count; ++i)
            m_LevelSprites.Add(m_SrcSprites[i], m_PrizeSprites[i]);
    }
    protected override void OnEnable()
	{
		base.OnEnable();
        UpdateWorlds();

		gameData.curWorldInfo.m_screenLevel = this;

        int worldNum = gameData.curWorldInfo.worldIDX;
        RectTransform trn = m_WorldObjs[worldNum] as RectTransform;
        if (m_ScrollRect)
			m_ScrollRect.content = (RectTransform)trn.parent;

        trn.localScale = Vector3.one;
        trn.localPosition = m_WorldImgPoses[worldNum];

        if (m_BezierSplines[worldNum] && gameData.AnswerStatus)
        {
            m_BezierSplines[worldNum].StartAction();
            gameData.AnswerStatus = false;
            //m_BezierSplines[worldNum].Index = ++m_LevelIndex;
        }

		UpdateLevels();
    }

	// Update is called once per frame
    void Update()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        if (Input.GetMouseButtonUp(1))
            Resize();
#elif UNITY_ANDROID
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began && touch.tapCount >= 2)
            {
                Resize();
                break;
            }
        }
#endif
    }
    #region Public Function
	//public int LevelIndex
	//{
	//	get { return m_LevelIndex; }
	//	set 
	//	{ 
	//		m_LevelIndex = value;
	//		UpdateLevels();
	//	}
	//}
    public void OnHelp()
    {

    }
    public void OnClickLevel(int _level)
    {
		if (gameData.userInfo.Lifes <= 0)
		{
			gameLogic.ChangeGameState(GameLogic.GameState.MoreLife);
			return;
		}

        if (gameLogic.IsInGameState(GameLogic.GameState.Level))
        {
            gameLogic.ChangeGameState(GameLogic.GameState.Game);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //if (eventData.clickCount >= 2)
        //    Resize();
    }
    public void UpdateLevels()
    {
        int worldNum = gameData.curWorldInfo.worldIDX;

        if (!m_Worlds.ContainsKey(worldNum))
            return;

        Levels levels = m_Worlds[worldNum];
        int curLevel = Mathf.Max(0, Mathf.Min(gameData.curWorldInfo.curQuestionNum - 1, levels.Count));

        if (gameData.curWorldInfo.m_prizeFinishTime < DateTime.Now)
        {
            GameData.prizePodState = 0;
            gameData.curWorldInfo.savePrizePodStat();
        }


        int i = 0;
        for (i = 0; i < levels.Count; ++i)
        {
            levels[i].button.interactable = i == curLevel;
            if (i == (gameData.curWorldInfo.prizeQuestionPosition - 1) &&
                (gameData.curWorldInfo.prizeQuestionPosition - 1) > 0 &&
				GameData.prizePodState == 1)
            {
                levels[i].button.image.sprite = m_LevelSprites[levels[i].srcSprite];
                levels[i].text.enabled = false;
            }
            else
            {
                levels[i].button.image.sprite = levels[i].srcSprite;
                levels[i].text.enabled = true;
            }
        }

		m_ScrollRect.normalizedPosition = new Vector2(0.5f, 0f);

		// already done
		if (WorldInfo.PassedWorlds >= (WorldIdentifier)WorldInfo.curWorlNum)
			m_popupWinner.OpenPopup (ScreenWinnerPopupType.ForWinnerMessage);

		UpdateFocus(gameData.curWorldInfo.curQuestionNum - 1);
    }

    public void UpdateFocus(int levelNum)
    {
        if (!m_ScrollRect)
            return;

		levelNum = levelNum >= 100 ? 99 : levelNum;

		if (m_ScrollRect != null)
		{
			int worldNum = gameData.curWorldInfo.worldIDX;

			if (!m_Worlds.ContainsKey(worldNum))
				return;

			Levels levels = m_Worlds[worldNum];

			RectTransform rectOfPod = levels[levelNum].button.GetComponent<RectTransform>();
			RectTransform containner = m_ScrollRect.content;
			if (rectOfPod != null)
			{
				float x = rectOfPod.anchoredPosition.x / containner.sizeDelta.x;
				float y = (containner.sizeDelta.y - Mathf.Abs(rectOfPod.anchoredPosition.y) - rectOfPod.sizeDelta.y) / containner.sizeDelta.y;

				m_ScrollRect.normalizedPosition = new Vector2(x, y);
			}

		}
    }

	public void OpenWinnerPopup()
	{
		m_popupWinner.OpenPopup(ScreenWinnerPopupType.ForWinnerMessage);
	}

	public void OnBackBtnClick()
	{
		gameLogic.ChangeGameState(GameLogic.GameState.SetUp);
	}

	public void requestWorldInfosAgain()
	{
		PuzzleNetLogic.requestWorldInfo(this, WorldInfo.curWorlNum, int.Parse(gameData.userInfo.id));
	}
    #endregion Public Function

    #region Private Function
    void UpdateWorlds()
    {
        int i = 0;
        int worldNum = gameData.curWorldInfo.worldIDX;
        for (i = 0; i < m_WorldObjs.Count; ++i)
			m_WorldObjs[i].parent.gameObject.SetActive(i == worldNum);
            
        Resize();
    }
    private void Resize()
    {
        int worldNum = gameData.curWorldInfo.worldIDX;
        RectTransform trn = m_WorldObjs[worldNum] as RectTransform;
        if (trn.localScale != Vector3.one)
        {
            trn.localScale = Vector3.one;
            trn.localPosition = m_WorldImgPoses[worldNum];
        }
        else
        {
            trn = m_WorldObjs[worldNum] as RectTransform;
            //float ratioX = Screen.width / m_WorldImgSizes[worldNum].x;
            //float ratioY = Screen.height / m_WorldImgSizes[worldNum].y;

            float ratioX = m_Canvas.pixelRect.width / m_WorldImgSizes[worldNum].x;
            float ratioY = m_Canvas.pixelRect.height / m_WorldImgSizes[worldNum].y;
            float canvasScaleX = 1 / m_Canvas.transform.localScale.x;
            float canvasScaleY = 1 / m_Canvas.transform.localScale.y;
            trn.localScale = new Vector3(ratioX * canvasScaleX, ratioY * canvasScaleY, 1f);
            trn.anchoredPosition = Vector3.zero;
        }
    }

	void LateUpdate()
	{
		if (m_txtPrizeTimeMins != null &&
		    m_txtPrizeTimeIndicator != null &&
		    m_txtPrizeTimeSeconds != null)
		{
			m_txtPrizeTimeMins.transform.parent.gameObject.SetActive(false);

			if (gameData.curWorldInfo.prizeQuestionPosition < gameData.curWorldInfo.curQuestionNum ||
                GameData.prizePodState != 1)
				return;

			if (gameData.curWorldInfo.m_prizeFinishTime >= DateTime.Now)
			{
				m_txtPrizeTimeMins.transform.parent.gameObject.SetActive(true);
				TimeSpan remainTime = gameData.curWorldInfo.m_prizeFinishTime - DateTime.Now;

				bool indicator = (remainTime.Seconds % 2) == 1;
				m_txtPrizeTimeMins.text = remainTime.Minutes.ToString("00");
				m_txtPrizeTimeIndicator.enabled = indicator;
				m_txtPrizeTimeSeconds.text = remainTime.Seconds.ToString("00");
			}
            else
            {
                GameData.prizePodState = 0;
                gameData.curWorldInfo.savePrizePodStat();
            }

		}
	}
    #endregion Private Function


	#region INetManagerDelegate methods
	public void willStartRequest(RequestState _state)
	{
		if (_state == RequestState.RequestWorldInfo)
			CELoadingBar.startLoadingAnimation("loading Questions ...");
	}
	public void parsingError(RequestState _state)
	{
		if (_state == RequestState.RequestWorldInfo)
			CELoadingBar.stopLoadingAnimation();
	}
	public void didRecievedSuccessResponseFromServer(object _data, RequestState _state)
	{
		if (_state == RequestState.RequestWorldInfo)
		{
			CELoadingBar.stopLoadingAnimation();
			gameData.curWorldInfo.parsingData(_data);
			gameData.curWorldInfo.nextQuestion();
		}
	}
	public void didRecievedFailedResponseFromServer(string _message, RequestState _state)
	{
		if (_state == RequestState.RequestWorldInfo)
			CELoadingBar.stopLoadingAnimation();
	}
	public void didReceivingError(string _error, RequestState _state)
	{
		if (_state == RequestState.RequestWorldInfo)
			CELoadingBar.stopLoadingAnimation();
	}
	public void didReceivingTimeOut(RequestState _state)
	{
		if (_state == RequestState.RequestWorldInfo)
			CELoadingBar.stopLoadingAnimation();
	}
	#endregion

}
