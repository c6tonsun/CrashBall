using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    private KartMovement[] _kartMovements;
    private KartPulse[] _kartPulse;

	// Use this for initialization
	void Start () {
        _kartMovements = FindObjectsOfType<KartMovement>();
        _kartPulse = FindObjectsOfType<KartPulse>();
    }
	
	// Update is called once per frame
	void Update () {
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
}
