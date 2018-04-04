using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour {

    [SerializeField]
    private Ball normalBallPrefab;
    public List<Ball> normalBalls;
    private int maxNormalBallCount;
    private float normalBallLikelyness;

    [SerializeField]
    private Ball stunBallPrefab;
    public List<Ball> stunBalls;
    private int maxStunBallCount;
    private float stunBallLikelyness;

    [SerializeField]
    private GameObject ballDeath;
    [SerializeField]
    private GameObject ballRespawn;


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
        // balls
        maxNormalBallCount = gameManager.normalBallCount;
        MakeBallList(normalBalls, normalBallPrefab, maxNormalBallCount, gameManager);

        maxStunBallCount = gameManager.stunBallCount;
        MakeBallList(stunBalls, stunBallPrefab, maxStunBallCount, gameManager);

        normalBallLikelyness = gameManager.normalBallLikelyness;
        stunBallLikelyness = gameManager.stunBallLikelyness;
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
    }

    private void MakeBallList(List<Ball> list, Ball prefab, int listLenght, GameManager gameManager)
    {
        for (int i = 0; i < listLenght; i++)
        {
            //Instantiates all balls to somewhere far away
            Ball ball = Instantiate(prefab, new Vector3(-25, 100, 25), transform.rotation, transform);
            ball.gameObject.SetActive(false);
            ball.canFly = gameManager.ballCanFly;
            ball.minSpeed = gameManager.ballMinSpeed;

            list.Add(ball);
        }
    }

    //Randomizes the next cannon to be fired. Cannot give same number twice in a row
    Transform RandomizeCannon()
    {
        if (Cannons.Length > 1)
        {
            _lastCannon = System.Array.IndexOf(Cannons, nextCannon);
            int new_random = Random.Range(0, CannonAmount);
            while (_lastCannon == new_random)
            {
                new_random = Random.Range(0, CannonAmount);
            }
            return Cannons[new_random];
        }
        else
        {
            return Cannons[0];
        }
    }
	
	// Physics related stuff in fixed update
	void FixedUpdate () {
        if (canFire)
        {
            Ball ball = null;
            if (Random.Range(0f, normalBallLikelyness + stunBallLikelyness) < normalBallLikelyness)
                ball = GetInactiveBall(normalBalls);
            else
                ball = GetInactiveBall(stunBalls);

            if (ball != null)
            {
                PrepareFire(ball);
                canFire = false;
            }
        }

    }

    private Ball GetInactiveBall(List<Ball> list)
    {
        foreach(Ball ball in list)
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
        StartCoroutine(Shoot(ball));        
        nextCannon = RandomizeCannon();
    }

    private IEnumerator Shoot(Ball ball)
    {

        //TODO: Add ground markings to tell this cannon is going to fire. 
        var cannon = nextCannon;
        var TurretAnimation = nextCannon.GetComponentInParent<TurretAnimSpeed>();
        if(TurretAnimation!=null)TurretAnimation.PauseAnimation(true); //This stops the cannons rotation for a while
        Destroy(Instantiate(ballRespawn, cannon.transform.position + cannon.forward, cannon.transform.rotation), 2.4f);
        yield return new WaitForSeconds(0.4f);
        ball.transform.position = cannon.position;
        ball.gameObject.SetActive(true);
        ball.Rb.AddForce((cannon.forward * (_fireForce)), ForceMode.Impulse);
        ball.transform.parent = transform;
        if (TurretAnimation != null) TurretAnimation.PauseAnimation(false); //Returns cannon movemnt
        StopCoroutine("Shoot");
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
