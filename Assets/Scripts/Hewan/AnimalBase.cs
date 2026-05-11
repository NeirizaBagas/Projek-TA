using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class AnimalBase : MonoBehaviour, IAnimals
{
    [Header("Waypoints")]
    public Transform[] waypoints;
    protected int currentWaypointIndex = -1;
    public float stopDistance = 0.5f;

    [Header("Navmesh Agent")]
    protected NavMeshAgent agent;
    public float patrolSpeed = 2f;
    public float runSpeed = 5f;
    public float normalIdleDuration = 3f;
    public float panicIdleDuration = 1.5f;

    [Header("Detection")]
    [SerializeField] private float dayDetectionRadius = 5f;
    [SerializeField] private float nightDetectionRadius = 3f;
    [SerializeField] private LayerMask playerLayer;

    public bool isPlayerInRange;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stopDistance;
    }

    public int GetRandomWayPointIndex()
    {
        if (waypoints.Length <= 1) return 0;
        int newIndex = currentWaypointIndex;
        while (newIndex == currentWaypointIndex)
        {
            newIndex = Random.Range(0, waypoints.Length);
        }
        return newIndex;
    }

    public bool IsReachedDestination()
    {
        return !agent.pathPending && agent.remainingDistance <= stopDistance;
    }

    public void UpdateCurrentIndex(int index)
    {
        currentWaypointIndex = index;
    }

    protected float GetCurrentTime()
    {   
        return DayNightCycle.isNight ? nightDetectionRadius : dayDetectionRadius;
    }

    public bool IsPlayerInRange()
    {
        float detectionRadius = GetCurrentTime();
        bool isInRange = Physics.CheckSphere(transform.position, detectionRadius, playerLayer);
        if (isInRange && !PlayerMovement.isCrouching)
        {
            isPlayerInRange = true;
            return true;
        }
        isPlayerInRange = false;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = DayNightCycle.isNight ? Color.red : Color.green;
            Gizmos.DrawWireSphere(transform.position, GetCurrentTime());
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, dayDetectionRadius);
        }
    }

    public abstract void PerformIdle();
    public abstract void PerformWalk();
}
