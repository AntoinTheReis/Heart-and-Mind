using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MainMenuSpeaker : MonoBehaviour
{

    public EventReference fmodEvent; // Assign FMOD Event in Inspector for Music
    private EventInstance eventInstance;

    public EventReference fmodEvent1; // Assign FMOD Event in Inspector for Toggle
    private EventInstance eventInstance1;

    public EventReference fmodEvent2; // Assign FMOD Event in Inspector for Confirm
    private EventInstance eventInstance2;

    // Start is called before the first frame update
    void Start()
    {
        eventInstance = RuntimeManager.CreateInstance(fmodEvent);
        eventInstance1 = RuntimeManager.CreateInstance(fmodEvent1);
        eventInstance2 = RuntimeManager.CreateInstance(fmodEvent2);

        eventInstance.start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playUIToggle()
    {
        eventInstance1.start();
    }
    public void playUIConfirm()
    {
        eventInstance2.start();
    }
}
