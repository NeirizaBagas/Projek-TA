using UnityEngine;
using UnityEngine.AI;

public enum AnimalState
{
    Idle,
    Walk,
    Run
}

public class AnimalAIController : MonoBehaviour
{
    private AnimalBase animal;
    public AnimalState currentState = AnimalState.Idle;

    private float timer;
    public float idleDuration = 3f;

    private void Awake()
    {
        animal = GetComponent<AnimalBase>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case AnimalState.Idle:
                animal.PerformIdle();
                timer -= Time.deltaTime;
                if (timer <= 0) TransitionToState(AnimalState.Walk);
                break;
            case AnimalState.Walk:
                animal.PerformWalk();
                if (animal.IsReachedDestination()) TransitionToState(AnimalState.Idle);
                break;
            case AnimalState.Run:
                animal.PerformWalk();
                break;
        }
    }

    private void TransitionToState(AnimalState nextState)
    {
        currentState = nextState;
        if (nextState == AnimalState.Idle)
        {
            timer = idleDuration;
        }
        else if (nextState == AnimalState.Walk)
        {
            int nextWayPoint = animal.GetRandomWayPointIndex();
            NavMeshAgent agent = animal.GetComponent<NavMeshAgent>();
            agent.SetDestination(animal.waypoints[nextWayPoint].position);
            agent.speed = animal.walkSpeed;
            agent.isStopped = false;
        }
    }
}