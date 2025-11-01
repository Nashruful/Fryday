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
    private Color startCookingColor; 

    public GameManager gameManager;
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (isInPan)
        {
            currentCookTime += Time.deltaTime;
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

        startCookingColor = chickenRenderer.material.color;
    }

    void EscapePan()
    {
        isInPan = false;



        playerController.WakeUpAndEscapePan();
        playerController.enabled = true;
    }
}