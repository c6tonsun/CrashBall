using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartMovement : MonoBehaviour
{
    public float speed;
    public Curve movementCurve;
    
    private float _curveTime = 0.5f;

    private void Start()
    {
        speed = FindObjectOfType<GameManager>().playerSpeed;
    }

    public void Move(float _input)
    {
        #region movement

        _curveTime += _input * speed * Time.deltaTime;
        if (_curveTime < 0)
            _curveTime = 0;
        if (_curveTime > 1)
            _curveTime = 1;

        transform.position = MathHelp.GetCurvePosition(movementCurve.start.position, movementCurve.middle.position, movementCurve.end.position, _curveTime);
        transform.rotation = MathHelp.GetCurveRotation(movementCurve.start.rotation, movementCurve.middle.rotation, movementCurve.end.rotation, _curveTime);
        #endregion

        transform.Rotate(Vector3.forward * 15 * _input);
    }
}
