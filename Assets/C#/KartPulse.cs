using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartPulse : MonoBehaviour
{
    public Transform PulseOrigin;
    public float PulseRadius;
    public float PulseForce;

    public void Pulse()
    {
        Collider[] colliders = Physics.OverlapSphere(PulseOrigin.position, PulseRadius, 8, QueryTriggerInteraction.Ignore);
        Ball[] balls = new Ball[colliders.Length];
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i] = colliders[i].GetComponent<Ball>();
        }

        foreach (Ball ball in balls)
        {
            ball.Rb.AddForce((PulseOrigin.position - ball.transform.position).normalized * PulseForce);
        }
    }
}
