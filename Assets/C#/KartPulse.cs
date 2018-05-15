using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartPulse : MonoBehaviour
{
    public LayerMask layerMask;
    public Transform PulseOrigin;
    private float PulseRadius;
    private float PulseForce;

    private float maxMagnetTime;
    private float magnetTimer;
    private float magnetCooldown = -1f;
    private float pulseCooldown;
    private float pulseTimer;

    private PlayFMODEvent pulseSound;

    private bool pulseHitsBalls;

    private ParticleSystem pulseParticles;
    [SerializeField]
    private ParticleSystem magnetParticles;

    private ParticleSystem.MinMaxGradient pulseColor;

    private Player player;

    private Coroutine secondPulse;

    Collider[] colliders;

    private void Start()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        PulseRadius = gameManager.pulseArea;
        PulseForce = gameManager.pulseForce;
        pulseCooldown = gameManager.pulseCooldown;
        maxMagnetTime = gameManager.maxMagnetTime;

        pulseSound = GetComponent<PlayFMODEvent>();

        player = GetComponentInParent<Player>();

        var PlayerColor = player.GetColor();

        pulseColor.colorMax = PlayerColor;
        pulseColor.colorMin = CarLightColor.CreateComplementaryColor(PlayerColor);

        pulseColor.mode = ParticleSystemGradientMode.TwoColors;

        pulseParticles = GetComponent<ParticleSystem>();
        var colour = pulseParticles.main;
        colour.startColor = pulseColor;


        magnetParticles = FindPulseOrigin();
        var magnetColour = magnetParticles.main;
        magnetColour.startColor = pulseColor;
    }

    private ParticleSystem FindPulseOrigin()
    {
        var PS_array = GetComponentsInChildren<ParticleSystem>();
        foreach (var PS in PS_array)
        {
            if (PS.gameObject.name.Contains("PulseOrigin"))
            {
                return PS;
            }
        }
        Debug.Log("Error: PulseOrigin not found in children");
        return null;
    }

    private void Update()
    {
        pulseTimer += Time.deltaTime;
        magnetTimer += Time.deltaTime;

        isMagneton = magnetParticles.isPlaying;
    }

    public float GetPulseTimer() { return pulseTimer; }
    public bool isLongCooldown { private set; get; }
    public bool isMagneton { private set; get; }

    public void Magnet()
    {        
        if ((pulseTimer < pulseCooldown) || magnetTimer < 0)
            return;

        if (magnetTimer > maxMagnetTime)
        {
            magnetParticles.Stop();
            magnetTimer = magnetCooldown;
            return;
        }
        if(!magnetParticles.isPlaying) magnetParticles.Play();

        colliders = Physics.OverlapSphere(transform.position, PulseRadius, layerMask);
        Ball[] balls = new Ball[colliders.Length];
        for (int i = 0; i < balls.Length; i++)
            balls[i] = colliders[i].GetComponent<Ball>();
        
        foreach (Ball ball in balls)
        {
            if (!ball.canBePulsed || !ball.canScore || !ball.canBeMagneted)
                continue;
            
            float inForce = ball.Rb.velocity.magnitude;
            Vector3 direction = PulseOrigin.position - ball.transform.position;
            ball.magnetedTime = Time.fixedDeltaTime * 4;

            if (direction.magnitude > 1)
            {
                if (inForce > ball.minSpeed)
                    inForce = ball.minSpeed;
                ball.Rb.velocity *= 0.9f;

                if (ball.isFixedY)
                    direction.y = 0f;

                ball.Rb.AddForce(direction.normalized * inForce * 0.4f, ForceMode.Impulse);
                ball.SetLastHitPlayer(player, false);
            }
        }
    }

    public void Pulse()
    {
        if (pulseTimer < pulseCooldown)
        {
            return;
        }

        pulseHitsBalls = false;

        pulseTimer = 0f;
        magnetTimer = magnetCooldown;
        isLongCooldown = false;

        magnetParticles.Stop();
        pulseParticles.Play(withChildren:false);

        pulseSound.Play(playAnyway:true);

        colliders = Physics.OverlapSphere(transform.position, PulseRadius * 0.75f, layerMask);
        Ball[] balls = new Ball[colliders.Length];
        for (int i = 0; i < balls.Length; i++)
            balls[i] = colliders[i].GetComponent<Ball>();

        foreach (Ball ball in balls)
        {
            if (ball.canBePulsed)
            {
                pulseHitsBalls = true;
                ball.Rb.velocity = Vector3.zero;
                Vector3 direction = ball.transform.position - transform.position;
                if (ball.isFixedY)
                    direction.y = 0f;
                ball.Rb.AddForce(direction.normalized * (PulseForce), ForceMode.Impulse);
                ball.SetLastHitPlayer(player, true);
                ball.SpawnPulseBlastOff(player, direction.normalized);
                
            }
        }
        secondPulse = StartCoroutine(SecondPulse());
    }

    IEnumerator SecondPulse()
    {
        yield return new WaitForSeconds(0.1f);

        colliders = Physics.OverlapSphere(transform.position, PulseRadius, layerMask);
        Ball[] balls = new Ball[colliders.Length];
        for (int i = 0; i < balls.Length; i++)
            balls[i] = colliders[i].GetComponent<Ball>();

        foreach (Ball ball in balls)
        {
            float distanceNow = (ball.transform.position - transform.position).sqrMagnitude;
            float distanceNext = ((ball.transform.position + (ball.Rb.velocity * Time.fixedDeltaTime)) - transform.position).sqrMagnitude;

            if (distanceNow < distanceNext)
                continue;

            if (ball.canBePulsed)
            {
                pulseHitsBalls = true;
                ball.Rb.velocity = Vector3.zero;
                Vector3 direction = ball.transform.position - transform.position;
                if (ball.isFixedY)
                    direction.y = 0f;
                ball.Rb.AddForce(direction.normalized * (PulseForce), ForceMode.Impulse);
                ball.SetLastHitPlayer(player, true);
                ball.SpawnPulseBlastOff(player, direction.normalized);
            }
        }
        if (!pulseHitsBalls)
        {
            pulseTimer = -0.8f;
            isLongCooldown = true;
        }
        StopCoroutine(secondPulse);
    }

    public void ResetMagnet()
    {
        if (magnetTimer > 0)
        {
            magnetTimer = 0f;
            magnetParticles.Stop();
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, PulseRadius);
    }
}
