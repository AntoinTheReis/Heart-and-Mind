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

    private bool onFloor;
    private bool onWalls;

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

        if (onFloor) horizontal_movement = input.MoveInput().x;
        else if (input.MoveInput().x == 0 && horizontal_movement != 0)
        {
            Deaccelerate();
        }
        else
        {
            horizontal_movement += input.MoveInput().x * airMoveMultiplier * Time.deltaTime;
        }

        if (horizontal_movement > maxXvelocity) horizontal_movement = maxXvelocity;
        else if (horizontal_movement < -maxXvelocity) horizontal_movement = -maxXvelocity;

        if (input.OnInteractPressed())
        {
            Debug.Log("Interact Pressed");
        }

        if (input.OnPrimaryPressed() && !isDashing && canDash)
        {
            Debug.Log("Primary Pressed");
            if(input.MoveInput().x != 0 || input.MoveInput().y != 0) Dash(input.MoveInput().x, input.MoveInput().y); 
            else Dash(1, 0);
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
        if(!isDashing && rb.velocity.x > maxActualSpeed.x) rb.velocity = new Vector2(maxActualSpeed.x, rb.velocity.y);
        if (!isDashing && rb.velocity.x > maxActualSpeed.x) rb.velocity = new Vector2(maxActualSpeed.x, rb.velocity.y);
    }

    private void Jump()
    {
        Debug.Log("Jumped!");
        rb.velocity = Vector2.up * jump;
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


    //Platforms need to be added to the "Platforms" layer in the editor. 
    private void FloorAndWallsCheck()
    {
        onFloor = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        onWalls = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer) || Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        if (!isDashing && onFloor)
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
    }

}
