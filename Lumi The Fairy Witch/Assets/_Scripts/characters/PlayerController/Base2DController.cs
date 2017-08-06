using UnityEngine;

public class Base2DController : RaycastController
{
    // Structs //
    // Stores the collision info to see what the actor has collided into
    public struct collisionInfo
    {
        // bool's storing what has been hit
        public bool above, below, left, right;
        public bool climbingSlope;
        public bool decendingSlope;
        public bool slidingDownMaxSlope;

        public float slopeAngle, slopeAngleOld;

        public int faceDirection;   // l = facing right, -1 = facing left

        public Vector2 moveAmountOld;
        public Vector2 slopeNormal;

        public bool fallingThroughPlatform;

        // Resets the bool's values to false
        // default values
        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = decendingSlope = slidingDownMaxSlope = false;
            fallingThroughPlatform = false; 

            slopeAngleOld = slopeAngle; 
            slopeAngle = 0;

            slopeNormal = moveAmountOld = Vector2.zero;
        }
    }

    // Public Variables
    [Space(40)]
    [TextAreaAttribute]
    public string Base2DControllerCS = " This is part of the Base2DController.cs Class.";

    [Header("Actor Angle Attributes")]
    [TooltipAttribute("The max angle in which an actor can ascend or decend a slope")]
    public float maxSlopeAngle = 80f;           // THe max angle in which the actor can ascend a slope


    // Stores collision info publically for other classes to access
    [HideInInspector]
    public collisionInfo collisions;            // Stores collision info

    [HideInInspector]
    public Vector2 _playerInput;        // Stores the current player input


    /////////////////////////
    // Public Class Methods
    /////////////////////////
    public void Move(Vector2 moveAmount, bool standingOnPlatform)
    {
        Move(moveAmount, Vector2.zero, standingOnPlatform);
    }

    public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
    {
        // Update the raycast origins
        UpdateRaycastOrigins();

        // Resets the actor's info every time the actor moves
        collisions.Reset();
        collisions.moveAmountOld = moveAmount;

        _playerInput = input; 

        // Checking if the actor is decending a slope
        if (moveAmount.y < 0)
        {
            DecendSlope(ref moveAmount);
        }

        //
        if (moveAmount.x != 0)
        {
            collisions.faceDirection = (int)Mathf.Sign(moveAmount.x);
        }


        // Calculating collisions

        HorizontalCollisions(ref moveAmount);

        if (moveAmount.y != 0)
        {
            VerticalCollisions(ref moveAmount);
        }


        // Moves the actor
        transform.Translate(moveAmount);

        if (standingOnPlatform == true)
        {
            collisions.below = true;
        }
    }


    ///////////////////////////
    // Private class methods
    ///////////////////////////
    // Runs the base start method plus this one too
    protected override void Start()
    {
        base.Start();

        // runs after base class start runs
        collisions.faceDirection = 1;
    }


    /// <summary>
    /// Calculates the horizontal collisions of the actor
    /// </summary>    
    private void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = collisions.faceDirection;
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        if (Mathf.Abs(moveAmount.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            // Sets the rayOrigin if moving down, rays start at bottom left corner. else, top left corner
            Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (_horizontalRaySpacing * i);

            // Creates a raycast for hitting a layer
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            // Draws the actor's rays
            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);


            // If the rays hit a valid layermask
            if (hit)
            {
                // Allows actor to move through a moving platform 
                if (hit.distance == 0)
                {
                    // skips current ray and uses the next ray to calculate collisions
                    continue;
                }

                // Get the angle of the surface that we hit
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (collisions.decendingSlope)
                    {
                        collisions.decendingSlope = false;
                        moveAmount = collisions.moveAmountOld;
                    }

                    // distance before the slope starts
                    float distanceToSlopeStart = 0;

                    // If the slope angle doesnt equal the old slope angle
                    //  starting to climb a slope
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        // Sets the distance to slope start 
                        distanceToSlopeStart = hit.distance - skinWidth;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }

                    // initiate climbing the slope
                    ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                    moveAmount.x += distanceToSlopeStart * directionX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    // set y moveAmount to the point in which the way hit an obstacle
                    // Ray distance
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    // If an actor is climbing slope when colliding with an objcect on the slope
                    //  Update moveAmount y to our x moveAmount so there is no jitter climbing the slope when hitting an obstacle
                    if (collisions.climbingSlope)
                    {
                        moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }

                    // If actor has hit something and is moving that direction
                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }


    /// <summary>
    /// Calculates the vertical collisions of the actor
    /// </summary>    
    private void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            // Sets the rayOrigin if moving down, rays start at bottom left corner. else, top left corner
            Vector2 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (_verticalRaySpacing * i + moveAmount.x);

            // Creates a raycast for hitting a layer
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            // Draws the actor's rays
            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);


            // If the layermask hit something
            if (hit)
            {
                /* 1 - 2 way platform logic */


                // two way platforms
                if (hit.collider.tag == "2WayPlatform")
                {
                    // If moving upwards, just ignore this ray and skip to the next ray
                    // We'll be able to jump through the platform and create a 1 way platform
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                    // If the actor is already falling through the platform
                    if(collisions.fallingThroughPlatform)
                    {
                        continue;
                    }
                    // Going through the top falling through the platform
                    if (_playerInput.y == -1)
                    {
                        collisions.fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", .5f);
                        continue;
                    }
                }
                /*
                // 1 way platform
                //  If player presses down on a platform they can fall through
                if (hit.collider.tag == "1WayThroughTop")
                {
                    // If the actor is already falling through the platform
                    if (collisions.fallingThroughPlatform)
                    {
                        continue;
                    }
                    // Going through the top falling through the platform
                    if (_playerInput.y == -1)
                    {
                        collisions.fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", .5f);
                        continue;
                    }
                }
                */
                
                // 1 way platform
                //  Jumping under it
                if (hit.collider.tag == "1WayThroughBottom")
                {
                    // If moving upwards, just ignore this ray and skip to the next ray
                    // We'll be able to jump through the platform and create a 1 way platform
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                }



                // set y moveAmount to the point in which the way hit an obstacle
                // Ray distance
                moveAmount.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                // If actor is climbing slope and they're hitting an obstacle above them
                //  Recalculate moveAmount x to be the same as moveAmount y to avoid jitter
                if (collisions.climbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                // If actor has hit something and is moving that direction
                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
        
        // When the actor is climbing a "curved slope" (slope with another slope along the ways, they get stuck for a 
        //  few frames, this will fix it.
        // If the actor is climbing the slope,
        // Fires ray to see if there is a new slope along the way
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if(hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (slopeAngle != collisions.slopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }
    }


    /// <summary>
    /// This Method allows an actor to climb slopes efficiently
    /// </summary>
    private void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
    {
        // want speed to be same as normally to climbing
        // Using trigonometry, this will be used to get the new moveAmount for climbing up the slope both ways
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        // If the actor's y moveAmount is less than the climbinb moveAmount
        //  Actor can jump, and move upwards the slope
        if (moveAmount.y <= climbmoveAmountY)
        {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal = slopeNormal;
        }

        
    }

    private void DecendSlope(ref Vector2 moveAmount)
    {
        // Need to be able to handle slopes that exceed the max slope angle
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(_raycastOrigins.bottomLeft, 
            Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(_raycastOrigins.bottomRight,
                    Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);

        // ^ <- exclusive or oporator
        //  If either one has hit a slope
        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            // Checking if sliding down a slope
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);   
        }

        

        if (!collisions.slidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);

            // cast ray downwards, if moving left, we want the ray to start at the bottom right corner.
            // Moving right will be the bottom left corner. Corner touching the slope
            Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;

            // Points a ray downwards a the origin, pointing down to infinity
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    // if actor is moving down the slope
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        // Check if the actor is close enough to the slope
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            // Actor is close enough to the slope
                            float moveDistance = Mathf.Abs(moveAmount.x);

                            // Decends the slope
                            float decendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= decendmoveAmountY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.decendingSlope = true;
                            collisions.below = true;
                            collisions.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }


        
    }

    private void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
    {
        // if the raycast has his a slope
        if (hit)
        {
            // gets the slope angle
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

            // If the slope angle exceeds the max slope angle, then slide down the slope
            if (slopeAngle > maxSlopeAngle)
            {
                // Gets the move amount by using a trig equation for velocity down a slope
                moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                // Stores the slope angle
                collisions.slopeAngle = slopeAngle;

                // player is sliding down a max slope
                collisions.slidingDownMaxSlope = true;

                collisions.slopeNormal = hit.normal;
            }
        }
    }

    // Resets the falling through platform variable after a set time
    private void ResetFallingThroughPlatform()
    {
        collisions.fallingThroughPlatform = false;
    }


    
}
