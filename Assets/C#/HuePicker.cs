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
    private float speed = 0.5f;

    [SerializeField]
    private Color picker;

    private bool isSet = false;

    [SerializeField]
    private int player;

    private static float[] playerHues = new float[4];

    public static int currentPlayer = 0;

    public void SetCurrentPlayer(int num)
    {
        currentPlayer = num;
    }

    void Start()
    {
        HueManager = FindObjectOfType<HuePickerManager>();
    }

    void Update()
    {
        if (currentPlayer == player)
        {
            var _input = 0f;                            //Get input from manager
            _input = -Input.GetAxisRaw("Horizontal1");  //Ditto

            if (Mathf.Abs(_input) > 0.2f && !isSet)
            {
                _curveTime = CheckBlockedAreas(_input);
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

        if (Input.GetButtonDown("Fire1") && !isSet && currentPlayer == player) //Convert to Rewired
        {
            isSet = true;
            playerHues[currentPlayer] = _curveTime;
            HueManager.colors[currentPlayer] = picker;
                
        }
        if(Input.GetButtonDown("Jump") && isSet && currentPlayer == player) //Convert to rewired
        {
            isSet = false;
            playerHues[currentPlayer] = 0;
        }
    }

    private float CheckBlockedAreas(float input)
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
