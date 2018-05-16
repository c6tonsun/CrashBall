using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // player options
    public int playerCount;             // implement
    [Range(2f, 4f)]
    public float playerSpeed;
    [Range(0.2f, 1f)]
    public float maxStunTime;
    [Range(0.2f, 1f)]
    public float pulseCooldown;
    [Range(15f, 30f)]
    public float pulseForce;
    [Range(2.5f, 4f)]
    public float pulseArea;
    [Range(0f, 5f)]
    public float maxMagnetTime;

    public Color[] Colors;
    [Range(1, 9)]
    public int[] Mustaches;
    
    // ball options
    [Range(3f, 7f)]
    public float ballMinSpeed;
    public Ball normalBallPrefab;
    public int normalBallCount;
    [Range(0f, 1f)]
    public float normalBallLikelyness;
    public Ball stunBallPrefab;
    public int stunBallCount;
    [Range(0f, 1f)]
    public float stunBallLikelyness;
    [Range(2f, 5f)]
    public float firingInterval;

    // game mode selection
    public bool isElimination;
    public int stageSceneID;

    // other
    public int playerLives;
    public int targetScore;
    public UIMenu menuToLoad;

    // noices
    public float musicNoice;
    public float soundNoice;
    
    public PlayFMODEvent soundTest;

    private void Awake()
    {
        if (FindObjectsOfType<GameManager>().Length == 1)
            DontDestroyOnLoad(gameObject);
        else
            Destroy(gameObject);

        if (SaveLoad.FindSaveFile())
            SaveLoad.Load();
        else
            SaveLoad.MakeSaveFile();

        musicNoice = SaveLoad.Noices[SaveLoad.MUSIC_NOICE];
        soundNoice = SaveLoad.Noices[SaveLoad.SOUND_NOICE];

        FMODUnity.RuntimeManager.GetVCA("vca:/Musics").setVolume(musicNoice);
        FMODUnity.RuntimeManager.GetVCA("vca:/Sounds").setVolume(soundNoice);
    }

    public void SaveMusicVolume(float volume)
    {
        musicNoice = volume;
        SaveLoad.Noices[SaveLoad.MUSIC_NOICE] = musicNoice;
        SaveLoad.Save();

        FMODUnity.RuntimeManager.GetVCA("vca:/Musics").setVolume(musicNoice);
    }

    public void SaveSoundVolume(float volume)
    {
        soundNoice = volume;
        SaveLoad.Noices[SaveLoad.SOUND_NOICE] = soundNoice;
        SaveLoad.Save();
        
        FMODUnity.RuntimeManager.GetVCA("vca:/Sounds").setVolume(soundNoice);
    }
}
