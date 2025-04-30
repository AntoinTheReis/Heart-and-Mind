using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public bool paused = true;
    public InputAction pauseAction;

    public Canvas canvas;

    private void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.performed += OnPauseActionPerformed;
        TogglePause();
    }

    private void OnPauseActionPerformed(InputAction.CallbackContext context)
    {
        TogglePause();
    }
    
    public void TogglePause()
    {
        paused = !paused;
        Time.timeScale = paused ? 1 : 0;
        canvas.enabled = !paused;
        
        if(!paused) DisableInputs(); else EnableInputs();
    }
    
    void DisableInputs()
    {
        Controls[] inputs = GameObject.FindObjectsByType<Controls>(FindObjectsSortMode.None);
        foreach (Controls input in inputs)
        {
            input.DisableInput();
        }
    }

    void EnableInputs()
    {
        Controls[] inputs = GameObject.FindObjectsByType<Controls>(FindObjectsSortMode.None);
        foreach (Controls input in inputs)
        {
            input.EnableInput();
        }
    }
    
}
