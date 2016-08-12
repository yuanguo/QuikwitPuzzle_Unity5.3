using UnityEngine;
using System.Collections;

using UnityEngine.UI;


using System;

public class Test : CEBehaviour
{

	public Text m_text = null;
	public Text m_textRemain = null;
	public Text m_textRemain1 = null;

	protected DateTime m_awakeupTime;

	protected override void Awake()
	{
		base.Awake();

		m_awakeupTime = DateTime.Now.AddSeconds(1800);
		m_textRemain1.text = m_awakeupTime.ToString();
	}
	void LateUpdate()
	{
		DateTime curTime = DateTime.Now;
		TimeSpan m_remainTime = curTime - m_awakeupTime;
		m_text.text = curTime.ToString();

		m_textRemain.text = m_remainTime.ToString();
	}
}
