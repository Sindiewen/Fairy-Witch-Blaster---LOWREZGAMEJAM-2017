using UnityEngine;

[RequireComponent(typeof(LumiController))]
public class EnemyInputManager : MonoBehaviour
{
    // Public variables
    
    // Defines the enemy input
    [HideInInspector]
    public Vector2 _directionalInput;

    //  Component references
    [HideInInspector]
    public LumiController _player;         // Reference to the base player controller


    // Use this for initialization
    private void Start()
    {
        // Initializes component references
        _player = GetComponent<LumiController>();       // Gets the lumicontroller component

    }

    // Update is called once per frame
    private void Update()
    {
        // Moves the character
        _player.SetDirecionalInput(_directionalInput);
    }
}
