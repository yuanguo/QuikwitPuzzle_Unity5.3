using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScreenSetup : CEBehaviour 
{
    public Text m_WorldName = null;
    public List<GameObject> m_HeartGroups = new List<GameObject>();
    public Text m_IQ = null;
    public Text m_Question = null;

	public Transform m_chPos = null;

	public ScreenPowerUps m_screenPowerUp = null;


	private GameObject chObject = null;
    #region Override Function
    // Use this for initialization
	protected override void OnEnable()
    {
		base.OnEnable ();

        if (m_WorldName)
            m_WorldName.text = WorldInfo.getWorldName(WorldInfo.curWorlNum - 1);

        int i = 0, j = 0;
        for (i = 0; i < gameData.userInfo.Lifes; ++i)
        {
            if (i > m_HeartGroups.Count - 1)
                break;

            m_HeartGroups[i].SetActive(true);
        }

        for (j = i; j < m_HeartGroups.Count; ++j)
            m_HeartGroups[j].SetActive(false);

        if (m_IQ)
            m_IQ.text = string.Format("IQ:{0}", (int)(gameData.userInfo.iq));

        if (m_Question)
		{
			int questionLevel = (gameData.curWorldInfo.curQuestionNum) > 100 ? 100 : (gameData.curWorldInfo.curQuestionNum);
			m_Question.text = string.Format("Question : {0}", questionLevel.ToString());
		}

		if (gameLogic.Character != null)
		{
			chObject = gameLogic.Character;
			chObject.transform.SetParent(m_chPos, true);
			chObject.transform.localPosition = Vector3.zero;
			chObject.transform.localRotation = Quaternion.identity;
			chObject.transform.localScale = Vector3.one;

			chObject.SetActive(true);
		}

    }
    #endregion Override Function

    #region Public Function
    public void OnPlay()
    {
        if (gameLogic.IsInGameState(GameLogic.GameState.SetUp))
            gameLogic.ChangeGameState(GameLogic.GameState.World);
    }
    public void OnSetting()
    {
        if (gameLogic.IsInGameState(GameLogic.GameState.SetUp))
            gameLogic.ChangeGameState(GameLogic.GameState.Setting);
    }
    public void OnPowerUp()
    {
//         if (gameLogic.IsInGameState(GameLogic.GameState.SetUp))
//             gameLogic.ChangeGameState(GameLogic.GameState.MoreLife);

		if (m_screenPowerUp != null)
		{
			m_screenPowerUp.haveAction = false;
			m_screenPowerUp.openItemWnd();
		}
    }
    #endregion Public Function

    #region Private Function
    private void Init()
    {
        if (m_WorldName)
            m_WorldName.text = gameData.curWorldInfo.worldName;
    }
    #endregion Private Function
}
