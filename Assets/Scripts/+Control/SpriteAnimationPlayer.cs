using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using System;
using System.Collections;
using System.Collections.Generic;

public interface ISpriteAnimationPlayer
{
	bool willStartAnimation(SpriteAnimationPlayer _aniPlayer, ItemKinds _kind);
	bool willFinishAnimation(SpriteAnimationPlayer _aniPlayer, ItemKinds _kind);
	bool willEndAnimation(SpriteAnimationPlayer _aniPlayer, ItemKinds _kind);
}

public class SpriteAnimationPlayer : MonoBehaviour
{
	public ISpriteAnimationPlayer m_delegate = null;
	public bool m_loop = false;

	public string[] AniPaths = {
		"Sprites/powerupAnimationImages/seq_rndr_trndo/tornado",
		"Sprites/powerupAnimationImages/seq_rndr_aud/Aud",
		"Sprites/powerupAnimationImages/seq_rndr_hrglss/hrglss",
		"Sprites/powerupAnimationImages/seq_rndr_skprope/skp_rope",
		"Sprites/powerupAnimationImages/seq_rndr_bmb/bomb",
	};

	public int[] AniKindLength = { 125, 99, 125, 74, 110 };
	public int[] AniSoundLength = { 3, 3, 3, 2, 4 };
	public Image m_img = null;

	public List<Sprite> m_sprtTornados = new List<Sprite> ();
	public List<Sprite> m_sprtAudienc = new List<Sprite> ();
	public List<Sprite> m_sprtHourGlass = new List<Sprite> ();
	public List<Sprite> m_sprtSkipRope = new List<Sprite> ();
	public List<Sprite> m_sprtBomb = new List<Sprite> ();

	[SerializeField]
	private ItemKinds m_itemKind = ItemKinds.tornado;

	[SerializeField]
	private bool m_playingFlag = false;


	private int m_aniFrameIdx = 0;
	private List<Sprite> m_curPlayAniSprits = null;

	private void Awake ()
	{
	}

	private void Start ()
	{
		m_playingFlag = false;
	}

	void OnEnable()
	{
	}

	void OnDisable()
	{
		m_playingFlag = false;
	}

	public static List<Sprite> GetSprite(string spritPath, int count)
	{
		List<Sprite> sprites = new List<Sprite>();
		//string fileFormat = count > 99 ? "000" : (count > 9 ? "00" : "0");
		string fileFormat = "00000";

		for (int idx = 0; idx < count; idx++ )
		{
			string spriteFilePath = spritPath + idx.ToString(fileFormat);
			Sprite[] sprts = Resources.LoadAll<Sprite>(spriteFilePath);
			sprites.AddRange(sprts);
		}
		
		return sprites;
	}

	public void loadAllResources()
	{
		if (m_sprtTornados != null) {
			m_sprtTornados.Clear ();
			m_sprtTornados.AddRange (GetSprite (AniPaths [0], AniKindLength [0]));
		}
		
		if (m_sprtAudienc != null) {
			m_sprtAudienc.Clear ();
			m_sprtAudienc.AddRange (GetSprite (AniPaths [1], AniKindLength [1]));
		}
		
		if (m_sprtHourGlass != null) {
			m_sprtHourGlass.Clear ();
			m_sprtHourGlass.AddRange (GetSprite (AniPaths [2], AniKindLength [2]));
		}
		
		if (m_sprtSkipRope != null) {
			m_sprtSkipRope.Clear ();
			m_sprtSkipRope.AddRange (GetSprite (AniPaths [3], AniKindLength [3]));
		}
		
		if (m_sprtBomb != null) {
			m_sprtBomb.Clear ();
			m_sprtBomb.AddRange (GetSprite (AniPaths [4], AniKindLength [4]));
		}
	}

	public void PlayAnimationType(ItemKinds _kind, bool _loop = false)
	{
		float aniSoundLength = 0;
		m_loop = _loop;
		m_itemKind = _kind;

		if (m_itemKind == ItemKinds.bomb)
			transform.localScale = new Vector3 (1.5f, 1.5f, 1.5f);
		else
			transform.localScale = Vector3.one;

		switch (_kind) {
		case ItemKinds.tornado:
		{
			m_curPlayAniSprits = m_sprtTornados;
			m_playingFlag = true;
			m_aniFrameIdx = 0;

			aniSoundLength = (float)AniSoundLength[0] / (float)m_curPlayAniSprits.Count;
			break;
		}
		case ItemKinds.audience:
		{
			m_curPlayAniSprits = m_sprtAudienc;
			m_playingFlag = true;
			m_aniFrameIdx = 0;

			aniSoundLength = (float)AniSoundLength[1] / (float)m_curPlayAniSprits.Count;
			break;
		}
		case ItemKinds.timer:
		{
			m_curPlayAniSprits = m_sprtHourGlass;
			m_playingFlag = true;
			m_aniFrameIdx = 0;

			aniSoundLength = (float)AniSoundLength[2] / (float)m_curPlayAniSprits.Count;
			break;
		}
		case ItemKinds.rope:
		{
			m_curPlayAniSprits = m_sprtSkipRope;
			m_playingFlag = true;
			m_aniFrameIdx = 0;
			aniSoundLength = (float)AniSoundLength[3] / (float)m_curPlayAniSprits.Count;
			break;
		}
		case ItemKinds.bomb:
		{
			m_curPlayAniSprits = m_sprtBomb;
			m_playingFlag = true;
			m_aniFrameIdx = 0;
			aniSoundLength = (float)AniSoundLength[4] / (float)m_curPlayAniSprits.Count;
			break;
		}
		}

		bool playAni = true;
		if (m_delegate != null) 
			playAni = m_delegate.willStartAnimation(this, m_itemKind);

		if (playAni == false)
			return;

		if (IsInvoking("updateFrames")){
		    CancelInvoke("updateFrames");
		}
		
		InvokeRepeating("updateFrames", 0.1f, aniSoundLength);
	}

	private void updateFrames()
	{
		if (m_playingFlag == false)
			return;
		
		if (m_curPlayAniSprits != null) {
			m_aniFrameIdx = m_aniFrameIdx >= m_curPlayAniSprits.Count ? m_curPlayAniSprits.Count - 1 : m_aniFrameIdx;
			m_img.sprite = m_curPlayAniSprits[m_aniFrameIdx];
			m_aniFrameIdx ++;

			if (m_aniFrameIdx >= m_curPlayAniSprits.Count)
			{
				if (m_loop)
				{
					bool stopAni = false;

					if (m_delegate != null)
						stopAni = m_delegate.willEndAnimation(this, m_itemKind);

					if (stopAni == true)
						StopAnimation();
					else
						m_aniFrameIdx = 0;
				}
				else
					StopAnimation();
			}
		}
	}

	public void playAnimTest(int _kind)
	{
		PlayAnimationType ((ItemKinds)_kind);
	}

	public bool IsPlayingAnimation()
	{
		return m_playingFlag;
	}

	public void StopAnimation()
	{
		bool stopAni = false;
		if (m_delegate != null)
			stopAni = m_delegate.willFinishAnimation (this, m_itemKind);

		if (stopAni) {
			m_playingFlag = false;
			m_aniFrameIdx = 0;
			
			m_curPlayAniSprits = null;

			CancelInvoke("updateFrames");
		}
	}
}
