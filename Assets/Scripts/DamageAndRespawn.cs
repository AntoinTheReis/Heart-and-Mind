using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DamageAndRespawn : MonoBehaviour
{
    public float damagePush;
    public float respawnTime;
    public float materializationTime = 0.2f;
    public bool respawning = false;
    public float dragDashMax;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private float heartOrMind;   //heart is 1, mind is 2
    private Movement movement;
    private MindMovement mMovement;
    private Collider2D collider2d;
    private Room lastActualRoom;

    // Start is called before the first frame update
    void Start()
    {
        collider2d = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if(gameObject.name == "Heart Player")
        {
            movement = GetComponent<Movement>();
            heartOrMind = 1;
        }
        else
        {
            mMovement = GetComponent<MindMovement>();
            heartOrMind = 2;
        }
    }

    private void Update()
    {
        if (RoomTracker.current_room != null && RoomTracker.current_room != lastActualRoom)
        {
            lastActualRoom = RoomTracker.current_room;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Damage")  //For spikes and stuff !Death with knockback!
        {
            if (heartOrMind == 1)
            {
                movement.ZeroMovement();
                movement.enabled = false;
            }
            else
            {
                mMovement.ZeroMovement();
                mMovement.enabled = false;
                GetComponent<MindBlockSpawning>().enabled = false;
            }
            respawning = true;
            rb.gravityScale = 0;
            rb.drag = 0;
            //collider2d.enabled = false;

            // Calculate Angle Between the collision point and the player
            Vector2 dir = collision.contacts[0].point - new Vector2(transform.position.x, transform.position.y);
            dir = -dir.normalized;

            DOVirtual.Float(dragDashMax, 0, respawnTime, RigidbodyDrag);
            rb.AddForce(dir * damagePush);

            StartCoroutine(Respawn());
        }

        if(collision.gameObject.tag == "Death")  //For deaths without knockback, such as falling in a pit
        {
            if (heartOrMind == 1)
            {
                movement.ZeroMovement();
                movement.enabled = false;
            }
            else
            {
                mMovement.ZeroMovement();
                mMovement.enabled = false;
                GetComponent<MindBlockSpawning>().enabled = false;
            }
            respawning = true;
            rb.gravityScale = 0;
            rb.drag = 0;

            StartCoroutine(Respawn());
        }

    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        rb.velocity = Vector2.zero;
        Color tmp = spriteRenderer.color;   //Setting color to transparent
        tmp.a = 0f;
        spriteRenderer.color = tmp;

        if(RoomTracker.current_room !=  null) transform.position = RoomTracker.current_room.checkpoint.position;  //Have to set the checkpoint/respawn point in the unity editor.
        else transform.position = lastActualRoom.checkpoint.position;

        DOVirtual.Float(0, 1, materializationTime, SpriteAlpha);
    }

    private void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    private void SpriteAlpha(float alpha)
    {
        Color tmp = spriteRenderer.color;
        tmp.a = alpha;
        spriteRenderer.color = tmp;

        if (alpha == 1)
        {
            if (heartOrMind == 1) movement.enabled = true;
            else
            {
                mMovement.enabled = true;
                GetComponent<MindBlockSpawning>().enabled = true;
            }
            respawning = false;
            rb.gravityScale = 5;
            //collider2d.enabled = true;
        }
    }

}
