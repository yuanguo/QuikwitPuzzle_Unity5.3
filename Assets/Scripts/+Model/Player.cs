using UnityEngine;
using System.Collections;

public enum PlayAnimationType
{
	idle = 0,
	walking,
	fistPump,
	happyDance,
	shrug,
	sadCrossHand,
}

public interface IPlayerAnimationDelegate
{
	void stopAnimation(Player _player, PlayAnimationType _type);
	void startAnimation(Player _player, PlayAnimationType _type);
}

public class Player : MonoBehaviour 
{
	public Animation m_animation = null;
	public PlayAnimationType m_aniType = PlayAnimationType.idle;

	public IPlayerAnimationDelegate m_delegate = null;

	private string[] animationName = {			"idle",
												"walking",
												"fistPump",
												"happyDance",
												"shrug",
												"sadCrossHand",
										   };

	

	public void OnStartAnimation(PlayAnimationType _type)
	{
		if (m_delegate != null)
			m_delegate.startAnimation(this, _type);
	}

	public void OnStopAnimation(PlayAnimationType _type)
	{
		if (m_delegate != null)
			m_delegate.stopAnimation(this, _type);
	}

	public void PlayIdleAnimation(IPlayerAnimationDelegate _delegate = null)
	{
		if (_delegate != null)
			m_delegate = _delegate;

		if (m_animation != null)
		{
			m_animation.Stop();
			m_animation.Play(animationName[(int)PlayAnimationType.idle]);
			m_aniType = PlayAnimationType.idle;
		}
	}

	public void StopAllAnimation(IPlayerAnimationDelegate _delegate = null)
	{
		if (_delegate != null)
			m_delegate = _delegate;

		if (m_animation != null)
			m_animation.Stop();
	}

	public void PlayWalkingAnimation(IPlayerAnimationDelegate _delegate = null)
	{
		if (_delegate != null)
			m_delegate = _delegate;

		if (m_animation != null)
		{
			//m_animation.Stop();
			m_animation.Play(animationName[(int)PlayAnimationType.walking]);
			m_aniType = PlayAnimationType.walking;
		}
	}

	public void PlayFistPumpAnimation(IPlayerAnimationDelegate _delegate = null)
	{
		if (_delegate != null)
			m_delegate = _delegate;

		if (m_animation != null)
		{
			m_animation.Stop();
			m_animation.Play(animationName[(int)PlayAnimationType.fistPump]);
			m_aniType = PlayAnimationType.fistPump;
		}
	}

	public void PlayHappDanceAnimation(IPlayerAnimationDelegate _delegate = null)
	{
		if (_delegate != null)
			m_delegate = _delegate;

		if (m_animation != null)
		{
			m_animation.Stop();
			m_animation.Play(animationName[(int)PlayAnimationType.happyDance]);
			m_aniType = PlayAnimationType.happyDance;
		}

	}

	public void PlayShrugAnimation(IPlayerAnimationDelegate _delegate = null)
	{
		if (_delegate != null)
			m_delegate = _delegate;

		if (m_animation != null)
		{
			m_animation.Stop();
			m_animation.Play(animationName[(int)PlayAnimationType.shrug]);
			m_aniType = PlayAnimationType.shrug;
		}
	}

	public void PlaySadCrossHandAnimation(IPlayerAnimationDelegate _delegate = null)
	{
		if (_delegate != null)
			m_delegate = _delegate;

		if (m_animation != null)
		{
			m_animation.Stop();
			m_animation.Play(animationName[(int)PlayAnimationType.sadCrossHand]);
			m_aniType = PlayAnimationType.sadCrossHand;
		}
	}

	public bool IsPlayingAnimation(PlayAnimationType _type)
	{
		if (GetComponent<Animation>() != null)
			return m_animation.IsPlaying(animationName[(int)_type]);

		return false;
	}
}
