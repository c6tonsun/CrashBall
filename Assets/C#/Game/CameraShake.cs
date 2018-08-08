using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 _defaultPos;
    private Vector3 _defaultRot;
    [SerializeField]
    private Vector3 minOffsetPos;
    [SerializeField]
    private Vector3 minOffsetRot;
    [SerializeField]
    private Vector3 maxOffsetPos;
    [SerializeField]
    private Vector3 maxOffsetRot;

    private bool shake;
    private float maxShakeTime;
    private float shakeTimer;

    private void Awake()
    {
        _defaultPos = transform.position;
        _defaultRot = transform.eulerAngles;
    }

    private void Update()
    {
        if (shake)
        {
            transform.position = _defaultPos + RandomVector3(minOffsetPos, maxOffsetPos);
            transform.eulerAngles = _defaultRot + RandomVector3(minOffsetRot, maxOffsetRot);

            shakeTimer += Time.deltaTime;
            if (shakeTimer > maxShakeTime)
            {
                shake = false;
                transform.position = _defaultPos;
                transform.eulerAngles = _defaultRot;
            }
        }
    }

    private Vector3 RandomVector3(Vector3 min, Vector3 max)
    {
        Vector3 result;
        result.x = Random.Range(min.x, max.x);
        result.y = Random.Range(min.y, max.y);
        result.z = Random.Range(min.z, max.z);
        return result;
    }

    public void SetShakeTime(float shaketime)
    {
        if (shaketime > maxShakeTime - shakeTimer)
        {
            maxShakeTime = shaketime;
            shakeTimer = 0f;
            shake = true;
        }
    }
}
