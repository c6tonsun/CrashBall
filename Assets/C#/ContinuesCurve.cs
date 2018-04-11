using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuesCurve : MonoBehaviour
{
    // OnDrawGismos stuff
    public bool initDone = false;
    public Transform[] curvePoints;
    public bool isLoop = false;
    public float angleTolerance = 1;
    [Range(0f, 1f)]
    public float maxVelocity = 0;

    [SerializeField][Tooltip("Target you want object to turn to. Leave null if you want to keep rotation")]
    private Transform LookAtTarget;

    public bool staticLoop;
    [Range(0f, 1f)]
    public float staticSpeed;

    public GameObject go;
    public float speed = 0.5f;
    private float _speedFactor;
    private float _totalTime;
    private float _time;
    private int _index;
    private int _targetIndex;
    private bool _growing;
    private bool _willLoop = false;

    private void OnDrawGizmos()
    {
        if (!initDone)
        {
            InitCurvePoints();
        }
        
        go.transform.position = curvePoints[0].position;
        go.transform.LookAt(LookAtTarget);

        Color color;
        #region draw curve
        for (int i = 0; i < curvePoints.Length - 1; i += 2)
        {
            Vector3 start = curvePoints[i].position;
            float t = 0f;

            while (t < 1)
            {
                t += 0.015625f; // 1 divided by 64

                Vector3 end = MathHelp.GetCurvePosition(
                    curvePoints[i].position,
                    curvePoints[i + 1].position,
                    curvePoints[i + 2].position,
                    t);

                float velocity = Vector3.Distance(start, end);
                float grayShade = Mathf.Clamp01(velocity / maxVelocity);
                color = new Color(grayShade, grayShade, grayShade);

                Debug.DrawLine(start, end, color);

                start = end;
            }
        }
        #endregion
        #region draw lines
        for (int i = 0; i < curvePoints.Length; i += 2)
        {
            // draw straight line
            {
                if (i == 0)
                {   
                    if (isLoop)
                    {   // first and last lines with angle check
                        float startEndAngle = MathHelp.AngleBetweenVector3(
                            curvePoints[curvePoints.Length - 2].position,
                            curvePoints[curvePoints.Length - 1].position,
                            curvePoints[i].position,
                            curvePoints[i + 1].position);

                        if (startEndAngle > -angleTolerance && startEndAngle < angleTolerance)
                        {
                            color = Color.white;
                        }
                        else
                        {
                            color = Color.yellow;
                        }

                        Debug.DrawLine(curvePoints[curvePoints.Length - 2].position, curvePoints[curvePoints.Length - 1].position, color);
                        Debug.DrawLine(curvePoints[i].position, curvePoints[i + 1].position, color);
                        continue;
                    }
                    // first line no angle check needed
                    color = Color.white;
                    Debug.DrawLine(curvePoints[i].position, curvePoints[i + 1].position, color);
                    continue;
                }
                else if (i == curvePoints.Length - 1)
                {
                    if (isLoop) continue;
                    // last line no angle check needed
                    color = Color.white;
                    Debug.DrawLine(curvePoints[i - 1].position, curvePoints[i].position, color);
                    continue;
                }
                // check angle and set correct color
                float angle = MathHelp.AngleBetweenVector3(
                    curvePoints[i - 1].position,
                    curvePoints[i].position,
                    curvePoints[i + 1].position);

                if (angle > -angleTolerance && angle < angleTolerance)
                {
                    color = Color.white;
                }
                else
                {
                    color = Color.yellow;
                }

                Debug.DrawLine(curvePoints[i - 1].position, curvePoints[i].position, color);
                Debug.DrawLine(curvePoints[i].position, curvePoints[i + 1].position, color);
            }
        }
#endregion
    }

    private void Update()
    {
        if (staticLoop)
            _totalTime += Time.deltaTime * staticSpeed;
        else
        {
            if (_totalTime == _targetIndex) { } // do nothing
            else if (_growing)
            {
                _totalTime += Time.deltaTime * speed * _speedFactor;
                if (_totalTime > _targetIndex && !_willLoop)
                    _totalTime = _targetIndex;
            }
            else
            {
                _totalTime -= Time.deltaTime * speed * _speedFactor;
                if (_totalTime < _targetIndex && !_willLoop)
                    _totalTime = _targetIndex;
            }
        }

        // looping
        if (_totalTime < 0)
        {
            _totalTime += curvePoints.Length - 1;
            _willLoop = false;
        }
        // skip odds
        if ((int)_totalTime % 2 == 1)
        {
            if (_growing || staticLoop) _totalTime++;
            else _totalTime--;
        }
        // looping
        if (_totalTime > curvePoints.Length - 2)
        {
            _totalTime -= curvePoints.Length - 1;
            _willLoop = false;
        }

        _index = (int)_totalTime;
        _time = _totalTime - _index;

        go.transform.position = MathHelp.GetCurvePosition(
            curvePoints[_index].position,
            curvePoints[_index + 1].position,
            curvePoints[_index + 2].position,
            _time);

        if (LookAtTarget != null)
        {
            go.transform.LookAt(LookAtTarget);
        }
        else
        {
            go.transform.rotation = MathHelp.GetCurveRotation(
                curvePoints[_index].rotation,
                curvePoints[_index + 1].rotation,
                curvePoints[_index + 2].rotation,
                _time);
        }
    }

    private void InitCurvePoints()
    {
        if (!initDone)
        {
            curvePoints = new Transform[transform.childCount];
            for (int i = 0; i < curvePoints.Length; i++)
            {
                curvePoints[i] = transform.GetChild(i);
            }
            initDone = true;
        }
    }

    public void MoveToPlayer(int player, bool longPath)
    {
        if (_targetIndex == player * 2 - 2)
            return;

        // fix player
        if (player == 2)
            player = 3;
        else if (player == 3)
            player = 4;
        else if (player == 4)
            player = 2;

        _targetIndex = player * 2 - 2;
        // numeral distanse
        _speedFactor = _totalTime - _targetIndex;
        _growing = true;
        _willLoop = false;
        if (_speedFactor < 0)
        {
            _speedFactor *= -1;
            _growing = false;
        }
        // if numeral order is short way
        if (_speedFactor < curvePoints.Length * 0.5f)
        {   // correct direction
            _growing = !_growing;
            _speedFactor = (curvePoints.Length - 1) - _speedFactor;
        }
        else _willLoop = true;      // go "wrong" way and loop

        // for longer path
        if (longPath)
        {
            _growing = !_growing;
            _willLoop = !_willLoop;
        }
    }
}