using UnityEngine;
using System.Collections;

public class EyeAnimation : MonoBehaviour {

	public Material mat;
	public string animation_name;
	public Texture eye_open;
	public Texture eye_shut;
	public float start_time;
	public float end_time;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		PlayAnimation ();
		ChangeEyeTexture ();
	}

	void PlayAnimation ()
	{
		if(!GetComponent<Animation>().isPlaying)
		{
			GetComponent<Animation>().Play(animation_name);
		}
	}

	void ChangeEyeTexture ()
	{
		if(GetComponent<Animation>().IsPlaying(animation_name))
		{
			if((GetComponent<Animation>()[animation_name].time > start_time) && (GetComponent<Animation>()[animation_name].time < end_time))
			{
				if(mat.mainTexture != eye_shut)
					mat.mainTexture = eye_shut;
			}
			else
			{
				if(mat.mainTexture != eye_open)
					mat.mainTexture = eye_open;
			}
		}
	}
}
