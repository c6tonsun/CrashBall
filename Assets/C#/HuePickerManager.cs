using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuePickerManager : MonoBehaviour {

    [SerializeField]
    public Color[] colors;

    public int[] players;

    public MeshRenderer[] playerColors;

	// Use this for initialization
	void Start () {
        players = new int[4];
	}
	
	// Update is called once per frame
	void Update () {
        for (int p = 0; p < players.Length; p++)
        {
            playerColors[p].material.color = colors[p];
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            FindObjectOfType<HuePicker>().SetCurrentPlayer(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            FindObjectOfType<HuePicker>().SetCurrentPlayer(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            FindObjectOfType<HuePicker>().SetCurrentPlayer(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            FindObjectOfType<HuePicker>().SetCurrentPlayer(3);
        }
    }
}
