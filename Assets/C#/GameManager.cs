using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // player options
    public int playerCount;
    [Range(1f, 4f)]
    public float playerSpeed;
    [Range(0f, 1f)]
    public float pulseCooldown;
    [Range(10f, 30f)]
    public float pulseForce;
    [Range(2.5f, 4f)]
    public float pulseArea;

    // ball options
    public int normalBallCount;
    [Range(0f, 1f)]
    public float normalBallLikelyness;
    public int stunBallCount;
    [Range(0f, 1f)]
    public float StunBallLikelyness;
    [Range(2f, 5f)]
    public float firingInterval;

    // game mode selection
    public enum GameMode
    {
        ERROR = 0,
        Elimination = 1
    }
    public GameMode currentMode = GameMode.Elimination;
    public int playerLives;
    public int targetScore;
}
