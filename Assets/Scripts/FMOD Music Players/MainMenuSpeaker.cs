using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MainMenuSpeaker : MonoBehaviour
{

    public EventReference fmodEvent; // Assign FMOD Event in Inspector for Music
    private EventInstance eventInstance;
    FMOD.Studio.PARAMETER_ID eventParameter;

    public float fadeValue = 0;

    public EventReference fmodEvent1; // Assign FMOD Event in Inspector for Toggle
    private EventInstance eventInstance1;

    public EventReference fmodEvent2; // Assign FMOD Event in Inspector for Confirm
    private EventInstance eventInstance2;

    public float currentValue1 = 0f;
    public float targetValue1 = 0f;

    // Start is called before the first frame update
    void Start()
    {
        eventInstance = RuntimeManager.CreateInstance(fmodEvent);

        FMOD.Studio.EventDescription eventDescription;
        eventInstance.getDescription(out eventDescription);
        FMOD.Studio.PARAMETER_DESCRIPTION eventParameterDescription;
        eventDescription.getParameterDescriptionByName("WhereYouAreFade", out eventParameterDescription);
        eventParameter = eventParameterDescription.id;

        eventInstance1 = RuntimeManager.CreateInstance(fmodEvent1);
        eventInstance2 = RuntimeManager.CreateInstance(fmodEvent2);

        eventInstance.start();
    }

    // Update is called once per frame
    void Update()
    {
        // Smoothly transition towards the target value
        currentValue1 = Mathf.Lerp(currentValue1, targetValue1, Time.deltaTime * 2);
        eventInstance.setParameterByID(eventParameter, currentValue1);
        if (currentValue1 >= 0.99)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            targetValue1 = 0;
        }
    }

    public void playUIToggle()
    {
        eventInstance1.start();
    }
    public void playUIConfirm()
    {
        eventInstance2.start();
    }

    public void fadeIntroMusic()
    {
        targetValue1 = 1;
    }
}
