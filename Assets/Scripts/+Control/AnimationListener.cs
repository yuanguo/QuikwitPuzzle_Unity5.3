using UnityEngine;
using System.Collections;


public interface IAnimationListenerDelegate
{
	void stopAnimation(AnimationListener _listener, int _type);
	void startAnimation(AnimationListener _listener, int _type);
}

public class AnimationListener : MonoBehaviour
{

	public IAnimationListenerDelegate m_listenerDelegate = null;


	public void OnStopAnimation(int _type)
	{
		Debug.Log("OnStopAnimation" + _type.ToString());

		if (m_listenerDelegate != null)
			m_listenerDelegate.stopAnimation(this, _type);
	}

	public void OnStartAnimation(int _type)
	{
		Debug.Log("OnStartAnimation" + _type.ToString());

		if (m_listenerDelegate != null)
			m_listenerDelegate.startAnimation(this, _type);
	}
}
