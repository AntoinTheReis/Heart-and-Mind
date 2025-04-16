using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class AppearByProximity : MonoBehaviour
{
    public GameObject target;
    public Vector3 offsetPosition;
    public float proximity;
    [Tooltip("Will fade out twice as fast as it will fade in")]
    public float fadeSpeed;

    private Renderer image;
    private void Start()
    {
        if(image == null) image = GetComponent<Renderer>();
        if (target == null) target = GameObject.Find("Heart");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + offsetPosition, proximity);
    }

    private void Update()
    {
        if (target == null) return;
        
        Color color = image.material.color;
        
        //if the distance is too large, lerp alpha to 0
        if (Vector3.Distance(transform.position + offsetPosition, target.transform.position) > proximity)
        {
            color.a = Mathf.Lerp(color.a, 0f, fadeSpeed * Time.deltaTime);
        }
        else //otherwise, lerp it to 1
        {
            color.a = Mathf.Lerp(color.a, 1f, fadeSpeed * Time.deltaTime / 2);
        }

        image.material.color = color;
    }
    

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    public void SetProximity(float proximity)
    {
        this.proximity = proximity;
    }
}
