using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Yarn.Unity;

public class AudioManager : MonoBehaviour
{

    public FMOD.Studio.Bus MX;
    public FMOD.Studio.Bus SFX;
    public FMOD.Studio.Bus Master;

    public float MXVolume = 0.5f;
    public float SFXVolume = 0.5f;
    public float MasterVolume = 0.5f;

    public EventReference sfxTestEvent; // Assign FMOD Event in Inspector for Music
    private EventInstance sfxTestEventInstance;

    public EventReference fmodEvent; // Assign FMOD Event in Inspector for Music
    private EventInstance eventInstance;
    FMOD.Studio.PARAMETER_ID eventParameter;

    public float fadeValue = 0;

    public EventReference fmodEvent1; // Assign FMOD Event in Inspector for Toggle
    private EventInstance eventInstance1;

    public EventReference fmodEvent2; // Assign FMOD Event in Inspector for Confirm
    private EventInstance eventInstance2;

    public EventReference glassBreakEvent; // Assign FMOD Event in Inspector for Glass Break
    private EventInstance glassBreakInstance;

    public EventReference endMusicEvent; // Assign FMOD Event in Inspector for End Credit Music
    private EventInstance endMusicInstance;

    public float currentValue1 = 0f;
    public float targetValue1 = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(this.gameObject);

        MX = FMODUnity.RuntimeManager.GetBus("bus:/Master/MX");
        SFX = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX");
        Master = FMODUnity.RuntimeManager.GetBus("bus:/Master");

        sfxTestEventInstance = RuntimeManager.CreateInstance(sfxTestEvent);

        eventInstance = RuntimeManager.CreateInstance(fmodEvent);

        FMOD.Studio.EventDescription eventDescription;
        eventInstance.getDescription(out eventDescription);
        FMOD.Studio.PARAMETER_DESCRIPTION eventParameterDescription;
        eventDescription.getParameterDescriptionByName("WhereYouAreFade", out eventParameterDescription);
        eventParameter = eventParameterDescription.id;

        eventInstance1 = RuntimeManager.CreateInstance(fmodEvent1);
        eventInstance2 = RuntimeManager.CreateInstance(fmodEvent2);

        glassBreakInstance = RuntimeManager.CreateInstance(glassBreakEvent);

        endMusicInstance = RuntimeManager.CreateInstance(endMusicEvent);
    }

    // Update is called once per frame
    void Update()
    {
        //Affect audio changes here
        MX.setVolume(MXVolume);
        SFX.setVolume(SFXVolume);

        // Smoothly transition towards the target value
        currentValue1 = Mathf.Lerp(currentValue1, targetValue1, Time.deltaTime * 2);
        eventInstance.setParameterByID(eventParameter, currentValue1);
        if (currentValue1 >= 0.99)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            targetValue1 = 0;
        }
    }

    [YarnCommand("playUIToggle")]
    public void playUIToggle()
    {
        eventInstance1.start();
    }
    [YarnCommand("playUIConfirm")]
    public void playUIConfirm()
    {
        eventInstance2.start();
    }
    [YarnCommand("playGlassBreak")]
    public void playGlassBreak()
    {
        glassBreakInstance.start();
    }
    [YarnCommand("playEndMusic")]
    public void playEndMusic()
    {
        endMusicInstance.start();
    }

    public void fadeIntroMusic()
    {
        targetValue1 = 1;
    }

    public void MasterVolumeLevel(float newMasterVolume)
    {
        MasterVolume = newMasterVolume;
        Debug.Log("MasterBus Changed");
    }
    public void MusicVolumeLevel(float newMusicVolume)
    {
        MXVolume = newMusicVolume;
        Debug.Log("MX Changed");
    }
    public void SFXVolumeLevel(float newSFXVolume)
    {
        SFXVolume = newSFXVolume;

        if (sfxTestEventInstance.isValid())
        {
            Debug.Log("SFX Valid");
            FMOD.Studio.PLAYBACK_STATE playbackState;
            sfxTestEventInstance.getPlaybackState(out playbackState);
            if (playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                sfxTestEventInstance.start();
                Debug.Log("SFX Stopped");
            }
        }
        Debug.Log("SFX Changed");
    }
}
