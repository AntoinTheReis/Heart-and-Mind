using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;
    public Color selectedColor;
    private Color defaultColor;
    public bool selected = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        defaultColor = sr.color;
    }

    public void SelectBlock()
    {

        rb.gravityScale = 0;
        rb.angularVelocity = 1;
        selected = true;
        sr.color = selectedColor;

    }

    public void DeselectBlock()
    {
        //col.excludeLayers &= ~(1 << ignoreWhenSelected);
        //col.excludeLayers = 0;
        rb.gravityScale = 1;
        rb.angularDrag = 0.05f;
        selected = false;
        sr.color = defaultColor;
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

    private void OnCollisionStay2D(Collision2D collision)
    {

        if (collision.gameObject.layer == 9)
        {
            //Debug.Log("Block hits cloud");
            collision.collider.isTrigger = selected;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //On trigger enter here will only trigger when a deselected block enters from the outside
        if (other.gameObject.layer == 9)
        {
            //Debug.Log("BLock enters cloud");
            other.isTrigger = selected;
        }
    }
}
