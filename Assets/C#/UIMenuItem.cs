using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenuItem : MonoBehaviour {

    public UIMenuItem upItem;
    public UIMenuItem downItem;
    public UIMenuItem leftItem;
    public UIMenuItem rightItem;
    [HideInInspector]
    public bool isHighlighted;

    public UIMenu nextMenu;
    public UIMenu backMenu;

    [Space(15)]
    public bool isMusicNoice;
    [Range(0f, 1f)]
    public float musicNoice;

    [Space(15)]
    public bool isSoundNoice;
    [Range(0f, 1f)]
    public float soundNoice;

    [Space(15)]
    public bool isHueValue;
    [Range(0.05f, 0.95f)]
    public float hueValue;

    [Space(15)]
    public bool isMustach;
    [Range(-1, 9)]
    public int mustach;

    [Space(15)]
    public bool isSceneID;
    [Range(0, 9)]
    public int sceneID;

    [Space(15)]
    public bool isModeSelection;
    public bool isElimination;

    [Space(15)]
    public bool isQuit;
}
