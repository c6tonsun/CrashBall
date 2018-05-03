using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreview : MonoBehaviour {

    public enum Players
    {
        ERROR = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4
    }
    public Players currentPlayer;

    public Color playerColor;

    private GameManager GameManager;

    private CarLightColor[] colorChangers;

    private void Start()
    {
        GameManager = FindObjectOfType<GameManager>();
        colorChangers = GetComponentsInChildren<CarLightColor>();
    }

    //private IEnumerator refreshColors()
    //{
    //    foreach (CarLightColor changer in colorChangers)
    //    {
    //        changer.RefreshColor();
    //    }
    //    StopCoroutine(refreshColors());
    //}

    // Update is called once per frame
    void Update () {
        playerColor = GameManager.Colors[(int)currentPlayer - 1];

        foreach (CarLightColor changer in colorChangers)
        {
            changer.RefreshColor();
        }
    }
}
