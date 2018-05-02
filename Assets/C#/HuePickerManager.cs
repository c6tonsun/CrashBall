using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuePickerManager : MonoBehaviour {

    [SerializeField]
    public Color[] colors;

    public HuePicker[] pickers;

    public MeshRenderer[] playerMesh;

    private GameManager gameManager;

    // Use this for initialization
    void Start () {
        pickers = GetComponentsInChildren<HuePicker>();
        playerMesh = GetComponentsInChildren<MeshRenderer>();
        gameManager = FindObjectOfType<GameManager>();
    }
	
	// Update is called once per frame
	public void DoUpdate () {
        for (int p = 0; p < pickers.Length; p++)
        {
            playerMesh[p].material.color = colors[p];
            pickers[p].DoUpdate();
        }
        
        gameManager.Colors = colors;
    }
}
