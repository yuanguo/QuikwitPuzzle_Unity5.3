using UnityEngine;
using System.Collections;

using UnityEngine.UI;


public class CELoadingBarBehaviour : CEBehaviour
{
	#region UI members
	[SerializeField] protected Image m_loadingBarBg = null;
	[SerializeField] protected Image m_loadingBar = null;
	[SerializeField] protected Text m_loadingBarText = null;
	#endregion

	#region public members
	public float smooth = -250F;
	#endregion

	#region protected members
	protected static CELoadingBar s_loadingBar = null;

	[SerializeField] protected bool m_loadingAnimationState = true; // false : stop, true : start animation
	#endregion

	#region public API Methods
	public void startAnimation(string _loadingText = "Loading ...")
	{
		m_loadingBar.rectTransform.rotation = Quaternion.identity;
		_startAnimation(_loadingText);
	}

	public void stopAnimation()
	{
		m_loadingBar.rectTransform.rotation = Quaternion.identity;
		_stopAnimation();
	}

	#endregion

	#region protected methods
	protected void _stopAnimation()
	{
		m_loadingAnimationState = false;
	}

	protected void _startAnimation(string _loadingText = "Loading ...")
	{
		m_loadingAnimationState = true;
		transform.position = new Vector3(transform.position.x, transform.position.y, -300);
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -300);

		if (m_loadingBarText != null)
			m_loadingBarText.text = _loadingText;
	}

	protected void Update()
	{
		if (m_loadingAnimationState == true)
		{
			m_loadingBar.rectTransform.Rotate(0f, 0f, Time.deltaTime * smooth);
		}
	}
	#endregion
}

public class CELoadingBar : CEObject
{
	public static CELoadingBar s_loadingBar = null;

	private static CELoadingBarBehaviour s_loadingBarBehaviour = null;
	private static GameObject s_loadingBarObject = null;

	public static bool isPlayingLoadingAni = false;

	private string m_loadingBarPrefabName = "LoadingBar";

	public static CELoadingBar defaultLoadingBar
	{
		get
		{
			if (s_loadingBar == null)
				s_loadingBar = new CELoadingBar();

			return s_loadingBar;
		}
	}


	public CELoadingBar()
	{
		initLoadingBar();
	}

	public void initLoadingBar ()
	{
		loadGameObject();
	}

	public static void startLoadingAnimation (string _loadingText = "Loading ...")
	{
		if (s_loadingBarObject == null)
			defaultLoadingBar.loadGameObject();

		if (s_loadingBarObject != null)
		{
			s_loadingBarObject.SetActive(true);

			if (s_loadingBarBehaviour != null)
				s_loadingBarBehaviour.startAnimation(_loadingText);

			isPlayingLoadingAni = true;
		}
	}

	public static void stopLoadingAnimation ()
	{
		if (s_loadingBarObject != null)
		{
			s_loadingBarObject.SetActive(false);

			if (s_loadingBarBehaviour != null)
				s_loadingBarBehaviour.stopAnimation();

			isPlayingLoadingAni = false;
		}
	}


	private GameObject loadGameObject ()
	{
		if (s_loadingBarObject == null)
		{
			GameObject obj = Resources.Load<GameObject>("Prefabs/" + m_loadingBarPrefabName);
			s_loadingBarObject = GameObject.Instantiate(obj) as GameObject;

			if (s_loadingBarObject != null &&
				s_loadingBarBehaviour == null)
				s_loadingBarBehaviour = s_loadingBarObject.GetComponent<CELoadingBarBehaviour>();

			RectTransform rectOfLoadingBar = s_loadingBarBehaviour.rectTransform;
			if (CEBehaviour.defaultParent != null)
				rectOfLoadingBar.SetParent(CEBehaviour.defaultParent.transform);

			rectOfLoadingBar.localPosition = Vector3.zero;
			rectOfLoadingBar.localScale = Vector3.one;
			rectOfLoadingBar.localRotation = Quaternion.identity;

			rectOfLoadingBar.anchoredPosition = Vector2.zero;
			rectOfLoadingBar.sizeDelta = Vector2.one;

			rectOfLoadingBar.SetAsLastSibling();

			return s_loadingBarObject;
		}

		return s_loadingBarObject;
	}
}


