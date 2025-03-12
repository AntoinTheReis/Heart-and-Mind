using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;
    public LayerMask ignoreWhenSelected;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void SelectBlock()
    {
        rb.gravityScale = 0;
        rb.angularVelocity = 1;
        col.excludeLayers = ignoreWhenSelected;

    }

    public void DeselectBlock()
    {
        col.excludeLayers = new LayerMask();
        rb.gravityScale = 1;
        rb.angularDrag = 0.05f;
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
