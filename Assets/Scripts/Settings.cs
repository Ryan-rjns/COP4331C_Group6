using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
// Functions for changing scene or exiting game
public class Settings : MonoBehaviour
{
    // Audio Mixer
    public AudioMixer audioMixer;
    // Array of available resolutions
    Resolution[] resolutions;
    // Resolution dropdown
    public Dropdown resolutionDropdown;
    // Settings Functions
    public void Volume(float v) => audioMixer.SetFloat("Volume", Mathf.Log10(v) * 20);
    public void Quality(int q) => QualitySettings.SetQualityLevel(q);
    public void FullScreen(bool b) => Screen.fullScreen = b;
    public void Resolution(int r) => Screen.SetResolution(resolutions[r].width, resolutions[r].height, Screen.fullScreen);
    // Initial Settings setup
    private void Start() {
        // Clear dropdown 
        resolutionDropdown.ClearOptions();
        // List to hold resolutions
        List<string> options = new List<string>();
        // Get resolutions
        resolutions = Screen.resolutions;
        // Current dropdown index
        int index = 0;
        // Add resolutions to options list
        for(int i = 0; i < resolutions.Length; i++) {
            string res = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(res);
            // Set default resolution
            if(resolutions[i].width == Screen.width && resolutions[i].height == Screen.height) {
                index = i;
            }
        }
        // Add options to dropdown
        resolutionDropdown.AddOptions(options);
        // Select default resolution
        resolutionDropdown.value = index;
        resolutionDropdown.RefreshShownValue();
    }
}