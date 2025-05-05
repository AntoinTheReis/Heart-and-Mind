using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class BindingDisplayStringToKeyboardFont
{
    public static string ActionToKeyboardFontString(InputAction action)
    {
        string displayString = action.GetBindingDisplayString();
        return DisplayStringToKeyboardFontString(displayString);
    }

    public static string DisplayStringToKeyboardFontString(string displayString)
    {
        return "big fat hog";
    }
}
