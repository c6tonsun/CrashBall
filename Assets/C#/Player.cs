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

    private void Awake()
    {
        movement = GetComponentInChildren<KartMovement>();
        pulse = GetComponentInChildren<KartPulse>();
    }
}
