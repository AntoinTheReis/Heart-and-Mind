using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Yarn.Unity;

public class FMODSpeakerAct2 : MonoBehaviour
{
    public string parameterName1; // Name of FMOD parameter Act 2 Progress
    public string parameterName2; // Name of FMOD parameter Act 2 Character
    public string parameterName3; // Name of FMOD parameter Paused

    public string triggerTag; // Name of Trigger Gameobject Tag

    public float smoothingSpeed = 2f; // Speed of parameter transition

    public EventReference fmodEvent; // Assign FMOD Event in Inspector
    private EventInstance eventInstance;
    FMOD.Studio.PARAMETER_ID eventParameter1;
    FMOD.Studio.PARAMETER_ID eventParameter2;
    FMOD.Studio.PARAMETER_ID eventParameter3;

    public float currentValue1 = 0f;
    public float targetValue1 = 0f;
    public float currentValue2 = 0f;
    public float targetValue2 = 0f;
    public float currentValue3 = 0f;
    public float targetValue3 = 0f;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        eventInstance = RuntimeManager.CreateInstance(fmodEvent);

        FMOD.Studio.EventDescription eventDescription1;
        eventInstance.getDescription(out eventDescription1);
        FMOD.Studio.PARAMETER_DESCRIPTION eventParameterDescription1;
        eventDescription1.getParameterDescriptionByName(parameterName1, out eventParameterDescription1);
        eventParameter1 = eventParameterDescription1.id;

        FMOD.Studio.EventDescription eventDescription2;
        eventInstance.getDescription(out eventDescription2);
        FMOD.Studio.PARAMETER_DESCRIPTION eventParameterDescription2;
        eventDescription2.getParameterDescriptionByName(parameterName2, out eventParameterDescription2);
        eventParameter2 = eventParameterDescription2.id;

        FMOD.Studio.EventDescription eventDescription3;
        eventInstance.getDescription(out eventDescription3);
        FMOD.Studio.PARAMETER_DESCRIPTION eventParameterDescription3;
        eventDescription3.getParameterDescriptionByName(parameterName3, out eventParameterDescription3);
        eventParameter3 = eventParameterDescription3.id;
    }

    void Update()
    {
        // Smoothly transition towards the target value
        currentValue1 = Mathf.Lerp(currentValue1, targetValue1, Time.deltaTime * smoothingSpeed);
        eventInstance.setParameterByID(eventParameter1, currentValue1);

        currentValue2 = Mathf.Lerp(currentValue2, targetValue2, Time.deltaTime * smoothingSpeed);
        eventInstance.setParameterByID(eventParameter2, currentValue2);

        currentValue3 = Mathf.Lerp(currentValue3, targetValue3, Time.deltaTime * smoothingSpeed);
        eventInstance.setParameterByID(eventParameter3, currentValue3);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerTag != null)
        {
            if (other.CompareTag(triggerTag)) // Only trigger if the object has the associate tag
            {
               PlaySound();
            }
        }
    }
    [YarnCommand("play")]
    public void PlaySound()
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

    [YarnCommand("mute")]
    public void StopSound()
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void SetTargetParameter1(float newValue)
    {
        targetValue1 = newValue;
    }
    public void SetTargetParameter2(float newValue)
    {
        targetValue2 = newValue;
    }
    public void SetTargetParameter3(float newValue)
    {
        targetValue3 = newValue;
    }

    public void OnDestroy()
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
