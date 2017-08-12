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

    // Private variables
    public SpriteRenderer _sprite;
    public Animator _anim;

    // Use this for initialization
    private void Start()
    {
        // Initializes component references
        _player = GetComponent<LumiController>();       // Gets the lumicontroller component
        _sprite = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    private void Update()
    {
        // Moves the character
        _player.SetDirecionalInput(_directionalInput);

        // If the enemy is moving right
        if (_directionalInput.x > 0)
        {
            _sprite.flipX = false;
        }
        // If the enemy is moving left
        else if (_directionalInput.x < 0)
        {
            _sprite.flipX = true;
        }
    }
}
