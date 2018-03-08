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
        _kartMovements[0].Move(Input.GetAxis("HorizontalP1"));
        if (Input.GetButtonDown("FireP1"))
        {
            //_kartPulse[0].Pulse();
        }

        //Player two
        _kartMovements[1].Move(Input.GetAxis("HorizontalP2"));
        if (Input.GetButtonDown("FireP2"))
        {
            //_kartPulse[1].Pulse();
        }
    }
}
