using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartPulse : MonoBehaviour
{
    public LayerMask layerMask;
    public Transform PulseOrigin;
    public float PulseRadius;
    public float PulseForce;

    Collider[] colliders;

    public void FixedUpdate()
    {
        
    }

    public void Pulse()
    {
        colliders = Physics.OverlapSphere(transform.position, PulseRadius, layerMask);
        Debug.Log("colliders recieved: " + colliders.Length);
        Ball[] balls = new Ball[colliders.Length];
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i] = colliders[i].GetComponent<Ball>();
        }

        foreach (Ball ball in balls)
        {
            ball.Rb.AddForce((ball.transform.position - PulseOrigin.position).normalized * PulseForce);
            Debug.Log("Pulse");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(PulseOrigin.position, PulseRadius);
    }
}
