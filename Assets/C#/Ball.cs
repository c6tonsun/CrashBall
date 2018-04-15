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
        //transform.rotation = Quaternion.LookRotation(Rb.velocity);
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

    public void ChangeTrailColor(Player player){
        trail.material.SetColor("_TintColor", player.GetColor());
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
}
