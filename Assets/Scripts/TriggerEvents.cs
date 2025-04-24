using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvents : MonoBehaviour
{
    private bool triggered = false;
    public UnityEvent OnFirstEnter;
    public UnityEvent OnEnter;
    public UnityEvent OnExit;

    public string tagToCheckFor;

    private void Start()
    {
        if(tagToCheckFor == null || tagToCheckFor == "") tagToCheckFor = "Player";
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(tagToCheckFor))
        {
            if (!triggered)
            {
                Debug.Log(name + "OnFirstEnter triggered");
                OnFirstEnter.Invoke();
            }
            triggered = true;
            Debug.Log(name + "Enter triggered");
            OnEnter.Invoke();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log(name + "Exit triggered");
        if(other.gameObject.CompareTag("Player")) OnExit.Invoke();
    }
}
