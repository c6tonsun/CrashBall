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
            if(stacheID == 0)
            {
                stacheID = Random.Range(1,10);
            }
            ChangeMustache(stacheID);
        }
        else
        {
            ChangeMustache(_defaultStyle);
        }
	}

    public void ChangeMustache(int Style)
    {
        foreach(Transform mustache in mustachios){
            if (mustache == transform) continue;
            if (mustache == mustachios[Style]) {
                mustache.gameObject.SetActive(true);
                continue;
            }
            mustache.gameObject.SetActive(false);
        }
    }

    public PlayFMODEvent GetVictoryCheer()
    {
        //Return avail cheer
        var returnable = GetComponentInChildren<PlayFMODEvent>(false);
        Debug.Log(returnable);
        return returnable;
    }
}
