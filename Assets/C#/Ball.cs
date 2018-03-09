using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    [SerializeField]
    private float _minSpeed;
    private bool _isFixedY = false;
    private float _fixedY = 0f;
    public bool stayOnFloor = false;

    [HideInInspector]
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

        if (_isFixedY) transform.position = new Vector3(transform.position.x, _fixedY, transform.position.z);
    }

    private void OnTriggerStay(Collider other)
    {
        if (_isFixedY || !stayOnFloor) return;

        Transform floor = other.transform;
        if (floor.name == "Floor")
        {
            _fixedY = floor.position.y + (floor.localScale.y * 0.5f) + (transform.localScale.y * 0.5f);
            _isFixedY = true;
        }
    }
}
