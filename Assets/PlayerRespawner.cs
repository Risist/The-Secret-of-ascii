using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawner : MonoBehaviour {

    public GameObject playerSpawner;
    GameObject player;
    private void Start()
    {
        player = Instantiate(playerSpawner, transform.position, transform.rotation);
    }

    // Update is called once per frame
    void Update () {
        if (!player && Input.GetKeyDown(KeyCode.Escape))
        {
            player = Instantiate(playerSpawner, transform.position, transform.rotation);
            Debug.Log("Player Spawned");
        }
	}
}
