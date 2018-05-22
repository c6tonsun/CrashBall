using UnityEngine;

public class DeathWallParticleHandler : MonoBehaviour {

    private ParticleSystem[] goalParticles;

	// Use this for initialization
	void Awake ()
    {
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
