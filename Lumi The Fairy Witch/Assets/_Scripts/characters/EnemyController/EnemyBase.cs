using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (EnemyInputManager))]
public class EnemyBase : MonoBehaviour
{
    // Public Variables
    [HeaderAttribute("Enemy Stat attributes")]
    public int playerHealth = 5;

    /// <summary>
    /// Destroys the enemy once called 
    /// 
    /// 
    /// </summary>
    private void destroyPlayer()
    {
        // TODO: kill animation
        Destroy(this.gameObject);
    }

    public void takeDamage(int damage)
    {
        playerHealth -= damage;

        // If the health is less or equal to 0, destroy the enemy
        if (playerHealth <= 0)
        {
            destroyPlayer();
        }
    }

}
