using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFMODEvent : MonoBehaviour {

    [FMODUnity.EventRef]
    public string selectSound;
    private FMOD.Studio.EventInstance _soundEvent;
    private FMOD.Studio.PLAYBACK_STATE _playBackState;

    private void Awake()
    {
        _soundEvent = FMODUnity.RuntimeManager.CreateInstance(selectSound);
    }

    private void OnDestroy()
    {
        _soundEvent.release();
    }

    public void Play()
    {
        _soundEvent.getPlaybackState(out _playBackState);

        if (_playBackState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
            _soundEvent.start();
    }
}
