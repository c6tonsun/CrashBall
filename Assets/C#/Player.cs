﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public enum Number
    {
        ERROR = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4
    }
    
    public Number currentPlayer;

    public bool hasController;

    [SerializeField]
    private GameObject DeathParticlePrefab;
    private float _deathParticleDuration;

    private CameraShake cameraShake;

    [HideInInspector]
    public KartMovement movement;
    [HideInInspector]
    public KartPulse pulse;

    public PlayerWall myWall;
    public Goal myGoal;
    [HideInInspector]
    public bool isLive = true;

    private Animator kartAnimator;

    private IEnumerator Death()
    {
        kartAnimator.SetBool("isDead", true);
        Destroy(Instantiate(DeathParticlePrefab, transform.position, transform.rotation), _deathParticleDuration);
        if (kartAnimator.GetBool("Bonus"))
        {
            yield return new WaitForSeconds(0.45f);
            cameraShake.SetShakeTime(0.5f);
            yield return new WaitForSeconds(1.5f);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }
        
        gameObject.SetActive(false);
        StopCoroutine("Death");
    }

    private void Awake()
    {
        movement = GetComponentInChildren<KartMovement>();
        pulse = GetComponentInChildren<KartPulse>();
        kartAnimator = GetComponentInChildren<Animator>();
        cameraShake = FindObjectOfType<CameraShake>();
        _deathParticleDuration = DeathParticlePrefab.GetComponent<ParticleSystem>().main.duration + 0.7f;
    }

    public void Die()
    {
        myWall.gameObject.SetActive(true);
        myWall.player = this;
        myWall.playerGoal = myGoal;
        isLive = false;
        StartCoroutine("Death");
    }
}
