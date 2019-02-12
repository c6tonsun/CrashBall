using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_GoalData : MonoBehaviour {
    public enum Number
    {
        ERROR = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4
    }
    public Number currentPlayer;

    public Collider collider;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }
}
