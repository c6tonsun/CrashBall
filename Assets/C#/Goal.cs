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
    private CameraShake _cameraShake;
    private GameManager _gameManager;

    private Collider[] _colliders;

    private ParticleSystem goalLines;
    private Color _defaultColor;
    private Coroutine flashlines;
    
    private int lives;
    private int currentLives;
    private bool canComeback;

    private int targetScore;
    private int currentScore = 0;

    private void Start()
    {
        _scoreHandler = FindObjectOfType<ScoreHandler>();
        _cameraShake = FindObjectOfType<CameraShake>();
        _gameManager = FindObjectOfType<GameManager>();

        _colliders = GetComponents<Collider>();
        foreach (Collider c in _colliders)
            c.enabled = c.isTrigger;

        goalLines = GetComponentInChildren<ParticleSystem>();
        _defaultColor = goalLines.main.startColor.colorMax;

        lives = _gameManager.playerLives;
        currentLives = lives;

        targetScore = _gameManager.targetScore;
    }

    IEnumerator FlashLines(){
        ParticleSystem.MainModule main = goalLines.main;
        float time = 0f;
        main.startColor = Color.red;
        yield return new WaitForSeconds(0.3f);
        while (time < 1)
        {
            main.startColor = Color.Lerp(Color.red, _defaultColor, time);
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;        
        }
        main.startColor = _defaultColor;
        StopCoroutine(flashlines);
    }

    private void OnTriggerStay(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (ball.canScore)
        {
            _cameraShake.SetShakeTime(0.25f);
            ball.canScore = false;

            // live and death
            currentLives--;
            _scoreHandler.LostLive((int)currentPlayer);

            flashlines = StartCoroutine(FlashLines());

            if (_gameManager.currentMode == GameManager.GameMode.Elimination && currentLives <= 0)
            {
                _cameraShake.SetShakeTime(0.5f);
                _scoreHandler.KillPlayer((int)currentPlayer);
                foreach (Collider c in _colliders)
                    c.enabled = !c.isTrigger;

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

        if (currentScore >= targetScore)
        {
            _cameraShake.SetShakeTime(0.5f);
            _scoreHandler.ScoreReached((int)currentPlayer);
        }
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
