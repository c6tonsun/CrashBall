using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartPulse : MonoBehaviour
{
    public LayerMask layerMask;
    public Transform PulseOrigin;
    public float PulseRadius;
    public float PulseForce;

    private ParticleSystem my_ParticleSystem;

    Collider[] colliders;

    public void Awake()
    {
        my_ParticleSystem = GetComponent<ParticleSystem>();
    }

    public void FixedUpdate()
    {
        
    }

    public void Pulse()
    {
        my_ParticleSystem.Play();
        colliders = Physics.OverlapSphere(transform.position, PulseRadius, layerMask);
        Ball[] balls = new Ball[colliders.Length];
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i] = colliders[i].GetComponent<Ball>();
        }

        foreach (Ball ball in balls)
        {
            var oldVelocity = ball.Rb.velocity.magnitude;
            ball.Rb.velocity = Vector3.zero;
            ball.Rb.AddForce((ball.transform.position - PulseOrigin.position).normalized * (PulseForce+oldVelocity), ForceMode.Impulse);
            Debug.Log("Pulse");
            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(PulseOrigin.position, PulseRadius);
    }
}
