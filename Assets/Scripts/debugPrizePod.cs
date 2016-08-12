using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

using System;
using System.Collections;
using System.Collections.Generic;


public class debugPrizePod : UIBehaviour
{
    public InputField m_txtPrizeState = null;
    public InputField m_txtPrizePos = null;
    public InputField m_txtPrizeFinishTime = null;

    protected InputField m_lastSelectedInputField = null;


    public InputField m_txtCurQuestionLevel = null;
    public InputField m_txtCurWorldIdx = null;

    public InputField m_txtPassedWorldNumState = null;

    public Toggle m_tglShowAnswers = null;
    protected override void OnEnable()
    {
        base.OnEnable();

        m_lastSelectedInputField = null;
        m_tglShowAnswers.isOn = GameData.DebugModeShowAnswer; 
    }

    public void OnClickApplyBtn()
    {
        GameData.prizePodState = int.Parse(m_txtPrizeState.text);
        WorldInfo.curWorldInfo.prizeQuestionPosition = int.Parse(m_txtPrizePos.text);
        WorldInfo.curWorldInfo.curQuestionNum = int.Parse(m_txtCurQuestionLevel.text);

        WorldInfo.curWorlNum = int.Parse(m_txtCurWorldIdx.text);

        WorldInfo.PassedWorlds = (WorldIdentifier)int.Parse(m_txtPassedWorldNumState.text);

        GameData.DebugModeShowAnswer = m_tglShowAnswers.isOn;
    }

    public void OnClickApplyPrizeFinishTimeBtn()
    {
        if (DateTime.TryParse(m_txtPrizeFinishTime.text, out GameData.Singleton.curWorldInfo.m_prizeFinishTime) == false)
        {
            GameData.Singleton.curWorldInfo.m_prizeFinishTime = new DateTime();
        }

        string szPrizeStartedTimerIDKey = UserInfo.UserInfoIentifier + GameData.Singleton.curWorldInfo.worldIDX.ToString() + "prizeStartedTimer";
        if (PlayerPrefs.HasKey(szPrizeStartedTimerIDKey) == false)
        {
            PlayerPrefs.SetString(szPrizeStartedTimerIDKey, GameData.Singleton.curWorldInfo.m_prizeFinishTime.ToString());
            PlayerPrefs.Save();
        }
    }

    public void OnClickRefreshBtn()
    {
        m_txtPrizeState.text = GameData.prizePodState.ToString();
        m_txtPrizePos.text = WorldInfo.curWorldInfo.prizeQuestionPosition.ToString();
        m_txtCurQuestionLevel.text = WorldInfo.curWorldInfo.curQuestionNum.ToString();
        m_txtCurWorldIdx.text = WorldInfo.curWorlNum.ToString();

        m_txtPrizeFinishTime.text = GameData.Singleton.curWorldInfo.m_prizeFinishTime.ToString();

        m_txtPassedWorldNumState.text = ((int)(WorldInfo.PassedWorlds)).ToString();

        m_tglShowAnswers.isOn = GameData.DebugModeShowAnswer;
    }

    public void OnSelectedInputField(InputField _selectingField)
    {
        m_lastSelectedInputField = _selectingField;
    }
}
