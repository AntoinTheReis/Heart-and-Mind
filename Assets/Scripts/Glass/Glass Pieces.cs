using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassPieces : MonoBehaviour
{

    [SerializeField] float timeToStopCollisions;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Created pieces");
        StartCoroutine(TimeToStopCollisions());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TimeToStopCollisions()
    {
        Collider2D collider = GetComponent<Collider2D>();
        yield return new WaitForSeconds(timeToStopCollisions);

        //LayerMask.Equals(gameObject.layer, LayerMask.GetMask("Default"));

        gameObject.layer = LayerMask.GetMask("Default");
        collider.excludeLayers = LayerMask.GetMask("Player");
    }
}
