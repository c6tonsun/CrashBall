using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class UIMenuHandler : MonoBehaviour {
    
    // input
    private static int SELECT_INPUT = 0;
    private static int BACK_INPUT = 1;
    private static int UP_INPUT = 2;
    private static int DOWN_INPUT = 3;
    private static int LEFT_INPUT = 4;
    private static int RIGHT_INPUT = 5;
    private bool[] _readyPlayers;
    private Rewired.Player[] _playersRewired;

    // menu
    public Camera cam;
    public GameObject[] highlightItems;
    [HideInInspector]
    public UIMenuItem[] activeItems;
    private UIMenu activeMenu;
    [HideInInspector]
    public bool isInTransition;

    private GameManager _gameManager;

    private void Start()
    {
        // gets rewired players exgluding system
        _playersRewired = new Rewired.Player[Rewired.ReInput.players.allPlayerCount - 1];
        for (int i = 0; i < _playersRewired.Length; i++)
            _playersRewired[i] = Rewired.ReInput.players.GetPlayer(i);
        // players ready array
        _readyPlayers = new bool[_playersRewired.Length];
        for (int i = 0; i < _readyPlayers.Length; i++)
            _readyPlayers[i] = false;

        _gameManager = FindObjectOfType<GameManager>();
        ToMenu(_gameManager.menuToLoad, instantly:true);
    }

    private void Update()
    {
        if (isInTransition)
            return;

        for (int i = 0; i < _playersRewired.Length; i++)
        {
            if (_playersRewired[i].GetButton(SELECT_INPUT))
                DoSelect(i);
            if (_playersRewired[i].GetButton(BACK_INPUT) && activeMenu.allPlayersNeedToBeReady == false)
                DoBack(i);
            if (_playersRewired[i].GetButtonLongPressDown(BACK_INPUT) && activeMenu.allPlayersNeedToBeReady)
                DoBack(i);
            if (_playersRewired[i].GetButton(UP_INPUT))
                DoUp(i, activeItems[i]);
            if (_playersRewired[i].GetButton(DOWN_INPUT))
                DoDown(i, activeItems[i]);
            if (_playersRewired[i].GetButton(LEFT_INPUT))
                DoLeft(i, activeItems[i]);
            if (_playersRewired[i].GetButton(RIGHT_INPUT))
                DoRight(i, activeItems[i]);
        }
    }

    
    private void DoSelect(int index)
    {
#region button stuff
        if (activeItems[index].isQuit)
            Application.Quit();

        if (activeItems[index].isMusicNoice)
            _gameManager.musicNoice = activeItems[index].musicNoice;

        if (activeItems[index].isSoundNoice)
            _gameManager.soundNoice = activeItems[index].soundNoice;

        if (activeItems[index].isHueValue)
            _gameManager.Colors[index] = Color.HSVToRGB(activeItems[index].hueValue, 1f, 1f);

        if (activeItems[index].isMustach)
            _gameManager.Mustaches[index] = activeItems[index].mustach;

        if (activeItems[index].isSceneID)
            _gameManager.stageSceneID = activeItems[index].sceneID;

        if (activeItems[index].isModeSelection)
        {
            _gameManager.isElimination = activeItems[index].isElimination;
            SceneManager.LoadScene(_gameManager.stageSceneID, LoadSceneMode.Single);
        }
#endregion
        if (activeMenu.allPlayersNeedToBeReady)
        {
            _readyPlayers[index] = true;
            foreach (bool ready in _readyPlayers)
            {
                if (!ready)
                    return;
            }
        }

        activeMenu.defaultItems = activeItems;
        ToMenu(activeItems[index].nextMenu, instantly:false);
    }

    private void DoBack(int index)
    {
        if (activeMenu.allPlayersNeedToBeReady)
            _readyPlayers[index] = false;

        activeMenu.defaultItems = activeItems;
        ToMenu(activeItems[index].backMenu, instantly:false);
    }

    private void DoUp(int index, UIMenuItem activeItem)
    {
        if (index >= activeItems.Length)
            return;

        UIMenuItem result = activeItem.upItem;
        if (result == null)
            return;

        if (result.isHighlighted)
            DoUp(index, result);
        else
            activeItems[index] = result;
    }

    private void DoDown(int index, UIMenuItem activeItem)
    {
        if (index >= activeItems.Length)
            return;

        UIMenuItem result = activeItem.downItem;
        if (result == null)
            return;

        if (result.isHighlighted)
            DoDown(index, result);
        else
            activeItems[index] = result;
    }

    private void DoLeft(int index, UIMenuItem activeItem)
    {
        if (index >= activeItems.Length)
            return;

        UIMenuItem result = activeItem.leftItem;
        if (result == null)
            return;

        if (result.isHighlighted)
            DoLeft(index, result);
        else
            activeItems[index] = result;
    }

    private void DoRight(int index, UIMenuItem activeItem)
    {
        if (index >= activeItems.Length)
            return;

        UIMenuItem result = activeItem.rightItem;
        if (result == null)
            return;

        if (result.isHighlighted)
            DoRight(index, result);
        else
            activeItems[index] = result;
    }


    public void ToMenu(UIMenu menu, bool instantly)
    {
        if (isInTransition)
            return;

        isInTransition = true;
        activeMenu = menu;
        menu.StartTransition(cam, this, instantly);

        // handle highlights
        for (int i = 0; i < highlightItems.Length; i++)
        {
            if (_playersRewired[i].controllers.hasKeyboard == false && _playersRewired[i].controllers.joystickCount == 0)
                continue;

            highlightItems[i].SetActive(activeMenu.allPlayersNeedToBeReady);
        }
        highlightItems[0].SetActive(true);

        for (int i = 0; i < activeItems.Length; i++)
        {
            if (highlightItems[i].activeSelf)
                MoveInMenu(activeItems[i], i);
        }
    }

    public void MoveInMenu(UIMenuItem item, int index)
    {
        activeItems[index].isHighlighted = false;
        activeItems[index] = item;
        activeItems[index].isHighlighted = true;

        highlightItems[index].transform.position = activeItems[index].transform.position;
        highlightItems[index].transform.rotation = activeItems[index].transform.rotation;
        highlightItems[index].transform.localScale = new Vector3(
            activeItems[index].transform.localScale.x * 1.2f,
            activeItems[index].transform.localScale.y * 1.2f,
            activeItems[index].transform.localScale.z * 0.8f);
    }
    

    public void SetMusicVolume(float volume)
    {
        _gameManager.musicNoice = volume;
    }

    public void SetSoundVolume(float volume)
    {
        _gameManager.soundNoice = volume;
    }
}
