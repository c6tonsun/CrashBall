using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public UIMenuButton[] activeItems;
    private UIMenu activeMenu;
    [HideInInspector]
    public bool isInTransition;

    private GameManager _gameManager;
    private HuePickerManager _colorManager;

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

        _colorManager = FindObjectOfType<HuePickerManager>();
    }

    private void Update()
    {
        if (isInTransition)
            return;

        if (activeMenu.isColorPickMenu)
            _colorManager.DoUpdate();

        for (int i = 0; i < _playersRewired.Length; i++)
        {
            if (i >= activeItems.Length || activeItems[i] == null)
                break;

            if (_playersRewired[i].GetButtonDown(SELECT_INPUT))
                DoSelect(i);

            if (_playersRewired[i].GetButtonDown(BACK_INPUT) && activeMenu.allPlayersNeedToBeReady == true)
                DoDeselect(i);

            if (_playersRewired[i].GetButtonDown(BACK_INPUT) && activeMenu.allPlayersNeedToBeReady == false)
                DoBack(i);
            if (_playersRewired[i].GetButtonLongPressDown(BACK_INPUT) && activeMenu.allPlayersNeedToBeReady)
                DoBack(i);

            if (_playersRewired[i].GetButtonDown(UP_INPUT))
                DoUp(i, activeItems[i]);

            if (_playersRewired[i].GetButtonDown(DOWN_INPUT))
                DoDown(i, activeItems[i]);

            if (activeItems[i].isMusicNoice || activeItems[i].isSoundNoice || activeMenu.isColorPickMenu)
            {
                if (_playersRewired[i].GetButton(LEFT_INPUT))
                    DoLeft(i, activeItems[i]);
                if (_playersRewired[i].GetButton(RIGHT_INPUT))
                    DoRight(i, activeItems[i]);
            }
            else
            {
                if (_playersRewired[i].GetButtonDown(LEFT_INPUT))
                    DoLeft(i, activeItems[i]);
                if (_playersRewired[i].GetButtonDown(RIGHT_INPUT))
                    DoRight(i, activeItems[i]);
            }
        }
    }

    
    private void DoSelect(int index)
    {
        #region button stuff
        if (activeItems[index].isQuit)
            Application.Quit();

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

        if (activeMenu.isColorPickMenu)
            _colorManager.pickers[index].DoSelect(index);

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

    private void DoDeselect(int index)
    {
        _readyPlayers[index] = false;

        if (activeMenu.isColorPickMenu)
            _colorManager.pickers[index].DoDeselect(index);
    }

    private void DoBack(int index)
    {
        if (activeMenu.allPlayersNeedToBeReady)
            _readyPlayers[index] = false;

        activeMenu.defaultItems = activeItems;
        ToMenu(activeItems[index].backMenu, instantly:false);
    }

    private void DoUp(int index, UIMenuButton activeItem)
    {
        if (index >= activeItems.Length)
            return;

        UIMenuButton result = activeItem.upItem;
        if (result == null)
            return;

        if (result.isHighlighted)
            DoUp(index, result);
        else
            MoveInMenu(result, index);
    }

    private void DoDown(int index, UIMenuButton activeItem)
    {
        if (index >= activeItems.Length)
            return;

        UIMenuButton result = activeItem.downItem;
        if (result == null)
            return;

        if (result.isHighlighted)
            DoDown(index, result);
        else
            MoveInMenu(result, index);
    }

    private void DoLeft(int index, UIMenuButton activeItem)
    {
        #region slider handling
        if (activeItem.isMusicNoice)
        {
            activeItem.musicNoice -= Time.deltaTime;

            if (activeItem.musicNoice < 0)
                activeItem.musicNoice = 0;

            activeItem.UpdateMusicSlider();
            MoveInMenu(activeItem, index);
            _gameManager.SaveMusicVolume(activeItem.musicNoice);
            return;
        }

        if (activeItem.isSoundNoice)
        {
            activeItem.soundNoice -= Time.deltaTime;

            if (activeItem.soundNoice < 0)
                activeItem.soundNoice = 0;

            activeItem.UpdateSoundSlider();
            MoveInMenu(activeItem, index);
            _gameManager.SaveSoundVolume(activeItem.soundNoice);
            return;
        }

        if (activeMenu.isColorPickMenu)
        {
            _colorManager.pickers[index].DoLeft();
            return;
        }

#endregion

        if (index >= activeItems.Length)
            return;

        UIMenuButton result = activeItem.leftItem;
        if (result == null)
            return;

        if (result.isHighlighted)
            DoLeft(index, result);
        else
            MoveInMenu(result, index);
    }

    private void DoRight(int index, UIMenuButton activeItem)
    {
        #region slider handling
        if (activeItem.isMusicNoice)
        {
            activeItem.musicNoice += Time.deltaTime;

            if (activeItem.musicNoice > 1)
                activeItem.musicNoice = 1;

            activeItem.UpdateMusicSlider();
            MoveInMenu(activeItem, index);
            _gameManager.SaveMusicVolume(activeItem.musicNoice);
            return;
        }

        if (activeItem.isSoundNoice)
        {
            activeItem.soundNoice += Time.deltaTime;

            if (activeItem.soundNoice > 1)
                activeItem.soundNoice = 1;

            activeItem.UpdateSoundSlider();
            MoveInMenu(activeItem, index);
            _gameManager.SaveSoundVolume(activeItem.soundNoice);
            return;
        }

        if (activeMenu.isColorPickMenu)
        {
            _colorManager.pickers[index].DoRight();
            return;
        }
        #endregion

        if (index >= activeItems.Length)
            return;

        UIMenuButton result = activeItem.rightItem;
        if (result == null)
            return;

        if (result.isHighlighted)
            DoRight(index, result);
        else
            MoveInMenu(result, index);
    }


    public void ToMenu(UIMenu menu, bool instantly)
    {
        if (isInTransition || menu == null)
            return;

        isInTransition = true;
        activeMenu = menu;
        menu.StartTransition(cam, this, instantly);

        UIMenuButton[] buttons = activeMenu.transform.GetComponentsInChildren<UIMenuButton>();
        foreach (UIMenuButton button in buttons)
        {
            if (button.isMusicNoice)
            {
                button.musicNoice = _gameManager.musicNoice;
                button.UpdateMusicSlider();
            }

            if (button.isSoundNoice)
            {
                button.soundNoice = _gameManager.soundNoice;
                button.UpdateSoundSlider();
            }
        }

#region handle highlights
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
#endregion
    }

    public void MoveInMenu(UIMenuButton item, int index)
    {
        if (item == null)
            return;

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
}
