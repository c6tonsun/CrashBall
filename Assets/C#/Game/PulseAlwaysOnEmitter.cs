using UnityEngine;

public class PulseAlwaysOnEmitter : MonoBehaviour {

    [SerializeField]
    private ParticleSystem.MinMaxGradient playerColor;
    [SerializeField]
    private ParticleSystem.MinMaxGradient cooldownColor;
    private ParticleSystem particles;

    private KartPulse pulseController;

    private float pulseCooldown;
    [SerializeField]
    private float pulseTimer;

    [SerializeField]
    private bool missedPulse;
    private bool magnetOn;


	// Use this for initialization
	void Start () {
        var playerGetcolor = GetComponentInParent<Player>().GetColor();
        var playerComplor = CarLightColor.CreateComplementaryColor(playerGetcolor);
        playerColor.colorMax = playerComplor;
        playerColor.colorMin = playerGetcolor;
        cooldownColor.colorMax = Color.red;
        cooldownColor.colorMin = Color.red * 2f;

        playerColor.mode = ParticleSystemGradientMode.TwoColors;
        cooldownColor.mode = ParticleSystemGradientMode.TwoColors;

        pulseController = GetComponentInParent<KartPulse>();

        particles = GetComponent<ParticleSystem>();

        pulseCooldown = FindObjectOfType<GameManager>().pulseCooldown;
	}
	
	// Update is called once per frame
	void Update () {
        pulseTimer = pulseController.GetPulseTimer();
        missedPulse = pulseController.isLongCooldown;
        magnetOn = pulseController.isMagneton;

        var particleMain = particles.main;

        if (pulseTimer < pulseCooldown && missedPulse)
        {
            if (!particles.isPlaying) particles.Play();
            particleMain.startColor = cooldownColor;            
        }
        else if (pulseTimer < pulseCooldown && !missedPulse || magnetOn)
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
