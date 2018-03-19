using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreHandler : MonoBehaviour {
    [HideInInspector]
    public Player[] players;
    private Goal[] _goals;

    private void Start()
    {
        Goal[] unorderedGoals = FindObjectsOfType<Goal>();
        _goals = new Goal[unorderedGoals.Length];
        foreach (Goal goal in unorderedGoals)
        {
            _goals[(int)goal.currentPlayer - 1] = goal;
        }
    }

    public void KillPlayer(int player)
    {
        players[player - 1].Die();

        int livingPLayerCount = 0;
        int winner = -1;
        foreach (Player p in players)
        {
            if (p.isLive)
            {
                livingPLayerCount++;
                winner = (int)p.currentPlayer;
            }
        }

        if (livingPLayerCount == 1)
        {
            Debug.Log("Winner is p" + winner);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
