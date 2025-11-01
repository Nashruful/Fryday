using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject optionsPanel;

    [Header("Settings")]
    public AudioMixer mainMixer;
    public Slider volumeSlider;
    public Slider sensitivitySlider;

    private bool isPaused = false;

    void Start()
    {
        // Start with everything hidden
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);

        // You can load settings here too to ensure sliders are correct
        volumeSlider.value = SettingsManager.MasterVolume;
        sensitivitySlider.value = SettingsManager.LookSensitivity;

        volumeSlider.onValueChanged.AddListener(SetVolume);
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // Pause the game
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Resume the game
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
            optionsPanel.SetActive(false); // Make sure options closes too
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // --- BUTTON FUNCTIONS ---
    public void Resume()
    {
        TogglePause();
    }

    public void OpenOptions()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(true);
    }



    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        // IMPORTANT: Always reset time scale when leaving a scene
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // <<< CHANGE "MainMenu" to your menu scene name
    }

    // --- SLIDER FUNCTIONS (Identical to MainMenu) ---
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