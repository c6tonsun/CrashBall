using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MustacheManager : MonoBehaviour {

    private Transform[] mustachios;
    [SerializeField][Range(0,9)]
    private int _defaultStyle = 0;

	void Start () {
        mustachios = 
        GetComponentsInChildren<Transform>(includeInactive:true);
        var player = GetComponentInParent<Player>();
        if (player != null)
        {
            var stacheID = player.GetStache();
            ChangeMustache(stacheID);
        }
        else
        {
            ChangeMustache(_defaultStyle);
        }
	}

    public void ChangeMustache(int Style)
    {
        foreach(var mustache in mustachios){
            if (mustache == mustachios[Style]) {
                mustache.gameObject.SetActive(true);
                continue;
            }
            mustache.gameObject.SetActive(false);
        }
    }
}
