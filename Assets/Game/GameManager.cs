using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public float physicsUpdateRate = 0.01f;

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
