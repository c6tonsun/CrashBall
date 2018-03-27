using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionScale : MonoBehaviour {

    Renderer renderer;
    Material mat;
    Color baseColor;

    [SerializeField]
    private Goal playerGoal;

    private float playerHP;


    // Use this for initialization
    void Start () {       
        renderer = GetComponent<Renderer>();
        mat = renderer.material;
        baseColor = new Color(0.13f, 0.27f, 0.54f);
    }
	
	// Update is called once per frame
	void Update () {
        playerHP = playerGoal.GetCurrentLives()+1;

        float emission = (1f-(5/playerHP));
        //Replace this with whatever you want for your base color at emission level '1'

        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

        mat.SetColor("_EmissionColor", finalColor);
    }
}
