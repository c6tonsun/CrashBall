using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSpots : MonoBehaviour {


    private UIMenu[] all_menus;
    private Vector3[] Targets;

    public float minimumDistance = 10f;

	// Use this for initialization
	void Awake () {
        all_menus = FindObjectsOfType<UIMenu>();
        Targets = new Vector3[all_menus.Length];
        int tries = 0;
        for (int i = 0; i < all_menus.Length;)
        {
            bool canFit = true;
            var randomspot = Random.insideUnitSphere * 30;
            for (int t = 0; t < Targets.Length; t++)
            {
                if (Vector3.Distance(Targets[t], randomspot) < minimumDistance)
                {
                    canFit = false;
                    break;
                }
            }
            if (canFit) {
                Targets[i] = randomspot;
                i++;
            }
            tries++;
            if (tries > 300)
            {
                Debug.Log("infinite loop aborting: i = " + i);
                break;
            }
        }    
    }

    private void Start()
    {
        for (int i = 0; i < all_menus.Length; i++)
        {
            all_menus[i].transform.position = Targets[i];
            //all_menus[i].transform.rotation = Random.rotation;
            all_menus[i].transform.LookAt(Vector3.zero);
        }
    }
}
