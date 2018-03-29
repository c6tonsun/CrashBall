using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartMovement : MonoBehaviour
{
    private float speed;
    private float maxStunTime;
    [HideInInspector]
    public float stunTimer;

    public Curve movementCurve;
    private float _curveTime = 0.5f;

    [SerializeField]
    private ParticleSystem stunParticles;

    private void Start()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        speed = gameManager.playerSpeed;
        maxStunTime = gameManager.maxStunTime;
        stunTimer = maxStunTime;
    }

    private void Update()
    {
        stunTimer += Time.deltaTime;
    }

    public void Move(float _input)
    {
        if (stunTimer < maxStunTime)
        {
            _input *= 0.25f;
            stunParticles.Play();
        }
        else
        {
            if (stunParticles.isPlaying) stunParticles.Stop();
        }

        _curveTime += _input * speed * Time.deltaTime;
        if (_curveTime < 0)
            _curveTime = 0;
        if (_curveTime > 1)
            _curveTime = 1;

        transform.position = MathHelp.GetCurvePosition(movementCurve.start.position, movementCurve.middle.position, movementCurve.end.position, _curveTime);
        transform.rotation = MathHelp.GetCurveRotation(movementCurve.start.rotation, movementCurve.middle.rotation, movementCurve.end.rotation, _curveTime);

        transform.Rotate(Vector3.forward * 15 * _input);
    }
}
