using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controls))]
public class MindMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Controls input;

    private float horizontal_movement;
    private float vertical_movement;

    private bool onFloor;
    private bool onWalls;
    private float wallSide;

    [Header ("Externally Manipulated")]
    public bool canMove = true;  //Mind skill conditional
    public bool turnedOn = false;  //Character switching conditional

    [Header("Miscelanious variables")]
    public float speed = 5f;
    public float airMoveMultiplier = 0.2f;
    public float airDeaccelerator = 0.8f;
    public float airCruisingCap = 1f;
    private float side = 1;
    public Vector2 maxActualSpeed;

    [Header("Jump variables")]
    public float jump = 5f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float maxXvelocity = 10f;
    public float coyoteTimeJump = 0.2f;
    private float coyoteTimeJumpCounter;
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Header("Floor and Wall Checks")]
    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    private Color debugCollisionColor = Color.red;
    public LayerMask groundLayer;

    private void Awake()
    {
        input = GetComponent<Controls>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        FloorAndWallsCheck();

        //calculates coyote time for jump
        if (onFloor) coyoteTimeJumpCounter = coyoteTimeJump;
        else coyoteTimeJumpCounter -= Time.deltaTime;

        //calculates buffer for jump press
        if (input.OnJumpPressed()) jumpBufferCounter = jumpBufferTime;
        else jumpBufferCounter -= Time.deltaTime;

        //Keeping track of what side the player is facing
        if (input.MoveInput().x > 0) side = 1;
        else if (input.MoveInput().x < 0) side = -1;

        if (onFloor)
        {
            if (onWalls && (wallSide == input.MoveInput().x)) horizontal_movement = 0;
            else if (turnedOn && canMove) horizontal_movement = input.MoveInput().x;
            else horizontal_movement = 0;
        }
        else if (input.MoveInput().x == 0 && horizontal_movement != 0)  //speed changes if the player is in the air
        {
            if (onWalls) horizontal_movement = 0;
            else Deaccelerate();
        }
        else
        {
            if (onWalls && (wallSide == input.MoveInput().x)) horizontal_movement = 0;
            else if (turnedOn && canMove)
            {
                if (!(wallSide == input.MoveInput().x && onWalls)) horizontal_movement += input.MoveInput().x * airMoveMultiplier * Time.deltaTime;
                else horizontal_movement += input.MoveInput().x * airMoveMultiplier * Time.deltaTime;
            }
        }

        //maxVelocity calculations
        if (horizontal_movement > maxXvelocity) horizontal_movement = maxXvelocity;
        else if (horizontal_movement < -maxXvelocity) horizontal_movement = -maxXvelocity;

        if (input.OnInteractPressed())
        {
            Debug.Log("Interact Pressed");
        }


        if ((coyoteTimeJumpCounter > 0) && (jumpBufferCounter > 0) && turnedOn && canMove)
        {
            Jump();
        }

        //Speed and position calcs
        transform.position += Vector3.right * (horizontal_movement * speed * Time.deltaTime);
        transform.position += new Vector3(0, vertical_movement * Time.deltaTime, 0);


        //Air time calculations
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !input.OnJumpHeld())
        {
            rb.velocity += Vector2.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        //Speed limiters
        if ((rb.velocity.x > maxActualSpeed.x)) rb.velocity = new Vector2(maxActualSpeed.x, rb.velocity.y);
        else if ((rb.velocity.x < -(maxActualSpeed.x))) rb.velocity = new Vector2(-(maxActualSpeed.x), rb.velocity.y);
    }

    private void Jump()
    {
        Debug.Log("Jumped!");
        rb.velocity = Vector2.up * jump;
        coyoteTimeJumpCounter = 0;
        jumpBufferCounter = 0;
    }

    private void Deaccelerate()
    {
        if ((horizontal_movement > 0 && horizontal_movement < airCruisingCap) || (horizontal_movement < 0 && horizontal_movement > -airCruisingCap))
        {
            horizontal_movement = 0;
        }
        else if (horizontal_movement > 0)
        {
            horizontal_movement -= airDeaccelerator * Time.deltaTime;
        }
        else if (horizontal_movement < 0)
        {
            horizontal_movement += airDeaccelerator * Time.deltaTime;
        }
    }

    private void FloorAndWallsCheck()
    {
        onFloor = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        onWalls = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer) || Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);

        if (Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer)) wallSide = 1;
        else if (Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer)) wallSide = -1;
        else wallSide = 0;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
    }
    public void ZeroMovement()
    {
        horizontal_movement = 0;
        vertical_movement = 0;
        rb.velocity = Vector2.zero;
    }

}