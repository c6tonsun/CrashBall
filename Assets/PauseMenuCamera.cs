using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuCamera : MonoBehaviour {

    public GameObject StageCam;
    public GameObject PauseCam;
    public InputManager inputManager;

    public Transform startpoint;
    public Transform endPoint;
    public float _time;

	// Use this for initialization
	void Start () {
		
	}

    private void OnDisable()
    {
        StageCam.SetActive(true);
        _time = 0;
    }

    // Update is called once per frame
    void Update () {
        if(inputManager.Paused) StageCam.SetActive(false);
        if (_time < 1)
        {
            Vector3 newPos = Vector3.Lerp(startpoint.position, endPoint.position, _time);
            Quaternion newRot = Quaternion.Lerp(startpoint.rotation, endPoint.rotation, _time);
            transform.position = newPos;
            transform.rotation = newRot;
            _time += Time.fixedUnscaledDeltaTime;
        }
	}
}
