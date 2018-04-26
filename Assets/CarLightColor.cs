using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLightColor : MonoBehaviour {

    private MeshRenderer neonLightRender;
    private MeshRenderer visorRender;

    private Color playerColor;
    private Light playerSpotlight;


	// Use this for initialization
	void Start () {
        var temp_meshRenderer = GetComponent<MeshRenderer>();
        if (temp_meshRenderer != null)
        {
            var matSize = temp_meshRenderer.materials.Length;
            if (matSize > 3)
            {
                //visorMat = temp_meshRenderer.materials[5];
                visorRender = temp_meshRenderer;
            }
            else
            {
                //neonLight_mat = temp_meshRenderer.material;
                neonLightRender = temp_meshRenderer;
            }
        }
        playerSpotlight = GetComponent<Light>();
        playerColor = GetComponentInParent<Player>().GetColor();      

        SetColours();
	}

    void SetColours()
    {
        if (playerSpotlight != null)
        {
            playerSpotlight.color = playerColor;
        }
        if (neonLightRender != null)
        {
            neonLightRender.material.color = playerColor;
            var adjustedColor = EmissionAdjust(playerColor, 3);
            neonLightRender.material.SetColor("_EmissionColor", adjustedColor);
        }
        if(visorRender != null)
        {
            visorRender.materials[5].color = playerColor;
            var adjustedColor = EmissionAdjust(playerColor, 3);
            visorRender.materials[5].SetColor("_EmissionColor", adjustedColor);
        }
    }

    private Color EmissionAdjust(Color originalColor, float brightness)
    {
        var multipliedColor = originalColor * brightness;

        var biggestValue = multipliedColor.r;
        if (multipliedColor.g > biggestValue) biggestValue = multipliedColor.g;
        if (multipliedColor.b > biggestValue) biggestValue = multipliedColor.b;

        if (biggestValue > 1f)
        {
            return
                new Color(
                    multipliedColor.r / biggestValue,
                    multipliedColor.g / biggestValue,
                    multipliedColor.b / biggestValue
                    );
        }
        else
        {
            return multipliedColor;
        }
    }
}
