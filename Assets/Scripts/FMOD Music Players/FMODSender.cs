using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODSender : MonoBehaviour
{
    public float parameterValue; // The new value to send on trigger

    public GameObject speaker;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (speaker != null)
        {
            speaker.GetComponent<FMODSpeaker>().SetTargetParameter(parameterValue);
        }
    }
}