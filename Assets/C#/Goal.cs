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
    private GameManager _gameManager;

    private Collider[] _colliders;
    [SerializeField]
    private DeathWallParticleHandler deathWallPS;

    private ParticleSystem goalLines;
    private ParticleSystem.MinMaxGradient _defaultColor;
    private ParticleSystem.MinMaxGradient _complColor;
    private Coroutine flashlines;
    
    private int lives;
    private int currentLives;
    private bool canComeback;

    private int targetScore;
    private int currentScore = 0;

    private PlayFMODEvent _goalSound;

    private void Start()
    {
        _scoreHandler = FindObjectOfType<ScoreHandler>();
        _gameManager = FindObjectOfType<GameManager>();

        _colliders = GetComponents<Collider>();
        foreach (Collider c in _colliders)
            c.enabled = c.isTrigger;

        goalLines = GetComponentInChildren<ParticleSystem>();
        if (goalLines != null)
        {
            var main = goalLines.main;
            var playerColor = _gameManager.Colors[(int)currentPlayer - 1];
            _defaultColor.colorMax = playerColor;
            _defaultColor.colorMin = CarLightColor.CreateComplementaryColor(playerColor);

            _defaultColor.mode = ParticleSystemGradientMode.TwoColors;
            main.startColor = _defaultColor;
        }


        lives = _gameManager.playerLives;
        currentLives = lives;

        targetScore = _gameManager.targetScore;

        _goalSound = GetComponent<PlayFMODEvent>();
    }

    IEnumerator FlashLines(){
        ParticleSystem.MainModule main = goalLines.main;
        float time = 0f;
        main.startColor = Color.red;
        yield return new WaitForSeconds(0.3f);
        while (time < 1)
        {
            main.startColor = Color.Lerp(Color.red, _defaultColor.colorMax, time);
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
            ball.canScore = false;
            currentLives--;

            if (goalLines != null)
                flashlines = StartCoroutine(FlashLines());
            // if died
            if (_scoreHandler.isElimination && currentLives <= 0)
            {
                _scoreHandler.KillPlayer((int)currentPlayer);
            }
            // killer and victim
            int[] players = ball.GetLastPlayerHits();
            if (players[0] != (int)currentPlayer)
                _scoreHandler.KillerHitVictim(players[0], (int)currentPlayer, currentLives <= 0);
            else
                _scoreHandler.KillerHitVictim(players[1], (int)currentPlayer, currentLives <= 0);
            
            _scoreHandler.UpdateSpotLight();

            _goalSound.Play(playAnyway: true);
        }
    }

    public void AddToScore()
    {
        currentScore++;

        if (currentScore >= targetScore)
        {
            _scoreHandler.ScoreReached((int)currentPlayer);
        }
    }

    public void GoalToWall()
    {
        //Activates death wall particles
        if(deathWallPS!=null)deathWallPS.ActivateParticleWall();
        if (goalLines != null) goalLines.Stop();

        foreach (Collider c in _colliders)
            c.enabled = !c.isTrigger;
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
