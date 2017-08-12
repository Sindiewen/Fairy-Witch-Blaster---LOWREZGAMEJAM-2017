using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the goblin enemy AI
/// 
/// Passes movement values to the enemyInputManager.cs class to move the enemy.
/// </summary>
[RequireComponent(typeof (EnemyInputManager))]
public class GoblinController : RaycastController
{
    // Public variables
   [HeaderAttribute("Toggles weather the goblin will pace back and forth. and how long they'll pace")]
    public bool canPace = false;
    public float timeToPace = 1.0f;

    [HeaderAttribute("If not pacing, Determines how big the aggro box will be")]
    public float aggroRaySize = 1;      // How big the aggro box size will be


    // Private variables
    // Reference to the enemy Input Manager for controling the enemy input
    private EnemyInputManager _enemyInput;  // Reference to the enemy input manager

    // Defines enemy movement
    private Vector2 moveDirection;          // Defines the direction the goblin will move

    // Checks if the enemy is invulnerable
    //private bool _invulnerable;

    // Stores transform values
    // Creates hitbox for the enemy's aggro range
    private Vector2 rayOrigin;     // Origin point of the enemy

    Vector3 gizmoBox;



    // Protected class methods 

    // Overidded start method
    protected override void Start()
    {
        base.Start();

        // gets component references
        _enemyInput = GetComponent<EnemyInputManager>();

        // sets invulnerability to false
        //_invulnerable = false;

        // Starts coroutine for pacing the goblin
        StartCoroutine("paceGoblin");

    }

    // Private class methods

    private void Update()
    {
        // Sets the player to be not invulnerable
        //_invulnerable = false;

        // Gets the current origin of the raycast
        rayOrigin = transform.position;


        // calculates raycasting
        aggroBoxCollisions();

        // Sets the directionalInput to the current move direction
        _enemyInput._directionalInput = moveDirection;

    }


    /// <summary>
    /// Calculates the hitbox collisions for the enemy
    /// 
    /// 
    /// </summary>
    private void aggroBoxCollisions()
    {
        gizmoBox = _boxCol.bounds.size + new Vector3(0.1f, 0.1f, 0.0f);

        // Creates 2 raycasts for checking if the enemy is colliding with the player to chase them
        RaycastHit2D hitAggroLeft = Physics2D.Raycast(rayOrigin, Vector2.left, aggroRaySize, collisionMask);
        RaycastHit2D hitAggroRight = Physics2D.Raycast(rayOrigin, Vector2.right, aggroRaySize, collisionMask);

        // Draws debug rays
        Debug.DrawRay(rayOrigin, Vector2.left * aggroRaySize, Color.blue);
        Debug.DrawRay(rayOrigin, Vector2.right * aggroRaySize, Color.blue);

        // If the box hit anything
        //  Move the enemy
        if (hitAggroLeft && !canPace)
        {
            // If hit on left side, move the enemy left
            if (hitAggroLeft.collider.tag == "Lumi")
            {
                moveDirection.x = -1;
            }
            
        }
        if (hitAggroRight && !canPace)
        {
            if (hitAggroRight.collider.tag == "Lumi")
            {
                moveDirection.x = 1;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        Gizmos.DrawCube(rayOrigin, gizmoBox);
    }

    private IEnumerator paceGoblin()
    {
        // runs forever. WHen called, the loop will run, causing the goblin to move back and forth. Only if though
        // the pace checkmark has been checked
        while (true && canPace)
        {
            moveDirection.x = -1;

            yield return new WaitForSeconds(timeToPace);

            moveDirection.x = 1;

            yield return new WaitForSeconds(timeToPace);
        }
    }

    /// <summary>
    /// Once called, the player will no longer be invulnerable to hits
    /// 
    /// </summary>
    private void resetInvulnerability()
    {
        //_invulnerable = false;
    }
}
