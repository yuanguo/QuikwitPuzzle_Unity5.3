using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System.Collections;

public class CEButton : Button
{
	#region UI members
	[FormerlySerializedAs("textTarget")]
	[SerializeField]
	private Text m_textTarget = null;
	[FormerlySerializedAs("colorsForText")]
	[SerializeField]
	private ColorBlock m_colorsForText = ColorBlock.defaultColorBlock;
	#endregion

	protected override void DoStateTransition(SelectionState state, bool instant)
	{
		base.DoStateTransition(state, instant);

		Color tintColor;

		switch (state)
		{
			case SelectionState.Normal:
				tintColor = m_colorsForText.normalColor;
				break;
			case SelectionState.Highlighted:
				tintColor = m_colorsForText.highlightedColor;
				break;
			case SelectionState.Pressed:
				tintColor = m_colorsForText.pressedColor;
				break;
			case SelectionState.Disabled:
				tintColor = m_colorsForText.disabledColor;
				break;
			default:
				tintColor = Color.black;
				break;
		}

		if (gameObject.activeInHierarchy)
		{
			if (m_textTarget != null)
				m_textTarget.color = tintColor;
		}
	}

}
