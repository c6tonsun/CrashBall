using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    [SerializeField]
    private float _minSpeed;

    public Rigidbody Rb;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (Rb.velocity.magnitude < _minSpeed)
        {
            Rb.velocity = Rb.velocity.normalized * _minSpeed;
        }
    }
}
