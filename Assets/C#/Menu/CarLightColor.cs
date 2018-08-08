using UnityEngine;

public class CarLightColor : MonoBehaviour {

    private MeshRenderer neonLightRender;
    private SkinnedMeshRenderer visorRender;

    [SerializeField]
    private Color playerColor;
    private Light playerSpotlight;


	// Use this for initialization
	void Start () {
        var temp_meshRenderer = GetComponent<MeshRenderer>();
        var temp_skinnedRenderer = GetComponent<SkinnedMeshRenderer>();
        var player = GetComponentInParent<Player>();

        if (temp_skinnedRenderer != null)
        {
            visorRender = temp_skinnedRenderer;
        }
        if(temp_meshRenderer)
        {
            neonLightRender = temp_meshRenderer;
        }

        playerSpotlight = GetComponent<Light>();
        if (player != null)
        {
            playerColor = player.GetColor();
        }
        else
        {
            playerColor = GetComponentInParent<PlayerPreview>().playerColor;
        } 

        SetColours();
	}

    public static Color CreateComplementaryColor(Color color)
    {
        var H = 0f;
        var S = 0f;
        var V = 0f;
        Color.RGBToHSV(color, out H, out S, out V);

        return Color.HSVToRGB(H, 0.3f, 1f);
    }

    public void RefreshColor()
    {
        var player = GetComponentInParent<Player>();
        if (player != null)
        {
            playerColor = player.GetColor();
        }
        else
        {
            playerColor = GetComponentInParent<PlayerPreview>().playerColor;
        }
        SetColours();
    }

    public void SetColours()
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
            visorRender.materials[3].color = playerColor;
            var adjustedColor = EmissionAdjust(playerColor, 3);
            visorRender.materials[3].SetColor("_EmissionColor", adjustedColor);
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
