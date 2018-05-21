using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    [SerializeField]
    PlayFMODEvent[] pulseSound;
    [SerializeField]
    ParticleSystem pulseParticles;

    ParticleSystem[] bombParticles;

    [SerializeField]
    private Color color;

    private Coroutine secondPulse;

    public bool HitGround = false;

    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float PulseRadius = 5f;
    [SerializeField]
    private float PulseForce = 20f;

    Collider[] colliders;

    private void Start()
    {
        bombParticles = GetComponentsInChildren<ParticleSystem>();
        pulseSound[0].Play();
    }
    private void Update()
    {
        if (!HitGround)
        {
            foreach (var system in bombParticles)
            {
                if (!system.isPlaying) system.Play();
            }
        }
        else
        {
            Destroy(gameObject, 1.3f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        HitGround = true;
        GetComponent<BoxCollider>().enabled = false;
        Pulse();
    }

    private void Pulse() {
        Instantiate(pulseParticles, transform.position, transform.rotation);

        pulseSound[1].Play(playAnyway:true);

        colliders = Physics.OverlapSphere(transform.position, PulseRadius* 0.75f, layerMask);
        Ball[] balls = new Ball[colliders.Length];
        for (int i = 0; i<balls.Length; i++)
            balls[i] = colliders[i].GetComponent<Ball>();

        foreach (Ball ball in balls)
        {
            if (ball.canBePulsed)
            {
                ball.Rb.velocity = Vector3.zero;
                Vector3 direction = ball.transform.position - transform.position;
                if (ball.isFixedY)
                    direction.y = 0f;
                ball.Rb.AddForce(direction.normalized* (PulseForce), ForceMode.Impulse);
                ball.SpawnPulseBlastOff(color, direction.normalized);
                
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
                ball.Rb.velocity = Vector3.zero;
                Vector3 direction = ball.transform.position - transform.position;
                if (ball.isFixedY)
                    direction.y = 0f;
                ball.Rb.AddForce(direction.normalized * (PulseForce), ForceMode.Impulse);
                ball.SpawnPulseBlastOff(color, direction.normalized);
            }
        }
        Debug.Log("before end");
        StopCoroutine(secondPulse);
        Debug.Log("end");
    }

}
