using UnityEngine;

public class MenuSpots : MonoBehaviour {


    private UIMenu[] allMenus;
    private Vector3[] Targets;

    public float minimumDistance = 10f;

	// Use this for initialization
	void Awake () {
        allMenus = FindObjectsOfType<UIMenu>();
        Targets = new Vector3[allMenus.Length];
        int tries = 0;
        for (int i = 0; i < allMenus.Length;)
        {
            if (allMenus[i].isMainMenu == false)
            {
                i++;
                continue;
            }

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
        for (int i = 0; i < allMenus.Length; i++)
        {
            if (allMenus[i].isMainMenu)
            {
                allMenus[i].transform.position = Targets[i];
                allMenus[i].transform.rotation = Random.rotation;
                //allMenus[i].transform.LookAt(Vector3.zero);
            }
        }
    }
}
