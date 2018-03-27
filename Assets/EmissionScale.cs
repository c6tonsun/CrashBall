using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionScale : MonoBehaviour {

    Renderer _renderer;
    Material mat;
    Color baseColor;

    [SerializeField]
    private Goal playerGoal;
    private ParticleSystem particles;

    private int playerMaxHP;
    private float playerHP;

    private float oldPlayerHP = 0;


    // Use this for initialization
    void Start () {       
        _renderer = GetComponent<Renderer>();
        mat = _renderer.material;
        particles = GetComponent<ParticleSystem>();
        baseColor = new Color(0.13f, 0.27f, 0.54f);
        playerMaxHP = FindObjectOfType<GameManager>().playerLives;
    }

    // Update is called once per frame
    void Update() {
        playerHP = playerGoal.GetCurrentLives();

        float emission = (1f - (playerHP / playerMaxHP));

        Color finalColor = baseColor * Mathf.LinearToGammaSpace(0);
        if (emission > 0.1f)
        {
            finalColor = baseColor * Mathf.LinearToGammaSpace(0.2f);
        }
        if (emission > 0.3f)
        {
            finalColor = baseColor * Mathf.LinearToGammaSpace(0.5f);
        }
        if (emission > 0.6)
        {
            finalColor = baseColor * Mathf.LinearToGammaSpace(1f);
            if(!particles.isPlaying)particles.Play();
        }
        if (emission >= 1f)
        {
            finalColor = baseColor * Mathf.LinearToGammaSpace(1.5f);
        }

        mat.SetColor("_EmissionColor", finalColor);
    }
}
