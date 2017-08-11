using UnityEngine;

/// <Summary>
///	Controls the attributes of the projectiles 
/// </Summary>
public class Lumi_Projectile_Controller : MonoBehaviour
{
    // public variables
    public LayerMask collisionMask;
    //public PlayerShooterController shooter;

	// Private variables

	private float _projectileSpeed;					// How fast the projectile will fire by time.deltatime and the vector 2 direction

    private SpriteRenderer _sprite;                 // Stores reference to the sprite renderer]

    private BoxCollider2D _boxCol;
	

	// Use this for initialization
	void Start () 
	{
        _boxCol = GetComponent<BoxCollider2D>();
    
        // Destroys the object after a set amount of time
        Invoke("destroyProjectile", 0.5f);
	}
	
	void Update()
	{
        // Gives the projectile velocity of the player in the x direction * speed	
        this.transform.Translate((Vector2.right) * Time.deltaTime * _projectileSpeed);

        projBoxCollisions();
	}

    // Public class methods

    /// <summary>
    /// Sets the projectile firing direction and speed
    /// </summary>
    public void setProjectileFiringValues(float projFireSpeed)
    {
        // Initializes the sprite renderer
        _sprite = GetComponent<SpriteRenderer>();

        // Sets the facing direction
        if (projFireSpeed < 0)
        {
            // Sets the sprite's direction to left
            _sprite.flipX = true;
        }
        else
        {
            _sprite.flipX = false;
        }

        // Sets the projectile speed
        _projectileSpeed = projFireSpeed;
    }


    private void projBoxCollisions()
    {
        Vector2 rayOrigin = transform.position;

        // Creates boxcast around the player
        RaycastHit2D hit = Physics2D.BoxCast(rayOrigin, _boxCol.bounds.size, 0, new Vector2(0, 0), 0, collisionMask);

        if (hit)
        {
            if (hit.collider.tag == "Enemy" || hit.collider.tag == "Enviroment")
            {
                destroyProjectile();
            }
        }
    }

    public void destroyProjectile()
    {
        //shooter.numProjectilesOnScreen--;
        Destroy(this.gameObject);
    }

   

}
