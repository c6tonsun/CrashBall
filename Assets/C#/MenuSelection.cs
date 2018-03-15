using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSelection : MonoBehaviour {

    public GameObject[] _menuButtons;
    int _currentlyChosen = 0;
    public Material[] _materials;
    bool _holding = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxisRaw("Vertical") > 0.5f && !_holding)
        {
            _holding = true;
            _menuButtons[_currentlyChosen].GetComponent<Renderer>().material = _materials[0];
            _currentlyChosen--;
            if (_currentlyChosen < 0)
            {
                _currentlyChosen = _menuButtons.Length - 1;
            }
            _menuButtons[_currentlyChosen].GetComponent<Renderer>().material = _materials[1];


        }
        else if (Input.GetAxisRaw("Vertical") < -0.5f && !_holding)
        {
            _holding = true;
            _menuButtons[_currentlyChosen].GetComponent<Renderer>().material = _materials[0];
            _currentlyChosen++;
            if (_currentlyChosen >= _menuButtons.Length)
            {
                _currentlyChosen = 0;
            }
            _menuButtons[_currentlyChosen].GetComponent<Renderer>().material = _materials[1];
        }
        else if (_holding && Mathf.Abs(Input.GetAxisRaw("Vertical")) <= 0.5f) 
        {
            _holding = false;
        }
	}
}
