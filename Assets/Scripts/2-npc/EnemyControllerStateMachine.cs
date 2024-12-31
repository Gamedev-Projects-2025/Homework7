using UnityEngine;

[RequireComponent(typeof(Runawayer))]
[RequireComponent(typeof(Chaser))]
[RequireComponent(typeof(Destroyer))]
[RequireComponent(typeof(Rotator))]
public class EnemyControllerStateMachine : StateMachine
{
    [SerializeField] float radiusToWatch = 5f;
    [SerializeField] float probabilityToRotate = 0.2f;
    [SerializeField] float probabilityToStopRotating = 0.2f;

   
    [Tooltip("If checked, enemy will chase the player")]
    [SerializeField] bool chase = true;

    [Tooltip("If checked, enemy will run away when seeing the player.")]
    [SerializeField] bool runaway = false;

    [Tooltip("If checked enemy will enter the building and try to destroy the machine")]
    [SerializeField] bool goDestroy = false;

    [SerializeField] float destroyReachDistance = 1f;

    private Chaser chaser;
    private Runawayer runawayer;
    private Destroyer destroyer;
    private Rotator rotator;

    private float DistanceToTarget()
    {
        return Vector3.Distance(transform.position,
            runaway ? runawayer.TargetObjectPosition() : chaser.TargetObjectPosition());
    }

    private float DistanceToDestroyTarget()
    {
        return Vector3.Distance(transform.position, destroyer.TargetObjectPosition());
    }

    private void Awake()
    {
        chaser = GetComponent<Chaser>();
        runawayer = GetComponent<Runawayer>();
        destroyer = GetComponent<Destroyer>();
        rotator = GetComponent<Rotator>();

        base
        .AddState(rotator)     // Initial state
        .AddState(chaser)
        .AddState(runawayer)
        .AddState(destroyer)

        // Transitions when player is spotted (only if not in destroy mode)
        .AddTransition(rotator, () => !goDestroy && DistanceToTarget() <= radiusToWatch && chase, chaser)
        .AddTransition(rotator, () => !goDestroy && DistanceToTarget() <= radiusToWatch && runaway, runawayer)

        // Transitions to destroyer only when goDestroy is true
        .AddTransition(rotator, () => goDestroy, destroyer)
        .AddTransition(chaser, () => goDestroy, destroyer)
        .AddTransition(runawayer, () => goDestroy, destroyer)

        // Transitions when player is out of range
        .AddTransition(chaser, () => DistanceToTarget() > radiusToWatch, rotator)
        .AddTransition(runawayer, () => DistanceToTarget() > radiusToWatch, rotator)

        // Destroyer transitions - only go back to rotator when target is reached
        .AddTransition(destroyer, () => DistanceToDestroyTarget() <= destroyReachDistance || !goDestroy, rotator)

        // Rotation probability transitions
        .AddTransition(rotator, () => Random.Range(0f, 1f) < probabilityToStopRotating * Time.deltaTime, rotator)
        .AddTransition(rotator, () => Random.Range(0f, 1f) < probabilityToRotate * Time.deltaTime, rotator)
        ;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusToWatch);
    }
}