using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controls : MonoBehaviour
{
    [SerializeField] PlayerInput input;

    private void Awake()
    {
        input.actions.Enable();  // Ensure input actions are enabled
        Debug.Log("PlayerInput enabled: " + input.enabled);
    }

    public Vector2 MoveInput() => input.actions["Move"].ReadValue<Vector2>();
    /// <summary>
    /// Returns true during the frame the 'jump' input is pressed
    /// </summary>
    public bool OnJumpPressed() => input.actions["Jump"].triggered;
    /// <summary>
    /// Returns true during the frame the 'jump' input is released
    /// </summary>
    public bool OnJumpReleased() => input.actions["Jump"].WasReleasedThisFrame();


    /// <summary>
    /// Returns true every frame that the 'primary' input is pressed
    /// </summary>
    public bool PrimaryPressed() => input.actions["Primary"].IsPressed();
    /// <summary>
    /// Returns true during the frame the 'primary' input is pressed
    /// </summary>
    public bool OnPrimaryPressed() => input.actions["Primary"].triggered;
    /// <summary>
    /// Returns true on the frame the 'primary' input is released
    /// </summary>
    public bool OnPrimaryReleased() => input.actions["Primary"].WasReleasedThisFrame();
    
    /// <summary>
    /// Returns true every frame that the 'interact' input is pressed
    /// </summary>
    public bool InteractPressed() => input.actions["Interact"].IsPressed();
    /// <summary>
    /// Returns true every frame that the 'interact' input is pressed
    /// </summary>
    public bool OnInteractPressed() => input.actions["Interact"].triggered;
    
}
