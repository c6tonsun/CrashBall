using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartPulse : MonoBehaviour
{
    public LayerMask layerMask;
    public Transform PulseOrigin;
    private float PulseRadius;
    private float PulseForce;

    private float pulseCooldown;
    private float pulseTimer;
    private bool canPulse;
    private ParticleSystem my_ParticleSystem;

    [SerializeField]
    private MeshRenderer my_meshRenderer;

    Collider[] colliders;

    private void Start()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        PulseRadius = gameManager.pulseArea;
        PulseForce = gameManager.pulseForce;
        pulseCooldown = gameManager.pulseCooldown;

        my_ParticleSystem = GetComponent<ParticleSystem>();
        var colour = my_ParticleSystem.main;
        colour.startColor = my_meshRenderer.material.color;
    }

    private void Update()
    {
        if (!canPulse)
            pulseTimer += Time.deltaTime;
    }

    public void Magnet()
    {
        if (pulseTimer < pulseCooldown)
            return;

        canPulse = true;
        pulseTimer = 0f;

        colliders = Physics.OverlapSphere(transform.position, PulseRadius, layerMask);
        Ball[] balls = new Ball[colliders.Length];
        for (int i = 0; i < balls.Length; i++)
            balls[i] = colliders[i].GetComponent<Ball>();

        foreach (Ball ball in balls)
        {
            float inForce = ball.Rb.velocity.magnitude;
            Vector3 direction = PulseOrigin.position - ball.transform.position;
            ball.isMagneted = true;

            if (direction.magnitude > 1)
            {
                ball.Rb.velocity *= 0.9f;
                ball.Rb.AddForce(direction.normalized * inForce * 0.2f, ForceMode.Impulse);
            }
        }
    }

    public void Pulse()
    {
        if (!canPulse)
            return;
        else
            canPulse = false;

        my_ParticleSystem.Play();
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
                ball.isMagneted = false;
                ball.Rb.AddForce(direction.normalized * (PulseForce + oldVelocity), ForceMode.Impulse);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(PulseOrigin.position, PulseRadius);
    }
}
