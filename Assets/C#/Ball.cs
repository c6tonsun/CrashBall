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
    public bool canFly = false;
    [HideInInspector]
    public bool canBePulsed = false;
    [HideInInspector]
    public bool isFixedY = false;

    [HideInInspector]
    public Rigidbody Rb;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        canScore = true;
        canBePulsed = false;
        isFixedY = false;
    }

    private void OnDisable()
    {
        Rb.velocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        magnetedTime -= Time.fixedDeltaTime;

        if (Rb.velocity.magnitude < minSpeed && magnetedTime < 0)
            Rb.velocity = Rb.velocity.normalized * minSpeed;

        RaycastHit hit;
        bool isOverStage = Physics.SphereCast(transform.position, transform.localScale.x, Vector3.down, out hit, 1f, LayerMask.NameToLayer("Floor"), QueryTriggerInteraction.Ignore);

        if (isFixedY && isOverStage)
            Rb.velocity = new Vector3(Rb.velocity.x, 0f, Rb.velocity.z);
        else if (transform.position.y < -5)
            gameObject.SetActive(false);

        //Makes balls turn towards their velocity direction. Prevents balls from rolling
        //transform.rotation = Quaternion.LookRotation(Rb.velocity);
    }

    protected void OnCollisionStay(Collision collision)
    {
        if (canBePulsed) return;
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            if (!canFly)
                isFixedY = true;

            canBePulsed = true;
        }
    }
}
