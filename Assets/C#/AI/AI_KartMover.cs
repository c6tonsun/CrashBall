using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_KartMover : MonoBehaviour {

    GameManager gameManager;
    UIMenuHandler _menuHandler;

    [SerializeField]
    private Player[] Players;
    [SerializeField]
    private Collider[] GoalColliders;
    [SerializeField]
    private List<Ball> Balls;

    private int trycount = 0;

    // Use this for initialization
    void Start () {
        gameManager = FindObjectOfType<GameManager>();
        _menuHandler = gameManager.GetComponentInChildren<UIMenuHandler>();
        Players = PopulatePlayers();
        GoalColliders = PopulateGoals();
        BallSpawner bs = FindObjectOfType<BallSpawner>();
        Balls.AddRange(bs.normalBalls);
        Balls.AddRange(bs.stunBalls);

        for(int i = 0; i<4; i++)
        {
            var center = GoalColliders[i].bounds.center;
            Debug.Log(GoalColliders[i].name + GoalColliders[i].bounds.min);
            Vector3 max;
            if (i < 2)
            {
                max = new Vector3(center.x + GoalColliders[i].bounds.extents.x, 0, center.z + GoalColliders[i].bounds.extents.z);
            }
            else
            {
                max = new Vector3(center.x + GoalColliders[i].bounds.extents.x, 0, center.z + GoalColliders[i].bounds.extents.z);
            }
            Debug.Log(GoalColliders[i].name + max);
        }
    }

    void Update()
    {
        if(_menuHandler.isGamePaused || _menuHandler.activeMenu == _menuHandler.scoreScreen)
            return;

        foreach (var player in Players)
        {
            if (player.AIControlled)
            {
                if (player.gameObject.activeSelf == false || player.isLive == false)
                    continue;

                int playerNum = (int)player.currentPlayer;
                int index = -1;
                float smallestTime = 99;
                for (int i = 0; i < Balls.Count; i++)
                {
                    if (Balls[i].AI.GoalTarget == playerNum && Balls[i].canScore)
                    {
                        if (Balls[i].AI.TimeToGoal < smallestTime)
                        {
                            smallestTime = Balls[i].AI.TimeToGoal;
                            index = i;
                        }
                    }
                    if (Vector3.Distance(player.pulse.transform.position, Balls[i].transform.position) < gameManager.pulseArea * 0.7f)
                    {
                        player.pulse.Pulse();
                    }
                }
                if (index >= 0)
                {
                    float number = Balls[index].AI.GoalHitPoint - player.movement.CurveTime;
                    if(Mathf.Abs(number)>0.05f)
                    player.movement.Move(Mathf.Sign(number)
                        //*Mathf.Abs(number*1.5f) //Smooths the movement
                        );
                }
            }

        }
    }

    Player[] PopulatePlayers()
    {
        Player[] _tempPlayers = FindObjectsOfType<Player>();
        Player[] output = new Player[_tempPlayers.Length];
        int populated = 0;
        while(populated < _tempPlayers.Length)
        {
            for(int i = 0; i< _tempPlayers.Length; i++)
            {
                if (populated+1 == (int)_tempPlayers[i].currentPlayer)
                {
                    output[populated] = _tempPlayers[i];
                    populated++;
                    Debug.Log("player found, added to players" + Time.unscaledTime);
                }
                else
                {
                    Debug.Log("continue");
                    continue;
                }
            }
            trycount++;
            if(trycount > 500)
            {
                Debug.Log("Too many tries, stopping Populate Players");
                break;
            }
        }
        return output;
    }

    Collider[] PopulateGoals()
    {
        Collider[] output;
        AI_GoalData[] Goals = FindObjectsOfType<AI_GoalData>();
        output = new Collider[Players.Length];
        int populated = 0;
        while (populated < Players.Length)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (populated+1 == (int)Goals[i].currentPlayer)
                {
                    output[populated] = Goals[i].collider;
                    populated++;
                    Debug.Log("goal found, added to goals" + Time.unscaledTime);
                }
                else
                {
                    continue;
                }
            }
            trycount++;
            if (trycount > 500)
            {
                break;
            }
        }
        return output;
    }

    public float CalculateGoalHitPoint(Vector3 hit_point, int targetPlayer)
    {
        targetPlayer--;
        float targetFloat, distanceToHit;
        if (targetPlayer < 2)
        {
            distanceToHit = 6 + hit_point.x;
        }
        else
        {
            distanceToHit = 6 + hit_point.z;
        }
        targetFloat = distanceToHit / 12;
        return targetFloat;
    }
}
