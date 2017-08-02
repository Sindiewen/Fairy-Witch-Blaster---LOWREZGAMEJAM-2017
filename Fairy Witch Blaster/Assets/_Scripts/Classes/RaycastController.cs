using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    // Structs //
    //  Defines the location of the raycast origins
    protected struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    // Public Variables //
    [TextAreaAttribute]
    public string RaycastControllerCS = "This is part of the RaycastController.cs class.";
    [Header("Layermask to Collide with")]
    public LayerMask collisionMask;             // Defines the layermask to collide with

    [Header("Actor Skin Width")]
    public const float skinWidth = 0.015f;      // Width of the actor's "skin"
    // Hidden public variabled
    [HideInInspector]
    public BoxCollider2D _boxCol;              // Reference to the box collider component


    // Protected class Variables
    // Defining the spacing for the rays depending how many the actor 
    protected float _horizontalRaySpacing;
    protected float _verticalRaySpacing;

    protected int horizontalRayCount;          // The count of the horizontal rays of the actor
    protected int verticalRayCount;            // The count of the vertical rays

    protected RaycastOrigins _raycastOrigins;     // Reference to the raycastOrigins struct

    // Private class variables
    private const float _dstBetweenRays = 0.25f;         // The distance between the rays


    ///////////////////////////
    // Private class methods
    ///////////////////////////

    // Use this for initialization
    protected virtual void Awake()
    {
        // Gets component box collider 2d
        _boxCol = GetComponent<BoxCollider2D>();


    }

    protected virtual void Start()
    {
        // Calculates the raycast spacing
        CalculateRaySpacing();
    }


    /// <summary>
    /// Updates the raycast origins for the actors
    /// </summary>
    protected void UpdateRaycastOrigins()
    {
        // Get the bounds of the collider and creates the skin width of the actor
        Bounds bounds = _boxCol.bounds;
        bounds.Expand(skinWidth * -2);

        // Sets the current raycast bounds for each of the 4 corners
        _raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        _raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        _raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        _raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

    }


    /// <summary>
    /// Calculates the raycast spacing depending on how many ray's the actor will have on them
    /// </summary>
    protected void CalculateRaySpacing()
    {
        // Gets the bounds of the collider and creates the skin width of the actor
        Bounds bounds = _boxCol.bounds;
        bounds.Expand(skinWidth * -2);

        // setting the bounds width of the rays
        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        // setting how many rays to send out
        horizontalRayCount = Mathf.RoundToInt(boundsHeight / _dstBetweenRays);
        verticalRayCount = Mathf.RoundToInt(boundsWidth / _dstBetweenRays);

        // Ensures the horizontal and vertical ray counts are >= 2
        //  When firing the rays, there should be at least 1 in each corner
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        // Calculates the ray spacing
        _horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        _verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);

    }

}
