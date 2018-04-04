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
    private float pulseCooldown;
    private float pulseTimer;
    private bool canPulse;
    private ParticleSystem pulseParticles;
    [SerializeField]
    private ParticleSystem magnetParticles;

    [SerializeField]
    private MeshRenderer my_meshRenderer;

    Collider[] colliders;

    private void Start()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        PulseRadius = gameManager.pulseArea;
        PulseForce = gameManager.pulseForce;
        pulseCooldown = gameManager.pulseCooldown;
        maxMagnetTime = gameManager.maxMagnetTime;

        pulseParticles = GetComponent<ParticleSystem>();
        var colour = pulseParticles.main;
        colour.startColor = my_meshRenderer.material.color;

        magnetParticles = FindPulseOrigin();
        var magnetColour = magnetParticles.main;
        magnetColour.startColor = my_meshRenderer.material.color;
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
        if (!canPulse)
            pulseTimer += Time.deltaTime;
        else
            magnetTimer += Time.deltaTime;
    }

    public void Magnet()
    {
        if (pulseTimer < pulseCooldown)
            return;

        canPulse = true;

        if (magnetTimer > maxMagnetTime)
        {
            Pulse();
            magnetParticles.Stop();
            return;
        }
        
        if (magnetTimer < 0.1f)
            return;

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
                
            }
        }
    }

    public void Pulse()
    {
        
        if (!canPulse)
            return;
        else
            canPulse = false;
            magnetParticles.Stop();

        pulseTimer = 0f;
        magnetTimer = 0f;

        pulseParticles.Play(withChildren:false);
        colliders = Physics.OverlapSphere(transform.position, PulseRadius, layerMask);
        Ball[] balls = new Ball[colliders.Length];
        for (int i = 0; i < balls.Length; i++)
            balls[i] = colliders[i].GetComponent<Ball>();

        foreach (Ball ball in balls)
        {
            if (ball.canBePulsed)
            {
                float oldVelocity = ball.Rb.velocity.magnitude;
                ball.Rb.velocity = Vector3.zero;
                Vector3 direction = ball.transform.position - transform.position;
                if (ball.isFixedY)
                    direction.y = 0f;
                ball.Rb.AddForce(direction.normalized * (PulseForce), ForceMode.Impulse);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(PulseOrigin.position, PulseRadius);
    }
}
