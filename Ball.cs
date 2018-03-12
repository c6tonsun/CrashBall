using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    [SerializeField]
    private float _minSpeed;
    private bool _isFixedY = false;
    private float _fixedY = 0f;
    public bool stayOnFloor = false;
    [Tooltip("If ball's x or z is over this value ball starts to fall (reset).")]
    public float ballDeadzone = 12f;

    [HideInInspector]
    public Rigidbody Rb;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        Debug.LogError("Add trigger to the center of Floor. Then remove this line.");
    }

    private void FixedUpdate()
    {
        if (Rb.velocity.magnitude < _minSpeed)
        {
            Rb.velocity = Rb.velocity.normalized * _minSpeed;
        }
        bool isXIn = transform.position.x > -ballDeadzone && transform.position.x < ballDeadzone;
        bool isZIn = transform.position.z > -ballDeadzone && transform.position.z < ballDeadzone;
        if (_isFixedY && isXIn && isZIn) transform.position = new Vector3(transform.position.x, _fixedY, transform.position.z);
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
