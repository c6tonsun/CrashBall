using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public enum Number
    {
        ERROR = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4
    }
    
    public Number currentPlayer;

    public bool hasController;

    [HideInInspector]
    public KartMovement movement;
    [HideInInspector]
    public KartPulse pulse;

    public PlayerWall myWall;
    public Goal myGoal;
    [HideInInspector]
    public bool isLive = true;

    private void Awake()
    {
        movement = GetComponentInChildren<KartMovement>();
        pulse = GetComponentInChildren<KartPulse>();
    }

    public void Die()
    {
        myWall.gameObject.SetActive(true);
        myWall.player = this;
        myWall.playerGoal = myGoal;
        isLive = false;
        gameObject.SetActive(false);
    }
}
