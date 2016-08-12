using UnityEngine;
using System.Collections;

public class IntroScene : MonoBehaviour 
{
	void Start()
	{
		Handheld.PlayFullScreenMovie("LOGO_720.mp4", Color.black, FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.AspectFit);
	}
}
