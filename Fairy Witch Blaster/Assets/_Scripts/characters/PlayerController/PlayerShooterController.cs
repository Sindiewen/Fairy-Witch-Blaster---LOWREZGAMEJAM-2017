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

    [Space(10)]
    [TooltipAttribute("How long the player will be invulnerable")]
    public int invulnerabilityTime = 2;     // Sets the default length of how long the player will be invulnerable after hit


    [HeaderAttribute("Projectile Values")]
    [TooltipAttribute("Stores the object for the player's fairy magic")]
    public GameObject fairyProjectile;      // Reference to the game object for the fairy projectile
    [TooltipAttribute("Stores the object for the player's witch magix")]
    public GameObject witchProjectile;      // Reference to the game ovject for the witch projectile

    [Space(10)]
    [TooltipAttribute("Speed value for the fairy projectile")]
    public float fairyProjFireSpeed;
    [TooltipAttribute("Speed value for the Witch Projectile")]
    public float witchProjFireSpeed;


    // Private class variables
    // Checks if the player is invulnerable or not
    private bool _invulnerable;


	// Use this for initialization
	void Start ()
    {
        // Initializes sliders to ensure they're full
        healthBar.value = playerHealth;
        manaBar.value = playerMana;

        // Sets the default state of the player's invulerability
        _invulnerable = false;
        

	}
	
	// Update is called once per frame
	private void Update ()
    {
        // Updates the player health and mana bars
        healthBar.value = playerHealth;
        manaBar.value = playerMana;


        // Calculates the player's box collisions
        playerBoxCollisions();
        
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
            
            // If the box cast it an eligable object
            // Enemy, projectile
            if (hit)
            {
                // subtract 1 from the player health
                playerHealth--;

                // Makes the player invulnerable
                _invulnerable = true;

                // Set the player to be invulnerable for a set period of time
                Invoke("resetInvulnerability", invulnerabilityTime); 
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
}
