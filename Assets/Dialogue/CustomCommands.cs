using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn;
using Yarn.Unity;

public class CustomCommands : MonoBehaviour
{
    private DialogueRunner runner;

    private void Awake()
    {
        CameraSystem cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraSystem>();
        runner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        
        //no custom commands implemented yet but this is where we'd put them using runner.AddCommandHandler()
        //https://docs.yarnspinner.dev/using-yarnspinner-with-unity/creating-commands-functions
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
