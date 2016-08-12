using UnityEngine;
using UnityEditor;

using System.Collections;

[CustomEditor(typeof(SpriteAnimationPlayer), true)]
public class SpriteAnimationPlayerEditor : Editor
{
	void OnEnable()
	{
		SpriteAnimationPlayer sprtPlayer = target as SpriteAnimationPlayer;

		if (sprtPlayer != null)
			sprtPlayer.loadAllResources ();
	}
}
