using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODSpeaker : MonoBehaviour
{
    public string parameterName; // Name of FMOD parameter

    public string triggerTag; // Name of Trigger Gameobject Tag

    public float smoothingSpeed = 2f; // Speed of parameter transition

    public EventReference fmodEvent; // Assign FMOD Event in Inspector
    private EventInstance eventInstance;
    FMOD.Studio.PARAMETER_ID eventParameter;

    public float currentValue = 0f;
    public float targetValue = 0f;

    void Start()
    {
        eventInstance = RuntimeManager.CreateInstance(fmodEvent);
        FMOD.Studio.EventDescription eventDescription;
        eventInstance.getDescription(out eventDescription);
        FMOD.Studio.PARAMETER_DESCRIPTION eventParameterDescription;
        eventDescription.getParameterDescriptionByName(parameterName, out eventParameterDescription);
        eventParameter = eventParameterDescription.id;
    }

    void Update()
    {
        // Smoothly transition towards the target value
        currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * smoothingSpeed);
        eventInstance.setParameterByID(eventParameter, currentValue);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerTag != null)
        {
            if (other.CompareTag(triggerTag)) // Only trigger if the object has the associate tag
            {
                if (eventInstance.isValid())
                {
                    FMOD.Studio.PLAYBACK_STATE playbackState;
                    eventInstance.getPlaybackState(out playbackState);
                    if (playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
                    {
                        eventInstance.start();
                    }
                }
            }
        }
    }

    public void SetTargetParameter(float newValue)
    {
        targetValue = newValue;
    }

    void OnDestroy()
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
    }
}
