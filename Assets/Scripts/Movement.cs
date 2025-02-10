using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controls))]
public class Movement : MonoBehaviour
{
    //Written just to see input and as a base, go cook antonio <3
    
    Controls input;

    private float horizontal_movement;
    public float speed = 5f;

    private void Awake()
    {
        input = GetComponent<Controls>();
    }
    
    // Update is called once per frame
    void Update()
    {
        horizontal_movement = input.MoveInput().x;
        if (input.OnJumpPressed())
        {
            Jump();
        }

        if (input.OnInteractPressed())
        {
            Debug.Log("Interact Pressed");
        }

        if (input.OnPrimaryPressed())
        {
            Debug.Log("Primary Pressed");
        }
        
        transform.position += Vector3.right * (horizontal_movement * speed * Time.deltaTime);
    }
    
    private void Jump()
    {
        Debug.Log("Jumped!");
    }
}
