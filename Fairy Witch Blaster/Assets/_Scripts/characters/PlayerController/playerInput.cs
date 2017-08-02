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
    }

}