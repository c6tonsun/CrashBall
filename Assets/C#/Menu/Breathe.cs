using UnityEngine;

public class Breathe : MonoBehaviour {

    private Vector3 originalSize;

    public bool Rescale = false;

    public bool bypassButtonSelected = false;

    public float CurveHeight = 1f;

    [SerializeField]
    private AnimationCurve curve;

    private UIMenuButton thisButton;

    private float randomStart;

    private void Start()
    {
        originalSize = transform.localScale;
        randomStart = Random.Range(0f, 0.8f);
        thisButton = GetComponent<UIMenuButton>();
    }

    void Update () {
        
        if (Rescale)
        {
            if (thisButton != null && !bypassButtonSelected)
            {
                float localCurveHeight;
                if (thisButton.isHighlighted)
                {
                    localCurveHeight = CurveHeight;
                }
                else
                {
                    localCurveHeight = 0;
                }
                float RescaleTimer = Mathf.PingPong(Time.unscaledTime + randomStart, 1);
                transform.localScale = originalSize * (1 + curve.Evaluate(RescaleTimer) * localCurveHeight);
            }
            else
            {
                float RescaleTimer = Mathf.PingPong(Time.unscaledTime + randomStart, 1);
                transform.localScale = originalSize * (1 + curve.Evaluate(RescaleTimer) * CurveHeight);
            }
        }
        else
        {
            transform.localScale = originalSize;
        }
	}
}
