using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public float lingerDuration;
    public float lingerFlashingFrequency;
    public float heavyBlockMassThreshold = 999;
    
    private Vector3 startPoint;
    private bool hasMovedDuringSelection;

    public LayerMask groundLayer;

    private bool lingering;

    private Room lastActualRoom;

    private Animator curtain;

    public bool heavy;

    #region Audio
    public FMODUnity.EventReference select;
    FMOD.Studio.EventInstance sfx_selectInstance;
    public FMODUnity.EventReference deselect;
    FMOD.Studio.EventInstance sfx_deselectInstance;
    #endregion

    //this is the most unoptimized pile of dogshit iv ever written but it works so well
    //so real past me

    private void Start()
    {
        #region Audio EventInstances
        sfx_selectInstance = FMODUnity.RuntimeManager.CreateInstance(select);
        sfx_deselectInstance = FMODUnity.RuntimeManager.CreateInstance(deselect);
        #endregion

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        defaultColor = sr.color;
        startPoint = gameObject.transform.position;

        overlappingColliders = new List<Collider2D>();


        curtain = GameObject.FindGameObjectWithTag("DeathCurtain").GetComponent<Animator>();
    }

    private void Update()
    {
        if(RoomTracker.current_room != null) lastActualRoom = RoomTracker.current_room;

    }

    public Block SelectBlock()
    {
        #region Block Select Audio
        if (sfx_selectInstance.isValid())
        {
            sfx_selectInstance.start();
        }
        #endregion

        StopAllCoroutines();
        lingering = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 0;
        rb.angularVelocity = 1;
        selected = true;
        sr.color = selectedColor;

        return this;
    }

    //will be called if the block is moved due to telekenisis (lets the mind cycle between blocks without the player falling through)
    public void OnMove()
    {
        collider.excludeLayers = excludeWhenSelected;
    }

    public bool IsUnderneathGround()
    {
        Bounds b = collider.bounds;
        return (Physics2D.OverlapBox(new Vector2(b.center.x, b.max.y + 0.1f), new Vector2(b.size.x, 0.05f), 0f, groundLayer));
    }

    public Block DropBlock()
    {
        StartCoroutine(BlockLinger());
        return DeselectBlock();
    }
    
    Block DropAndWait(Block block, float lingerTime)
    {
        StartCoroutine(BlockLinger());
        return block.DeselectBlock();
    }
    IEnumerator BlockLinger()
    {
        //Won't linger if grounded
        Bounds b = collider.bounds;
        if(Physics2D.OverlapBox(new Vector2(b.center.x, b.min.y - 0.1f), new Vector2(b.size.x, 0.05f), 0f,groundLayer)) yield break;
        lingering = true;
        
        //freeze block in place
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        
        //fuck `yield return new WaitForSeconds(seconds)`, we're real men
        for (float i = 0; i < lingerDuration; i+= Time.deltaTime)
        {
            Color c = sr.color;
            if (Mathf.Floor(i * lingerFlashingFrequency) % 2 == 0)
                c.a = 0.8f;
            else c.a = 1;
            sr.color = c; //have to do this since sr returns color by value not reference. "Property 'color' access returns temporary value. Cannot modify struct member when accessed struct is not classified as a variable"
            
            yield return null;
        }
        //unfreeze block
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        lingering = false;
        //I FOUND A BUG IN THE UNITY PHYSICS ENGINE!!! That's like, a boyscout badge, right?
        rb.gravityScale = 1.01f; //So I have to put this here to update the physics object, since it stays in sleep for some reason otherwise. Can't even just set it to 1f.
    }

    private Block DeselectBlock()
    {
        #region Block Select Audio
        if (sfx_deselectInstance.isValid())
        {
            sfx_deselectInstance.start();
        }
        #endregion

        rb.gravityScale = 1;
        rb.angularDrag = 0.05f;
        selected = false;
        sr.color = defaultColor;

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.layerMask = collider.excludeLayers;
        trigger.OverlapCollider(contactFilter, overlappingColliders);
        Debug.Log(contactFilter);
        foreach (Collider2D col in overlappingColliders)
        {
            if (collider.excludeLayers == (collider.excludeLayers | (1 << col.gameObject.layer)))
            {
                //ignore if block is below overlap, or you're the player
                if(transform.position.y < col.transform.position.y || col.gameObject.layer == 6)
                    Physics2D.IgnoreCollision(collider, col);
            }
        }
        
        collider.excludeLayers &= ~(collider.excludeLayers); //set excludeLayers to nothing in the bit-pilled way

        return this;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case 6: //player: ignore collision if the bottom of the block is above the top of the player
                Physics2D.IgnoreCollision(collider, collision.collider, collider.bounds.min.y > collision.collider.bounds.max.y);// && delta_y <= 0 && collision.gameObject.GetComponent<Movement>().onFloor);
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
            if(gameObject.name == "Player Block")
            {
                StartCoroutine(PlayerBlockRespawn());
            }
            else
            {
                Debug.Log("Respawning block");
                transform.position = startPoint;
            }
        }

        if(heavy && rb.velocity == Vector2.zero && !selected)
        {
            if ((collision.gameObject.name == "Player Block" || collision.gameObject.tag == "Player")) rb.bodyType = RigidbodyType2D.Static;  //Heavy block cannot be pushed by players
            else if (collision.gameObject.tag == "Blocks" && !collision.gameObject.GetComponent<Block>().heavy) rb.bodyType = RigidbodyType2D.Static;  ////Heavy block cannot be pushed by light block
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(heavy && rb.velocity == Vector2.zero && !selected)
        {
            if ((collision.gameObject.name == "Player Block" || collision.gameObject.tag == "Player")) rb.bodyType = RigidbodyType2D.Dynamic;  //Heavy block cannot be pushed by players
            else if (collision.gameObject.tag == "Blocks" && !collision.gameObject.GetComponent<Block>().heavy) rb.bodyType = RigidbodyType2D.Dynamic;  ////Heavy block cannot be pushed by light block
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {

        /*if(collision.gameObject.GetComponent<Room>() != null)
        {
            Debug.Log("Block exited a room");
            transform.position = startPoint;
        }*/

        if(Physics2D.GetIgnoreCollision(collider, collision))
        {
            Physics2D.IgnoreCollision(collider, collision, false);
        }

        if(rb.mass < heavyBlockMassThreshold && collision.gameObject.tag == "Blocks")
        {

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

    IEnumerator PlayerBlockRespawn()
    {
        curtain.SetTrigger("Died");
        yield return new WaitForSecondsRealtime(0.2f);
        transform.position = lastActualRoom.checkpoint.position;
    }

    private void OnDrawGizmos()
    {

    }

}
