using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseAlwaysOnEmitter : MonoBehaviour {

    [SerializeField]
    private Color playerColor;
    [SerializeField]
    private Color cooldownColor;
    private ParticleSystem particles;

    private KartPulse pulseController;

    private float pulseCooldown;
    [SerializeField]
    private float pulseTimer;

    [SerializeField]
    private bool missedPulse;


	// Use this for initialization
	void Start () {
        playerColor = GetComponentInParent<Player>().GetColor();
        cooldownColor = Color.red;

        pulseController = GetComponentInParent<KartPulse>();

        particles = GetComponent<ParticleSystem>();

        pulseCooldown = FindObjectOfType<GameManager>().pulseCooldown;
	}
	
	// Update is called once per frame
	void Update () {
        pulseTimer = pulseController.GetPulseTimer();
        missedPulse = pulseController.isLongCooldown();
        var particleMain = particles.main;

        if (pulseTimer < pulseCooldown && missedPulse)
        {
            if (!particles.isPlaying) particles.Play();
            particleMain.startColor = cooldownColor;            
        }
        else if (pulseTimer < pulseCooldown && !missedPulse)
        {
            if(particles.isPlaying) particles.Stop();
        }
        else
        {
            particleMain.startColor = playerColor;
            if (!particles.isPlaying) particles.Play();
        }
	}
}
