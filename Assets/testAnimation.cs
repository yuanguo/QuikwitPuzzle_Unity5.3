using UnityEngine;
using System.Collections;

public class testAnimation : MonoBehaviour, ISpriteAnimationPlayer
{
	public SpriteAnimationPlayer m_player = null;

	public void Start()
	{
		m_player.m_delegate = this;
	}

	public void OnPlayAni(int _kind)
	{
		m_player.PlayAnimationType ((ItemKinds)_kind);
	}

	public void OnStopAni ()
	{
		m_player.StopAnimation ();
	}

	public bool willStartAnimation(SpriteAnimationPlayer _aniPlayer, ItemKinds _kind)
	{
		Debug.Log ("willStartAnimation");
		return true;
	}
	public bool willFinishAnimation(SpriteAnimationPlayer _aniPlayer, ItemKinds _kind){
		Debug.Log ("willFinishAnimation");
		return true;
	}

	public bool willEndAnimation(SpriteAnimationPlayer _aniPlayer, ItemKinds _kind){
		Debug.Log ("willEndAnimation");
		return true;
	}
}
