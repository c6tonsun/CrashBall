using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSelection : MonoBehaviour {

	/***
	 * Changing this to affect all menus.
	 * First is mainmenu, with two buttons.
	 * Second is characterselect, with areas for all players.
	 * These areas have first a simple "press left or right to change character",
	 * and when a character has been selected a small menu to choose the characters color.
	 * Third menu is mode-stage-ball_select, and it's cut into three parts.
	 * Left part has multiple buttons to choose a mode from.
	 * Middle part has a picture of chosen stage, with buttons on the left and right to change it.
	 * Third part has buttons for ball options, that you can choose to activate or de-activate, and a play button(?)
	 *** 
	 * I will put the buttons in seperate arrays.
	 * mainmenu will have just the two buttons.
	 * charselect will have an array for each player, which have both the left and right arrows, and the color selections.
	 * gameoptions will have multiple arrays, one for modes, one for stages and one for ball options.
	 ***/

    public GameObject[] _menuButtons;
    int _currentlyChosen = 0;
    public Material[] _materials;
    bool _holding = false;

	[SerializeField]
	GameObject[] _mainMenu;

	[SerializeField]
	GameObject[] _char1Menu;

	[SerializeField]
	GameObject[] _char2Menu;

	[SerializeField]
	GameObject[] _char3Menu;

	[SerializeField]
	GameObject[] _char4Menu;

	[SerializeField]
	GameObject[] _modeMenu;

	[SerializeField]
	GameObject[] _stageMenu;

	[SerializeField]
	GameObject[] _ballMenu;

	string _currentMenu = "main";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		switch (_currentMenu) 
		{
			case "main":
				if (Input.GetAxisRaw ("Vertical") > 0.5f && !_holding) 
				{
					_holding = true;
					_menuButtons [_currentlyChosen].GetComponent<Renderer> ().material = _materials [0];
					_currentlyChosen--;
					if (_currentlyChosen < 0) {
						_currentlyChosen = _menuButtons.Length - 1;
					}
					_menuButtons [_currentlyChosen].GetComponent<Renderer> ().material = _materials [1];


				} 
				else if (Input.GetAxisRaw ("Vertical") < -0.5f && !_holding) 
				{
					_holding = true;
					_menuButtons [_currentlyChosen].GetComponent<Renderer> ().material = _materials [0];
					_currentlyChosen++;
					if (_currentlyChosen >= _menuButtons.Length) {
						_currentlyChosen = 0;
					}
					_menuButtons [_currentlyChosen].GetComponent<Renderer> ().material = _materials [1];
				}
				/*else if(Input.GetAxisRaw("fire"))
				{
					if (_currentlyChosen == 0) {
						_currentMenu = "char";
					} 
					else 
					{
						Application.Quit;
					}
					
				*/else if (_holding && Mathf.Abs (Input.GetAxisRaw ("Vertical")) <= 0.5f) 
				{
					_holding = false;
				}
				break;
			default:
				Debug.Log ("something went wrong, currentMenu not recognized");
				break;
		}
	}
}
