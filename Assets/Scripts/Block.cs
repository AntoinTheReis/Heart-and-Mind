using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public Color selectedColor;
    private Color defaultColor;
    public bool selected = false;

    public Collider2D collider;
    public Collider2D trigger;

    public LayerMask excludeWhenSelected;
    List<Collider2D> overlappingColliders;
    
    private Vector3 startPoint;

    private float previous_y;
    private float delta_y;

    private void OnDrawGizmos()
    {
        
    }

    //this is the most unoptimized pile of dogshit iv ever written but it works so well

    private void Update()
    {
        delta_y = transform.position.y - previous_y;

        previous_y = transform.position.y;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        defaultColor = sr.color;
        startPoint = gameObject.transform.position;
        previous_y = startPoint.y;

        overlappingColliders = new List<Collider2D>();

    }

    public void SelectBlock()
    {   
        rb.gravityScale = 0;
        rb.angularVelocity = 1;
        selected = true;
        sr.color = selectedColor;

        collider.excludeLayers = excludeWhenSelected;
    }

    public void DeselectBlock()
    {
        rb.gravityScale = 1;
        rb.angularDrag = 0.05f;
        selected = false;
        sr.color = defaultColor;

        collider.excludeLayers &= ~(excludeWhenSelected); //im so bit pilled, and not core
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.layerMask = excludeWhenSelected;
        trigger.OverlapCollider(contactFilter, overlappingColliders);
        Debug.Log(contactFilter);
        foreach (Collider2D col in overlappingColliders)
        {
            if (excludeWhenSelected == (excludeWhenSelected | (1 << col.gameObject.layer)))
            {
                if(col.gameObject.layer == 6 || transform.position.y < col.transform.position.y)
                    Physics2D.IgnoreCollision(collider, col);
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case 6: //player
                Physics2D.IgnoreCollision(collider, collision.collider, transform.position.y > collision.transform.position.y && delta_y <= 0 && collision.gameObject.GetComponent<Movement>().onFloor);
                return;

            case 9: //cloud
                Physics2D.IgnoreCollision(collider, collision.collider, transform.position.y < collision.transform.position.y);
                return;


            default:
                break;
        }

        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Death")
        {
            Debug.Log("Respawning block");
            transform.position = startPoint;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if(Physics2D.GetIgnoreCollision(collider, collision))
        {
            Physics2D.IgnoreCollision(collider, collision, false);
        }
    }

    private void OnBecameVisible()
    {
        Debug.Log("Added block to list!");
        BlockTracker.BlocksOnScreen.AddLast(gameObject);
    }

    void OnBecameInvisible()
    {
        Debug.Log("Removed block to list!");
        BlockTracker.BlocksOnScreen.Remove(gameObject);
    }

    public bool IsOffScreen()
    {
        return !GetComponent<Renderer>().isVisible;
    }

}
