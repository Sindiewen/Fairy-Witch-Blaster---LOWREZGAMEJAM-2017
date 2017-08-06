using UnityEngine;

/// <Summary>
///	Controls the attributes of the projectiles 
/// </Summary>
public class Lumi_Projectile_Controller : MonoBehaviour
{
	// Private variables

	private float _projectileSpeed;					// How fast the projectile will fire by time.deltatime and the vector 2 direction

    private SpriteRenderer _sprite;                 // Stores reference to the sprite renderer
	

	// Use this for initialization
	void Start () 
	{
        // Destroys the object after a set amount of time
        Destroy(this.gameObject, 3.0f);
	}
	
	void Update()
	{
        // Gives the projectile velocity of the player in the x direction * speed	
        this.transform.Translate((Vector2.right) * Time.deltaTime * _projectileSpeed);
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
	
}
