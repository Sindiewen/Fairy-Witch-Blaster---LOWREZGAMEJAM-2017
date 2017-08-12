using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Projectile_Controller : MonoBehaviour
{
    // public variables
    public LayerMask collisionMask;

    // Private variables

    private float _projectileSpeed;					// How fast the projectile will fire by time.deltatime and the vector 2 direction

    private SpriteRenderer _sprite;                 // Stores reference to the sprite renderer]

    private BoxCollider2D _boxCol;


    

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

    // Private class methods

    // Use this for initialization
    void Start()
    {
        _boxCol = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        // Gives the projectile velocity of the player in the x direction * speed	
        this.transform.Translate((Vector2.right) * Time.deltaTime * _projectileSpeed);

        projBoxCollisions();
    }

    private void projBoxCollisions()
    {
        Vector2 rayOrigin = transform.position;

        // Creates boxcast around the player
        RaycastHit2D hit = Physics2D.BoxCast(rayOrigin, _boxCol.bounds.size, 0, new Vector2(0, 0), 0, collisionMask);

        // If the projectile hit a valid raycast
        if (hit)
        {
            // debug prints the name of the object
            //Debug.Log(hit.transform.name);

            // Gets the component of anything thats of EnemyBase
            EnemyBase enemy = hit.transform.GetComponent<EnemyBase>();

            // If not null, has enemyBase component
            if (enemy != null)
            {
                // Have the enemy take damage
                enemy.takeDamage();
            }

            // If the projectile has hit an enemy or an enviroment, destroy the projectile
            if (hit.collider.tag == "Lumi" || hit.collider.tag == "Enviroment")
            {
                destroyProjectile();
            }
        }
    }

    public void destroyProjectile()
    {
        // Projectile is not being deactivated after a certain ammount have been spawned
        this.gameObject.SetActive(false);
    }
}
