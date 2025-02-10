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
    private float vertical_movement;

    public float speed = 5f;
    public float gravity = 10f;
    public float jump = 5f;

    private bool onFloor;
    private bool onWalls;


    private void Awake()
    {
        input = GetComponent<Controls>();
    }
    
    // Update is called once per frame
    void Update()
    {
        horizontal_movement = input.MoveInput().x;

        if (input.OnInteractPressed())
        {
            Debug.Log("Interact Pressed");
        }

        if (input.OnPrimaryPressed())
        {
            Debug.Log("Primary Pressed");
        }

        if(onFloor && input.OnJumpPressed())
        {
            Jump();
        }

        if (!onFloor)
        {

        }
        
        transform.position += Vector3.right * (horizontal_movement * speed * Time.deltaTime);
        transform.position += new Vector3(0, vertical_movement * Time.deltaTime, 0);
    }
    
    private void Jump()
    {
        Debug.Log("Jumped!");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Floor") onFloor = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Floor") onFloor = false;
    }

}
