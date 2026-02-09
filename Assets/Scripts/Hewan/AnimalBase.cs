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
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

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

    public abstract void PerformIdle();
    public abstract void PerformWalk();
    public abstract void PerformRun();
}
