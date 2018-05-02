using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuePickerManager : MonoBehaviour {

    [SerializeField]
    public Color[] colors;

    public HuePicker[] pickers;

    public MeshRenderer[] floorMesh;

    private GameManager gameManager;

    // Use this for initialization
    void Start () {
        pickers = GetComponentsInChildren<HuePicker>();
        gameManager = FindObjectOfType<GameManager>();

        Transform[] tempQuads = transform.GetComponentsInChildren<Transform>();
        int index = 0;
        for (int i = 0; i < tempQuads.Length; i++)
        {
            if (tempQuads[i].name.Contains("Quad"))
            {
                floorMesh[index] = tempQuads[i].GetComponent<MeshRenderer>();
                index++;
            }
        }
    }
	
	// Update is called once per frame
	public void DoUpdate () {
        for (int p = 0; p < pickers.Length; p++)
        {
            floorMesh[p].material.color = colors[p];
            pickers[p].DoUpdate();
        }
        
        gameManager.Colors = colors;
    }
}
