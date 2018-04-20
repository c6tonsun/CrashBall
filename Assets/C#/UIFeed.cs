using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFeed : MonoBehaviour {

	public enum Feed
    {
        ERROR = 0,
        Position = 1,
        Kill = 2,
        Start = 3,
        End = 4
    }
    public Feed currentFeed;
}
