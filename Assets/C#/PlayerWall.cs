using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWall : MonoBehaviour {

    public float timeToRespawnPlayer = 1f;
    private float respawnTimer = -1f;
    [HideInInspector]
    public Player player;
    [HideInInspector]
    public Goal playerGoal;

    private void OnEnable()
    {
        respawnTimer = -1f;
    }

    private void Update()
    {
        respawnTimer += Time.deltaTime;
        if (respawnTimer > timeToRespawnPlayer)
        {
            if (playerGoal.ResetLives())
            {
                player.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
}
