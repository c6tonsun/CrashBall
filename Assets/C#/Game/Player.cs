using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum Number
    {
        ERROR = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4
    }
    
    public Number currentPlayer;

    public bool isVerticalCurve;

    public bool AIControlled = true;

    public int[] kills;

    [SerializeField]
    private GameObject DeathParticlePrefab;
    private IEnumerator playerDeath;
    [SerializeField]
    public GameObject GoalConfetti;
    private float _deathParticleDuration;

    [HideInInspector]
    public KartMovement movement;
    [HideInInspector]
    public KartPulse pulse;

    [SerializeField]
    private Color playerColour;

    private int stacheID;
    
    [HideInInspector]
    public bool isLive = true;

    private Animator kartAnimator;

    public Color GetColor()
    {
        return playerColour;
    }

    public int GetStache()
    {
        return stacheID;
    }

    private void Awake()
    {
        movement = GetComponentInChildren<KartMovement>();
        pulse = GetComponentInChildren<KartPulse>();
        kartAnimator = GetComponentInChildren<Animator>();
        stacheID = FindObjectOfType<GameManager>().Mustaches[(int)currentPlayer-1];
        playerColour = FindObjectOfType<GameManager>().Colors[(int)currentPlayer-1];
        _deathParticleDuration = DeathParticlePrefab.GetComponent<ParticleSystem>().main.duration + 0.7f;

        playerDeath = Death();
    }

    private IEnumerator Death()
    {
        kartAnimator.SetBool("isDead", true);
        Destroy(Instantiate(DeathParticlePrefab, transform.position, transform.rotation), _deathParticleDuration);
        if (kartAnimator.GetBool("Bonus"))
        {
            yield return new WaitForSeconds(0.45f);
            //cameraShake.SetShakeTime(0.5f);
            yield return new WaitForSeconds(1.5f);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }
        
        gameObject.SetActive(false);
        StopCoroutine(playerDeath);
    }

    public void GoalCelebration(){
        Destroy(Instantiate (GoalConfetti, this.transform), 5.5f);
    }

    public void Die()
    {
        if (isLive)
        {
            gameObject.SetActive(true);
            StartCoroutine(playerDeath);
            isLive = false;
        }
    }
}
