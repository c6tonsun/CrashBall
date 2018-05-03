using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStarter : MonoBehaviour {

    private void Awake()
    {
        FindObjectOfType<UIMenuHandler>().cam = GetComponent<Camera>();
        FindObjectOfType<UIMenuHandler>().Start();
    }
}
