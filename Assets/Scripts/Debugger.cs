using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    public new bool enabled;
    public float timeScale;
    private void Awake()
    {
        if (!enabled) return;
        Time.timeScale = timeScale;
    }
}
