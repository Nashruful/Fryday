using UnityEngine;
using UnityEngine.UI; // We need this new line to work with UI elements

public class PanEscape : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Assign the 'SpacebarPrompt' Image from your Canvas here.")]
    public Image spacebarPromptUI; // --- NEW ---

    [Header("Mechanics")]
    public bool isInPan = false;
    public float cookTime = 5f;
    public int escapePresses = 10;

    [Header("Visuals")]
    public Renderer chickenRenderer;
    public Color fullyCookedColor = Color.black;

    // --- NEW "ENGAGING" UI VARIABLES ---
    [Header("Engaging UI Animation")]
    [Tooltip("How fast the UI prompt pulses.")]
    public float pulseSpeed = 2f;
    [Tooltip("The smallest size the prompt will pulse to.")]
    public float minScale = 0.9f;
    [Tooltip("The largest size the prompt will pulse to.")]
    public float maxScale = 1.1f;
    [Tooltip("The size the prompt 'pops' to when space is pressed.")]
    public float pressScale = 1.3f;
    [Tooltip("How quickly the prompt returns to normal size after a pop.")]
    public float popReturnSpeed = 10f;


    private float currentCookTime = 0f;
    private int currentPresses = 0;
    private Color startCookingColor;
    private GameManager gameManager;
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        gameManager = FindFirstObjectByType<GameManager>(); // A good way to find the manager

        // Make sure the UI is turned off at the start of the game
        if (spacebarPromptUI != null)
        {
            spacebarPromptUI.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isInPan)
        {
            // Cooking logic
            currentCookTime += Time.deltaTime;
            chickenRenderer.material.color = Color.Lerp(startCookingColor, fullyCookedColor, currentCookTime / cookTime);

            // Animate the UI
            AnimateSpacebarPrompt();

            // Player input
            if (Input.GetKeyDown(KeyCode.Space))
            {
                currentPresses++;

                // "Pop" the UI for feedback
                if (spacebarPromptUI != null)
                {
                    spacebarPromptUI.transform.localScale = Vector3.one * pressScale;
                }

                if (currentPresses >= escapePresses)
                {
                    EscapePan();
                }
            }

            // Lose condition
            if (currentCookTime >= cookTime)
            {
                isInPan = false;
                Sound.Instance.StopCookingSound(); // Stop cooking sound
                if (spacebarPromptUI != null) spacebarPromptUI.gameObject.SetActive(false);
                gameManager.LoseGame();
            }
        }
    }

    // --- NEW --- This function handles the pulsing animation
    void AnimateSpacebarPrompt()
    {
        if (spacebarPromptUI == null) return;

        // Calculate the target scale based on the pulsing sine wave
        float targetScaleValue = minScale + (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2 * (maxScale - minScale);
        Vector3 targetScale = Vector3.one * targetScaleValue;

        // Smoothly move the current scale towards the target scale (this also handles the "pop" return)
        spacebarPromptUI.transform.localScale = Vector3.Lerp(spacebarPromptUI.transform.localScale, targetScale, Time.deltaTime * popReturnSpeed);
    }

    public void EnterPan()
    {
        isInPan = true;
        currentCookTime = 0;
        currentPresses = 0;
        playerController.enabled = false;
        startCookingColor = chickenRenderer.material.color;
        Sound.Instance.StartCookingSound(); // Start cooking sound

        // --- NEW --- Turn on the UI
        if (spacebarPromptUI != null)
        {
            spacebarPromptUI.gameObject.SetActive(true);
        }
    }

    void EscapePan()
    {
        isInPan = false;
        Sound.Instance.StopCookingSound(); // Stop cooking sound
        playerController.WakeUpAndEscapePan();
        playerController.enabled = true;

        // --- NEW --- Turn off the UI
        if (spacebarPromptUI != null)
        {
            spacebarPromptUI.gameObject.SetActive(false);
        }
    }
}