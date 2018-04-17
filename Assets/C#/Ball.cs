using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    [HideInInspector]
    public float minSpeed;
    [HideInInspector]
    public float magnetedTime;
    [HideInInspector]
    public bool canScore = true;
    [HideInInspector]
    public bool canBePulsed = false;
    [HideInInspector]
    public bool isFixedY = false;
    [HideInInspector]
    public bool canBeMagneted = true;

    [HideInInspector]
    public Rigidbody Rb;
    private TrailRenderer trail;
    [SerializeField]
    private Color neutralColor;

    [SerializeField]
    protected ParticleSystem BallBlastEffect;

    private int lastPlayerHit;
    private int secondLastPlayerHit;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        neutralColor = new Color(0.5f,0.5f,0.5f,1f);
    }

    private void OnEnable()
    {
        canScore = true;
        canBePulsed = false;
        isFixedY = false;

        lastPlayerHit = -1;
        secondLastPlayerHit = -1;
    }

    private void OnDisable()
    {
        Rb.velocity = Vector3.zero;
        trail.material.SetColor("_TintColor", neutralColor);
    }

    protected void FixedUpdate()
    {
        magnetedTime -= Time.fixedDeltaTime;

        if (Rb.velocity.magnitude < minSpeed && magnetedTime < 0)
            Rb.velocity = Rb.velocity.normalized * minSpeed;

        RaycastHit hit;
        bool isOverStage = Physics.SphereCast(transform.position, transform.localScale.x, Vector3.down, out hit, 2f, LayerMask.NameToLayer("Floor"), QueryTriggerInteraction.Ignore);

        if (isFixedY && isOverStage)
            Rb.velocity = new Vector3(Rb.velocity.x, 0f, Rb.velocity.z);
        else if (transform.position.y < -5)
            gameObject.SetActive(false);

        //Makes balls turn towards their velocity direction. Prevents balls from rolling
        transform.rotation = Quaternion.LookRotation(Rb.velocity, Vector3.up);
    }

    protected void OnCollisionStay(Collision collision)
    {
        Player player = collision.collider.GetComponentInParent<Player>();
        if (player != null){
            SetLastPlayerHit((int)player.currentPlayer);
            ChangeTrailColor(player);           
        } 

        if (canBePulsed) return;
        
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            isFixedY = true;
            canBePulsed = true;
        }
    }

    protected void OnCollisionExit(Collision collision){
        OnCollisionStay(collision);
    }

    //TODO: Trailcolour should be that color who gets the score. Pulse always changes Trail Colour.
    public void ChangeTrailColor(Player player){
        trail.material.SetColor("_TintColor", player.GetColor());
    }

    //TODO: Maybe make it save the effects first time it makes em, to reduce extra calcs.
    //TODO: Get the quaternion spawn right.
    
    public void SpawnPulseBlastOff(Player player, Vector3 direction){
        StartCoroutine(BlastOff(player, direction));
    }

    protected IEnumerator BlastOff(Player player, Vector3 direction){
        var BallBlastParts = BallBlastEffect.GetComponentsInChildren<ParticleSystem>();
        foreach(var part in BallBlastParts){
            var partStartColor = part.main;
            partStartColor.startColor = player.GetColor();        
        }

        yield return new WaitForFixedUpdate();
        Destroy(Instantiate(BallBlastEffect, transform.position, Quaternion.LookRotation(direction)), 1.8f);
        StopCoroutine("BlastOff");
    }

    public void SetLastPlayerHit(int player)
    {
        if (lastPlayerHit == player)
            return;

        secondLastPlayerHit = lastPlayerHit;
        lastPlayerHit = player;
    }

    public int[] GetLastPlayerHits()
    {
        return new int[2] { lastPlayerHit, secondLastPlayerHit };
    }

    void OnDrawGizmos(){
        Gizmos.DrawLine(transform.position, transform.position+transform.forward*3);
    }
}
