using UnityEngine;
using UnityEngine.AI;

public class ChefAI : MonoBehaviour
{
    // Our AI now has more states to manage the new mechanic.
    public enum ChefState { Cooking, Chasing, CarryingPlayer, PlacingInPan }
    public ChefState currentState;

    private NavMeshAgent agent;
    public Transform player;
    public Transform chefPanLocation; // Assign the chef's pan location in the Inspector.

    // Timers and settings
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    private float timer;

    public float chaseDuration = 20f;
    private float chaseTimer;

    public float timeBetweenChases = 15f;
    private float chaseCooldown;

    public Transform playerDropOffPoint;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = ChefState.Cooking;
        timer = wanderTimer;
        chaseCooldown = timeBetweenChases;
    }

    void Update()
    {
        switch (currentState)
        {
            case ChefState.Cooking:
                Wander();
                chaseCooldown -= Time.deltaTime;
                if (chaseCooldown <= 0)
                {
                    currentState = ChefState.Chasing;
                    chaseTimer = chaseDuration;
                }
                break;

            case ChefState.Chasing:
                ChasePlayer();
                chaseTimer -= Time.deltaTime;
                if (chaseTimer <= 0)
                {
                    currentState = ChefState.Cooking;
                    chaseCooldown = Random.Range(15f, 30f); // Reset cooldown
                }
                // Check if the chef has caught the player
                if (Vector3.Distance(transform.position, player.position) < 2.0f)
                {
                    CatchPlayer();
                }
                break;

            case ChefState.CarryingPlayer:
                // Chef moves to the pan. The player is already attached and will move with him.
                // Check if the chef has arrived at the pan.
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    currentState = ChefState.PlacingInPan;
                }
                break;

            case ChefState.PlacingInPan:
                // This state is instant. It places the player and returns to cooking.
                PlacePlayerInPan();
                currentState = ChefState.Cooking;
                chaseCooldown = Random.Range(15f, 30f); // Reset cooldown
                break;
        }
    }

    void Wander()
    {
        agent.speed = 3.5f;
        timer += Time.deltaTime;
        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    void ChasePlayer()
    {
        agent.speed = 6f; // Chef is faster when chasing!
        agent.SetDestination(player.position);
    }

    void CatchPlayer()
    {
        currentState = ChefState.CarryingPlayer;

        // Disable player movement.
        player.GetComponent<PlayerController>().enabled = false;

        // Make the player a child of the chef to carry them.
        player.SetParent(this.transform);
        player.localPosition = new Vector3(0, 1.5f, 1f); // Adjust position to look right

        // Set the chef's destination to his pan.
        agent.SetDestination(chefPanLocation.position);
    }

    void PlacePlayerInPan()
    {
        // Un-parent the player from the chef.
        player.SetParent(null);

        player.position = playerDropOffPoint.position;


        // Put the player in the pan and start the escape mechanic.
        player.GetComponent<PanEscape>().EnterPan();
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
}