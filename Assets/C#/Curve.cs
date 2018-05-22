using UnityEngine;

public class Curve : MonoBehaviour
{
    public Transform start;
    public Transform middle;
    public Transform end;

    public Transform LookAtTarget;
    public bool Inverse;

    private void OnDrawGizmos()
    {
        float t = 0;
        Vector3 startPoint =
            MathHelp.GetCurvePosition(start.position, middle.position, end.position, t);
        Vector3 endPoint;

        while (t < 1)
        {
            //t += 0.015625f; // 1 divided by 64
            t += 0.001f;
            endPoint =
                MathHelp.GetCurvePosition(start.position, middle.position, end.position, t);

            if (t < 0.084)
            {
                Debug.DrawLine(startPoint, endPoint, Color.red);
            }
            else if (t > 1 - 0.084)
            {
                Debug.DrawLine(startPoint, endPoint, Color.red);
            }
            else
            {
                Debug.DrawLine(startPoint, endPoint, Color.black);
            }

            startPoint = endPoint;
        }
    }

    private void Start()
    {
        if (LookAtTarget != null)
        {
            start.LookAt(LookAtTarget);
            middle.LookAt(LookAtTarget);
            end.LookAt(LookAtTarget);

            if (Inverse)
            {
                start.Rotate(Vector3.up, 180);
                middle.Rotate(Vector3.up, 180);
                end.Rotate(Vector3.up, 180);
            }
        }
    }
}