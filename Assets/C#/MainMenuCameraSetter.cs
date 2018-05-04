using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraSetter : MonoBehaviour {

    private void Start()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        Camera cam = GetComponent<Camera>();
        UIMenuHandler menuHandler = FindObjectOfType<UIMenuHandler>();
        menuHandler.SetCamera(cam);
        menuHandler.isGamePaused = true;
        menuHandler.ToMenu(gameManager.menuToLoad, instantly: true);
    }
}
