﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using ReAnim;
using Character;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public float physicsUpdateRate = 0.01f;
    [System.Serializable]
    public class Team
    {
        public Color color;
        public string fractionCode;
    }
    public GameObject[] characters;
    public Team[] teams;
    public GameObject[] inputs;
    
    public GameObject SpawnPlayer(Transform transform, int teamId, int inputTypeId, int characterId)
    {
        var p = Instantiate(characters[characterId], transform.position, transform.rotation);

        var healthStateDisplayer = p.GetComponentsInChildren<HealthStateDisplayer>();
        foreach (var it in healthStateDisplayer)
            it.color = teams[teamId].color;

        var indicators = p.GetComponentInChildren<CharacterUiIndicator>();
        Debug.Assert(indicators);
        indicators.InitIndicators(indicators.animationIndicators, teams[teamId].color);
        indicators.InitIndicators(indicators.environmentIndicators, teams[teamId].color);

        /*var dirInput = p.GetComponentInChildren<CharacterUiIndicator>();
        dirInput.alpha = teams[teamId].color.a;
        var directionIndicatorRenderers = dirInput.GetComponentsInChildren<SpriteRenderer>();
        foreach (var it in directionIndicatorRenderers)
            it.color = new Color(
                teams[teamId].color.r,
                teams[teamId].color.g,
                teams[teamId].color.b,
                teams[teamId].color.a);
        if (dirInput.viewfinder)
        {
            directionIndicatorRenderers = dirInput.viewfinder.GetComponentsInChildren<SpriteRenderer>();
            foreach (var it in directionIndicatorRenderers)
                it.color = new Color(
                    teams[teamId].color.r,
                    teams[teamId].color.g,
                    teams[teamId].color.b,
                    teams[teamId].color.a);
        }*/


        var fraction = p.GetComponent<AiFraction>();
        fraction.fractionName = teams[teamId].fractionCode;

        Instantiate(inputs[inputTypeId], p.transform);

        return p;
    }


    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Time.fixedDeltaTime = physicsUpdateRate;
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        if (Input.GetButtonUp("Restart"))
            SceneManager.LoadScene(0);
    }


    


}
