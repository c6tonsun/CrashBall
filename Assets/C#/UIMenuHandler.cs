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
    private bool[] _isActivePlayer;
    private int activePlayerCount = 0;
    private Rewired.Player[] _playersRewired;

    // menu
    public Camera cam;
    public float transitionDelay;
    public GameObject[] highlightItems;
    public UIMenu pauseMenu;
    public UIMenu scoreScreen;
    private Vector3 defaultCamPos;
    private Quaternion defaultCamRot;
    [HideInInspector]
    public UIMenuButton[] activeItems;
    [HideInInspector]
    public UIMenu activeMenu;
    [HideInInspector]
    public UIMenu[] allMenus;
    [HideInInspector]
    public bool isInTransition;
    [HideInInspector]
    public bool isGamePaused;
    [HideInInspector]
    public bool isGameStarting;

    private GameManager _gameManager;
    private HuePickerManager _colorManager;

    private PlayFMODEvent[] blips;

    private void Start()
    {
        // gets rewired players exgluding system
        _playersRewired = new Rewired.Player[Rewired.ReInput.players.allPlayerCount - 1];
        for (int i = 0; i < _playersRewired.Length; i++)
            _playersRewired[i] = Rewired.ReInput.players.GetPlayer(i);
        
        _readyPlayers = new bool[_playersRewired.Length];
        _isActivePlayer = new bool[_playersRewired.Length];

        _gameManager = FindObjectOfType<GameManager>();
        _colorManager = FindObjectOfType<HuePickerManager>();

        blips = GetComponents<PlayFMODEvent>();
        allMenus = GetComponentsInChildren<UIMenu>();
    }

    private void Update()
    {
        if (isGamePaused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;

        if (isInTransition || isGamePaused == false || isGameStarting)
            return;

        if (activeMenu.isColorPickMenu == false)
        {
            for (int i = 0; i < highlightItems.Length; i++)
            {
                if (highlightItems[i].activeSelf)
                    UpdateHighlightItem(i);
            }
        }
        
        if (activeMenu.isColorPickMenu)
            _colorManager.DoUpdate();

        int index;
        for (int i = 0; i < _playersRewired.Length; i++)
        {
            if (isInTransition)
                break;

            // dynamic player detection
            if (activeMenu.isColorPickMenu)
            {
                if (_playersRewired[i].GetButtonDown(SELECT_INPUT) && _isActivePlayer[i] == false)
                {
                    _isActivePlayer[i] = true;
                    _colorManager.SetActivePickerCount(_isActivePlayer);
                }
                if (_playersRewired[i].GetButtonDown(BACK_INPUT) &&
                    _playersRewired[i].controllers.hasKeyboard == false &&
                    _readyPlayers[i] == false &&
                    _isActivePlayer[i] == true)
                {
                    _isActivePlayer[i] = false;
                    _colorManager.SetActivePickerCount(_isActivePlayer);
                }
            }

            if (_isActivePlayer[i] == false)
                continue;

            // all players can control in menus that have only one activeItem
            // colorPickMenu is exception because it works differently
            if ((i >= activeItems.Length || activeItems[i] == null) && activeMenu.isColorPickMenu == false)
                index = 0;
            else
                index = i;
            
            UIMenuButton activeItem;
            if (activeMenu.isColorPickMenu)
                activeItem = _colorManager.pickers[index];
            else
                activeItem = activeItems[index];

            if (_playersRewired[i].GetButtonDown(SELECT_INPUT))
                DoSelect(i, activeItem);

            if (_playersRewired[i].GetButtonDown(BACK_INPUT) && activeMenu.allPlayersNeedToBeReady == true)
                DoDeselect(i);

            if (_playersRewired[i].GetButtonDown(BACK_INPUT) && activeMenu.allPlayersNeedToBeReady == false)
                DoBack(i, activeItem);
            if (_playersRewired[i].GetButtonLongPressDown(BACK_INPUT) && activeMenu.allPlayersNeedToBeReady)
                DoBack(i, activeItem);

            if (isInTransition)
                break;

            // ready player can not move their highlight
            if (_readyPlayers[i])
                continue;

            if (_playersRewired[i].GetButtonDown(UP_INPUT))
                DoUp(index, activeItem);

            if (_playersRewired[i].GetButtonDown(DOWN_INPUT))
                DoDown(index, activeItem);

            if (activeItem.isMusicNoice || activeItem.isSoundNoice || activeMenu.isColorPickMenu)
            {
                if (_playersRewired[i].GetButton(LEFT_INPUT))
                    DoLeft(index, activeItem);
                if (_playersRewired[i].GetButton(RIGHT_INPUT))
                    DoRight(index, activeItem);
            }
            else
            {
                if (_playersRewired[i].GetButtonDown(LEFT_INPUT))
                    DoLeft(index, activeItem);
                if (_playersRewired[i].GetButtonDown(RIGHT_INPUT))
                    DoRight(index, activeItem);
            }
        }
    }

    
    private void DoSelect(int index, UIMenuButton activeItem)
    {
        #region button stuff
        if (activeItem.isQuit)
            Application.Quit();

        if (activeItem.isMustach)
            _gameManager.Mustaches[index] = activeItem.mustach;

        if (activeItem.isSceneID)
            _gameManager.stageSceneID = activeItem.sceneID;

        if (activeItem.isModeSelection)
        {
            _gameManager.menuToLoad = activeItem.backMenu;
            _gameManager.isElimination = activeItem.isElimination;
            StopAllCoroutines();
            SceneManager.LoadScene(_gameManager.stageSceneID, LoadSceneMode.Single);
        }

        if (activeItem.isContinue)
            DoUnpause();

        if (activeItem.isExitToMenu)
        {
            StopAllCoroutines();
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
        #endregion

        if (activeMenu.isColorPickMenu && _readyPlayers[index] == false)
            _colorManager.pickers[index].DoSelect(index);

        PlayMenuBlip(true);

        if (activeMenu.allPlayersNeedToBeReady)
        {
            _readyPlayers[index] = true;
            for (int i = 0; i < _playersRewired.Length; i++)
            {
                if (_isActivePlayer[i] && _readyPlayers[i] == false)
                    return;
            }
        }

        if (activeMenu.isColorPickMenu)
        {
            ToMenu(activeMenu.nextMenu, instantly: false);
            return;
        }

        activeMenu.defaultItems = activeItems;
        ToMenu(activeItem.nextMenu, instantly:false);
    }

    private void DoDeselect(int index)
    {
        _readyPlayers[index] = false;

        if (activeMenu.isColorPickMenu)
            _colorManager.pickers[index].DoDeselect(index);

        PlayMenuBlip(true, true);
    }

    private void DoBack(int index, UIMenuButton activeItem)
    {
        if (activeMenu.allPlayersNeedToBeReady)
            _readyPlayers[index] = false;

        if (activeMenu.isColorPickMenu)
        {
            ToMenu(activeMenu.backMenu, instantly: false);
            return;
        }

        activeMenu.defaultItems = activeItems;
        ToMenu(activeItem.backMenu, instantly:false);
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

        PlayMenuBlip(true);
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

        PlayMenuBlip(true);
    }

    private void DoLeft(int index, UIMenuButton activeItem)
    {
        #region slider handling
        if (activeItem.isMusicNoice)
        {
            activeItem.musicNoice -= Time.unscaledDeltaTime;

            if (activeItem.musicNoice < 0)
                activeItem.musicNoice = 0;

            activeItem.UpdateMusicSlider();
            MoveInMenu(activeItem, index);
            _gameManager.SaveMusicVolume(activeItem.musicNoice);
            return;
        }

        if (activeItem.isSoundNoice)
        {
            activeItem.soundNoice -= Time.unscaledDeltaTime;

            if (activeItem.soundNoice < 0)
                activeItem.soundNoice = 0;

            activeItem.UpdateSoundSlider();
            _gameManager.SaveSoundVolume(activeItem.soundNoice);
            return;
        }

        if (activeMenu.isColorPickMenu)
        {
            ((HuePicker)activeItem).DoLeft();
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

        PlayMenuBlip(true);
    }

    private void DoRight(int index, UIMenuButton activeItem)
    {
        #region slider handling
        if (activeItem.isMusicNoice)
        {
            activeItem.musicNoice += Time.unscaledDeltaTime;

            if (activeItem.musicNoice > 1)
                activeItem.musicNoice = 1;

            activeItem.UpdateMusicSlider();
            MoveInMenu(activeItem, index);
            _gameManager.SaveMusicVolume(activeItem.musicNoice);
            return;
        }

        if (activeItem.isSoundNoice)
        {
            activeItem.soundNoice += Time.unscaledDeltaTime;

            if (activeItem.soundNoice > 1)
                activeItem.soundNoice = 1;

            activeItem.UpdateSoundSlider();
            _gameManager.SaveSoundVolume(activeItem.soundNoice);
            return;
        }

        if (activeMenu.isColorPickMenu)
        {
            ((HuePicker)activeItem).DoRight();
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

        PlayMenuBlip(true);
    }


    public void SetCamera(Camera camera, bool isMainMenu)
    {
        cam = camera;
        defaultCamPos = cam.transform.position;
        defaultCamRot = cam.transform.rotation;
        
        if (isMainMenu)
        {
            foreach (UIMenu menu in allMenus)
                menu.gameObject.SetActive(menu.isMainMenu);
        }
        else
        {
            foreach (UIMenu menu in allMenus)
                menu.gameObject.SetActive(false);
        }
    }


    public void ToMenu(UIMenu menu, bool instantly)
    {
        if (isInTransition || menu == null)
            return;
        
        _gameManager.soundTest.Stop();

        for (int i = 0; i < _isActivePlayer.Length; i++)
        {
            bool hasController = _playersRewired[i].controllers.hasKeyboard || _playersRewired[i].controllers.joystickCount > 0;
            _isActivePlayer[i] = hasController;
            _readyPlayers[i] = false;
            
            if (menu.isColorPickMenu)
            {
                _colorManager.pickers[i].DoDeselect(i);
                _colorManager.SetActivePickerCount(_isActivePlayer);
            }
        }
        
        if (activeMenu != null && activeMenu.isColorPickMenu)
        {
            activePlayerCount = 0;
            foreach (bool activePlayer in _isActivePlayer)
            {
                if (activePlayer)
                    activePlayerCount++;
            }

            _colorManager.DoUpdate();
            _gameManager.playerCount = activePlayerCount;
        }

        isInTransition = true;
        StartCoroutine(TransitionDelay(menu, instantly));
    }

    IEnumerator TransitionDelay(UIMenu menu, bool instantly)
    {
        if (instantly == false)
            yield return new WaitForSecondsRealtime(transitionDelay);

        UIMenu oldMenu = activeMenu;
        activeMenu = menu;

        if (menu.isMainMenu == false)
            menu.gameObject.SetActive(true);
        
        menu.StartTransition(cam, this, instantly);

        UIMenuButton[] buttons = menu.transform.GetComponentsInChildren<UIMenuButton>();
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
            if (_isActivePlayer[i] == false || menu.isColorPickMenu)
            {
                highlightItems[i].SetActive(false);
                continue;
            }

            highlightItems[i].SetActive(menu.allPlayersNeedToBeReady);
        }

        if (menu.isColorPickMenu == false)
            highlightItems[0].SetActive(true);

        for (int i = 0; i < activeItems.Length; i++)
        {
            if (highlightItems[i].activeSelf)
                MoveInMenu(activeItems[i], i);
        }
        #endregion

        yield return new WaitForSecondsRealtime(1f);
        
        if (oldMenu != null && oldMenu.isMainMenu == false)
            oldMenu.gameObject.SetActive(false);

        StopCoroutine(TransitionDelay(menu, instantly));
    }

    public void MoveInMenu(UIMenuButton item, int index)
    {
        if (item == null)
            return;

        if (activeItems[index].isSoundNoice)
            _gameManager.soundTest.Stop();

        activeItems[index].isHighlighted = false;
        activeItems[index] = item;
        activeItems[index].isHighlighted = true;

        if (activeItems[index].isSoundNoice)
            _gameManager.soundTest.Play(false);
    }

    public void UpdateHighlightItem(int index)
    {
        highlightItems[index].GetComponent<MeshFilter>().mesh = activeItems[index].GetComponent<MeshFilter>().mesh;

        highlightItems[index].transform.position = activeItems[index].transform.position;
        highlightItems[index].transform.rotation = activeItems[index].transform.rotation;
        highlightItems[index].transform.localScale = new Vector3(
            activeItems[index].transform.localScale.x * 1.2f,
            activeItems[index].transform.localScale.y * 1.2f,
            activeItems[index].transform.localScale.z * 0.8f);
    }

    public void DoPause()
    {
        if (isGameStarting || isInTransition)
            return;
        isGamePaused = true;
        ToMenu(pauseMenu, false);
    }

    public void DoUnpause()
    {
        if (isGameStarting || isInTransition)
            return;
        isInTransition = true;
        StartCoroutine(TransitionBackToGame());
    }

    public void PlayMenuBlip(bool playAnyway, bool reverse=false)
    {
        if (!reverse)
        {
            blips[0].Play(playAnyway);
        }
        else
        {
            blips[1].Play(playAnyway);
        }
    }

    IEnumerator TransitionBackToGame()
    {
        Vector3 camStartPos = cam.transform.position;
        Quaternion camStartRot = cam.transform.rotation;
        
        float time = 0f;

        yield return new WaitForSecondsRealtime(transitionDelay);

        while (time < 1)
        {
            cam.transform.position = Vector3.Lerp(camStartPos, defaultCamPos, time);
            cam.transform.rotation = Quaternion.Lerp(camStartRot, defaultCamRot, time);

            time += Time.unscaledDeltaTime * 2;

            yield return new WaitForEndOfFrame();
        }

        time = 0f;
        cam.transform.position = defaultCamPos;
        cam.transform.rotation = defaultCamRot;
        isInTransition = false;
        isGamePaused = false;
        StopCoroutine(TransitionBackToGame());
    }
}
