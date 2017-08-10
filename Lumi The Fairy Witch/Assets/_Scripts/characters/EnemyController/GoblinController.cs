using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the goblin enemy AI
/// 
/// Passes movement values to the enemyInputManager.cs class to move the enemy.
/// </summary>

public class GoblinController : RaycastController
{
    // Public variables

    [HeaderAttribute("Player Stat attributes")]
    public int playerHealth = 5;

    [HeaderAttribute("Determins the direction the goblin will go")]
    public bool goesRight;

    [HeaderAttribute("Toggles weather the goblin will pace back and forth. and how long they'll pace")]
    public bool canPace = false;
    public float timeToPace = 1.0f;


    [HeaderAttribute("Determines how big the aggro box will be")]
    public float aggroBoxSize = 1;      // How big the aggro box size will be


    // Private variables


    private Vector2 moveDirection;       // Defines the direction the goblin will move

    private EnemyInputManager _enemy;

    private bool _invulnerable;

    // Stores transform values
    // Creates hitbox for the enemy's aggro range
    private Vector2 rayOrigin;     // Origin point of the enemy

    private Vector3 giz_AggroSize;

    protected override void Start()
    {
        base.Start();

        // gets component references
        _enemy = GetComponent<EnemyInputManager>();

        // sets invulnerability to false
        _invulnerable = false;

        // Starts coroutine for pacing the goblin
        StartCoroutine("paceGoblin");

    }

    private void Update()
    {
        // Gets the current origin of the raycast
        rayOrigin = transform.position;


        // calculates raycasting
        enemyBoxCollisions();

        // Sets the directionalInput to the current move direction
        _enemy._directionalInput = moveDirection;


        // If the health is at 0, destroy player
        if (playerHealth <= 0)
        {
            destroyPlayer();
        }

    }


    /// <summary>
    /// Calculates the hitbox collisions for the enemy
    /// 
    /// 
    /// </summary>
    private void enemyBoxCollisions()
    {
        // Creates boxcast around the player
        // creating temp value to store the box size
        Vector2 boxSize = _boxCol.bounds.size;
        boxSize.x += aggroBoxSize;
        boxSize.y += aggroBoxSize;
        giz_AggroSize = boxSize;


        RaycastHit2D hit = Physics2D.BoxCast(rayOrigin, _boxCol.bounds.size, 0, new Vector2(0, 0), 0, collisionMask);
        RaycastHit2D hitAggro = Physics2D.BoxCast(rayOrigin, boxSize, 0, new Vector2(0, 0), 0, collisionMask);

        // If the box hit anything
        if (hitAggro && !canPace)
        {
            if (hitAggro.collider.tag == "Lumi")
            {
                if (goesRight)
                {
                    moveDirection.x = 1;
                }
                else
                {
                    moveDirection.x = -1;
                }
            }
        }

        
        if (hit)
        {
            /// If the enemy hit lumi projectile, subtract health
            /// 
            if (hit.collider.tag == "Lumi_Projectile")
            {
                playerHealth--;
                _invulnerable = true;

                Invoke("resetInvulnerability", 0.1f);
            }

            /// If the enemy hit a hazard, subtract health
            /// 
        }
        
        
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


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        Gizmos.DrawCube(rayOrigin, giz_AggroSize);
    }

    /// <summary>
    /// Once called, the player will no longer be invulnerable to hits
    /// 
    /// </summary>
    private void resetInvulnerability()
    {
        _invulnerable = false;
    }

    private void destroyPlayer()
    {
        // TODO: kill animation
        Destroy(this.gameObject);
    }
}
