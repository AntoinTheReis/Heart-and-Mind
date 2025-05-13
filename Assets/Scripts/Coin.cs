using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{

    TransitCoin coinManager;
    SpriteRenderer spriteRenderer;
    Animator animator;
    Collider2D col;

    // Start is called before the first frame update
    void Start()
    {
        coinManager = GameObject.FindGameObjectWithTag("CoinManager").GetComponent<TransitCoin>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        coinManager.CoinGot(gameObject);
    }

    public void TurnOff()
    {
        spriteRenderer.enabled = false;
        col.enabled = false;
    }


}
