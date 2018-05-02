using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuePicker : MonoBehaviour {


    private HuePickerManager HueManager;

    [SerializeField]
    private Curve movementCurve;
    [SerializeField]
    private float _curveTime = 0.5f;

    [SerializeField]
    private Transform BlockArea;

    [SerializeField]
    private float moveSpeed = 0.5f;
    private float maxSpeed;
    private float currSpeed;

    private bool moving = false;
    private float moveTimer;

    [SerializeField]
    private Color picker;

    private bool isSet = false;

    [SerializeField]
    private int player;

    private static float[] playerHues = new float[4];

    void Start()
    {
        HueManager = FindObjectOfType<HuePickerManager>();

        maxSpeed = moveSpeed * 1.5f;
        currSpeed = 0.1f;
    }

    //Input methods

    public void DoLeft()
    {
        if (!isSet) {
            moving = true;
            _curveTime = CheckBlockedAreas(1, currSpeed);
            if (currSpeed < maxSpeed)
            {
                currSpeed += Time.unscaledDeltaTime;
            }
        }
    }

    public void DoRight()
    {
        if (!isSet)
        {
            moving = true;
            _curveTime = CheckBlockedAreas(-1, currSpeed);
            if (currSpeed < maxSpeed)
            {
                currSpeed += Time.unscaledDeltaTime * 2;
            }
        }
    }

    public void DoSelect(int index)
    {
        if (!isSet)
        {
            isSet = true;
            playerHues[index] = _curveTime;
            HueManager.colors[index] = picker;
        }
    }

    public void DoDeselect(int index)
    {
        if (isSet)
        {
            isSet = false;
            playerHues[index] = 0;
        }
    }

    //update

    public void DoUpdate()
    {

        BlockArea.gameObject.SetActive(isSet);

        moving = false;
        //var _input = 0f;
        //_input = Input.GetAxisRaw("Horizontal1");

        //if (_input < 0)
        //{
        //    DoLeft();
        //}
        //if(_input > 0)
        //{
        //    DoRight();
        //}
        //if (Input.GetButtonDown("Fire1"))
        //{
        //    DoSelect();
        //}
        //if (Input.GetButtonDown("Jump"))
        //{
        //    DoDeselect();
        //}

        if (!moving && moveTimer<1f)
        {
            moveTimer += Time.unscaledDeltaTime;
            if (moveTimer > 0.2f)
            {
                currSpeed = 0.1f;
                moveTimer = 0f;
            }
        }


        transform.position = MathHelp.GetCurvePosition(movementCurve.start.position, movementCurve.middle.position, movementCurve.end.position, _curveTime);
        transform.rotation = MathHelp.GetCurveRotation(movementCurve.start.rotation, movementCurve.middle.rotation, movementCurve.end.rotation, _curveTime);


        picker = Color.HSVToRGB(_curveTime, 1f, 1f);

        var temp = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < 2; i++)
        {
            temp[i].material.color = picker;
        }


    }

    

    private float CheckBlockedAreas(float input, float speed)
    {
        float result = _curveTime; 
        result += input * speed * Time.unscaledDeltaTime;

        if(result > 0.95)
        {
            result = 0.95f;
        }
        if (result < 0.05)
        {
            result = 0.05f;
        }

        int doneCount = 0;
        while (doneCount < playerHues.Length) {

            for (int i = 0; i < playerHues.Length; i++)
            {
                if (playerHues[i] <= 0)
                {
                    doneCount++;
                    continue;
                }
                if (Mathf.Abs(result - playerHues[i]) < 0.06f)
                {
                    float temp = playerHues[i] + 0.065f * Mathf.Sign(input);
                    if (temp < 0.95 && temp > 0.05)
                    {
                        result = temp;
                    }
                    else
                    {
                        input *= -1;
                        result = playerHues[i] + 0.065f * Mathf.Sign(input);
                    }
                    doneCount = 0;
                    break;
                }
                else
                {
                    doneCount++;
                }
            }
        }
        
        return result;

    }


}
