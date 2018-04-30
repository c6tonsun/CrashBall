using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenu : MonoBehaviour {

    public bool allPlayersNeedToBeReady;
    public UIMenuItem[] defaultItems;

    public Transform camTarget;
    private Transform camStart;
    private Camera cam;
    private UIMenuHandler menuHandler;
    
    private float time = 0f;

    public void StartTransition(Camera camera, UIMenuHandler handler, bool instanly)
    {
        cam = camera;
        camStart = cam.transform;

        menuHandler = handler;
        menuHandler.activeItems = defaultItems;

        if (instanly)
            time = 1f;

        StartCoroutine(TransitionToThisMenu());
    }

    IEnumerator TransitionToThisMenu()
    {
        time += Time.unscaledDeltaTime;

        cam.transform.position = Vector3.Lerp(camStart.position, camTarget.position, time);
        cam.transform.rotation = Quaternion.Lerp(camStart.rotation, camTarget.rotation, time);
        
        if (time > 1)
        {
            camStart = null;
            time = 0f;
            cam.transform.position = camTarget.position;
            cam.transform.rotation = camTarget.rotation;
            menuHandler.isInTransition = false;
            StopCoroutine(TransitionToThisMenu());
        }

        yield return new WaitForEndOfFrame();
    }
}
