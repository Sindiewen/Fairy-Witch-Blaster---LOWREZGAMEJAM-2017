using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFireLocationController : MonoBehaviour
{
    // Public variables
    public Vector2[] FireLocations;     // Stores 2 locations where the fire location will be


    /// <summary>
    /// Changes the firing direction of the wand
    /// </summary>
    /// <param name="facingRight"></param>
    public void changeShootingDirection(bool facingRight)
    {
        // If the player is facing right, move the fire location to the right
        if (facingRight)
        {
            transform.localPosition = new Vector2(FireLocations[0].x, FireLocations[0].y);
        }
        // Move the fire location to the left
        else
        {
            transform.localPosition = new Vector2(FireLocations[1].x, FireLocations[1].y);
        }
    }
}
