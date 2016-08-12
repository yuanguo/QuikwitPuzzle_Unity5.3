using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScreenWitBank : CEBehaviour
{
	public void openScreen()
	{
		rectTransform.SetAsLastSibling();
		gameObject.SetActive(true);
	}
	
	public void closeScreen()
	{
		rectTransform.SetAsFirstSibling();
		gameObject.SetActive(false);
	}
	
	public void OnCloseBtnClick()
	{
		closeScreen();
	}
}
