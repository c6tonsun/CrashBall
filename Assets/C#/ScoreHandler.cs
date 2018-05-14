using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Collections;

public class ScoreHandler : MonoBehaviour {

    
    private Player[] _players;
    private Goal[] _goals;

    private int[] scores;
    private int[] lives;

    public PlayFMODEvent thunder;

    private ContinuesCurve _leaderSpotLight;
    private GameManager _gameManager;
    private UIHandler _UIHandler;
    private UIMenuHandler _menuHandler;

    [HideInInspector]
    public bool isElimination;

    private void Start()
    {
        _UIHandler = GetComponent<UIHandler>();
        _menuHandler = GetComponentInChildren<UIMenuHandler>();
    }

    public void StageStart(Player[] players, int playerCount)
    {
        InitArrays(players, playerCount);
        KillExtraPlayers(playerCount);

        isElimination = _gameManager.isElimination;
        _leaderSpotLight = GameObject.FindGameObjectWithTag("Player").GetComponent<ContinuesCurve>();

        _UIHandler.StageStart(_players, lives, scores, isElimination);

        StartCoroutine(StartCountdown());
    }

    private void InitArrays(Player[] players, int playerCount)
    {
        _players = players;
        foreach (Player player in _players)
        {
            player.kills = new int[_players.Length];
            for (int i = 0; i < _players.Length; i++)
                player.kills[i] = 0;
        }

        Goal[] unorderedGoals = FindObjectsOfType<Goal>();
        _goals = new Goal[unorderedGoals.Length];
        foreach (Goal goal in unorderedGoals)
            _goals[(int)goal.currentPlayer - 1] = goal;
        
        scores = new int[_players.Length];
        lives = new int[_players.Length];

        _gameManager = GetComponent<GameManager>();
        int playerlives = _gameManager.playerLives;

        for (int i = 0; i < _players.Length; i++)
        {
            scores[i] = 0;
            lives[i] = playerlives;
        }
    }

    private void KillExtraPlayers(int playerCount)
    {
        Player[] allPlayers = FindObjectsOfType<Player>();
        foreach (Player player in allPlayers)
        {
            if (playerCount < (int)player.currentPlayer)
                player.Die();
        }

        Goal[] allGoals = FindObjectsOfType<Goal>();
        foreach (Goal goal in allGoals)
        {
            if (playerCount < (int)goal.currentPlayer)
                goal.GoalToWall();
        }
    }


    public void KillerHitVictim(int killer, int victim, bool victimDied)
    {
        // update lives
        lives[victim - 1] = _goals[victim - 1].GetCurrentLives();
        // update UI
        _UIHandler.KillFeed(killer, victim, victimDied);
        if (isElimination)
            _UIHandler.PositionalUpdate(lives);

        if (killer == -1)
            return;

        // update player's kills
        _players[killer - 1].kills[victim - 1]++;
        // update scores
        _goals[killer - 1].AddToScore();
        scores[killer - 1] = _goals[killer - 1].GetCurrentScore();
        // party
        _players[killer - 1].GoalCelebration();

        if (!isElimination)
            _UIHandler.PositionalUpdate(scores);
    }

    public void KillPlayer(int player)
    {
        _players[player - 1].Die();
        _goals[player - 1].GoalToWall();
        thunder.Play(playAnyway: false);
        // check living player count
        int livingPLayerCount = 0;
        int winner = -1;
        foreach (Player p in _players)
        {
            if (p.isLive)
            {
                livingPLayerCount++;
                winner = (int)p.currentPlayer;
            }
        }
        // end stage if we have winner
        if (livingPLayerCount == 1)
            StartCoroutine(EndStage(winner));
    }
    
    public void ScoreReached(int player)
    {
        foreach (Player p in _players)
        {
            if (player == (int)p.currentPlayer)
                continue;

            p.Die();
            _goals[(int)p.currentPlayer - 1].GoalToWall();
        }

        StartCoroutine(EndStage(player));
    }
    
    public void UpdateSpotLight()
    {
        int temp = 0;
        int playerToLight = 1;

        if (isElimination)
        {
            for (int i = 0; i < lives.Length; i++)
            {
                if (lives[i] > temp)
                {
                    temp = lives[i];
                    playerToLight = i + 1;
                }
            }
        }
        else
        {
            for (int i = 0; i < scores.Length; i++)
            {
                if (scores[i] > temp)
                {
                    temp = scores[i];
                    playerToLight = i + 1;
                }
            }
        }

        _leaderSpotLight.MoveToPlayer(playerToLight, true);
        return;
    }

    IEnumerator StartCountdown()
    {
        TMPro.TextMeshPro stratFeed = _UIHandler.startFeed;
        _menuHandler.isGameStarting = true;

        yield return new WaitForSecondsRealtime(0.2f);

        stratFeed.SetText("3");
        stratFeed.color = new Color(stratFeed.color.r, stratFeed.color.g, stratFeed.color.b, 1f);
        yield return new WaitForSecondsRealtime(1f);

        stratFeed.SetText("2");
        stratFeed.color = new Color(stratFeed.color.r, stratFeed.color.g, stratFeed.color.b, 1f);
        yield return new WaitForSecondsRealtime(1f);

        stratFeed.SetText("1");
        stratFeed.color = new Color(stratFeed.color.r, stratFeed.color.g, stratFeed.color.b, 1f);
        yield return new WaitForSecondsRealtime(0.9f);

        thunder.Play(playAnyway: false);
        yield return new WaitForSecondsRealtime(0.1f);

        stratFeed.SetText("GO");
        stratFeed.color = new Color(stratFeed.color.r, stratFeed.color.g, stratFeed.color.b, 1f);

        _menuHandler.isGamePaused = false;
        _menuHandler.isGameStarting = false;

        StopCoroutine(StartCountdown());
    }

    IEnumerator EndStage(int winner)
    {
        TMPro.TextMeshPro endFeed = _UIHandler.endFeed;
        endFeed.SetText("WINNER");
        endFeed.color = _players[winner - 1].GetColor();
        
        yield return new WaitForSeconds(2);

        _menuHandler.isGamePaused = true;
        _menuHandler.ToMenu(_menuHandler.scoreScreen, false);
        StopCoroutine(EndStage(winner));
    }
}
