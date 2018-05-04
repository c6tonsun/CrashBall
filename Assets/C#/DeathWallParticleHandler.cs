using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWallParticleHandler : MonoBehaviour {

    private ParticleSystem[] goalParticles;

	// Use this for initialization
	void Start () {
        goalParticles = GetComponentsInChildren<ParticleSystem>();

	}
	
    public void ActivateParticleWall()
    {
        if (goalParticles == null)
            return;

        foreach(var goal in goalParticles)
        {
            goal.Play();
        }
    }
}
