using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAnimSpeed : MonoBehaviour {

    private Animator turretAnimator;
    private float timer;
    private float timerMax = 2;


	void Start () {
        turretAnimator = GetComponent<Animator>();
	}

    public void PauseAnimation(bool pause)
    {
        if (pause)
        {
            turretAnimator.SetFloat("AnimSpeed", 0);
        }
        else
        {
            rerollAnimSpeed();
        }
    }

    void rerollAnimSpeed()
    {
        var value = randomNumber(0.1f, 1.2f);
        turretAnimator.SetFloat("AnimSpeed", value);
    }

    float randomNumber(float min, float max)
    {
        return Random.Range(min, max);
    }
}
