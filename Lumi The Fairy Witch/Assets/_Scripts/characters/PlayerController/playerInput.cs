using UnityEngine;

[RequireComponent(typeof(LumiController))]
public class playerInput : MonoBehaviour
{

    // Public Variables
    [HeaderAttribute("Fire Location Object")]
    [TooltipAttribute("The location to fire the magic")]
    public ProjectileFireLocationController fireLoc;

    [HeaderAttribute("Main camera object")]
    public CameraFollow mainCamera;

    [HeaderAttribute("Control camera values for player control")]
    public float verticalOffsetBase;
    public float vertOffsetJump;
    public float vertOffsetFall;
    public float vertOffsetYMax;
    public float vertOffsetYMin;

    [Space(10)]
    public float verticalSmoothTimeBase;
    public float vertSmoothTimeJump;
    public float vertSmoothTimeFall;


    // private Variables
    private LumiController _player;             // Reference to the player Input
    private PlayerShooterController _shooter;   // player shooter reference

    private SpriteRenderer _sprite;             // Reference to the player sprite renderer

    // private Animator controller
    private Animator _anim;                     // Reference to the player's animation controller

    

    // Defines the playe rinput
    private Vector2 _directionalInput;          // Defines the horizontal and vertial movement of the player

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

        // Changes animation state to correspong to walking
        _anim.SetFloat("anim_isWalking", _directionalInput.x);


        // If the player is grounded, initialize animation states
        if (_player._playerController.collisions.below)
        {
            _anim.SetBool("anim_isGrounded", true);     // Player is grounded

            // when grounded, set the vertical offset to the base
            mainCamera.verticalOffset = verticalOffsetBase;

            // Sets the base vertical smooth time
            mainCamera.verticalSmoothTime = verticalSmoothTimeBase;
        }


        // Handle wall jumping and wall sliding:
        // wall Sliding:
        // If the player is wall sliding, move the camera downwards
        if (_player.wallSliding)
        {
            mainCamera.verticalOffset = vertOffsetFall;
        }
        // Wall Jumping - MMX
        if (_player.wallSliding && _player.wallDirX == _directionalInput.x && _jump)
        {
            mainCamera.verticalOffset = vertOffsetJump;

            // When jumping set the vertical smooth time
            mainCamera.verticalSmoothTime = vertSmoothTimeJump;
        }
        // Jumping off wall without x
        else if (_player.wallSliding && _directionalInput.x == 0 && _jump)
        {
            mainCamera.verticalOffset = verticalOffsetBase;
        }



        // If the player is pressing on the jump key
        //  Have the player jump
        if (_jump)
        {
            // Changes the player animation to show jumping
            _anim.SetBool("anim_isGrounded", false);

            _player.onJumpInputDown();

            // When jumping, set the vertical offset so the player can see whats above
            mainCamera.verticalOffset = vertOffsetJump;

            // When jumping set the vertical smooth time
            mainCamera.verticalSmoothTime = vertSmoothTimeJump;
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

        if (!_jump && _player._lumiVelocity.y < 0)
        {
            // When falling, set vertical offset so the player can see below
            mainCamera.verticalOffset = vertOffsetFall;

            // When falling, set the vertical smooth time
            mainCamera.verticalSmoothTime = vertSmoothTimeFall;
        }

        // If the player is in the air, and they're pressing the jump key mid air
        if (_jump && _isGliding && !_player.wallSliding)
        {
            // Change animation to player gliding
            _anim.SetBool("anim_isGliding", true);

            // Player is gliding now
            _player.onJumpGlide();

            // When falling, set vertical offset so the player can see below
            mainCamera.verticalOffset = vertOffsetFall;
        }


        



        // Controls player sprite
        //  Controls camera pointing direction alongside player facing direction
        if (_directionalInput.x > 0)
        {
            // Player moving right
            _facingRight = true;
            _sprite.flipX = false;
        }
        if (_directionalInput.x < 0)
        {
            // player moving left
            _facingRight = false;
            _sprite.flipX = true;
        }
        
        // If the player is pressing up, the camera will move upwards to show whats above
        if (_directionalInput.y > 0 && !_jump && !_isGliding)
        {
            mainCamera.verticalOffset = vertOffsetYMax;
        }
        // If the player is pressing down, the camera will move downwards to show whats below
        if (_directionalInput.y < 0 && !_jump && !_isGliding)
        {
            mainCamera.verticalOffset = vertOffsetYMin;
        }
       
        
        // Changes the fire location according if the player is facing left or right
        fireLoc.changeShootingDirection(_facingRight);


        // Controls player firing
        if (_firingFairy)
        {
            
            // If the player is standing still and the animation is not playing
            // Play the idle firing animation
            if ((_directionalInput.x == 0 && _player._playerController.collisions.below) && !_anim.GetCurrentAnimatorStateInfo(0).IsName("Lumi_Idle_shoot"))
            {
                _anim.Play("Lumi_Idle_Shoot");
            }

            // If the player is moving and firing while animation not playing
            else if ((_directionalInput.x < 0 || _directionalInput.x > 0 && _player._playerController.collisions.below) && !_anim.GetCurrentAnimatorStateInfo(0).IsName("Lumi_Walk_Shoot"))
            {
                _anim.Play("Lumi_Walk_Shoot");
            }

            // If the player is in the air and fires
            else if (!_player._playerController.collisions.below && !_anim.GetCurrentAnimatorStateInfo(0).IsName("Lumi_Jump_Shoot"))
            {
                _anim.Play("Lumi_Jump_Shoot");
            }

;
            // If the player fires the fairy magic, shoot the magic
            _shooter.fireFairy(_facingRight);
        }

        // if player is fiuring witch magic
        if (_firingWitch)
        {
            // fires witch magic

            // If the player is standing still and the animation is not playing
            // Play the idle firing animation
            if ((_directionalInput.x == 0 && _player._playerController.collisions.below) && !_anim.GetCurrentAnimatorStateInfo(0).IsName("Lumi_Idle_shoot"))
            {
                _anim.Play("Lumi_Idle_Shoot");
            }

            // If the player is moving and firing while animation not playing
            else if ((_directionalInput.x < 0 || _directionalInput.x > 0 && _player._playerController.collisions.below) && !_anim.GetCurrentAnimatorStateInfo(0).IsName("Lumi_Walk_Shoot"))
            {
                _anim.Play("Lumi_Walk_Shoot");
            }

            // If the player is in the air and fires
            else if (!_player._playerController.collisions.below && !_anim.GetCurrentAnimatorStateInfo(0).IsName("Lumi_Jump_Shoot"))
            {
                _anim.Play("Lumi_Jump_Shoot");
            }

            // Fire the magic
            _shooter.fireWitch(_facingRight);
        }
    }
    
    /// <summary>
    /// Resets animations for the player
    /// 
    /// Must be invoked
    /// </summary>
    private void resetAnimations()
    {
        // Resets firing animation
        _anim.SetBool("anim_isFiring", false);
    }
}