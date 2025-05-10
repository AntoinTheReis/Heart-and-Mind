using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODSenderAct2 : MonoBehaviour
{
    public float parameterValue; // The new value to send on trigger

    public int parameter;

    public bool pause; // True if pause audio, false if is trigger

    public bool destroy;

    public GameObject speaker;

    private void Start()
    {
        if (speaker == null)
        {
            speaker = GameObject.FindGameObjectWithTag("Speaker");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("1");
        if (speaker != null && !pause)
        {
            Debug.Log("2");
            if (parameter == 1)
            {
                Debug.Log("3");
                speaker.GetComponent<FMODSpeakerAct2>().SetTargetParameter1(parameterValue);
            }
            if (parameter == 2)
            {
                speaker.GetComponent<FMODSpeakerAct2>().SetTargetParameter2(parameterValue);
            }
            if (parameter == 3)
            {
                speaker.GetComponent<FMODSpeakerAct2>().SetTargetParameter3(parameterValue);
            }
        }
        else
        {
            speaker.GetComponent<FMODSpeakerAct2>().StopSound();
        }
        if (destroy)
        {
            speaker.GetComponent<FMODSpeakerAct2>().Destroy();
        }
    }
}