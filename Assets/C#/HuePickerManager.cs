using UnityEngine;

public class HuePickerManager : MonoBehaviour {
    
    public Color[] colors;

    public HuePicker[] pickers;
    public MeshRenderer[] floorMeshes;
    public GameObject[] visualPlayers;

    private GameManager gameManager;
    
    void Start ()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
	
	public void DoUpdate ()
    {
        for (int p = 0; p < pickers.Length; p++)
        {
            if (pickers[p].gameObject.activeSelf == false)
                continue;

            floorMeshes[p].material.color = colors[p];
            pickers[p].DoUpdate();
        }
        
        gameManager.Colors = colors;
    }

    public void RefreshAllPickers(HuePicker sender)
    {
        foreach (var picker in pickers)
        {
            if (picker != sender && picker.gameObject.activeSelf && picker.isSet == false)
            {
                picker.RefreshSpot();
            }
        }
    }

    public void SetActivePickerCount(bool[] activePlayers)
    {
        int pickerCount = 0;
        foreach (bool activePlayer in activePlayers)
        {
            if (activePlayer)
                pickerCount++;
        }

        for (int p = 0; p < pickers.Length; p++)
        {
            pickers[p].gameObject.SetActive(p < pickerCount);
            floorMeshes[p].gameObject.SetActive(p < pickerCount);
            visualPlayers[p].gameObject.SetActive(p < pickerCount);
        }

        float step = 1 / pickerCount + 1;
        float[] times = new float[pickerCount]; 
        for (int i = 0; i < pickerCount; i++)
        {
            times[i] = (step * i) + step;
            // TODO: set this time as curve time
        }
    }
}
