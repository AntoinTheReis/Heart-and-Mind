using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class Controls : MonoBehaviour
{
    [SerializeField] PlayerInput input;

    private bool disabled = false;


    public void DisableInput()
    {
        disabled = true;
    }

    public void EnableInput()
    {
        disabled = false;
    }
    
    private void Awake()
    {
        InputUser.PerformPairingWithDevice(Keyboard.current, input.user, InputUserPairingOptions.None);  //forces keybaord to the playerInput

        input.actions.Enable();  // Ensure input actions are enabled
        Debug.Log("PlayerInput enabled: " + input.enabled);
    }

    public Vector2 MoveInput() => disabled ? Vector2.zero : input.actions["Move"].ReadValue<Vector2>();
    /// <summary>
    /// Returns true during the frame the 'jump' input is pressed
    /// </summary>
    public bool OnJumpPressed() => disabled ? false : input.actions["Jump"].triggered;
    /// <summary>
    /// Returns true during the frame the 'jump' input is released
    /// </summary>
    public bool OnJumpReleased() => disabled ? false : input.actions["Jump"].WasReleasedThisFrame();

    public bool OnJumpHeld() => disabled ? false : input.actions["Jump"].IsPressed();


    /// <summary>
    /// Returns true every frame that the 'primary' input is pressed
    /// </summary>
    public bool PrimaryPressed() => disabled ? false : input.actions["Primary"].IsPressed();
    /// <summary>
    /// Returns true during the frame the 'primary' input is pressed
    /// </summary>
    public bool OnPrimaryPressed() => disabled ? false : input.actions["Primary"].triggered;
    /// <summary>
    /// Returns true on the frame the 'primary' input is released
    /// </summary>
    public bool OnPrimaryReleased() => disabled ? false : input.actions["Primary"].WasReleasedThisFrame();
    
    /// <summary>
    /// Returns true every frame that the 'interact' input is pressed
    /// </summary>
    public bool InteractPressed() => disabled ? false : input.actions["Interact"].IsPressed();
    /// <summary>
    /// Returns true every frame that the 'interact' input is pressed
    /// </summary>
    public bool OnInteractPressed() => disabled ? false : input.actions["Interact"].triggered;
    
}
