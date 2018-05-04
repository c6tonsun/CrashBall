using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIHandler : MonoBehaviour {
    
    public TextMeshPro[] _positionFeeds;
    public Color[] _playerColors;
    
    // TODO: add string arrays for variation for killfeed
    private TextMeshPro[] _killFeeds;

    [HideInInspector]
    public TextMeshPro startFeed;
    [HideInInspector]
    public TextMeshPro endFeed;

    public void StageStart(Player[] players, int[] lives, int[] scores, bool isElimination)
    {   // get feeds
        UIFeed[] feeds = FindObjectsOfType<UIFeed>();
        foreach (UIFeed feed in feeds)
        {
            if (feed.currentFeed == UIFeed.Feed.Position)
                _positionFeeds = feed.GetComponentsInChildren<TextMeshPro>();
            else if (feed.currentFeed == UIFeed.Feed.Kill)
                _killFeeds = feed.GetComponentsInChildren<TextMeshPro>();
            else if (feed.currentFeed == UIFeed.Feed.Start)
                startFeed = feed.GetComponentInChildren<TextMeshPro>();
            else if (feed.currentFeed == UIFeed.Feed.End)
                endFeed = feed.GetComponentInChildren<TextMeshPro>();
        }
        // correct position feed
        for (int i = 0; i < _positionFeeds.Length; i++)
        {
            if (i >= players.Length)
                _positionFeeds[i].gameObject.SetActive(false);
        }
        // get colors
        _playerColors = new Color[players.Length];
        for (int i = 0; i < players.Length; i++)
            _playerColors[i] = players[i].GetColor();

        if (isElimination)
            PositionalUpdate(lives);
        else
            PositionalUpdate(scores);
    }
    
    public void KillFeed(int killer, int victim, bool victimDied)
    {
        TextMeshPro killerText = _killFeeds[0];
        TextMeshPro doingText = _killFeeds[1];
        TextMeshPro victimText = _killFeeds[2];
        // find free feed spot
        for (int i = 0; i < _killFeeds.Length; i += 3)
        {
            if (_killFeeds[i].color.a < 0.1f)
            {
                killerText = _killFeeds[i];
                doingText = _killFeeds[i + 1];
                victimText = _killFeeds[i + 2];
                break;
            }
        }
        
        // set text
        if (victimDied)
        {
            killerText.text = "P" + killer;
            doingText.text = "killed";
            victimText.text = "P" + victim;
        }
        else
        {
            killerText.text = "P" + killer;
            doingText.text = "scored";
            victimText.text = "P" + victim;
        }

        if (killer>0)
        {
            // set color
            killerText.color = _playerColors[killer - 1];
            doingText.color = new Color(doingText.color.r, doingText.color.g, doingText.color.b, 1f);
            victimText.color = _playerColors[victim - 1];
        }
        else // gray ball scores player
        {
            killerText.text = "P" + victim;
            doingText.text = "  got rekt";
            victimText.text = ":3";
            killerText.color = _playerColors[victim - 1];
            doingText.color = new Color(doingText.color.r, doingText.color.g, doingText.color.b, 1f);
            victimText.color = new Color(doingText.color.r, doingText.color.g, doingText.color.b, 1f);
        }
    }

    public void PositionalUpdate(int[] array)
    {
        int[] positions = SortPositions(array);
        int position = 1;
        int positionFeedIndex = 0;
        string positionString = "1st";

        while (true)
        {   // set correct text and color
            for (int i = 0; i < positions.Length; i++)
            {
                if (positions[i] == position)
                {
                    _positionFeeds[positionFeedIndex].SetText(positionString + " : " + array[i]);
                    _positionFeeds[positionFeedIndex].color = _playerColors[i];
                    positionFeedIndex++;
                }
            }
            // update position and positionString
            position++;
            if (position == 2)
                positionString = "2nd";
            else if (position == 3)
                positionString = "3rd";
            else
                positionString = position + "th";
            // are we done
            if (positionFeedIndex >= positions.Length)
                break;
        }
    }

    public int[] SortPositions(int[] array)
    {
        int[] tempArray = new int[array.Length];
        int[] result = new int[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            tempArray[i] = array[i];
            result[i] = -1;
        }

        int position = 1;
        int doneCount = 0;

        while (doneCount < result.Length)
        {
            int biggest = int.MinValue;
            // find biggest
            for (int i = 0; i < tempArray.Length; i++)
            {
                if (biggest < tempArray[i])
                    biggest = tempArray[i];
            }
            // every biggest is 1st/2nd/3rd...
            for (int i = 0; i < tempArray.Length; i++)
            {
                if (biggest == tempArray[i])
                {
                    result[i] = position;
                    tempArray[i] = int.MinValue;
                    doneCount++;
                }
            }
            position++;
        }

        return result;
    }
}
