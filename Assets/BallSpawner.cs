using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour {

    public GameObject[] Balls;

    public Transform cannon;

    //[SerializeField]
    //float _firingInterval = 5f;
    [SerializeField]
    float _fireForce = 5f;


	// Use this for initialization
	void Start () {
		foreach(GameObject ball in Balls)
        {
            ball.transform.position = cannon.position;
        }
	}
	
	// Physics related stuff in fixed update
	void FixedUpdate () {
        foreach(GameObject ball in Balls)
        {
            if(ball.transform.position.y < -5)
            {
                ball.SetActive(false);
                ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                
            }
            if (!ball.activeSelf)
            {
                ball.transform.position = cannon.position;
                ball.SetActive(true);
                ball.GetComponent<Rigidbody>().AddForce(cannon.up * _fireForce, ForceMode.Impulse);
            }
        }
    }
}
