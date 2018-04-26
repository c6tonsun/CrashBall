using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyInputManager : MonoBehaviour {

    private static int HORIZONTAL_INPUT = 6;
    private static int VERTICAL_INPUT = 7;
    private static int MAGNET_INPUT = 9;
    private static int PULSE_INPUT = 8;
    private static int PAUSE_INPUT = 10;

    private Rewired.Player[] _playersRewired;
    private Player[] _players;
    
	private void Start ()
    {
        Player[] unorderedPlayers = FindObjectsOfType<Player>();
        _players = new Player[unorderedPlayers.Length];
        _playersRewired = new Rewired.Player[unorderedPlayers.Length];
        foreach (Player player in unorderedPlayers)
        {
            int index = (int)player.currentPlayer - 1;
            _players[index] = player;
            _playersRewired[index] = Rewired.ReInput.players.GetPlayer(index);
        }
        
        ScoreHandler _scoreHandler = FindObjectOfType<ScoreHandler>();
        _scoreHandler.StageStart();
    }
	
	private void Update () {

        foreach (Player player in _players)
        {
            ActPlayerInput(player);
        }
    }

    private void ActPlayerInput(Player player)
    {
        if (player.gameObject.activeSelf == false || player.isLive == false)
            return;

        int index = (int)player.currentPlayer - 1;

        float movementInput = 0f;
        if (player.isVerticalCurve)
            movementInput = _playersRewired[index].GetAxis(VERTICAL_INPUT);
        else
            movementInput = _playersRewired[index].GetAxis(HORIZONTAL_INPUT);

        bool magnetInput = _playersRewired[index].GetButton(MAGNET_INPUT);
        bool pulseInput = _playersRewired[index].GetButtonDown(PULSE_INPUT);
        
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
            player.pulse.Pulse();
    }
}
