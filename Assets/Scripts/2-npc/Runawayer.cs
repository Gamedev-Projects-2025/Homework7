using UnityEngine;
using UnityEngine.AI;
using System.Linq;

/**
 * This component represents an enemy NPC that runs away from the player
 * by moving to the target that is farthest from the player.
 */
[RequireComponent(typeof(NavMeshAgent))]
public class Runawayer : MonoBehaviour
{
    [Tooltip("The object that this enemy runs away from")]
    [SerializeField]
    GameObject player = null;

    [Tooltip("A game object whose children have a Target component. Each child represents a target.")]
    [SerializeField]
    private Transform targetFolder = null;
    private Target[] allTargets = null;

    [Header("For debugging")]
    [SerializeField] private Vector3 playerPosition;
    [SerializeField] private Target currentTarget = null;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private float rotationSpeed = 5f;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Get all available targets
        allTargets = targetFolder.GetComponentsInChildren<Target>(false);
        Debug.Log("Found " + allTargets.Length + " active targets.");

        // Find initial farthest target
        FindAndSetFarthestTarget();
    }

    private void Update()
    {
        playerPosition = player.transform.position;

        // If we've reached the current target or don't have one, find the new farthest target
        if (!navMeshAgent.hasPath || Vector3.Distance(transform.position, navMeshAgent.destination) < 0.1f)
        {
            FindAndSetFarthestTarget();
        }

        // If we're moving, face the direction we're going
        if (navMeshAgent.hasPath)
        {
            FaceDirection();
        }
    }

    private void FindAndSetFarthestTarget()
    {
        // Find the target that is farthest from the player
        Target farthestTarget = allTargets
            .OrderByDescending(target => Vector3.Distance(player.transform.position, target.transform.position))
            .FirstOrDefault();

        if (farthestTarget != null)
        {
            currentTarget = farthestTarget;
            navMeshAgent.destination = currentTarget.transform.position;
            Debug.Log("Moving to farthest target: " + currentTarget.name);
        }
    }

    private void FaceDirection()
    {
        Vector3 direction = (navMeshAgent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    // Added to match interface used by state machine
    public Vector3 TargetObjectPosition()
    {
        return player.transform.position;
    }
}