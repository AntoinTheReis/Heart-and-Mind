using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class DialogueRunnerEvents : MonoBehaviour
{
    private UnityAction DialogueStart;
    private UnityAction DialogueEnd;
    
    // Start is called before the first frame update
    void Start()
    {
        //Must set Dialogue Start and Dialogue End events here, so that they dont have to be reassigned every time we drag the prefab in for testing
        //having it sit in the prefab doesnt work since the reference to the player scripts for disabling input are references to them in the scene
        //and project view objects can't have reference to things in a scene, so theyre empty references

        
        DialogueRunner runner = GetComponent<DialogueRunner>();
        CameraSystem cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraSystem>();

        DialogueStart += DisableInputs;
        DialogueEnd += EnableInputs;
        DialogueEnd += cam.resetCamSize;
        
        runner.onDialogueStart.AddListener(DialogueStart);
        runner.onDialogueComplete.AddListener(DialogueEnd);
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
