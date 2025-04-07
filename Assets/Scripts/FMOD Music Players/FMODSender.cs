using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODSender : MonoBehaviour
{
    public float parameterValue; // The new value to send on trigger

    public bool pause; // True if pause audio, false if is trigger

    public GameObject speaker;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (speaker != null && !pause)
        {
            speaker.GetComponent<FMODSpeaker>().SetTargetParameter(parameterValue);
        }
        else
        {
            speaker.GetComponent<FMODSpeaker>().StopSound();
        }
    }
}