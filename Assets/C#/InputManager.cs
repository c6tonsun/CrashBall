using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    private KartMovement[] _kartMovements;
    private KartPulse[] _kartPulse;

    public string[] ControllerNames;
    public Player[] Players;

	// Use this for initialization
	void Start () {
        _kartMovements = FindObjectsOfType<KartMovement>();
        _kartPulse = FindObjectsOfType<KartPulse>();

        Player[] unorderedPlayers = FindObjectsOfType<Player>();
        Players = new Player[unorderedPlayers.Length];
        foreach (Player player in unorderedPlayers)
        {
            Players[(int)player.currentPlayer - 1] = player;
        }

        PairConrollersToPlayers();
    }
	
	// Update is called once per frame
	void Update () {

        if (ControllerNames.Length != Input.GetJoystickNames().Length)
        {
            PairConrollersToPlayers();
        }
        
        //Player one
        float player1_input = Input.GetAxisRaw("HorizontalP1");
        float player2_input = Input.GetAxisRaw("HorizontalP2");
        if (Mathf.Abs(player1_input) < 0.5f)
        {
            player1_input = 0;
        }
        _kartMovements[0].Move(player1_input);
        if (Input.GetButtonDown("FireP1"))
        {
            Debug.Log("pulse1 fired");
            _kartPulse[0].Pulse();
        }

        //Player two
        _kartMovements[1].Move(player2_input);
        if (Input.GetButtonDown("FireP2"))
        {
            Debug.Log("pulse2 fired");
            _kartPulse[1].Pulse();
            
        }
    }

    private void PairConrollersToPlayers()
    {
        ControllerNames = Input.GetJoystickNames();

        foreach (Player player in Players)
        {
            player.controllerName = null;
        }

        for (int i = 0; i < ControllerNames.Length; i++)
        {
            ControllerNames[i] = "Controller " + i;
            Players[i].controllerName = ControllerNames[i];
        }
    }
}
