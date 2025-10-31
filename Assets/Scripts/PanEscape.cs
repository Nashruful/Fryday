using UnityEngine;

public class PanEscape : MonoBehaviour
{
    public bool isInPan = false;
    public float cookTime = 5f;
    private float currentCookTime = 0f;
    public int escapePresses = 10;
    private int currentPresses = 0;

    public Renderer chickenRenderer;
    public Color fullyCookedColor = Color.black;
    private Color startCookingColor; // MODIFIED: This will store the color when cooking starts

    public GameManager gameManager;
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        // We don't need to store the original color anymore
    }

    void Update()
    {
        if (isInPan)
        {
            currentCookTime += Time.deltaTime;
            // MODIFIED: Lerp from the color we had when we entered the pan to fully cooked
            chickenRenderer.material.color = Color.Lerp(startCookingColor, fullyCookedColor, currentCookTime / cookTime);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                currentPresses++;
                if (currentPresses >= escapePresses)
                {
                    EscapePan();
                }
            }

            if (currentCookTime >= cookTime)
            {
                isInPan = false;
                gameManager.LoseGame();
            }
        }
    }

    public void EnterPan()
    {
        isInPan = true;
        currentCookTime = 0;
        currentPresses = 0;
        playerController.enabled = false;

        // NEW: Capture the chicken's current color at the moment it enters the pan
        startCookingColor = chickenRenderer.material.color;
    }

    void EscapePan()
    {
        isInPan = false;

        // REMOVED: The line "chickenRenderer.material.color = originalColor;" is gone.
        // The color is now persistent.

        playerController.PerformJump();
        playerController.enabled = true;
    }
}