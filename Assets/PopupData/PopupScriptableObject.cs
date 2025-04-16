using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PopupData", menuName = "ScriptableObjects/PopupScriptableObject", order = 1)]
public class PopupScriptableObject : ScriptableObject
{
    public enum Recipient
    {
        NONE, HEART, MIND
    }
    public enum Location
    {
        LEFT, RIGHT, TOP, TOP_LEFT, TOP_RIGHT
    }
    
    public TMP_FontAsset messageFont;
    public TMP_FontAsset titleFont;
    public string message;
    public string title;
    public Recipient recipient;
    public string exitAction;
    public PopupScriptableObject followupPopup;
    public Location location;

}
