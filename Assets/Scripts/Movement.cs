using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{

    public Controls input;

    private void Awake()
    {
        input = GetComponent<Controls>();
        if (input == null) Debug.Log("FUCKED");
        GetComponent<PlayerInput>().enabled = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (input.OnJumpPressed())
        {
            Debug.Log("WORK - Jump detected!");
            Jump();
        }
        if (input.PrimaryPressed())
        {
            Debug.Log("Primary pressed");
        }
    }

    private void Jump()
    {
        Debug.Log("Jump");
    }
}
