using UnityEngine;

[RequireComponent(typeof(LumiController))]
public class EnemyInputManager : RaycastController
{
    // Public variables
    public ProjectileFireLocationController fireLoc;
    
    // Defines the enemy input
    [HideInInspector]
    public Vector2 _directionalInput;

    //  Component references
    [HideInInspector]
    public LumiController _player;         // Reference to the base player controller

    [HideInInspector]
    // Bool checks the direction the enemy is facing
    public bool _facingRight;

    // Private variables
    private SpriteRenderer _sprite;
    private Animator _anim;

    

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        // Initializes component references
        _player = GetComponent<LumiController>();       // Gets the lumicontroller component
        _sprite = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();

        // Initializes the facing direction
        _facingRight = true;
    }

    // Update is called once per frame
    private void Update()
    {
        // Moves the character
        _player.SetDirecionalInput(_directionalInput);

        // If the enemy is moving right
        if (_directionalInput.x > 0)
        {
            _facingRight = true;
            _sprite.flipX = false;

            if (fireLoc != null)
            {
                fireLoc.changeShootingDirection(_facingRight);
            }
        }
        // If the enemy is moving left
        else if (_directionalInput.x < 0)
        {
            _facingRight = false;
            _sprite.flipX = true;

            if (fireLoc != null)
            {
                fireLoc.changeShootingDirection(_facingRight);
            }
        }
    }
}
