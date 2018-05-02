using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenuButton : MonoBehaviour {

    public UIMenuButton upItem;
    public UIMenuButton downItem;
    public UIMenuButton leftItem;
    public UIMenuButton rightItem;
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

    [Space(15)]
    public Transform mute;
    public Transform noice;
    
    public void UpdateMusicSlider()
    {
        transform.position = Vector3.Lerp(mute.position, noice.position, musicNoice);
        transform.rotation = Quaternion.Lerp(mute.rotation, noice.rotation, musicNoice);
    }

    public void UpdateSoundSlider()
    {
        transform.position = Vector3.Lerp(mute.position, noice.position, soundNoice);
        transform.rotation = Quaternion.Lerp(mute.rotation, noice.rotation, soundNoice);
    }
}
