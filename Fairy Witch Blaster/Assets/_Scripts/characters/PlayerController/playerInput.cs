using UnityEngine;

[RequireComponent(typeof(LumiController))]
public class playerInput : MonoBehaviour
{

    // Public Variables
    [HeaderAttribute("Fire Location Object")]
    [TooltipAttribute("The location to fire the magic")]
    public ProjectileFireLocationController fireLoc;

    // private Variables
    private LumiController _player;             // Reference to the player Input
    private PlayerShooterController _shooter;   // player shooter reference

    private SpriteRenderer _sprite;             // Reference to the player sprite renderer

    // private Animator controller
    private Animator _anim;                     // Reference to the player's animation controller

    

    // Defines the playe rinput
    Vector2 _directionalInput;          // Defines the horizontal and vertial movement of the player

    private string _jumpKey = "Jump";   // Defines the jump key used for jumping
    private string _fire_Fairy = "Fire_Fairy";
    private string _fire_Witch = "Fire_Witch";

    private bool _jump;         // Defines if the player is jumping - pressing the jump key
    private bool _notJump;      // If the player is not jumping - letting go of the jump key 
    private bool _isGliding;    // Checks if the player is gliding or not
    private bool _firingFairy;
    private bool _firingWitch;

    // Stores the facing direction
    private bool _facingRight;
    



    // Private class methods
    private void Start()
    {
        // Gets the component for player input
        _player = GetComponent<LumiController>();
        _shooter = GetComponent<PlayerShooterController>();

        // Initializes componenets
        _sprite = GetComponent<SpriteRenderer>();   // Initializes the sprite renderer
        _anim = GetComponent<Animator>();           // Initializes the animator component

        // initializes the facing direction
        _facingRight = true;

    }

    private void Update()
    {
        // Defines the _direcionalInput to be the horizontal and vertical axis
        _directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Defines the jumping
        _jump = Input.GetButtonDown(_jumpKey);
        _notJump = Input.GetButtonUp(_jumpKey);

        // Defines firing
        _firingFairy = Input.GetButtonDown(_fire_Fairy);
        _firingWitch = Input.GetButtonDown(_fire_Witch);



        ///
        // Initiates player control
        ///
        


        // Checks if the player is colliding with the floor
        _isGliding = (!_player._playerController.collisions.below);

        // Sets the direcional input of the player
        _player.SetDirecionalInput(_directionalInput);


        // If the player is grounded, initialize animation states
        if (_player._playerController.collisions.below)
        {
            _anim.SetBool("anim_isGrounded", true);     // Player is grounded
        }


        // If the player is pressing on the jump key
        //  Have the player jump
        if (_jump)
        {
            // Changes the player animation to show jumping
            _anim.SetBool("anim_isGrounded", false);

            _player.onJumpInputDown();
        }
        
        // If the player is not pressing the jump key
        //  Have the player fall
        if (_notJump)
        {
        // When the player lifts up off the jump key
            _player.onJumpInputUp();

            // Player is not gliding
            _anim.SetBool("anim_isGliding", false);
        }

        // If the player is in the air, and they're pressing the jump key mid air
        if (_jump && _isGliding)
        {
            // Change animation to player gliding
            _anim.SetBool("anim_isGliding", true);

            // Player is gliding now
            _player.onJumpGlide();
        }
        

        // Controls player sprite
        if (_player._lumiVelocity.x > 0)
        {
            // Player moving right
            print("Moving Right");
            _facingRight = true;
            _sprite.flipX = false;
        }
        if (_player._lumiVelocity.x < 0)
        {
            // player moving left
            print("Moving Left");
            _facingRight = false;
            _sprite.flipX = true;
        }

        // Changes animation state to correspong to walking
        _anim.SetFloat("anim_isWalking", _player._lumiVelocity.x);

        // Changes the fire location according if the player is facing left or right
        fireLoc.changeShootingDirection(_facingRight);


        
        // Controls player firing
        if (_firingFairy)
        {
            // If the player fires the fairy magic, shoot the magic
            _shooter.fireFairy(_facingRight);
        }

        // if player is fiuring witch magic
        if (_firingWitch)
        {
            // fires witch magic
            _shooter.fireWitch(_facingRight);
        }
    }
}