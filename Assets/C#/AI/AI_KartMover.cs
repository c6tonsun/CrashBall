using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_KartMover : MonoBehaviour {

    public enum CPU_Difficulty
    {
        Error = 0,
        Cheating = 1,
        Hard = 2,
        Handicap = 3,
        Bad = 4
    }
    public CPU_Difficulty difficulty;

    GameManager gameManager;
    UIMenuHandler _menuHandler;

    [SerializeField]
    private Player[] Players;
    [SerializeField]
    private Collider[] GoalColliders;
    [SerializeField]
    private List<Ball> Balls;

    private int trycount, playerCount = 0;
    private float pulseTimer;
    private float[] OldNumber, SpeedMulti;
    public bool[] WillPulse;

    // Use this for initialization
    void Start () {
        gameManager = FindObjectOfType<GameManager>();
        _menuHandler = gameManager.GetComponentInChildren<UIMenuHandler>();
        
        Players = PopulatePlayers();
        GoalColliders = PopulateGoals();

        WillPulse = new bool[playerCount];
        OldNumber = new float[playerCount];
        SpeedMulti = new float[playerCount];

        BallSpawner bs = FindObjectOfType<BallSpawner>();
        Balls.AddRange(bs.normalBalls);
        Balls.AddRange(bs.stunBalls);

        difficulty = (CPU_Difficulty)gameManager.Difficulty;

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
            WillPulse[i] = true;
        }
    }

    void Update()
    { 
        if (_menuHandler.isGamePaused || _menuHandler.activeMenu == _menuHandler.scoreScreen)
            return;


        if (pulseTimer < 1f)
        {
            pulseTimer += Time.deltaTime;
        }

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
                        DetermineWillPulse(playerNum - 1);
                        if (WillPulse[playerNum-1])
                        {
                            player.pulse.Pulse();
                        }
                    }
                }
                float number;
                if (index >= 0)
                {
                    number = Balls[index].AI.GoalHitPoint - player.movement.CurveTime;
                }
                else
                {
                    number = 0.5f - player.movement.CurveTime;
                }
                //Move karts
                if (Mathf.Sign(number) == Mathf.Sign(OldNumber[playerNum - 1]))
                {
                    SpeedMulti[playerNum - 1] += Time.deltaTime * 3;
                    if (SpeedMulti[playerNum - 1] > DetermineMaxSpeed())
                        SpeedMulti[playerNum - 1] = DetermineMaxSpeed();
                }
                else
                {
                    SpeedMulti[playerNum - 1] = 0;
                }
                //No acceleration for cheating CPU
                if (difficulty == CPU_Difficulty.Cheating)
                    SpeedMulti[playerNum - 1] = DetermineMaxSpeed();

                if (Mathf.Abs(number) > 0.05f)
                    player.movement.Move(Mathf.Sign(number) * SpeedMulti[playerNum - 1]);
                OldNumber[playerNum - 1] = number;
            }

        }
    }
    private float DetermineMaxSpeed()
    {
        switch (difficulty)
        {
            case CPU_Difficulty.Error:
                return 1;
            case CPU_Difficulty.Cheating:
                return 1.2f;
            case CPU_Difficulty.Hard:
                return 1f;
            case CPU_Difficulty.Handicap:
                return 0.8f;
            case CPU_Difficulty.Bad:
                return 0.5f;
            default:
                return 1;
        }

    }

    private void DetermineWillPulse(int playerNum)
    {
        switch (difficulty) 
        {
            case CPU_Difficulty.Error:
                break;
            case CPU_Difficulty.Cheating:
                WillPulse[playerNum] = true;
                break;
            case CPU_Difficulty.Hard:
                if (pulseTimer < 0.2f)
                {
                    WillPulse[playerNum] = (Random.Range(0, 10) > 2);
                    pulseTimer = 0;
                }
                break;
            case CPU_Difficulty.Handicap:
                if (pulseTimer < 0.5f)
                {
                    WillPulse[playerNum] = (Random.Range(0, 10) > 6);
                    pulseTimer = 0;
                }
                break;
            case CPU_Difficulty.Bad:
                if (pulseTimer < 1f)
                {
                    WillPulse[playerNum] = (Random.Range(0, 10) > 8);
                    pulseTimer = 0;
                }
                break;
            default:
                break;
        }
    }

    Player[] PopulatePlayers()
    {
        Player[] _tempPlayers = FindObjectsOfType<Player>();
        Player[] output = new Player[_tempPlayers.Length];
        playerCount = _tempPlayers.Length;
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
