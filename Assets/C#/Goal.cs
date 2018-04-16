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

    private ParticleSystem goalLines;
    
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
        goalLines = GetComponentInChildren<ParticleSystem>();

        lives = gameManager.playerLives;
        currentLives = lives;

        score = gameManager.targetScore;
    }

    IEnumerator FlashLines(){
        var variable = goalLines.main;
        var oldColor = goalLines.main.startColor;
        var time = 0f;
        variable.startColor = Color.red;
        yield return new WaitForSeconds(0.3f);
        while(time<1){
        variable.startColor = Color.Lerp(Color.red, oldColor.colorMax, time);
        yield return new WaitForEndOfFrame();
        time += Time.deltaTime;        
        }
        variable.startColor = oldColor;
        StopCoroutine("FlashLines");
    }

    private void OnTriggerStay(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (ball.canScore)
        {
            cameraShake.SetShakeTime(0.25f);
            ball.canScore = false;

            // live and death
            currentLives--;
            _scoreHandler.LostLive((int)currentPlayer);

            StartCoroutine("FlashLines");

            if (gameManager.currentMode == GameManager.GameMode.Elimination && currentLives <= 0)
            {
                cameraShake.SetShakeTime(0.5f);
                _scoreHandler.KillPlayer((int)currentPlayer);
            }

            // score and kill
            int[] players = ball.GetLastPlayerHits();
            if (players[0] != (int)currentPlayer)
            {
                _scoreHandler.AddScore(players[0]);
                _scoreHandler.AddKill(players[0], (int)currentPlayer);
            }
            else
            {
                _scoreHandler.AddScore(players[1]);
                _scoreHandler.AddKill(players[1], (int)currentPlayer);
            }

            _scoreHandler.UpdateSpotLight();
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
