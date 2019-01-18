using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachBase : MonoBehaviour {

	public string type;
	public Timer stayTime;

	// Use this for initialization
	protected void Start () {
		stayTime.Restart();
	}

	// Update is called once per frame
	protected void Update ()
	{
		if(stayTime.IsReady())
		{
			Destroy(gameObject);
		}
	}
}
