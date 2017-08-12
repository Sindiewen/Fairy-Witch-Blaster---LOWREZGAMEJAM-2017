using UnityEngine;

[RequireComponent (typeof (Base2DController))]
public class LumiController : MonoBehaviour
{
    // Public Variables
    [Header("Lumi Movement Attributes")]
    [TooltipAttribute("Player Ground Movement Speed")]
    public float moveSpeed = 6;                     // Defines the movespeed for Lumi
    [TooltipAttribute("How much airborne acceleration needed before player reaches max airborne movement speed")]
    public float accelerationTimeAirborne = 0.2f;   // How long before the player reaches full speed in mid air
    [TooltipAttribute("How much gliding acceleration before the player reaches max glide speed")]
    public float accelerationTimeGliding = 0.4f;    // How long before the player reaches full speed gliding in air
    [TooltipAttribute("How much ground acceleration needed before player reaches max ground movement speed.")]
    public float accelerationTimeGounded = 0.1f;    // How long vefore the player can move full speed on the ground

    [Space(10)]
    [Header("Lumi Wall Jumping")]
    [TooltipAttribute("Can the player Wall Jump?")]
    public bool canWallJump;                        // Toggles weather the player can wall jump
    [TooltipAttribute("Can the player Slide on the Walls?")]
    public bool canWallSlide;                       // Toggles weather the player can wall slide
    [TooltipAttribute("Max speed for sliding down a wall")]
    public float wallSlideSpeedMax = 3;             // The max speed for wall sliding
    [TooltipAttribute("How long the player will stick to the wall before sliding off of it")]
    public float wallStickTime = 0.25f;             // How long the player will stick to the wall before sliding off of it


    [Space(10)]
    // controls Lumi wallJumpClimb
    //  Super Meat Boy/Mega Man X styled wall climbing
    [TooltipAttribute("If player can climb wall Mega Man X/ Super Meat Boy Styled")]
    public bool canWallJumpClimb;
    [TooltipAttribute("x = x direction of jump | y = y direction of jump")]
    public Vector2 wallJumpclimb;

    [Space(10)]
    // Controls Lumi wallJumpOff
    //  Player can just go into opposite direction to get off the wall with no x input
    [TooltipAttribute("If player jumps off wall with no x input")]
    public bool canWallJumpOff;
    [TooltipAttribute("x = x direction of jump | y = y direction of jump")]
    public Vector2 wallJumpOff;

    [Space(10)]
    // Controls wallLeap
    // controls the base Wall Jump where they can jump off the wall to the opposite direction with x input
    [TooltipAttribute("If player jumps off wall with x input opposite of wall direction")]
    public bool canWallLeap;
    [TooltipAttribute("x = x direction of jump | y = y direction of jump")]
    public Vector2 wallLeap;
    
    [Space(10)]
    [Header("Lumi Jump Attributes")]
    [TooltipAttribute("The max height of the jump")]
    public float maxJumpHeight = 4;         // Max height
    [TooltipAttribute("The min height of the jump")]
    public float minJumpHeight = 1;         // mIn jump height
    [TooltipAttribute("How long it takes before player reaches jump apex")]
    public float timeToJumpApex = .4f;      // Time to the height apex (max height) 
    


    // Reference to the base 2D controller
    [HideInInspector]
    public Base2DController _playerController;

    // Stores Lumi's velocity
    [HideInInspector]
    public Vector3 _lumiVelocity;            // Stores the current velocity

    // Defines the direction a wall is by x
    [HideInInspector]
    public int wallDirX;               // Defines the wall direction x

    // bools to check if player is wall jumping
    [HideInInspector]
    public bool wallJumpMMX;
    [HideInInspector]
    public bool wallJumpNoXInput;
    [HideInInspector]
    public bool wallJumpOppositeXInput;
    [HideInInspector]
    public bool wallSliding;           // If the player is wall sliding

    // Private variables
    


    // Defines gravity and jump Velocity
    private float _maxJumpVelocity;     // Defines the jump velocity for Lumi
    private float _minJumpVelocity;     // Defines the min jump velocity
    private float _gravity;             // The gravity for Lumi 


    // Helps smooth out the x axis
    private float _velocityXSmoothing;

    // Defines Lumi's input
    private Vector2 _directionalInput;

    // Gameplay checks
    private bool _canGlide;             // CHecks weather the player can glide


    // Defines wall jumping and unsticking
    private float _timeToWallUnstick;


    // Public class methods

    /// <summary>
    /// 
    /// </summary>
    public void SetDirecionalInput(Vector2 input)
    {
        // Sets the direcional input to our new passed in input
        _directionalInput = input;
    }

    public void onJumpInputDown()
    {
        if (wallSliding && canWallJump)
        {

            // wall Jump Climb - Mega man X/SMB Styled
            if (wallDirX == _directionalInput.x && canWallJumpClimb)
            {
                _lumiVelocity.x = -wallDirX * wallJumpclimb.x;
                _lumiVelocity.y = wallJumpclimb.y;
            }
            // Jumping off wall without x input 
            else if (_directionalInput.x == 0 && canWallJumpOff)
            {
                _lumiVelocity.x = -wallDirX * wallJumpOff.x;
                _lumiVelocity.y = wallJumpOff.y;
            }
            // Jumping off with with x input opposite of wall direction
            else if (canWallLeap)
            {
                _lumiVelocity.x = -wallDirX * wallLeap.x;
                _lumiVelocity.y = wallLeap.y;
            }
        }
        // Regular Jump
        if (_playerController.collisions.below)
        {
            // if player is sliding down max slope
            if (_playerController.collisions.slidingDownMaxSlope)
            {
                // not jumping against max slope
                if (_directionalInput.x != Mathf.Sign(_playerController.collisions.slopeNormal.x))
                {
                    _lumiVelocity.y = _maxJumpVelocity * _playerController.collisions.slopeNormal.y;
                    _lumiVelocity.x = _maxJumpVelocity * _playerController.collisions.slopeNormal.x;
                }
            }
            else
            {
                // Move Lumi upwards to jump
                _lumiVelocity.y = _maxJumpVelocity;
            }
       }
    }

    /// <summary>
    /// 
    /// </summary>
    public void onJumpInputUp()
    {
        if (_lumiVelocity.y > _minJumpVelocity)
        {
            _lumiVelocity.y = _minJumpVelocity;
        }

        // Player cannot glide yet
        _canGlide = false;
    }

    /// <summary>
    /// gliding:
    /// 
    /// If player is holding jump in mid air after letting go of the jump key, they can glide.
    /// 
    /// gliding is calculates msking the _lumiVelocity.y = _minJumpVelocity / (Some value to allow gliding)
    /// </summary>
    public void onJumpGlide()
    {
        
        // If the player is in the air
        //  Not colliding with ground or celing or any walls
        if (!_playerController.collisions.above && !_playerController.collisions.below 
            && !_playerController.collisions.left && !_playerController.collisions.right)
        {
            _canGlide = true;
        }
        
    }


    // Private class methods

    private void Start()
    {
        // Gets reference the base 2d Controller
        _playerController = GetComponent<Base2DController>();

        // Getting the gravity and the jump velocity from values jumpHeight and timeToJumpApex
        _gravity =  -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        _maxJumpVelocity = Mathf.Abs(_gravity * timeToJumpApex);
        _minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_gravity) * minJumpHeight);

        // Player defaults to not being able to glide
        _canGlide = false;
    }

    private void Update()
    {
        if (!_canGlide)
        {
            // Calculates the player velocity
            calculateVelocity();
        }
        // Checks to see if the player can glide in real time
        // If the player can glide (they're holding the jump key mid air
        else
        {
            // Calculates gliding velocity
            calculateGLideVelocity();

            // Slow their decent by applying upward force to the velocity
            _lumiVelocity.y = -(_lumiVelocity.y + 4);
        }
        // Checks if the player can wall slide
        HandleWallSliding();


        // Moves Lumi depending on the change of velocity in lumi
        _playerController.Move(_lumiVelocity * Time.deltaTime,  _directionalInput);


        // If lumi is on the ground or hit something above, reset the velocity to 0
        if (_playerController.collisions.above || _playerController.collisions.below)
        {
            // Defines the wall jump variables to be false when touching the ceeling or floor
            wallJumpMMX = false;
            wallJumpNoXInput = false;
            wallJumpOppositeXInput = false;

            // If sliding down a max slope, gradually increase velocity
            if (_playerController.collisions.slidingDownMaxSlope)
            {
                _lumiVelocity.y += _playerController.collisions.slopeNormal.y * -_gravity * Time.deltaTime;
            }
            // If not sliding down a max slope
            //  set velocity.y to 0
            else
            {
                _lumiVelocity.y = 0;
            }

            // The player cannot glide
            _canGlide = false;

        }

       
        // Check if the player has released the jump key

    }

    private void HandleWallSliding()
    {
        // If facing left, -1, else, 1
        wallDirX = (_playerController.collisions.left) ? -1 : 1;

        // Enables wall sliding
        wallSliding = false;

        // Toggle, if the player Lumi can wall slide 
        if (canWallSlide)
        {
            // for wall sliding, if player is touching the wall on left or right side, not touching ground, and moving downwards
            if ((_playerController.collisions.left || _playerController.collisions.right) && !_playerController.collisions.below && _lumiVelocity.y < 0)
            {
                wallSliding = true;

                if (_lumiVelocity.y < -wallSlideSpeedMax)
                {
                    _lumiVelocity.y = -wallSlideSpeedMax;
                }

                // How long it takes to unstick from the wall
                // If greater than 0, decrease by Time.DeltaTime
                if (_timeToWallUnstick > 0)
                {
                    // Resets x velocity
                    _velocityXSmoothing = 0;
                    _lumiVelocity.x = 0;

                    if (_directionalInput.x != wallDirX && _directionalInput.x != 0)
                    {
                        _timeToWallUnstick -= Time.deltaTime;
                    }
                    else // Resets time to wall unstick
                    {
                        _timeToWallUnstick = wallStickTime;
                    }
                }
                else
                {
                    _timeToWallUnstick = wallStickTime;
                }
            }
        }
    }

    /// <summary>
    /// Called in Update()
    /// 
    /// Calculates the players velocity
    /// </summary>
    private void calculateVelocity()
    {
        // updates lumi's velocity to the input of the player
        float targetVelocityX = _directionalInput.x * moveSpeed;
        _lumiVelocity.x = Mathf.SmoothDamp(_lumiVelocity.x, targetVelocityX, ref _velocityXSmoothing,
            (_playerController.collisions.below) ? accelerationTimeGounded : accelerationTimeAirborne);
        // Applies gravity every frame
        _lumiVelocity.y += _gravity * Time.deltaTime;
    }
    
    private void calculateGLideVelocity()
    {
        // updates lumi's velocity to the input of the player
        float targetVelocityX = _directionalInput.x * moveSpeed;
        _lumiVelocity.x = Mathf.SmoothDamp(_lumiVelocity.x, targetVelocityX, ref _velocityXSmoothing,
            (_playerController.collisions.below) ? accelerationTimeGounded : accelerationTimeGliding);
    }
}
