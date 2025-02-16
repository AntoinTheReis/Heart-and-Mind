using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

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
    public float airDeaccelerator = 0.8f;
    public float airCruisingCap = 1f;
    private float side = 1;
    public Vector2 maxActualSpeed;

    [Header("Dash variables")]
    public float dashSpeed;
    public float dashWait = 0.3f;
    private bool isDashing = false;
    public bool canDash = true;

    [Header ("Jump variables")]
    public float jump = 5f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float maxXvelocity = 10f;

    [Header("Wall Slide & Wall Jump")]
    public float wallSticky;
    public float slideSpeed;
    public float wallAcceleration;
    public float wallJumpAir = 0.5f;
    public float wallJumpAirTime = 0.5f;
    public float wallJumpHorizontal = 1;
    public float wallJumpVertical = 1;
    private float wallTime;
    private float currentWallSpeed;
    public bool wallJumping;
    public float currentWallJumpAir;


    private bool onFloor;
    private bool onWalls;
    private float wallSide;

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

        //Deactivat wallJumping bool
        if (onFloor && !isDashing) wallJumping = false;

        //Keeping track of what side the player is facing
        if (input.MoveInput().x > 0) side = 1;
        else if (input.MoveInput().x < 0) side = -1;

        //speed changes if the player is in the air
        if (onFloor) horizontal_movement = input.MoveInput().x;
        else if (input.MoveInput().x == 0 && horizontal_movement != 0)
        {
            Deaccelerate();
        }
        else
        {
            if (!wallJumping) horizontal_movement += input.MoveInput().x * airMoveMultiplier * Time.deltaTime;
            else horizontal_movement += input.MoveInput().x * airMoveMultiplier * currentWallJumpAir * Time.deltaTime;
        }

        //maxVelocity calculations
        if (horizontal_movement > maxXvelocity) horizontal_movement = maxXvelocity;
        else if (horizontal_movement < -maxXvelocity) horizontal_movement = -maxXvelocity;

        if (input.OnInteractPressed())
        {
            Debug.Log("Interact Pressed");
        }

        //Dash Inputs
        if (input.OnPrimaryPressed() && !isDashing && canDash)
        {
            Debug.Log("Primary Pressed");
            if(input.MoveInput().x != 0 || input.MoveInput().y != 0) Dash(input.MoveInput().x, input.MoveInput().y); 
            else Dash(side, 0);
        }

        if(onFloor && input.OnJumpPressed())
        {
            Jump();
        }
        
        //Speed and position calcs
        transform.position += Vector3.right * (horizontal_movement * speed * Time.deltaTime);
        transform.position += new Vector3(0, vertical_movement * Time.deltaTime, 0);


        //Air time calculations
        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !input.OnJumpHeld())
        {
            rb.velocity += Vector2.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        //Speed limiters
        if(!isDashing && rb.velocity.x > maxActualSpeed.x) rb.velocity = new Vector2(maxActualSpeed.x, rb.velocity.y);

        //Wall Slide Calcs
        if (onWalls && !onFloor && !wallJumping)
        {
            side = wallSide * -1;
            rb.gravityScale = 0;
            if (wallTime >= wallSticky)
            {
                WallSlide();
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                wallTime += Time.deltaTime;
            }
            if (input.OnJumpPressed())
            {
                WallJump(side);
            }
        }
        else if (!isDashing)
        {
            currentWallSpeed = 0;
            wallTime = 0;
            rb.gravityScale = 5;
        }
    }

    private void Jump()
    {
        Debug.Log("Jumped!");
        rb.velocity = Vector2.up * jump;
    }

    private void WallJump(float side)
    {
        Debug.Log("Wall Jumped");
        wallJumping = true;
        DOVirtual.Float(wallJumpAir, 1, wallJumpAirTime, CurrentWallJumpAir);
        Vector2 dir = new Vector2(side * wallJumpHorizontal, wallJumpVertical);
        rb.velocity = dir * jump;
    }

    private void Deaccelerate()
    {
        if((horizontal_movement > 0 && horizontal_movement < airCruisingCap) || (horizontal_movement < 0 && horizontal_movement > -airCruisingCap))
        {
            horizontal_movement = 0;
        }
        else if (horizontal_movement > 0)
        {
            horizontal_movement -= airDeaccelerator * Time.deltaTime;
        }
        else if(horizontal_movement < 0)
        {
            horizontal_movement += airDeaccelerator * Time.deltaTime;
        }
    }

    //Platforms need to be added to the "Platforms" layer in the editor. 
    private void FloorAndWallsCheck()
    {
        onFloor = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        onWalls = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer) || Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        if (Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer)) wallSide = 1;
        else wallSide = -1;

        if (!isDashing && onFloor && !onWalls)
        {
            canDash = true;
        }
    }

    private void Dash(float x, float y)
    {
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);
        DOVirtual.Float(14, 0, .8f, RigidbodyDrag);

        rb.velocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        isDashing = true;
        canDash = false;

        yield return new WaitForSeconds(dashWait);

        rb.gravityScale = 5;
        isDashing = false;
    }

    private void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }
    private void CurrentWallJumpAir(float x)
    {
        currentWallJumpAir = x;
    }

    private void WallSlide()
    {
        if(currentWallSpeed < slideSpeed)
        {
            currentWallSpeed += wallAcceleration * Time.deltaTime;
        }
        rb.velocity = new Vector2(rb.velocity.x, -currentWallSpeed);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
    }

}
