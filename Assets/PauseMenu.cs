using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.InputSystem.Android;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public bool paused;
    public InputAction pauseAction;

    public Canvas canvas;
    public GameObject startingMenu;
    
    private List<GameObject> resetables;
    private List<Vector3> resetableValues;
    //X = transform.position.x
    //Y = transform.position.y
    //Z = type of object. 1) Block 2) Glass
    private List<Vector3> resetableAngles;

    private void Start()
    {
        resetables = new List<GameObject>();
        resetableAngles = new List<Vector3>();
        resetableValues = new List<Vector3>();
        
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Blocks");
        for (int i = 0; i < blocks.Length; i++)
        {
            resetables.Add(blocks[i]);
        }
        GameObject[] glasses = GameObject.FindGameObjectsWithTag("Platform");
        for(int i = 0;i < glasses.Length; i++)
        {
            if(glasses[i].GetComponent<BreakableGlass>() != null) resetables.Add(glasses[i]);
        }
        Debug.Log("Resetables Count: " + resetables.Count);
        int count = resetables.Count;
        for(int i = 0; i < count; i++)
        {
            resetableValues.Add(new Vector3(resetables[i].transform.position.x, resetables[i].transform.position.y, 0));
            resetableAngles.Add(resetables[i].transform.eulerAngles); 
            if (resetables[i].GetComponent<Block>() != null) resetableValues[i] += new Vector3(0, 0, 1);
            else resetableValues[i] += new Vector3(0, 0, 2);
        }
    }

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
            foreach (MenuScript menu in FindObjectsOfType<MenuScript>())
            {
                menu.gameObject.SetActive(false);
            }
            startingMenu.SetActive(true);
            //DisableInputs();
        }
        else
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            //EnableInputs();
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

    public void Retry()
    {
        Switcher switcher = GameObject.FindGameObjectWithTag("Switcher").GetComponent<Switcher>();
        GameObject player = switcher.activeCharacter == 1 ? switcher.heartObject : switcher.mindObject;
        
        player.GetComponent<DamageAndRespawn>().ForeignRespawn(0f);
        TogglePause();
    }
    
    

    
}
