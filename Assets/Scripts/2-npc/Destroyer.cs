using UnityEngine;
using UnityEngine.AI;
using System.Linq;

/**
 * This component represents an enemy NPC that moves to destroy targets in sequential order.
 */
[RequireComponent(typeof(NavMeshAgent))]
public class Destroyer : MonoBehaviour
{
    [Tooltip("A game object whose children have a Target component. Each child represents a target.")]
    [SerializeField]
    private Transform targetFolder = null;
    private Target[] allTargets = null;
    private int currentTargetIndex = 0;

    [Header("These fields are for display only")]
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Target currentTarget = null;

    [Header("Movement Settings")]
    [SerializeField] private float targetReachDistance = 1.5f;

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

        SelectNextTarget();
    }

    private void Update()
    {
        if (currentTarget == null)
        {
            SelectNextTarget();
        }
        else
        {
            // Check if we've reached the current target
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (distanceToTarget <= targetReachDistance)
            {
                SelectNextTarget();
            }
        }

        if (navMeshAgent.hasPath)
        {
            FaceDirection();
        }
    }

    private void SelectNextTarget()
    {
        if (allTargets != null && allTargets.Length > 0)
        {
            // Remove destroyed targets from the array
            allTargets = allTargets.Where(t => t != null).ToArray();

            if (currentTargetIndex < allTargets.Length)
            {
                currentTarget = allTargets[currentTargetIndex];
                navMeshAgent.destination = currentTarget.transform.position;
                Debug.Log($"Moving to target {currentTargetIndex + 1}/{allTargets.Length}: {currentTarget.name}");
                currentTargetIndex++;
            }
            else
            {
                Debug.Log("No more targets remaining!");
                currentTarget = null;
            }
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
        return currentTarget ? currentTarget.transform.position : transform.position;
    }
}