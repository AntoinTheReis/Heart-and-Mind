using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{

    public float timeToDecay = 1f;
    public float timeToReset = 3f;
    public float colorFade = 0.2f;
    public float onFlooorCheckWait = 0.1f;
    public float speed;
    public float amount;

    private bool decaying;
    private bool alive;
    private bool disappeared;

    public Transform spriteTransform;
    private SpriteRenderer spriteRenderer;
    private Vector2 startPos;
    private Collider2D collider2d;
    private GameObject targetPlayer;

    [Header("Overlap Check")]
    private bool overlappingWithPlayer;
    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    private Color debugCollisionColor = Color.black;
    public LayerMask playerLayer;

    private bool wantToReturn = false;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        collider2d = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Shaking animation  -- Fucks up positioning
        /*if (decaying)
        {
            spriteTransform.position = new Vector2(startPos.x + Mathf.Sin(Time.time * speed) * amount, startPos.y + (Mathf.Sin(Time.time * speed) * amount));
        }*/

        OverlapCheck();

        if (wantToReturn && !overlappingWithPlayer)
        {
            Color tmp = spriteRenderer.color;
            tmp.a = 1;
            spriteRenderer.color = tmp;

            collider2d.enabled = true;

            wantToReturn = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided and it's a " + collision.gameObject.name);
        targetPlayer = collision.gameObject;

        if (!decaying)
        {
            Debug.Log("Decaying");
            if (collision.gameObject.name == "Heart Player")
            {
                //Debug.Log("Got a heart");
                StartCoroutine(HeartCheck());
            }
            else if (collision.gameObject.name == "Mind Player")
            {
                //Debug.Log("Got a mind");
                StartCoroutine(MindCheck());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            StartCoroutine(Disappear());
        }
    }

    IEnumerator DecayStart()
    {
        startPos.x = 0;
        startPos.y = 0;
        decaying = true;

        yield return new WaitForSecondsRealtime(timeToDecay);
        if(decaying) StartCoroutine(Disappear());
    }

    IEnumerator Disappear()
    {
        collider2d.enabled = false;
        decaying = false;
        DOVirtual.Float(1, 0, colorFade, DashColor);

        yield return new WaitForSecondsRealtime(timeToReset);
        wantToReturn = true;
    }

    IEnumerator HeartCheck()
    {
        yield return new WaitForSecondsRealtime(onFlooorCheckWait);
        if (targetPlayer.GetComponent<Movement>().onFloor)
        {
            StartCoroutine(DecayStart());
            //Debug.Log("Was on floor");
        }
        //else Debug.Log("Not on floor");
    }

    IEnumerator MindCheck()
    {
        yield return new WaitForSecondsRealtime(onFlooorCheckWait);
        if (targetPlayer.GetComponent<MindMovement>().onFloor)
        {
            StartCoroutine(DecayStart());
            //Debug.Log("Was on floor");
        }
        //else Debug.Log("Not on floor");

    }

    private void DashColor(float alpha)
    {
        Color tmp = spriteRenderer.color;
        tmp.a = alpha;
        spriteRenderer.color = tmp;
    }

    IEnumerator LeavePlatform(GameObject collision)
    {
        yield return new WaitForSecondsRealtime(onFlooorCheckWait * 2);
        if ((collision.name == "Heart Player") || (collision.name == "Mind Player"))
        {
            Debug.Log("Decaying: " + decaying);
            if (decaying) StartCoroutine(Disappear());   // !! Need to make sure it's already decaying to make it so that walljumps won't kill it, but it aint that easy! !!
        }
    }

    private void OverlapCheck()
    {
        overlappingWithPlayer = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, playerLayer) || Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, playerLayer) || Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, playerLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
    }

}
