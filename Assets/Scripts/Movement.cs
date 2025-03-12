using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using FMODUnity;

[RequireComponent(typeof(Controls))]
public class Movement : MonoBehaviour
{
    //Written just to see input and as a base, go cook antonio <3
    
    Controls input;
    Rigidbody2D rb;
    DamageAndRespawn respawn;

    private float horizontal_movement;
    private float vertical_movement;

    public bool onFloor;
    private bool onWalls;
    private float wallSide;

    [Header("Externally Manipulated")]
    public bool turnedOn = false;  //Character switching conditional

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
    public float dragDashDuration = 0.8f;
    public float dragDashMax = 14f;
    private bool isDashing = false;
    private bool canDash = true;

    [Header ("Jump variables")]
    public float jump = 5f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float maxXvelocity = 10f;
    public float coyoteTimeJump = 0.2f;
    private float coyoteTimeJumpCounter;
    public float jumpBufferTime = 0.2f;
    public float jumpBufferCounter;

    [Header("Wall Slide & Wall Jump")]
    public float wallSticky;
    public float slideSpeed;
    public float wallAcceleration;
    public float wallJumpAir = 0.5f;
    public float wallJumpAirTime = 0.5f;
    public float wallJumpHorizontal = 1;
    public float wallJumpVertical = 1;
    public float coyoteTimeWall = 0.2f;
    private float coyoteTimeWallCounter;
    private float wallTime;
    private float currentWallSpeed;
    private bool wallJumping;
    private float currentWallJumpAir;
    private bool leftWall;   //as in leaving, not as in the side

    [Header("Floor and Wall Checks")]
    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    private Color debugCollisionColor = Color.red;
    public LayerMask groundLayer;


    #region Audio
    public FMODUnity.EventReference sfx_jump;
    FMOD.Studio.EventInstance sfx_jumpInstance;

    public FMODUnity.EventReference sfx_dialogue;
    FMOD.Studio.EventInstance sfx_dialogueInstance;
    FMOD.Studio.PARAMETER_ID sfx_dialogueCharacter;
    public int charVoice;
    #endregion

    private void Awake()
    {
        input = GetComponent<Controls>();
        rb = GetComponent<Rigidbody2D>();
        respawn = GetComponent<DamageAndRespawn>();

        #region Audio EventInstances
        sfx_jumpInstance = FMODUnity.RuntimeManager.CreateInstance(sfx_jump);

        sfx_dialogueInstance = FMODUnity.RuntimeManager.CreateInstance(sfx_dialogue);
        FMOD.Studio.EventDescription dialogueDescription;
        sfx_dialogueInstance.getDescription(out dialogueDescription);
        FMOD.Studio.PARAMETER_DESCRIPTION dialogueParameterDescription;
        dialogueDescription.getParameterDescriptionByName("H1M2", out dialogueParameterDescription);
        sfx_dialogueCharacter = dialogueParameterDescription.id;
        sfx_dialogueInstance.setParameterByID(sfx_dialogueCharacter, 1);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {

        FloorAndWallsCheck();

        //Deactivat wallJumping bool
        if (onFloor && !isDashing) wallJumping = false;

        //calculates coyote time for jump
        if (onFloor) coyoteTimeJumpCounter = coyoteTimeJump;
        else coyoteTimeJumpCounter -= Time.deltaTime;

        //calculates buffer for jump press
        if (input.OnJumpPressed()) jumpBufferCounter = jumpBufferTime;
        else jumpBufferCounter -= Time.deltaTime;

        //Keeping track of what side the player is facing
        if (input.MoveInput().x > 0) side = 1;
        else if (input.MoveInput().x < 0) side = -1;

        if (!isDashing) //Player can only influence movement with WASD if not Dashing
        {
            if (onFloor)
            {
                currentWallSpeed = 0;
                if (onWalls && ((wallSide == 1 && input.MoveInput().x > 0) || (wallSide == -1 && input.MoveInput().x < 0)))
                {
                    Debug.Log("Horizontal input: " + input.MoveInput().x + "| making horizontal movement 0");
                    horizontal_movement = 0;
                }
                else if (turnedOn)
                {
                    if (input.MoveInput().x > 0) horizontal_movement = 1;
                    else if (input.MoveInput().x < 0) horizontal_movement -= 1;
                    else horizontal_movement = 0;
                }
                else horizontal_movement = 0;
            }
            else if (input.MoveInput().x == 0 && horizontal_movement != 0)  //speed changes if the player is in the air
            {
                if (onWalls && !wallJumping) horizontal_movement = 0;
                else Deaccelerate();
            }
            else
            {
                if (onWalls && ((wallSide == 1 && input.MoveInput().x > 0) || (wallSide == -1 && input.MoveInput().x < 0)))
                {
                    Debug.Log("Horizontal input: " + input.MoveInput().x + "| making horizontal movement 0");
                    horizontal_movement = 0;
                }
                else if (turnedOn)
                {
                    if (!wallJumping) horizontal_movement += input.MoveInput().x * airMoveMultiplier * Time.deltaTime;
                    else if (!(wallSide == input.MoveInput().x && onWalls)) horizontal_movement += input.MoveInput().x * airMoveMultiplier * currentWallJumpAir * Time.deltaTime;
                }
            }
        }

        //maxVelocity calculations
        if (horizontal_movement > maxXvelocity) horizontal_movement = maxXvelocity;
        else if (horizontal_movement < -maxXvelocity) horizontal_movement = -maxXvelocity;

        if (input.OnInteractPressed())
        {

        }

        //Dash Inputs
        if (input.OnPrimaryPressed() && !isDashing && canDash && turnedOn)
        {
            if(input.MoveInput().x != 0 || input.MoveInput().y != 0) Dash(input.MoveInput().x, input.MoveInput().y); 
            else Dash(side, 0);
        }

        if((coyoteTimeJumpCounter > 0) && (jumpBufferCounter > 0) && turnedOn)
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
        else if (rb.velocity.y > 0 && !input.OnJumpHeld() && !isDashing)
        {
            rb.velocity += Vector2.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        //Speed limiters
        if(!isDashing && (rb.velocity.x > maxActualSpeed.x)) rb.velocity = new Vector2(maxActualSpeed.x, rb.velocity.y);
        else if(!isDashing && (rb.velocity.x < -(maxActualSpeed.x))) rb.velocity = new Vector2(-(maxActualSpeed.x), rb.velocity.y);

        //Wall Slide Calcs
        if (onWalls && !onFloor && (!wallJumping || leftWall))
        {
            wallJumping = false;
            coyoteTimeWallCounter = coyoteTimeWall;
            side = wallSide * -1;
            if(input.MoveInput().x == wallSide)
            {
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
            }
            else if(!respawn.respawning)
            {
                rb.gravityScale = 5;
            }
        } 
        else if (!isDashing)
        {
            coyoteTimeWallCounter -= Time.deltaTime;
            currentWallSpeed = 0;
            wallTime = 0;
            if (!respawn.respawning) rb.gravityScale = 5;
        }

        //wall jumping with coyote time
        if ((jumpBufferCounter > 0) && (coyoteTimeWallCounter > 0) && turnedOn) WallJump(side);

        //A check to make consecutive walljumps possible
        leftWall = !onWalls && wallJumping && !onFloor;

        if (isDashing && rb.velocity == Vector2.zero) DashCancel();  //Cancel dash effects if player is still (hit a wall or floor)
    }

    private void Jump()
    {
        #region Jump Audio
        if (sfx_jumpInstance.isValid())
        {
            FMOD.Studio.PLAYBACK_STATE playbackstate;
            sfx_jumpInstance.getPlaybackState(out playbackstate);
            if (playbackstate == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                sfx_jumpInstance.start();
            }
        }
        //if (sfx_dialogueInstance.isValid())
        //{
        //    FMOD.Studio.PLAYBACK_STATE playbackstate;
        //    sfx_dialogueInstance.getPlaybackState(out playbackstate);
        //    if (playbackstate == FMOD.Studio.PLAYBACK_STATE.STOPPED)
        //    {
        //        sfx_dialogueInstance.start();
        //    }
        //}
        #endregion

        rb.velocity = Vector2.up * jump;
        coyoteTimeJumpCounter = 0;
        jumpBufferCounter = 0;
        DashCancel();
    }

    private void WallJump(float side)
    {
        wallJumping = true;
        DOVirtual.Float(wallJumpAir, 1, wallJumpAirTime, CurrentWallJumpAir);
        Vector2 dir = new Vector2(Mathf.Sign(side) * wallJumpHorizontal, wallJumpVertical);
        rb.velocity = Vector2.zero; 
        rb.velocity = dir * jump;
        rb.AddForce(dir * jump);
        coyoteTimeWallCounter = 0;
        jumpBufferCounter = 0;
        DashCancel();

        //Debug.Log($"WallJump - Side: {side}, Dir: {dir}, Jump: {jump}, Velocity Before: {rb.velocity}");
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
        else if (Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer)) wallSide = -1;
        else wallSide = 0;

        if (!isDashing && onFloor && !onWalls)
        {
            canDash = true;
        }
    }

    private void Dash(float x, float y)
    {
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        horizontal_movement = 0;
        Vector2 dir = new Vector2(x, y);
        DOVirtual.Float(dragDashMax, 0, dragDashDuration, RigidbodyDrag);

        rb.velocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        isDashing = true;
        canDash = false;

        yield return new WaitForSeconds(dashWait);

        if (!respawn.respawning) rb.gravityScale = 5;
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

    private void DashCancel()
    {
        isDashing = false;
        if (!respawn.respawning) rb.gravityScale = 5;
    }

    public void ZeroMovement()
    {
        horizontal_movement = 0;
        vertical_movement = 0;
        rb.velocity = Vector2.zero;
    }

}
