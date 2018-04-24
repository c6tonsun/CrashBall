using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    
    /**
     * Rewrite when implementing Rewired
     */

    private string[] _controllerNames;
    private Player[] _players;
    public float inputDeadzone = 0.5f;
    private int oldTriggerInput = 0;

    [SerializeField]
    GameObject pauseMenu;
    public bool Paused = false;
    
	private void Start ()
    {
        Player[] unorderedPlayers = FindObjectsOfType<Player>();
        _players = new Player[unorderedPlayers.Length];
        foreach (Player player in unorderedPlayers)
        {
            _players[(int)player.currentPlayer - 1] = player;
        }
        PairConrollersToPlayers();
        
        ScoreHandler _scoreHandler = FindObjectOfType<ScoreHandler>();
        _scoreHandler.StageStart();
        //_scoreHandler._players = _players;
    }
	
	private void Update () {

        // GetJoystickNames keeps lost controllers as empty strings in array. Worked around this weature.
        PairConrollersToPlayers();

        foreach (Player player in _players)
        {
            ActPlayerInput(player);
        }
    }

    private void PairConrollersToPlayers()
    {
        foreach (Player player in _players)
            player.hasController = false;

        _controllerNames = Input.GetJoystickNames();

        int pairLenght = 1;
        pairLenght = (_controllerNames.Length < _players.Length) ? _controllerNames.Length : _players.Length;

        for (int i = 0; i < pairLenght; i++)
        {
            _players[i].hasController = _controllerNames[i].Length != 0;
        }
    }

    private void ActPlayerInput(Player player)
    {
        if (player.gameObject.activeSelf == false || player.isLive == false)
        {
            return;
        }
        int playerNumber = (int)player.currentPlayer;

        float movementInput = 0f;
        bool pulseInput = false;
        bool magnetInput = false;

        if (player.hasController)
        {
            movementInput = Input.GetAxisRaw("P" + playerNumber);
            pulseInput = Input.GetButtonDown("FireP" + playerNumber);
            magnetInput = Input.GetButton("FireM" + playerNumber);
        }
        else
        {
            movementInput = Input.GetAxisRaw("Key" + playerNumber);
            pulseInput = Input.GetButton("FireKey" + playerNumber);
        }

        if (movementInput > -inputDeadzone && movementInput < inputDeadzone)
            movementInput = 0f;

        if (magnetInput)
        {
            player.movement.Move(movementInput * 0.5f);
            player.pulse.Magnet();
        }
        else
        {
            player.movement.Move(movementInput);
            player.pulse.ResetMagnet();
        }

        if (pulseInput)
        {
            player.pulse.Pulse();
        }

    }

    private bool ActTriggerInput(int playerNumber, bool doPulse)
    {
        int newTriggerInput;

        float triggerInput = Input.GetAxisRaw("FireT" + playerNumber);
        if (triggerInput < -0.5f)
            newTriggerInput = -1;
        else if (triggerInput > 0.5f)
            newTriggerInput = 1;
        else newTriggerInput = 0;

        if (oldTriggerInput == 1 && newTriggerInput == -1 ||    // from + to - = button down
            oldTriggerInput == -1 && newTriggerInput == 1 ||    // from - to + = button down
            oldTriggerInput == 0 && newTriggerInput != 0)       // from 0 to ! = button down
            doPulse = true;

        oldTriggerInput = newTriggerInput;
        return doPulse;
    }
}
