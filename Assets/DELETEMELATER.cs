using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DELETEMELATER : MonoBehaviour
{
    public GameObject heart;
    private bool done = false;
    private void Start()
    {
        
    }

    private void LateUpdate()
    {
        if (!done) heart.transform.position = transform.position;
        
        if(Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.E)) done = true;
    }
}
