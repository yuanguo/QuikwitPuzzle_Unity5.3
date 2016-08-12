using UnityEngine;
using System.Collections;

public class CEObject
{
	public static bool isTest = false;
	public static int instanceID = 0;

	public CEObject()
	{
		InitObject();

		if (isTest == true)
			this.testValue();
	}

	~CEObject()
	{
		instanceID--;
	}

	public virtual void InitObject()
	{
	}

	public virtual void testValue()
	{
		instanceID++;
	}


}
