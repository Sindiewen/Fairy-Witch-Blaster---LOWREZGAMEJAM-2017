using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyInputManager))]
public class ShooterController : MonoBehaviour
{

    // Public variables
    [HeaderAttribute("Projectile Firing Controller")]
    public Enemy_Projectile_Controller enemyProj;   // Reference to the enemy projectile to fire
    public Transform projectileSpawnLocation;

    [TooltipAttribute("How many projectiles to fire in a burst")]
    public int burstFireCount;                  // How many projectiles to fire in a burst
    [TooltipAttribute("How long to wait between each projectile shot")]
    public float fireTimeBetweenShots = 1f;     // How long before each shot is fired
    [TooltipAttribute("How long to wait before the next set of burst fired shots")]
    public float bursFireWaitTime;              // How long to wait before the next burst fire
    [TooltipAttribute("How fast the projectile will fly")]
    public float projectileFireSpeed;           // COntrols how fast the projectile will fire
    [TooltipAttribute("How long before the projectile despawns")] 
    public float projectileDespawnTime;
    [TooltipAttribute("Projectile fire sound")]
    public AudioClip projectileFire;


    [HeaderAttribute("If not pacing, Determines how big the aggro box will be")]
    public float aggroRaySize;      // How big the aggro box size will be
    public Vector2 aggroBoxSize;

    // Private variables
    // Bools to check if the player has been found
    private bool _LumiFound;

    // audio source reference
    private AudioSource _audio;

    


    // Reference to the enemy Input Manager for controling the enemy input
    private EnemyInputManager _enemyInput;  // Reference to the enemy input manager

    // Defines enemy movement
    private Vector2 moveDirection;          // Defines the direction the goblin will move

    // Stores transform values
    // Creates hitbox for the enemy's aggro range
    private Vector2 rayOrigin;     // Origin point of the enemy

    // Stores how bix the aggroBox will be to render with a gizmo
    Vector3 giz_AggroBox;



    // Private class methods 
    private void Start()
    {
        // gets component references
        _enemyInput = GetComponent<EnemyInputManager>();
        _audio = GetComponent<AudioSource>();
        _audio.playOnAwake = false;

        // Creates pool for the enemy projectiles
        enemyPoolManager.instance.CreatePool(enemyProj, 8);
        //globalPoolManager.instance.CreateEnemyPool(enemyProj, 5);

        // Starts coroutine for pacing the goblin
        StartCoroutine("ShooterControl");

        // Defaults lumi to be found being false
        _LumiFound = false;
    }
    // Private class methods

    /// <summary>
    /// Coroutine for the shooter control
    /// 
    /// Controls shooting
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShooterControl()
    {
        // Coroutine lasts forever
        while (true)
        {
            
            // If the player Lumi has been found, start shooting
            if (_LumiFound)
            {
                // Loops through burst fire count to shoot that many projectiles
                for (int i = 0; i < burstFireCount; i++)
                {
                    // fires projectile
                    fireProjectile();

                    // Waits an ammount of time before the next shot can be fired
                    yield return new WaitForSeconds(fireTimeBetweenShots);
                }

                // Waits a specific amount of time before the burst fire can be started again
                yield return new WaitForSeconds(bursFireWaitTime);
                
            }
            else
            {
                yield return null;
            }
        }
    }
    

    private void Update()
    {
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
        // Defines the box size
        giz_AggroBox = _enemyInput._boxCol.bounds.size + new Vector3(0.1f, 0.1f, 0.0f);

        // Creates 2 raycasts for checking if the enemy is colliding with the player to chase them
        RaycastHit2D hitAggroLeft = Physics2D.Raycast(rayOrigin, Vector2.left, aggroRaySize, _enemyInput.collisionMask);
        RaycastHit2D hitAggroRight = Physics2D.Raycast(rayOrigin, Vector2.right, aggroRaySize, _enemyInput.collisionMask);

        // Draws debug rays
        Debug.DrawRay(rayOrigin, Vector2.left * aggroRaySize, Color.blue);
        Debug.DrawRay(rayOrigin, Vector2.right * aggroRaySize, Color.blue);

        // If the box hit anything
        //  Move the enemy
        if (hitAggroLeft)
        {
            // If hit on left side, move the enemy left
            if (hitAggroLeft.collider.tag == "Lumi")
            {
                // "move" the enemy to the left
                moveDirection.x = -1;

                // lumi has been found on the left
                _LumiFound = true;
            }

        }
        else if (hitAggroRight)
        {
            if (hitAggroRight.collider.tag == "Lumi")
            {
                // "move" the enemy to the right
                moveDirection.x = 1;

                // Lumi has been found on the right
                _LumiFound = true;
            }
        }
        else
        {
            _LumiFound = false;
        }
    }

    /// <summary>
    /// Fires the enemy projetile from the pool
    /// 
    /// 
    /// </summary>
    private void fireProjectile()
    {
        _audio.clip = projectileFire;
        _audio.Play();

        Vector2 spawnLoc;
        spawnLoc.x = projectileSpawnLocation.transform.position.x;
        spawnLoc.y = projectileSpawnLocation.transform.position.y;
        enemyPoolManager.instance.ReuseObject(enemyProj, spawnLoc, Quaternion.identity, _enemyInput._facingRight, projectileDespawnTime, projectileFireSpeed);
        //globalPoolManager.instance.ReuseEnemyObject(enemyProj, spawnLoc, Quaternion.identity, _facingRight, projectileDespawnTime, projectileFireSpeed);
    }

    /// <summary>
    /// Draws the aggro box size
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        Gizmos.DrawCube(rayOrigin, giz_AggroBox);
    }

}
