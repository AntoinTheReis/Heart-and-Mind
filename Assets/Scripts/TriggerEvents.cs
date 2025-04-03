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
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if(!triggered) OnFirstEnter.Invoke();
            triggered = true;
            OnEnter.Invoke();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player") OnExit.Invoke();
    }
}
