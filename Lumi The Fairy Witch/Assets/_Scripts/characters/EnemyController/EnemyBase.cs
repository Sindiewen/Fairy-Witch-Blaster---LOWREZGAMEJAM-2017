using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof (EnemyInputManager))]
public class EnemyBase : MonoBehaviour
{
    // Public Variables
    [HeaderAttribute("Enemy Stat attributes")]
    public int playerHealth = 5;

    // Text mesh pro text for displaying enemy health
    public TextMeshPro text;

    // enemy hit audio for when the enemy gets hit
    public AudioClip hitSound;

    // private variables
    private AudioSource _audio;

    private void Start()
    {
        // Sets the health value of the enemies
        text.SetText(playerHealth.ToString());

        // Initializes the audio
        _audio = GetComponent<AudioSource>();
        _audio.playOnAwake = false;

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

    public void takeDamage(int damage)
    {
        // Plays hit sound
        _audio.clip = hitSound;
        _audio.Play();

        // Subtracts enemy health
        playerHealth -= damage;
        text.SetText(playerHealth.ToString());

        // If the health is less or equal to 0, destroy the enemy
        if (playerHealth <= 0)
        {
            destroyPlayer();
        }
    }

}
