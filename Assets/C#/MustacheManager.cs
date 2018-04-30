using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MustacheManager : MonoBehaviour {

    private Transform[] mustachios;
    [SerializeField][Range(1,9)]
    private int _defaultStyle = 1;

	void Start () {
        mustachios = 
        GetComponentsInChildren<Transform>(includeInactive:true);
        var player = GetComponentInParent<Player>();
        if (player != null && FindObjectOfType<GameManager>()!=null)
        {
            var stacheID = player.GetStache();
            ChangeMustache(stacheID);
        }
        else
        {
            ChangeMustache(_defaultStyle);
            Debug.Log("No GM or player");
        }
	}

    public void ChangeMustache(int Style)
    {
        foreach(Transform mustache in mustachios){
            if (mustache == transform) continue;
            if (mustache == mustachios[Style]) {
                mustache.gameObject.SetActive(true);
                Debug.Log("Stachio: " + Style);
                continue;
            }
            mustache.gameObject.SetActive(false);
        }
    }
}
