using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

    public enum Number
    {
        ERROR = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4
    }
    public Number currentPlayer;
    
    private ScoreHandler _scoreHandler;
    private CameraShake cameraShake;
    private GameManager gameManager;
    
    private int lives;
    private int currentLives;
    private bool canComeback;

    private int score;
    private int currentScore = 0;

    private void Start()
    {
        _scoreHandler = FindObjectOfType<ScoreHandler>();
        cameraShake = FindObjectOfType<CameraShake>();
        gameManager = FindObjectOfType<GameManager>();

        lives = gameManager.playerLives;
        currentLives = lives;

        score = gameManager.targetScore;
    }

    private void OnTriggerStay(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (ball.canScore)
        {
            cameraShake.SetShakeTime(0.25f);
            currentLives--;
            _scoreHandler.LostLive((int)currentPlayer);
            ball.canScore = false;

            // Eliminate player
            if (gameManager.currentMode == GameManager.GameMode.Elimination && currentLives <= 0)
            {
                cameraShake.SetShakeTime(0.5f);
                _scoreHandler.KillPlayer((int)currentPlayer);
            }
            // ScoreRun winner
            if (gameManager.currentMode == GameManager.GameMode.ScoreRun)
            {
                int[] players = ball.GetLastPlayerHits();
                if (players[0] != (int)currentPlayer)
                    _scoreHandler.AddToScore(players[0]);
                else
                    _scoreHandler.AddToScore(players[1]);
            }
        }
    }

    public void AddToScore()
    {
        currentScore++;

        if (currentScore >= score)
        {
            cameraShake.SetShakeTime(0.5f);
            _scoreHandler.ScoreReached((int)currentPlayer);
        }
    }

    public bool ResetLives()
    {
        if (canComeback)
        {
            currentLives = lives;
            canComeback = false;
            return true;
        }
        return false;
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }
}
