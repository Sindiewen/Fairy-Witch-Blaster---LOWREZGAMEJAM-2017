using UnityEngine;

public class ResolutionScale : MonoBehaviour
{
    private void Start()
    {
        Screen.SetResolution(64, 64, false);
    }

    /*
    
    // TODO: Scale the resolution of the game down to the 64/64 resolution
    public RenderTexture renderTexture;

    public float scaledResHeight;
    public float scaledResWidth;
    private void Start()
    {
        print("Old Height: " + Screen.height + " | Old Width: " + Screen.width);

        // Uses math to scale the resolution down to 64 x 64
        scaledResHeight = Screen.height / (Screen.height / 64) - 2;
        scaledResWidth = Screen.width / (Screen.width / 64) - 2;

        renderTexture.height = (int)scaledResHeight;
        renderTexture.width = (int)scaledResWidth;


        print("New Height: " + scaledResHeight + " | New Width: " + scaledResWidth);
        
    }
    */


    /*
    int renderSizeX = 64;
    int renderSizeY = 64;
    RenderTexture renderTex;
    
    void Start()
    {
        renderTex = new RenderTexture(renderSizeX, renderSizeY, 24, RenderTextureFormat.Default);
        renderTex.filterMode = FilterMode.Point;
        renderTex.Create();
        GetComponent<Camera>().targetTexture = renderTex;
    }
    */


}
