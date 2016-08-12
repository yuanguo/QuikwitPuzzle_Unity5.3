using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;

public class ScreenWorld : CEBehaviour, IPuzzleNetLogicDelegates, IAnimationListenerDelegate, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    #region Public Member
    public Image m_Map = null;
    public Text m_WorldName = null;
	public Text m_WorldDescription = null;
    public List<Sprite> m_MapSprites = new List<Sprite>();
    public RectTransform m_WorldScoreRT = null;
    public Animator m_WorldAni = null;
    public AnimationListener m_listener = null;
	public Button m_btnPlay = null;
    //[Range(0f, 1f)]public float score = 1.0f;


    #endregion Public Member

    #region Private Member
    public Dictionary<int, Sprite> m_MapDicts = new Dictionary<int, Sprite>();
    private float m_WorldScroeWidth = 1.0f;

	private bool canPlayWorld = false;


    #endregion Private Member
	protected override void Awake()
    {
		base.Awake ();
        for (int i = 0; i < m_MapSprites.Count; ++i)
            m_MapDicts.Add(i, m_MapSprites[i]);

        m_WorldScroeWidth = m_WorldScoreRT != null ? m_WorldScoreRT.sizeDelta.x : 1.0f;

        m_listener.m_listenerDelegate = this;
        base.Awake();
    }

	protected override void OnEnable()
    {
		base.OnEnable ();

		if (m_WorldName)
			m_WorldName.text = WorldInfo.getWorldName(WorldInfo.curWorlNum - 1);

		if (m_WorldDescription)
			m_WorldDescription.text = WorldInfo.getWorlDescription(WorldInfo.curWorlNum - 1);

        float score = WorldInfo.getWorldDifficult(WorldInfo.curWorlNum - 1);
        m_WorldScoreRT.sizeDelta = new Vector2(score * m_WorldScroeWidth, m_WorldScoreRT.sizeDelta.y);

		m_WorldAni.SetInteger("globeMapKind", WorldInfo.curWorlNum-1);
        m_WorldAni.SetBool("playAnimation", false);
        m_WorldAni.SetBool("left", true);

		// initialize user's point
		UserInfo.defaultUser().rightCount = 0;
		UserInfo.defaultUser().point = 0;
    }

    void Update()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.Q))
        {
			m_WorldAni.SetInteger("globeMapKind", WorldInfo.curWorlNum-1);
            m_WorldAni.SetBool("left", false);
            m_WorldAni.SetBool("playAnimation", true);
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
			m_WorldAni.SetInteger("globeMapKind", WorldInfo.curWorlNum-1);
            m_WorldAni.SetBool("left", true);
            m_WorldAni.SetBool("playAnimation", true);
        }
#endif
    }

	void requestGameData()
	{
		if (gameLogic.IsInGameState(GameLogic.GameState.World))
		{
			if (WorldInfo.PassedWorlds >= (WorldIdentifier)WorldInfo.curWorlNum)
			{
				if (gameLogic.IsInGameState(GameLogic.GameState.World))
					gameLogic.ChangeGameState(GameLogic.GameState.Level);
			}
			else
			{
				PuzzleNetLogic.requestWorldInfo(this, WorldInfo.curWorlNum, int.Parse(gameData.userInfo.id));
			}
		}
	}

    public void OnHelp()
    {

    }
    public void OnPlay()
	{
		WorldIdentifier curWorldId = (WorldIdentifier)WorldInfo.curWorlNum;

		if (curWorldId > WorldInfo.PassedWorlds)
			canPlayWorld = true;

		if (curWorldId <= (WorldInfo.PassedWorlds + 1))
		{
			if (gameLogic.IsInGameState(GameLogic.GameState.World))
				requestGameData();
		}
		else if (curWorldId > (WorldInfo.PassedWorlds + 1))
		{
			GameLogic.Singleton.ErrorBox("Sorry, You can't play this world.");
		}
    }

	public void OnBackBtnClick()
	{
		gameLogic.ChangeGameState(GameLogic.GameState.SetUp);
	}

	public void stopAnimation(AnimationListener _listener, int _type)
    {
		Debug.Log("_CurWorldNum:" + WorldInfo.curWorlNum.ToString());
		m_WorldAni.SetInteger("globeMapKind", WorldInfo.curWorlNum - 1);
		
		if (m_WorldName)
			m_WorldName.text = WorldInfo.getWorldName(WorldInfo.curWorlNum - 1);

		if (m_WorldDescription)
			m_WorldDescription.text = WorldInfo.getWorlDescription(WorldInfo.curWorlNum - 1);

        float score = WorldInfo.getWorldDifficult(WorldInfo.curWorlNum - 1);
        m_WorldScoreRT.sizeDelta = new Vector2(score * m_WorldScroeWidth, m_WorldScoreRT.sizeDelta.y);
        gameLogic.SetWorldSound(WorldInfo.curWorlNum - 1);
        m_WorldAni.SetBool("playAnimation", false);
    }
    public void startAnimation(AnimationListener _listener, int _type)
    {
    }

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

			if (GameLogic.Singleton.IsInGameState(GameLogic.GameState.World))
				GameLogic.Singleton.ChangeGameState(GameLogic.GameState.Level);
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

	#region IDragEventDelegate methods
	public void OnBeginDrag(PointerEventData eventData)
	{
	}

	public void OnDrag(PointerEventData eventData)
	{
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if ((eventData.pressPosition.x - eventData.position.x) < -100f)
		{
			WorldInfo worldInfo = WorldInfo.prevWorldInfo;
			if (worldInfo.GetType() == typeof(WorldInfo))
			{
				m_WorldAni.SetInteger("globeMapKind", WorldInfo.curWorlNum - 1);
				m_WorldAni.SetBool("left", true);
				m_WorldAni.SetBool("playAnimation", true);
			}
		}
		else if ((eventData.pressPosition.x - eventData.position.x) > 100f)
		{
			WorldInfo worldInfo = WorldInfo.nextWorldInfo;
			if (worldInfo.GetType() == typeof(WorldInfo))
			{
				m_WorldAni.SetInteger("globeMapKind", WorldInfo.curWorlNum - 1);
				m_WorldAni.SetBool("left", false);
				m_WorldAni.SetBool("playAnimation", true);
			}
		}
	}
	#endregion
}
