using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour {

    public List<Ball> Balls;

    [SerializeField]
    private Ball ballPrefab;

    public int maxBAAALLSSSSSSSSS = 8;

    public Transform[] Cannons = new Transform[4];

    [SerializeField]
    float _firingInterval = 5f;
    float _ballTimer = 0f;
    [SerializeField]
    float _fireForce = 5f;

    bool canFire = false;


	// Use this for initialization
	void Start ()
    {
        for(int i = 0; i<maxBAAALLSSSSSSSSS; i++)
        {
            Balls.Add(Instantiate(ballPrefab, transform.position, transform.rotation, transform));
        }
	}

    private void Update()
    {
        if (_ballTimer >= 0)
        {
            _ballTimer -= Time.deltaTime;
        }
        else
        {
            canFire = true;
            _ballTimer = _firingInterval;
        }
    }

    Transform RandomizeCannon()
    {
        return Cannons[Random.Range(0, 4)];
    }
	
	// Physics related stuff in fixed update
	void FixedUpdate () {
        int inActiveBalls = 0;        
        foreach(Ball ball in Balls)
        {
            if(ball.transform.position.y < -5)
            {
                ball.Rb.velocity = Vector3.zero;
                ball.gameObject.SetActive(false);
                
            }
            if (!ball.gameObject.activeSelf && canFire)
            {
                FireBall(ball);               
                canFire = false;
            }
        }
        foreach (Ball ball in Balls)
        {
            if (!ball.gameObject.activeSelf) inActiveBalls++;
            if (inActiveBalls == Balls.Count)
            {
                FireBall(ball);
            }
        }
    }

    private void FireBall(Ball ball)
    {
        var cannon = RandomizeCannon();
        var offsetDirection = (Mathf.Sign(Random.Range(-1f, 1f))==-1)? -2: 2;
        ball.gameObject.SetActive(true);
        ball.transform.position = cannon.position;
        ball.Rb.AddForce(cannon.forward * _fireForce + cannon.right * offsetDirection, ForceMode.VelocityChange);
    }
    private void OnDrawGizmos()
    {
        for (int i = 0; i < Cannons.Length; i++)
        {
            var cannon = Cannons[i];
            Debug.DrawLine(cannon.position, (cannon.position)+cannon.forward*3, Color.red);
        }
    }
}
