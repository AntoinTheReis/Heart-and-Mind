using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableGlass : MonoBehaviour
{

    [SerializeField] GameObject brokenPrefab;
    [SerializeField] Collider2D thisCollider;
    public GameObject heart;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 2; i++)
        {
            if(GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<Movement>() != null)
            {
                heart = GameObject.FindGameObjectsWithTag("Player")[i];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(heart.GetComponent<Movement>().isDashing) thisCollider.enabled = false;
        else thisCollider.enabled = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*thisCollider.enabled = false;
        if((collision.gameObject.tag != "Player" || !collision.gameObject.GetComponent<Movement>().isDashing)) thisCollider.enabled = true;*/
        if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<Movement>().isDashing)
        {   
            Instantiate(brokenPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

}
