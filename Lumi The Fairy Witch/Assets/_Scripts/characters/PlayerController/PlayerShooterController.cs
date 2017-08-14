using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 2D Shooter controller for the player
/// 
/// Variables:
/// // Health and mana
///     2 variables that controls the sliders for the health and the mana bars in the ui
///         When touched by an enemy, or by an enemy projectile, deal damage, and move the player backwards slightly. Give
///         temporary invulnerability for a period of time.
/// 
/// // Shooting mechanics
/// Both fairy and witch spells as gameObjects with colliders.
///     Projectile speeds
///     
///     
/// when the projectile is fired, is deals a specific ammount of damage in a variable
/// </summary>

[RequireComponent (typeof (playerInput))]
public class PlayerShooterController : RaycastController
{

    // Public Variables
    [HeaderAttribute("UI References")]
    public Slider healthBar;                // Stores reference to the player's health bar
    public Slider manaBar;                  // Stores reference to the player's mana bar

    [HeaderAttribute("Player Stats")]
    [TooltipAttribute("Stores the players current health")]
    [RangeAttribute(0, 10)]
    public int playerHealth = 10;           // Stores the player health
    [TooltipAttribute("Stores the player's current mana")]
    [RangeAttribute(0, 20)]
    public int playerMana = 20;             // Stores the player's mana
    [TooltipAttribute("How long in seconds to regenerate the player mana")]
    public float manaRegenRate = 1.0f;      // How long per second to regen mana
    [TooltipAttribute("How much to regen mana per second")]
    public int manaRegenAmmount = 1;   // How much mana to regen persecond

    [Space(10)]
    [TooltipAttribute("How long the player will be invulnerable")]
    public int invulnerabilityTime = 2;     // Sets the default length of how long the player will be invulnerable after hit


    [HeaderAttribute("Projectile Values")]
    [TooltipAttribute("Location to fire the projectile")]
    public Transform projectileSpawnLocation;               // Location to spawn the projectile

    [TooltipAttribute("Stores the object for the player's fairy magic")]
    //public Lumi_Projectile_Controller fairyProjectile;      // Reference to the game object for the fairy projectile
    public Lumi_Projectile_Controller fairyProjectile;
    [TooltipAttribute("Stores the object for the player's witch magix")]
    public Lumi_Projectile_Controller witchProjectile;      // Reference to the game ovject for the witch projectile

    [Space(10)]
    [TooltipAttribute("Speed value for the projectile")]
    public float projFireSpeed;

    [TooltipAttribute("Max number of projectiles the player can have on screen")]
    public int maxNumProjOnScreen;

    [TooltipAttribute("The time in seconds how long a projectile lasts in the scene before removing it")]
    public float projectileDespawnTime = 0.5f;

    // Stores the number of projectiles the player has shot currently
    public int numProjectilesOnScreen;

    // Private class variables
    private bool _invulnerable;     // Checks if the player is invulnerable or not
    private bool _canMagic;         // can cast magic




    // Gets the lumicontroller for velocity
    //private LumiController _lumi;


    /* -------------------- */
    // Public class methods
    /* -------------------- */

    /// <summary>
    /// Controls the player firing fairy magic
    /// </summary>
    /// <param name="facingRight"></param>
    public void fireFairy(bool facingRight)
    {
        if (_canMagic && numProjectilesOnScreen < maxNumProjOnScreen)
        {
            Vector2 spawnLoc;
            spawnLoc.x = projectileSpawnLocation.transform.position.x;
            spawnLoc.y = projectileSpawnLocation.transform.position.y;
            fairyPoolManager.instance.ReuseObject(fairyProjectile, spawnLoc, Quaternion.identity, facingRight, projFireSpeed, projectileDespawnTime, ref numProjectilesOnScreen);
            //globalPoolManager.instance.ReuseFairyObject(fairyProjectile, spawnLoc, Quaternion.identity, facingRight, projFireSpeed, projectileDespawnTime, ref numProjectilesOnScreen);

            /*
            // Stores reference to the projectile clone
            //Lumi_Projectile_Controller projClone;
            GameObject projClone;

            // Creates temp speed value for manipulation
            float projSPeed = projFireSpeed;

            // Instantiates the projectile clone
            //projClone = Instantiate(fairyProjectile, projectileSpawnLocation.transform.position, Quaternion.identity) as Lumi_Projectile_Controller;
            projClone = Instantiate(fairyProjectile, projectileSpawnLocation.transform.position, Quaternion.identity) as GameObject;

             /*
            // If facing left
            if (!facingRight)
            {
                // Sets the direction and speed of the projectile
                // Sets the firespeed to go left
                projFireSpeed *= -1;
            }
            else
            {
                projFireSpeed = Mathf.Abs(projFireSpeed);
            }


            // Sets the projectile values
            //projClone.setProjectileFiringValues(projSPeed);
            projClone.GetComponent<Lumi_Projectile_Controller>().setProjectileFiringValues(projSPeed);
            
            */
            // Decrement your mana
            playerMana--;

            // Increase number of projectiles on screen
            numProjectilesOnScreen++;

        }
    }

    /// <summary>
    /// Controls the player firing witch magic
    /// </summary>
    /// <param name="facingRight"></param>
    public void fireWitch(bool facingRight)
    {
        if (_canMagic && numProjectilesOnScreen < maxNumProjOnScreen)
        {
            Vector2 spawnLoc;
            spawnLoc.x = projectileSpawnLocation.transform.position.x;
            spawnLoc.y = projectileSpawnLocation.transform.position.y;
            witchPoolManager.instance.ReuseObject(witchProjectile, spawnLoc, Quaternion.identity, facingRight, projFireSpeed, projectileDespawnTime, ref numProjectilesOnScreen);
            //globalPoolManager.instance.ReuseWitchObject(witchProjectile, spawnLoc, Quaternion.identity, facingRight, projFireSpeed, projectileDespawnTime, ref numProjectilesOnScreen);

            /*
            // Stores reference to the projectile clone
            Lumi_Projectile_Controller projClone;

            // Creates temp speed value for manipulation
            float projSPeed = projFireSpeed;

            // Instantiates the projectile clone
            projClone = Instantiate(witchProjectile, projectileSpawnLocation.transform.position, Quaternion.identity) as Lumi_Projectile_Controller;

            // If facing left
            if (!facingRight)
            {
                // Sets the direction and speed of the projectile
                // Sets the firespeed to go left
                projSPeed *= -1;
            }

            // Sets the projectile values
            projClone.setProjectileFiringValues(projSPeed);

            */
            // Decrement your mana
            playerMana--;

            // Increase number of projectiles on screen
            numProjectilesOnScreen++;
        }
    }


    /* -------------------- */
    // Private class methods
    /* -------------------- */

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();

        // Object pooling
        //  Creates pool for the player projectiles
        fairyPoolManager.instance.CreatePool(fairyProjectile, 5);
        witchPoolManager.instance.CreatePool(witchProjectile, 5);
        //globalPoolManager.instance.CreateFairyPool(fairyProjectile, 5);
        //globalPoolManager.instance.CreateWitchPool(witchProjectile, 5);

        // Initializes sliders to ensure they're full
        healthBar.value = playerHealth;
        manaBar.value = playerMana;

        // Sets the default state of the player's invulerability
        _invulnerable = false;

        // player defaults to can cast magic
        _canMagic = true;

        // Starts coroutine to start the mana regeneration process imediately
        StartCoroutine("manaRegen");

        // Creates a starting ammount of projectiles
        numProjectilesOnScreen = 0;
	}
	
	// Update is called once per frame
	private void FixedUpdate ()
    {
        // Updates the player health and mana bars
        healthBar.value = playerHealth;
        manaBar.value = playerMana;

        // Updates how many projectiles are in the game world
        numProjectilesOnScreen = GameObject.FindGameObjectsWithTag("Lumi_Projectile").Length;

        // Calculates the player's box collisions
        playerBoxCollisions();



        // Checks if the player is dead.
        //  If dead, game over
        if (playerHealth <= 0)
        {
            gameOver();
        }
        
        // If the mana is less than 0,
        //  player can't cast their magic anymore
        if (playerMana <= 0)
        {
            _canMagic = false;
        }
        else // The player can cast magic
        {
            _canMagic = true;
        }

	}



    /// <summary>
    /// Calculates the player's collisions for damage against objects
    /// </summary>
    private void playerBoxCollisions()
    {
        if (!_invulnerable)
        {
            Vector2 rayOrigin = transform.position;     // Origin at the players current position
            
            // Creates a boxcast around the player
            // Creates box cast at origin, the size of the player's collider, no angle, no direction, no distance, and at a specific layer
            RaycastHit2D hit = Physics2D.BoxCast(rayOrigin, _boxCol.bounds.size, 0, new Vector2(0,0), 0, collisionMask);
            
            if (hit)
            {
                // If the box cast it an eligable object
                // Enemy, projectile
                if (hit.collider.tag == "Enemy_Projectile" || hit.collider.tag == "Enemy")
                {
                    // subtract 1 from the player health
                    playerHealth--;

                    // Makes the player invulnerable
                    _invulnerable = true;

                    // Set the player to be invulnerable for a set period of time
                    Invoke("resetInvulnerability", invulnerabilityTime);
                }

                // If the player has hit an instakil hazard (spikes, water, pitfalls)
                if (hit.collider.tag == "Hazard_Instakill")
                {
                    playerHealth = 0;
                }
            }
           
        }
    }

    /// <summary>
    /// Once called, the player will no longer be invulnerable to hits
    /// 
    /// </summary>
    private void resetInvulnerability()
    {
        _invulnerable = false;
    }


    /// <Summary>
    /// Kills player and game over
    /// </Summary>
    private void gameOver()
    {
        // TODO: kill player, game over screen, respawn player after button press

        // sets the health bar to 0
        healthBar.value = playerHealth;

        // disables the character 
        gameObject.SetActive(false);
    }

    
    /// <summary>
    /// Coroutuine for renerating mana.
    /// 
    /// Every second, regen mana
    /// </summary>
    /// <returns></returns>
    private IEnumerator manaRegen()
    { 
        // Always runs, loops forever
        while (true)
        {
            // If the player mana is less than 20
            //  Regenerate the mana by ammount for a set ammount of time
            if (playerMana < 20)
            {
                playerMana += manaRegenAmmount;
                yield return new WaitForSeconds(manaRegenRate);
            }
            else // If the mana is greater than max, just yield
            {
                yield return null;
            }
        }
    }
}
