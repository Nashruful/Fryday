using UnityEngine;
using UnityEngine.AI;

public class ChefAI : MonoBehaviour
{
    public enum ChefState { GoingToTable, Cooking, Chasing, CarryingPlayer, PlacingInPan }
    public ChefState currentState;

    [Header("Object References")]
    [Tooltip("The Animator component on the Chef model.")]
    public Animator animator;
    [Tooltip("The player's Transform.")]
    public Transform player;
    [Tooltip("An empty GameObject where the Chef stands to cook.")]
    public Transform cookingTableLocation;
    [Tooltip("An empty GameObject where the Chef takes the player to put them in the pan.")]
    public Transform chefPanLocation;
    [Tooltip("The exact spot where the player is dropped off at the pan.")]
    public Transform playerDropOffPoint;
    // --- NEW ---
    [Tooltip("An empty GameObject parented to the Chef's hand. This is where the player will snap to when carried.")]
    public Transform playerCarryPoint;


    [Header("AI Settings")]
    [Tooltip("How long the Chef cooks before starting a chase.")]
    public float timeBetweenChases = 15f;
    [Tooltip("How long a chase lasts before the Chef gives up.")]
    public float chaseDuration = 20f;
    // --- MODIFIED ---
    [Tooltip("How close the Chef needs to be to catch the player. Should be a small value.")]
    public float catchDistance = 1.2f;
    // --- NEW ---
    [Tooltip("How close the Chef needs to be to the pan to drop the player off.")]
    public float dropOffDistance = 1.0f;

    private NavMeshAgent agent;
    private float chaseTimer;
    private float chaseCooldown;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Start by going to the cooking table
        currentState = ChefState.GoingToTable;
        agent.SetDestination(cookingTableLocation.position);
        chaseCooldown = timeBetweenChases;
    }

    void Update()
    {
        // This handles the walking/idle animation automatically
        UpdateAnimator();

        switch (currentState)
        {
            case ChefState.GoingToTable:
                // Check if we've arrived at the table
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    currentState = ChefState.Cooking;
                    // Make the chef face the table (optional but looks good)
                    transform.LookAt(new Vector3(cookingTableLocation.position.x, transform.position.y, cookingTableLocation.position.z));
                }
                break;

            case ChefState.Cooking:
                chaseCooldown -= Time.deltaTime;
                if (chaseCooldown <= 0)
                {
                    currentState = ChefState.Chasing;
                    chaseTimer = chaseDuration;
                    Sound.Instance.StartChefChaseSound(); // Start chase sound when entering chase state
                }
                break;

            case ChefState.Chasing:
                agent.SetDestination(player.position);
                chaseTimer -= Time.deltaTime;
                // If the chase timer runs out, go back to the table
                if (chaseTimer <= 0)
                {
                    Sound.Instance.StopChefChaseSound(); // Stop chase sound
                    currentState = ChefState.GoingToTable;
                    agent.SetDestination(cookingTableLocation.position);
                    chaseCooldown = Random.Range(timeBetweenChases, timeBetweenChases + 15f);
                }
                // If we catch the player
                if (Vector3.Distance(transform.position, player.position) < catchDistance)
                {
                    CatchPlayer();
                }
                break;

            case ChefState.CarryingPlayer:
                // Check if we've arrived at the pan
                // --- MODIFIED ---
                if (!agent.pathPending && agent.remainingDistance < dropOffDistance)
                {
                    currentState = ChefState.PlacingInPan;
                }
                break;

            case ChefState.PlacingInPan:
                PlacePlayerInPan();
                // After placing, go back to the table
                currentState = ChefState.GoingToTable;
                agent.SetDestination(cookingTableLocation.position);
                chaseCooldown = Random.Range(timeBetweenChases, timeBetweenChases + 15f);
                break;
        }
    }

    public void AlertChef()
    {
        // We don't want to interrupt the chef if he has already caught the player.
        if (currentState == ChefState.CarryingPlayer || currentState == ChefState.PlacingInPan)
        {
            return; // Do nothing if the player is already caught
        }

        Debug.Log("Chef has been alerted by the player falling!");
        currentState = ChefState.Chasing;
        chaseTimer = chaseDuration; // Reset the chase timer to its full duration
        Sound.Instance.StartChefChaseSound(); // Start chase sound when alerted
    }

    public void PlayerIsInSafeZone()
    {
        // We only care if the Chef is currently chasing the player.
        if (currentState == ChefState.Chasing)
        {
            Debug.Log("Player entered a safe zone! Chef is giving up the chase.");
            Sound.Instance.StopChefChaseSound(); // Stop chase sound when entering safe zone

            // Change state to go back to the table.
            currentState = ChefState.GoingToTable;
            agent.SetDestination(cookingTableLocation.position);

            // Reset the chase cooldown so he doesn't immediately start again.
            chaseCooldown = Random.Range(timeBetweenChases, timeBetweenChases + 15f);
        }
    }

    // This function is called every frame to set the IsMoving parameter
    void UpdateAnimator()
    {
        // Check if the agent has a path and is moving
        if (agent.velocity.magnitude > 0.1f)
        {
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    void CatchPlayer()
    {
        currentState = ChefState.CarryingPlayer;
        Sound.Instance.StopChefChaseSound(); // Stop chase sound
        Sound.Instance.PlayCatchSound(); // Play catch sound

        // Trigger the catching animation
        animator.SetTrigger("Catch");

        // Stop the player
        player.GetComponent<PlayerController>().enabled = false;

        // --- MODIFIED ---
        // Parent the player to the specified carry point
        player.SetParent(playerCarryPoint);
        // Snap the player's position to the center of the carry point
        player.localPosition = Vector3.zero;

        // Set the new destination: the pan
        agent.SetDestination(chefPanLocation.position);
    }

    void PlacePlayerInPan()
    {
        // Trigger the animation to go back to normal
        animator.SetTrigger("PlacedPlayer");

        // Unparent the player
        player.SetParent(null);

        // Move player to the drop off spot
        player.position = playerDropOffPoint.position;

        // Activate the PanEscape minigame for the player
        player.GetComponent<PanEscape>().EnterPan();
    }
}