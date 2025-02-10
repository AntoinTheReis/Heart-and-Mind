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
    Rigidbody2D rb;

    private float horizontal_movement;
    private float vertical_movement;

    [Header ("Miscelanious variables")]
    public float speed = 5f;
    public float airMoveMultiplier = 0.2f;

    [Header ("Jump variables")]
    public float jump = 5f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float maxXvelocity = 10f;

    private bool onFloor;
    private bool onWalls;


    private void Awake()
    {
        input = GetComponent<Controls>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (onFloor) horizontal_movement = input.MoveInput().x;
        else horizontal_movement += input.MoveInput().x * airMoveMultiplier;

        if (horizontal_movement > maxXvelocity) horizontal_movement = maxXvelocity;
        else if (horizontal_movement < -maxXvelocity) horizontal_movement = -maxXvelocity;

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
        
        transform.position += Vector3.right * (horizontal_movement * speed * Time.deltaTime);
        transform.position += new Vector3(0, vertical_movement * Time.deltaTime, 0);

        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !input.OnJumpHeld())
        {
            rb.velocity += Vector2.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void Jump()
    {
        Debug.Log("Jumped!");
        rb.velocity = Vector2.up * jump;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Platform")
        {
            onFloor = true;
            vertical_movement = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Platform") onFloor = false;
    }

}
