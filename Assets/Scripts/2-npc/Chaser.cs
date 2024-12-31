using UnityEngine;
using UnityEngine.AI;
using System.Linq;

/**
 * This component represents an enemy NPC that moves to the target closest to the player.
 */
[RequireComponent(typeof(NavMeshAgent))]
public class Chaser : MonoBehaviour
{

    [Tooltip("The player object used to determine closest target")]
    [SerializeField]
    GameObject player = null;

    [Tooltip("A game object whose children have a Target component. Each child represents a target.")]
    [SerializeField]
    private Transform targetFolder = null;
    private Target[] allTargets = null;

    [Header("These fields are for display only")]
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

        FindAndSetClosestTarget();
    }

    private void Update()
    {
        playerPosition = player.transform.position;
        FindAndSetClosestTarget();

        if (navMeshAgent.hasPath)
        {
            FaceDirection();
        }
    }

    private void FindAndSetClosestTarget()
    {
        // Find the target that is closest to the player
        Target closestTarget = allTargets
            .OrderBy(target => Vector3.Distance(player.transform.position, target.transform.position))
            .FirstOrDefault();

        if (closestTarget != null)
        {
            currentTarget = closestTarget;
            navMeshAgent.destination = currentTarget.transform.position;
            //Debug.Log("Moving to closest target: " + currentTarget.name);
        }
    }

    private void FaceDirection()
    {
        Vector3 direction = (navMeshAgent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    internal Vector3 TargetObjectPosition()
    {
        return player.transform.position;
    }
}