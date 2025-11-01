using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio; // Required for the Audio Mixer

public static class SettingsManager
{
    // --- SENSITIVITY ---
    // We'll use a "property" to automatically save the value whenever it's changed.
    public static float LookSensitivity
    {
        // When another script asks for the value, get it from PlayerPrefs.
        // If it doesn't exist, return a default value of 2f.
        get { return PlayerPrefs.GetFloat("LookSensitivity", 2f); }
        // When another script sets the value, save it to PlayerPrefs.
        set { PlayerPrefs.SetFloat("LookSensitivity", value); }
    }

    // --- SOUND ---
    public static float MasterVolume
    {
        get { return PlayerPrefs.GetFloat("MasterVolume", 1f); }
        set { PlayerPrefs.SetFloat("MasterVolume", value); }
    }

    // This is a helper function to apply the saved volume to the game's audio mixer.
    public static void ApplyVolume(AudioMixer audioMixer)
    {
        if (audioMixer == null) return;

        // PlayerPrefs stores volume from 0.0001 to 1. The mixer uses decibels (-80 to 0).
        // This formula converts our slider value to the decibel scale.
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(MasterVolume) * 20);
    }
}
