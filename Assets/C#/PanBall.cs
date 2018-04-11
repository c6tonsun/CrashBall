using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanBall : Ball {

    [SerializeField]
    private GameObject DeathParticles;
    private float particleDuration;

    private void Start()
    {
        particleDuration = DeathParticles.GetComponent<ParticleSystem>().main.duration+0.5f;
    }

    new void FixedUpdate () {
        base.FixedUpdate();
        if (!canScore)
        {
            Destroy(Instantiate(DeathParticles, transform.position, transform.rotation), particleDuration);
            gameObject.SetActive(false);
        }
	}
}
