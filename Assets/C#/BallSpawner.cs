using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour {

    public List<Ball> normalBalls;
    private int maxNormalBallCount;

    [SerializeField]
    private GameObject ballDeath;
    [SerializeField]
    private GameObject ballRespawn;

    [SerializeField]
    private Ball ballPrefab;

    public Transform[] Cannons;
    private Transform nextCannon;
    private int CannonAmount;
    
    float _firingInterval;
    float _ballTimer = 0f;
    [SerializeField]
    float _fireForce = 5f;

    private int _lastCannon = -1;

    bool canFire = false;

    // Use this for initialization
    void Start ()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        // normal balls
        maxNormalBallCount = gameManager.normalBallCount;
        for(int i = 0; i < maxNormalBallCount; i++)
        {
            //Instantiates all balls to somewhere far away
            Ball ball = Instantiate(ballPrefab, new Vector3(-25, 100, 25), transform.rotation, transform);
            ball.gameObject.SetActive(false);
            ball.canFly = gameManager.ballCanFly;
            ball.minSpeed = gameManager.ballMinSpeed;

            normalBalls.Add(ball);
        }
        // cannon
        _firingInterval = gameManager.firingInterval;
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
        if (normalBalls.Count != maxNormalBallCount)
        {
            var missing = maxNormalBallCount - normalBalls.Count;
            for (int i = 0; i < missing; i++)
            {
                normalBalls.Add(Instantiate(ballPrefab, new Vector3(-25, 100, 25), transform.rotation, transform));
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
        return Cannons [new_random];
    }
	
	// Physics related stuff in fixed update
	void FixedUpdate () {
        /*
        foreach(Ball ball in normalBalls)
        {
            if (ball != null)
            {
                if (ball.transform.position.y < -5 && ball.gameObject.activeSelf)
                {
                    ball.Rb.velocity = Vector3.zero;
                    ball.gameObject.SetActive(false);
                    //nextCannon = RandomizeCannon();
                    Destroy(Instantiate(ballDeath, ball.transform.position, ballDeath.transform.rotation), 2.6f);
                }
            }
        }
        */
        if (canFire)
        {
            Ball ball = GetInactiveBall();
            if (ball != null)
            {
                PrepareFire(ball);
                canFire = false;
            }
        }

    }

    private Ball GetInactiveBall()
    {
        foreach(Ball ball in normalBalls)
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
        ball.Rb.AddForce((cannon.forward * (_fireForce)), ForceMode.Impulse); // TODO: This works irregurarily, no idea why. Possible fix: actually lerp the cannon to turn around.
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
