using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : RaycastController
{
    // Structs
    private struct PassengerMovement
    {
        // Public variables
        public Transform transform;
        public Vector3 velocity;

        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _transform, Vector3 _velocity, 
            bool _standingOnPlatform, bool _moveBeforePlatform)
        {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
    }

    // Containers
    // Stores a list of passengers
    private List<PassengerMovement> passengerMovement;

    // Reduced get component calls by storing them here
    private Dictionary<Transform, Base2DController> passengerDictionary = new Dictionary<Transform, Base2DController>();


    // Public variables
    [Space(40)]
    [TextAreaAttribute]
    public string movingPlatformControllerCS = "This is a part of the movingPlatformController.cs Class.";

    [Header("What are Passengers to the layermask?")]
    [TooltipAttribute("Defines what is a passenger and can ride the moving platform.")]
    public LayerMask passengerMask;     // Selects what a passenger is to ride the platform


    [Header("Platform Movement Attributes")]
    [TooltipAttribute("How fast the platform will move")]
    public float speed;                 // How fast the platform will move
    [TooltipAttribute("How long the platform will wait before moving to the next location")]
    public float waitTime;              // How long the platform will wait before moving to the next location
    [TooltipAttribute("How much the platform will ease into the next location")]
    [Range(0, 2)]
    public float easeAmount;            // How much the platform will ease into the next location

    [TooltipAttribute("True = Platform will restart at the beginning once reaches the end waypoint | " +
        "False = Will go in reverse order")]
    public bool isCyclic;               // Toggle weather the platform will go in reverse or just start again to the original pos
                                        // true = will resstart at begining, false = will go in reverse
    
    [Header("Platform stop Waypoints")]
    [TooltipAttribute("All the waypoint locations for the platform to move. Done in local position of the platform")]
    public Vector3[] localWaypoints;    // Locations to where the platform will move to automatically
                                        // bunch of positions relative to the platform


    // Private class variables
    private Vector3[] _globalWaypoints;     // Stoers locations of global waypoints
    private int fromWaypointIndex;          // from waypoint index
    private float percentBetweenWaypoints;  // between 0 - 1

    private float nextMoveTime;

    // Runs the base start method plus this one too
    protected override void Start()
    {
        base.Start();

        // Runs next after base.start()

        // Creates the new array of global waypoints
        _globalWaypoints = new Vector3[localWaypoints.Length];

        // Sets the global waypoints to equal the local waypoints + the platforms location
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            _globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }


    // Called once each frame
    private void Update()
    {
        // Updates raycasts
        UpdateRaycastOrigins();

        // Velocity for how fast to move the platform
        Vector3 velocity = calculatePlatformMovement();

        // Moves the platform
        CalculatePassengerMovement(velocity);

        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }

    /// <summary>
    /// Eases the platforms to the next location
    /// </summary>
    /// <returns></returns>
    private float platformEasing(float x)
    {
        float a = easeAmount + 1;

        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    /// <summary>
    /// Calculates the platform movement on where the platform will move next.
    /// 
    /// Using an array of waypoints that will be set in the inspector, the platform will move to each platform.
    /// Once moved, the user can set in the inspector how long the platform will wait before the platform can move again.
    /// 
    /// TODO: inplement easing so the platform can "ease" into the next location
    /// </summary>
    /// <returns></returns>
    private Vector3 calculatePlatformMovement()
    {
        // Do not move at all if the time is less then the next move time
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        // resets to zero
        fromWaypointIndex %= _globalWaypoints.Length;

        // Stores the next value in the array
        int toWaypointIndex = (fromWaypointIndex + 1) % _globalWaypoints.Length;

        // need to know distance between waypoints
        float distanceBetweenWaypoints = Vector3.Distance(_globalWaypoints[fromWaypointIndex], _globalWaypoints[toWaypointIndex]);

        // increase percentage each frame
        // move faster for waypoints farther away
        percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;

        // ensures it's clamped between 0 and 1
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);

        // Eases the platform
        float easedPercetntBetweenWaypoints = platformEasing(percentBetweenWaypoints);

        // Lerps between each of the waypoints
        Vector3 newPos = Vector3.Lerp(_globalWaypoints[fromWaypointIndex], _globalWaypoints[toWaypointIndex], easedPercetntBetweenWaypoints);
        
        // If the platform has reached the next waypoint
        if (percentBetweenWaypoints >= 1)
        {
            // reset percent to 0
            percentBetweenWaypoints = 0;

            // Increment the fromwaypointindex to go to the next waypoint in the array
            fromWaypointIndex++;

            // If the platform is not cyclic
            if (!isCyclic)
            {
                // If platform has reached end of the waypoints array
                if (fromWaypointIndex >= _globalWaypoints.Length - 1)
                {
                    // weve reached end of the array
                    fromWaypointIndex = 0;

                    // reverse aray of waypoints
                    System.Array.Reverse(_globalWaypoints);
                }
            }

            // when platform has reached new waypoint
            // Next move time beceomes the time to wait before moving
            nextMoveTime = Time.time + waitTime;

        }
       
        
        return newPos - transform.position;
    }

    /// <summary>
    /// A little tricky to figure out, but becuase of how programming work with one line of code running at a time, we must
    /// change when we want to move platforms. If above, we must move the actor before the platform to ensure that there is
    /// no way to have the actor potentially fall through the platform.
    /// 
    /// For horizontal, doesn't matter too much although we want ot avoid the actor moving through the platform.
    /// 
    /// For above and below, it doesnt matter so we move the platform after the actor
    /// </summary>
    /// <param name="beforeMovePlatform"></param>
    private void MovePassengers(bool beforeMovePlatform)
    {
        foreach(PassengerMovement passenger in passengerMovement)
        {
            // If a passenger in the dictionary does not contain the a passenger,
            //  add them to the dictionary with the baseController2D component
            if (!passengerDictionary.ContainsKey(passenger.transform))
            {
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Base2DController>());
            }

            // If the passenger can be moved before the platform,
            //  Move them before the platform
            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
            }
        }
    }

    /// <summary>
    /// Any controller 2D bring affected by the platform will be moved by the platform.
    /// If the player, or any actor in the scene stands on it, they'll be moved by the platform
    /// </summary>
    /// <param name="velocity"></param>
    private void CalculatePassengerMovement(Vector3 velocity)
    {
        // Variables
        //  Hashset of transforms. Stores how many passengers to move on the platforms
        //  Prevents the issue of passengers being moved multiple times perframe
        HashSet<Transform> movedPassengers = new HashSet<Transform>();

        // Every time we calculate passenger movement, we want to update the list
        passengerMovement = new List<PassengerMovement>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        // Vertically moving platfortm
        //  This is a vertially moving platform because y velocity != 0
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                // Sets the rayOrigin if moving down, rays start at bottom left corner. else, top left corner
                Vector2 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (_verticalRaySpacing * i);

                // Creates a raycast for hitting a layer
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                // If the platform raycasts are hitting a valid layer (an actor is standing on it)
                //  Move the actor with the platform
                if (hit && hit.distance != 0)
                {
                    // If the hashset of moved passengers does not contain a said passenger
                    // Add the passenger to the set and move them
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);


                        // pushX: actor shouldn't be affected by the x velocity unless they're standing on the platform
                        float pushX = (directionY == 1) ? velocity.x : 0;

                        // pushY: moving the platform vertically
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                        // moves the platform and Actor
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }
                    
                }
            }
        }

        // Horizontally moving platform
        //  Actor is being moved to the side
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                // Sets the rayOrigin if moving down, rays start at bottom left corner. else, top left corner
                Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (_horizontalRaySpacing * i);

                // Creates a raycast for hitting a layer
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);


                // If the platform raycasts are hitting a valid layer (an actor is standing on it)
                //  Move the actor with the platform
                if (hit && hit.distance != 0)
                {
                    // If the hashset of moved passengers does not contain a said passenger
                    // Add the passenger to the set and move them
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);


                        // pushX: actor shouldn't be affected by the x velocity unless they're standing on the platform
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;

                        // pushY: moving the platform vertically
                        float pushY = -skinWidth;

                        // moves the platform and Actor
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }

                }
            }
        }

        // Passenger is on top of hotizontally or downward moving platform
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = skinWidth * 2;

            for (int i = 0; i < verticalRayCount; i++)
            {
                // Ray always pointing upwards
                Vector2 rayOrigin = _raycastOrigins.topLeft + Vector2.right * (_verticalRaySpacing * i);

                // Creates a raycast for hitting a layer
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                // If the platform raycasts are hitting a valid layer (an actor is standing on it)
                //  Move the actor with the platform
                if (hit)
                {
                    // If the hashset of moved passengers does not contain a said passenger
                    // Add the passenger to the set and move them
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);


                        // pushX: actor shouldn't be affected by the x velocity unless they're standing on the platform
                        float pushX = velocity.x;

                        // pushY: moving the platform vertically
                        float pushY = velocity.y;

                        // moves the platform and Actor
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }
            }
        }
    }


    /// <summary>
    /// Draws gizmos to the scene view to display where the platforms will be going
    /// </summary>
    private void OnDrawGizmos()
    {
        // As long as the localwaypoints array has values in it
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;

            // the size of the drawn gizmos
            float size = 0.3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                // convert local pos to global pos
                Vector3 globalWaypointPos = (Application.isPlaying) ? _globalWaypoints[i] : localWaypoints[i] + transform.position;

                // drawing a cross where a point is
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }

        }
        
    }


}
