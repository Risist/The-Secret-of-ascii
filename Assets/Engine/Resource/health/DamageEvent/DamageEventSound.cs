﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DamageEventSound : MonoBehaviour {

	public AudioClip audioClip;
	public Timer restartCd;
	public float pitchBase = 1.0f;
	public float pitchDmgScale = 0.0f;
	public float pitchRandom = 0.0f;
	public float volumeBase = 1.0f;
	public float volumeDmgScale = 0.0f;
	public float volumeRandom = 0.0f;

	// Use this for initialization
	void Start () {
	}

	void OnReceiveDamage(HealthController.DamageData data)
	{
		if (data.damage >= 0 || !restartCd.IsReadyRestart() )
			return;

		AudioManager.inst.CreateInstance(audioClip, transform.position,
			volumeBase + (-data.damage) * volumeDmgScale + (Random.value - 0.5f) * 2.0f * volumeRandom,
			pitchBase + (-data.damage) * pitchDmgScale + (Random.value - 0.5f) * 2.0f * pitchRandom
			);
	}
}
