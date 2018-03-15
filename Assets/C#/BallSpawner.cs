using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour {

    public List<Ball> Balls;

    [SerializeField]
    private GameObject ballDeath;
    [SerializeField]
    private GameObject ballRespawn;

    [SerializeField]
    private Ball ballPrefab;

    public int maxBAAALLSSSSSSSSS = 8;

    public Transform[] Cannons;
    private Transform nextCannon;
    private int CannonAmount;

    [SerializeField]
    float _firingInterval = 5f;
    float _ballTimer = 0f;
    [SerializeField]
    float _fireForce = 5f;

    private int _lastCannon = -1;

    bool canFire = false;

    // Use this for initialization
    void Start ()
    {
        for(int i = 0; i<maxBAAALLSSSSSSSSS; i++)
        {
            //Instantiates all balls to somewhere far away
            Balls.Add(Instantiate(ballPrefab, new Vector3(-25,-25, 25), transform.rotation, transform));
        }
        CannonAmount = Cannons.Length;
        //Randomizes the first cannon to shoot.
        nextCannon = RandomizeCannon();
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

        _firingInterval += -(Time.deltaTime/90f);
        CheckBallCount();
    }

    //Checks if number of balls is still maxballs, sometimes they get deleted by trash collector or something
    private void CheckBallCount()
    {
        if (Balls.Count != maxBAAALLSSSSSSSSS)
        {
            var missing = maxBAAALLSSSSSSSSS - Balls.Count;
            for (int i = 0; i < missing; i++)
            {
                Balls.Add(Instantiate(ballPrefab, new Vector3(-25, -25, 25), transform.rotation, transform));
            }
        }
    }

    //Randomizes the next cannon to be fired. Cannot give same number twice in a row
    Transform RandomizeCannon()
    {
        _lastCannon = System.Array.IndexOf(Cannons, nextCannon);
        int new_random = Random.Range (0, CannonAmount);
        while (_lastCannon == new_random)
        {
            new_random = Random.Range(0, CannonAmount);
        }
        Debug.Log(new_random);
        return Cannons [new_random];
    }
	
	// Physics related stuff in fixed update
	void FixedUpdate () {     
        foreach(Ball ball in Balls)
        {
            if (ball != null)
            {
                if (ball.transform.position.y < -0.5 && ball.gameObject.activeSelf)
                {
                    ball.Rb.velocity = Vector3.zero;
                    ball.gameObject.SetActive(false);
                    //nextCannon = RandomizeCannon();
                    Destroy(Instantiate(ballDeath, ball.transform.position, ballDeath.transform.rotation), 2.6f);
                }
            }
        }
        if (canFire)
        {
            if (GetInactiveBall() != null)
            {
                PrepareFire(GetInactiveBall());
                canFire = false;
            }
        }

    }

    private Ball GetInactiveBall()
    {
        foreach(Ball ball in Balls)
        {
            if (!ball.gameObject.activeSelf)
            {
                return ball;
            }
        }
        return null;
    }
    private void PrepareFire(Ball ball)
    {
        var cannon = nextCannon;
        var offsetDirection = (Random.Range(0, 2) == 0) ? -2 : 2;  // optimized :D
        ball.transform.position = cannon.position;
        Destroy(Instantiate(ballRespawn, cannon.transform.position+cannon.forward, cannon.transform.rotation), 2.4f);
        ball.gameObject.SetActive(true);        
        ball.Rb.AddForce((cannon.forward * (_fireForce+1*offsetDirection) + cannon.right * offsetDirection), ForceMode.Impulse); // TODO: This works irregurarily, no idea why. Possible fix: actually lerp the cannon to turn around.
        nextCannon = RandomizeCannon();
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < Cannons.Length; i++)
        {
            var cannon = Cannons[i];
            Debug.DrawLine(cannon.position, (cannon.position)+cannon.forward*18, Color.red);
        }
    }
}
