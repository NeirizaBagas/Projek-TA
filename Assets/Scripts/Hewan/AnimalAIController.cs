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
    private NavMeshAgent agent;

    private void Awake()
    {
        animal = GetComponent<AnimalBase>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        TransitionToState(AnimalState.Idle);
    }

    private void Update()
    {
        switch (currentState)
        {
            case AnimalState.Idle:
                if (animal.IsPlayerInRange())
                {
                    TransitionToState(AnimalState.Run);
                    break;
                }
                animal.PerformIdle();
                timer -= Time.deltaTime;
                if (timer <= 0) TransitionToState(AnimalState.Walk);
                break;
            case AnimalState.Walk:
                if (animal.IsPlayerInRange())
                {
                    TransitionToState(AnimalState.Run);
                    break;
                }
                animal.PerformWalk();
                if (animal.IsReachedDestination()) TransitionToState(AnimalState.Idle);
                break;
            case AnimalState.Run:
                animal.PerformWalk();
                if (animal.IsReachedDestination()) TransitionToState(AnimalState.Idle);
                break;
        }
    }

    private void TransitionToState(AnimalState nextState)
    {
        if (nextState == AnimalState.Idle)
        {
            if (currentState == AnimalState.Run) timer = animal.panicIdleDuration;
            else timer = animal.normalIdleDuration;
            agent.isStopped = true;
            
        }
        else if (nextState == AnimalState.Walk)
        {
            SetDestinationAndSpeed(animal.patrolSpeed);
        }
        else if (nextState == AnimalState.Run)
        {
            SetDestinationAndSpeed(animal.runSpeed);
        }
        currentState = nextState;
    }

    private void SetDestinationAndSpeed(float speed)
    {
        int nextWayPoint = animal.GetRandomWayPointIndex();
        animal.UpdateCurrentIndex(nextWayPoint);
        agent.SetDestination(animal.waypoints[nextWayPoint].position);
        agent.speed = speed;
        agent.isStopped = false;
    }
}
