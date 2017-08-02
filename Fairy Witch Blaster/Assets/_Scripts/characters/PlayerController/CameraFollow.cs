using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    /* Structs */
    private struct FocusArea
    {
        // Public variables
        public Vector2 center;
        public Vector2 velocity;
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        // Public Struct Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetBounds"></param>
        public void Update(Bounds targetBounds)
        {
            // Creates the bounds for the x axsis
            float shiftX = 0;

            // CHecks if target is moving against the left or right edge
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }

            left += shiftX;
            right += shiftX;


            // Creates the bounds for the x axsis
            float shiftY = 0;

            // CHecks if target is moving against the top of bottom edge
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }

            top += shiftY;
            bottom += shiftY;

            center = new Vector2((left + right) / 2, (top + bottom) / 2);

            // Updates velocity
            velocity = new Vector2(shiftX, shiftY);
        }
    }




    // Public Variables
    [Header("Camera Follow Attributes")]
    [Tooltip("This will choose what the camera will follow")]
    public Base2DController target;         // Target to follow

    [Tooltip("Vertical offset for the camera follow")]
    public float verticalOffset;            // vertical offset

    [Tooltip("Distance to look ahead to on the x axis")]
    public float lookAheadDstX;
    [Tooltip("smoothing time")]
    public float lookSmoothTimeX;
    [Tooltip("Vertical smothing time")]
    public float verticalSmoothTime;

    [Tooltip("How big the focus area for following the camera will be")]
    public Vector2 focusAreaSize;           // Focus Area to focus on the object to follow


    // Private variables
    FocusArea focusArea;                // Creates an object of the focusArea for the camera follow

    // Private values for cameramovement
    private float currentLookAheadX;    // The current ammount the camera is looking ahead
    private float targetLookAheadX;     // The target's lookahead x value to look towards
    private float lookAheadDirX;        // The direction to move the camera towards
    private float smoothLookVelocityX;  // The velocity for looking smoothly
    private float smoothVelocityY;      // The smooth velocity in the y axis

    private bool lookAheadStopped;  


    // Private class Methods

    private void Start()
    {
        // creates new focusArea with the tartget and the focus area size
        focusArea = new FocusArea(target._boxCol.bounds , focusAreaSize);
    }

    /// <summary>
    /// Runs after the other update methods.
    /// 
    /// great for camera movement
    /// </summary>
    private void LateUpdate()
    {
        // Calling the focus area update with the target's bounds
        focusArea.Update(target._boxCol.bounds);

        // Defines the focus area posiion
        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

        // velocity of focus area of the x axis is not 0
        //  Set look ahead direction equal to the sign of focusArea.velocity.x
        if (focusArea.velocity.x != 0)
        {
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);

            // For smoothing out camera when dealing with lkeft and right player input
            if (Mathf.Sign(target._playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target._playerInput.x != 0)
            {
                lookAheadStopped = false;

                // Sets target look ahead to the full ammount
                targetLookAheadX = lookAheadDirX * lookAheadDstX;
            }
            else
            {
                if (!lookAheadStopped)
                {
                    lookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4;
                }
            }
        }


        // Sets the current lookahead X
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        // Y smoothing
        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);

        // Sets the new focus position ahead of the player 
        focusPosition += Vector2.right * currentLookAheadX;



        // Moves the camera when the player touches the side of the bounding box
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    }

    /// <summary>
    /// Drawing gizmos to the scene view
    /// 
    /// </summary>
    private void OnDrawGizmos()
    {
        // Creating transparent red colored gizmo
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }
}
