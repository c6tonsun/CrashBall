using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 _defaultPos;
    private Vector3 _defaultRot;

    public bool shake;

    private void Awake()
    {
        _defaultPos = transform.position;
        _defaultRot = transform.eulerAngles;
    }
}
