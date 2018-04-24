using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIFadeOut : MonoBehaviour {

    private TextMeshPro _fadeOut;
    [SerializeField]
    private float FadeOutTime = 2f;

    private void Awake()
    {
        _fadeOut = GetComponent<TextMeshPro>();
        _fadeOut.color = new Color(_fadeOut.color.r, _fadeOut.color.g, _fadeOut.color.b, 0f);
    }

    private void Update()
    {
        _fadeOut.color = new Color(_fadeOut.color.r, _fadeOut.color.g, _fadeOut.color.b, _fadeOut.color.a - Time.unscaledDeltaTime/FadeOutTime);
    }
}
