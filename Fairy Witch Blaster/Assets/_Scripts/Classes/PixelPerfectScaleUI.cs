using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PixelPerfectScaleUI : MonoBehaviour
{
    // Public Enums
    // How to best fit the window
    public enum Fitmodes
    {
        BEST_FIT,
        MAINTAIN_ASPECT,
        SCALETOFIT
    };

    // Public variables
    public Fitmodes fitmodes = Fitmodes.BEST_FIT;

    public float gameHorizontalPixels;
    public float gameVerticalPixels;

    // Private variable
    private float minimumMultiplier = 1;

    private float screenPixelsY = 0;

    RectTransform rt;

    // Private class methods

    private void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (screenPixelsY != (float) Screen.height)
        {
            switch (fitmodes)
            {
                // Scales the window to best fit the current aspect ratio
                case Fitmodes.BEST_FIT:
                    BestFit();
                    break;

                // Scales the window to maintain the current aspect ratio
                case Fitmodes.MAINTAIN_ASPECT:
                    MaintainAspectFit();
                    break;

                // Scales the window to fill the aspect ratio
                case Fitmodes.SCALETOFIT:
                    ScaleToFit();
                    break;

                    
            }
        }
    }

    /// <summary>
    /// Scales the window to best fit the current aspect ratio
    /// </summary>
    private void BestFit()
    {
        float targetHeight = gameVerticalPixels;
        float multiplier = minimumMultiplier;

        multiplier = screenPixelsY / targetHeight;
        multiplier -= multiplier % 2;
        if (multiplier < 2)
        {
            multiplier = minimumMultiplier;
        }

        float aspect = gameHorizontalPixels / gameVerticalPixels;
        float height = gameVerticalPixels * multiplier;
        float width = height * aspect;


        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    /// <summary>
    /// Scales window to maintain the current aspect ratio
    /// </summary>
    private void MaintainAspectFit()
    {
        float aspect = gameHorizontalPixels / gameVerticalPixels;

        float targetWidth;
        float targetHeight;

        if (Screen.width < Screen.height)
        {
            // fit to width
            targetWidth = Screen.width;
            targetHeight = Screen.width / aspect;
        }
        else
        {

            targetHeight = Screen.height;
            targetWidth = Screen.height * aspect;
        }

        
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
    }
    
    /// <summary>
    /// Scales window to fill aspect ratio
    /// </summary>
    private void ScaleToFit()
    {
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
    }
}
