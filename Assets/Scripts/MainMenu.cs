using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainPanel;
    public GameObject optionsPanel;

    [Header("Settings")]
    public AudioMixer mainMixer;
    public Slider volumeSlider;
    public Slider sensitivitySlider;

    void Start()
    {
        // Load settings and apply them to the UI
        LoadSettings();

        // Make sure the main panel is visible and options are hidden
        mainPanel.SetActive(true);
        optionsPanel.SetActive(false);

        // Add "listeners" to the sliders. This makes them call our functions when moved.
        volumeSlider.onValueChanged.AddListener(SetVolume);
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
    }

    void LoadSettings()
    {
        // Set the slider values from our saved settings
        volumeSlider.value = SettingsManager.MasterVolume;
        sensitivitySlider.value = SettingsManager.LookSensitivity;

        // Apply the loaded volume to the mixer
        SettingsManager.ApplyVolume(mainMixer);
    }

    // --- BUTTON FUNCTIONS ---
    public void StartGame()
    {
        // Make sure you have your game scene in File > Build Settings!
        SceneManager.LoadScene("SampleScene"); // <<< CHANGE "GameScene" to your actual scene name
    }

    public void OpenOptions()
    {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    // --- SLIDER FUNCTIONS ---
    public void SetVolume(float volume)
    {
        SettingsManager.MasterVolume = volume;
        SettingsManager.ApplyVolume(mainMixer);
    }

    public void SetSensitivity(float sensitivity)
    {
        SettingsManager.LookSensitivity = sensitivity;
    }
}