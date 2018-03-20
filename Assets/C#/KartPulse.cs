using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartPulse : MonoBehaviour
{
    public LayerMask layerMask;
    public Transform PulseOrigin;
    public float PulseRadius;
    public float PulseForce;

    public float pulseCooldown = 0.5f;
    private float pulseTimer;
    private ParticleSystem my_ParticleSystem;

    [SerializeField]
    private MeshRenderer my_meshRenderer;

    Collider[] colliders;

    public void Start()
    {
        my_ParticleSystem = GetComponent<ParticleSystem>();
        var colour = my_ParticleSystem.main;
        colour.startColor = my_meshRenderer.material.color;
    }

    private void Update()
    {
        pulseTimer += Time.deltaTime;
    }

    public void Pulse()
    {
        if (pulseTimer < pulseCooldown)
            return;
        else
            pulseTimer = 0f;

        my_ParticleSystem.Play();
        colliders = Physics.OverlapSphere(transform.position, PulseRadius, layerMask);
        Ball[] balls = new Ball[colliders.Length];
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i] = colliders[i].GetComponent<Ball>();
        }

        foreach (Ball ball in balls)
        {
            if (ball.canBePulsed)
            {
                float oldVelocity = ball.Rb.velocity.magnitude;
                ball.Rb.velocity = Vector3.zero;
                Vector3 direction = ball.transform.position - PulseOrigin.position;
                if (ball.isFixedY)
                    direction.y = 0f;
                ball.Rb.AddForce(direction.normalized * (PulseForce + oldVelocity), ForceMode.Impulse);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(PulseOrigin.position, PulseRadius);
    }
}
