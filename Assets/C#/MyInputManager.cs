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

    private ScoreHandler _scoreHandler;
    private UIMenuHandler _menuHandler;

    private void Start ()
    {
        _scoreHandler = FindObjectOfType<ScoreHandler>();

        // sort players
        int playerCount = FindObjectOfType<GameManager>().playerCount;
        Player[] unorderedPlayers = FindObjectsOfType<Player>();
        _players = new Player[playerCount];
        _playersRewired = new Rewired.Player[playerCount];
        foreach (Player player in unorderedPlayers)
        {
            int index = (int)player.currentPlayer - 1;
            if (index < playerCount)
            {
                _players[index] = player;
                _playersRewired[index] = Rewired.ReInput.players.GetPlayer(index);
            }
        }

        _scoreHandler.StageStart(_players, playerCount);

        _menuHandler = _scoreHandler.GetComponentInChildren<UIMenuHandler>();
        _menuHandler.SetCamera(GetComponent<Camera>());
    }

    private void Update ()
    {
        if (_menuHandler.activeMenu == _menuHandler.scoreScreen)
            return;

#region pause input
        for (int i = 0; i < _playersRewired.Length; i++)
        {
            if (_playersRewired[i].GetButtonDown(PAUSE_INPUT))
            {
                if (_menuHandler.isGamePaused)
                    _menuHandler.DoUnpause();
                else
                    _menuHandler.DoPause();

                break;
            }
        }

        if (_menuHandler.isGamePaused)
            return;
#endregion

        foreach (Player player in _players)
            ActPlayerInput(player);
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
