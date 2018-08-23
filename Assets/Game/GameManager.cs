using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public float physicsUpdateRate = 0.01f;

    private void Start()
    {
        Time.fixedDeltaTime = physicsUpdateRate;
    }
}
