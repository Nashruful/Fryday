using UnityEngine;

/// <summary>
/// Central sound management system for the game.
/// All sound functionality is contained in this single file.
/// </summary>
public class Sound : MonoBehaviour
{
    // Singleton instance for easy access from anywhere
    private static Sound instance;
    public static Sound Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject soundObject = new GameObject("SoundManager");
                instance = soundObject.AddComponent<Sound>();
                DontDestroyOnLoad(soundObject);
            }
            return instance;
        }
    }

    [Header("Audio Sources")]
    [Tooltip("AudioSource for background music (plays continuously)")]
    public AudioSource backgroundMusicSource;
    [Tooltip("AudioSource for chef chase sound")]
    public AudioSource chefChaseSource;
    [Tooltip("AudioSource for catching sound")]
    public AudioSource catchSoundSource;
    [Tooltip("AudioSource for cooking sound")]
    public AudioSource cookingSoundSource;
    [Tooltip("AudioSource for grabbing items")]
    public AudioSource grabItemSource;
    [Tooltip("AudioSource for dropping items")]
    public AudioSource dropItemSource;

    [Header("Audio Clips - Assign these in the Inspector")]
    [Tooltip("Background music that plays throughout the game")]
    public AudioClip backgroundMusic;
    [Tooltip("Sound that plays when chef starts chasing the player")]
    public AudioClip chefChaseSound;
    [Tooltip("Sound that plays when the chef catches the player")]
    public AudioClip catchSound;
    [Tooltip("Sound that plays while the player is cooking in the pan")]
    public AudioClip cookingSound;
    [Tooltip("Sound that plays when grabbing an ingredient")]
    public AudioClip grabItemSound;
    [Tooltip("Sound that plays when dropping an ingredient in the pan")]
    public AudioClip dropItemSound;

    [Header("Settings")]
    [Tooltip("Volume for background music (0-1)")]
    [Range(0f, 1f)]
    public float backgroundMusicVolume = 0.5f;
    [Tooltip("Volume for sound effects (0-1)")]
    [Range(0f, 1f)]
    public float soundEffectsVolume = 1f;

    private bool isChefChasing = false;
    private bool isCooking = false;

    void Awake()
    {
        // Ensure singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Start playing background music
        PlayBackgroundMusic();
    }

    /// <summary>
    /// Initializes all AudioSource components if they don't exist
    /// </summary>
    void InitializeAudioSources()
    {
        // Background Music Source
        if (backgroundMusicSource == null)
        {
            backgroundMusicSource = gameObject.AddComponent<AudioSource>();
            backgroundMusicSource.loop = true;
            backgroundMusicSource.playOnAwake = false;
            backgroundMusicSource.volume = backgroundMusicVolume;
        }

        // Chef Chase Source
        if (chefChaseSource == null)
        {
            GameObject chaseObject = new GameObject("ChefChaseSource");
            chaseObject.transform.SetParent(transform);
            chefChaseSource = chaseObject.AddComponent<AudioSource>();
            chefChaseSource.loop = true;
            chefChaseSource.playOnAwake = false;
            chefChaseSource.volume = soundEffectsVolume;
        }

        // Catch Sound Source
        if (catchSoundSource == null)
        {
            GameObject catchObject = new GameObject("CatchSoundSource");
            catchObject.transform.SetParent(transform);
            catchSoundSource = catchObject.AddComponent<AudioSource>();
            catchSoundSource.loop = false;
            catchSoundSource.playOnAwake = false;
            catchSoundSource.volume = soundEffectsVolume;
        }

        // Cooking Sound Source
        if (cookingSoundSource == null)
        {
            GameObject cookingObject = new GameObject("CookingSoundSource");
            cookingObject.transform.SetParent(transform);
            cookingSoundSource = cookingObject.AddComponent<AudioSource>();
            cookingSoundSource.loop = true;
            cookingSoundSource.playOnAwake = false;
            cookingSoundSource.volume = soundEffectsVolume;
        }

        // Grab Item Source
        if (grabItemSource == null)
        {
            GameObject grabObject = new GameObject("GrabItemSource");
            grabObject.transform.SetParent(transform);
            grabItemSource = grabObject.AddComponent<AudioSource>();
            grabItemSource.loop = false;
            grabItemSource.playOnAwake = false;
            grabItemSource.volume = soundEffectsVolume;
        }

        // Drop Item Source
        if (dropItemSource == null)
        {
            GameObject dropObject = new GameObject("DropItemSource");
            dropObject.transform.SetParent(transform);
            dropItemSource = dropObject.AddComponent<AudioSource>();
            dropItemSource.loop = false;
            dropItemSource.playOnAwake = false;
            dropItemSource.volume = soundEffectsVolume;
        }
    }

    /// <summary>
    /// Starts playing background music continuously
    /// </summary>
    public void PlayBackgroundMusic()
    {
        if (backgroundMusicSource != null && backgroundMusic != null)
        {
            backgroundMusicSource.clip = backgroundMusic;
            backgroundMusicSource.volume = backgroundMusicVolume;
            backgroundMusicSource.Play();
        }
    }

    /// <summary>
    /// Stops background music
    /// </summary>
    public void StopBackgroundMusic()
    {
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop();
        }
    }

    /// <summary>
    /// Starts playing the chef chase sound (loops until stopped)
    /// Called when chef enters Chasing state
    /// </summary>
    public void StartChefChaseSound()
    {
        if (isChefChasing) return; // Already playing

        if (chefChaseSource != null && chefChaseSound != null)
        {
            isChefChasing = true;
            chefChaseSource.clip = chefChaseSound;
            chefChaseSource.volume = soundEffectsVolume;
            chefChaseSource.Play();
        }
    }

    /// <summary>
    /// Stops the chef chase sound
    /// Called when chef catches the player or stops chasing
    /// </summary>
    public void StopChefChaseSound()
    {
        if (!isChefChasing) return; // Already stopped

        if (chefChaseSource != null)
        {
            isChefChasing = false;
            chefChaseSource.Stop();
        }
    }

    /// <summary>
    /// Plays the catch sound (one-shot)
    /// Called when chef catches the player
    /// </summary>
    public void PlayCatchSound()
    {
        if (catchSoundSource != null && catchSound != null)
        {
            catchSoundSource.clip = catchSound;
            catchSoundSource.volume = soundEffectsVolume;
            catchSoundSource.Play();
        }
    }

    /// <summary>
    /// Starts playing the cooking sound (loops until stopped)
    /// Called when player enters the pan
    /// </summary>
    public void StartCookingSound()
    {
        if (isCooking) return; // Already playing

        if (cookingSoundSource != null && cookingSound != null)
        {
            isCooking = true;
            cookingSoundSource.clip = cookingSound;
            cookingSoundSource.volume = soundEffectsVolume;
            cookingSoundSource.Play();
        }
    }

    /// <summary>
    /// Stops the cooking sound
    /// Called when player escapes from the pan
    /// </summary>
    public void StopCookingSound()
    {
        if (!isCooking) return; // Already stopped

        if (cookingSoundSource != null)
        {
            isCooking = false;
            cookingSoundSource.Stop();
        }
    }

    /// <summary>
    /// Plays the grab item sound (one-shot)
    /// Called when player picks up an ingredient
    /// </summary>
    public void PlayGrabItemSound()
    {
        if (grabItemSource != null && grabItemSound != null)
        {
            grabItemSource.clip = grabItemSound;
            grabItemSource.volume = soundEffectsVolume;
            grabItemSource.Play();
        }
    }

    /// <summary>
    /// Plays the drop item sound (one-shot)
    /// Called when player drops an ingredient in the cooking pan
    /// </summary>
    public void PlayDropItemSound()
    {
        if (dropItemSource != null && dropItemSound != null)
        {
            dropItemSource.clip = dropItemSound;
            dropItemSource.volume = soundEffectsVolume;
            dropItemSource.Play();
        }
    }

    /// <summary>
    /// Updates volume settings at runtime
    /// </summary>
    public void UpdateVolumes()
    {
        if (backgroundMusicSource != null)
            backgroundMusicSource.volume = backgroundMusicVolume;
        
        if (chefChaseSource != null)
            chefChaseSource.volume = soundEffectsVolume;
        
        if (catchSoundSource != null)
            catchSoundSource.volume = soundEffectsVolume;
        
        if (cookingSoundSource != null)
            cookingSoundSource.volume = soundEffectsVolume;
        
        if (grabItemSource != null)
            grabItemSource.volume = soundEffectsVolume;
        
        if (dropItemSource != null)
            dropItemSource.volume = soundEffectsVolume;
    }
}

