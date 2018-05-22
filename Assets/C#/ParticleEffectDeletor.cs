using UnityEngine;

public class ParticleEffectDeletor : MonoBehaviour {

    private float maxLifetime = 1f;

    //Checks maximum lifetime of particle system + 0.2f and destroys object after that
	void Start () {
        maxLifetime = GetComponent<ParticleSystem>().main.startLifetime.constantMax + 0.2f;
        Destroy(gameObject, maxLifetime);
	}
}
