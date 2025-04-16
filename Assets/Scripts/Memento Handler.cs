using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MementoHandler : MonoBehaviour
{

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Player Block")
        {
            Debug.Log("Memento found");
            animator.SetBool("Got", true);
        }
    }

}
