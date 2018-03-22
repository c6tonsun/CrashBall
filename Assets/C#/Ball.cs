using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    [SerializeField]
    private float _minSpeed;
    
    [HideInInspector]
    public bool canScore = true;

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

    private void FixedUpdate()
    {
        if (Rb.velocity.magnitude < _minSpeed)
            Rb.velocity = Rb.velocity.normalized * _minSpeed;

        RaycastHit hit;
        bool isOverStage = Physics.SphereCast(transform.position, transform.localScale.x, Vector3.down, out hit, 1f, LayerMask.NameToLayer("Floor"), QueryTriggerInteraction.Ignore);

        if (isFixedY && isOverStage)
            Rb.velocity = new Vector3(Rb.velocity.x, 0f, Rb.velocity.z);

        //Makes balls turn towards their velocity direction. Prevents balls from rolling
        //transform.rotation = Quaternion.LookRotation(Rb.velocity);
    }

    private void OnCollisionStay(Collision collision)
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
