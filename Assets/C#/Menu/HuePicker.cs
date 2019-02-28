using UnityEngine;

public class HuePicker : UIMenuButton {


    private HuePickerManager HueManager;

    [SerializeField]
    private Curve movementCurve;
    [SerializeField]
    private float _curveTime = 0.5f;

    [SerializeField]
    private Transform BlockArea;

    [SerializeField]
    private float moveSpeed = 0.5f;
    
    private float moveTimer;

    [SerializeField]
    private Color picker;
    private Color pickerReal;

    [HideInInspector]
    public bool isSet = false;

    [SerializeField]
    private int player;

    private static float[] playerHues = new float[4] { 0f, 0f, 0f, 1f };

    void Start()
    {
        HueManager = FindObjectOfType<HuePickerManager>();
    }

    //Input methods

    public void DoLeft()
    {
        if (!isSet)
        {
            _curveTime = CheckBlockedAreas(1, moveSpeed);
        }
    }

    public void DoRight()
    {
        if (!isSet)
        {
            _curveTime = CheckBlockedAreas(-1, moveSpeed);
        }
    }

    public void DoSelect(int index)
    {
        if (!isSet)
        {
            isSet = true;
            playerHues[index] = _curveTime;
            HueManager.colors[index] = pickerReal;
            HueManager.RefreshAllPickers(this);
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

    // basicly update
    public void DoUpdate()
    {
        BlockArea.gameObject.SetActive(isSet);
        
        transform.position = MathHelp.GetCurvePosition(movementCurve.start.position, movementCurve.middle.position, movementCurve.end.position, _curveTime);
        transform.rotation = MathHelp.GetCurveRotation(movementCurve.start.rotation, movementCurve.middle.rotation, movementCurve.end.rotation, _curveTime);
        
        picker = Color.HSVToRGB(_curveTime, 1f, 1f);
        pickerReal = Color.HSVToRGB(_curveTime, 0.9f, 0.9f);

        HueManager.colors[player] = pickerReal;

        var temp = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < 2; i++)
        {
            temp[i].material.color = picker;
        }
    }


    public void RefreshSpot()
    {
        _curveTime = CheckBlockedAreas(-0.1f, 0f);
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
