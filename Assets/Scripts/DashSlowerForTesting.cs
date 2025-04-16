using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSlowerForTesting : MonoBehaviour
{
    public int targetRate = 10;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = targetRate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
