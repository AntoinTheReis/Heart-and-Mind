using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public bool paused;
    public InputAction pauseAction;

    public Canvas canvas;
    public Button startingButton;

    private void OnEnable()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
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
        Time.timeScale = paused ? 0 : 1;
        canvas.enabled = paused;

        if (paused)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(startingButton.gameObject);
            DisableInputs();
        }
        else
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            EnableInputs();
        }
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

    public void setSelectedButton()
    {

    }

    
}
