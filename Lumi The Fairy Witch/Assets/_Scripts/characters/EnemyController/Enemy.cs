using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [HeaderAttribute("Enemy Stat attributes")]
    public int playerHealth = 5;
	
	// Update is called once per frame
	void Update ()
    {


        // If the health is at 0, destroy player
        if (playerHealth <= 0)
        {
            destroyPlayer();
        }
    }

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

    public void damagePlayer()
    {
        playerHealth--;
    }
}
