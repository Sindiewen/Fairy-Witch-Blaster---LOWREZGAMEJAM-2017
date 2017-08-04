using UnityEngine;

[RequireComponent(typeof(LumiController))]
public class playerInput : MonoBehaviour
{

    // private Variables
    LumiController _player;     // Reference to the player Input

    // Defines the playe rinput
    Vector2 _directionalInput;          // Defines the horizontal and vertial movement of the player

    private string _jumpKey = "Jump";   // Defines the jump key used for jumping

    private bool _jump;         // Defines if the player is jumping - pressing the jump key
    private bool _notJump;      // If the player is not jumping - letting go of the jump key 
    private bool _isGliding;



    // Private class methods
    private void Start()
    {
        // Gets the component for player input
        _player = GetComponent<LumiController>();

        //  new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void Update()
    {
        // Defines the _direcionalInput to be the horizontal and vertical axis
        _directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Defines the jumping
        _jump = Input.GetButtonDown(_jumpKey);
        _notJump = Input.GetButtonUp(_jumpKey);



        // Checks if the player is colliding with the floor
        _isGliding = (!_player._playerController.collisions.below);

        // Sets the direcional input of the player
        _player.SetDirecionalInput(_directionalInput);

        // If the player is pressing on the jump key
        //  Have the player jump
        if (_jump)
        {
            _player.onJumpInputDown();
        }
        // If the player is not pressing the jump key
        //  Have the player fall
        if (_notJump)
        {
            _player.onJumpInputUp();
        }

        // If the player is in the air, and they're pressing the jump key mid air
        if (_jump && _isGliding)
        {
            // Player is gliding now
            _player.onJumpGlide();
        }
    }
}