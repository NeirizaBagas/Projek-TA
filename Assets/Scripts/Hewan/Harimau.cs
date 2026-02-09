using UnityEngine;

public class Harimau : AnimalBase
{
    public override void PerformIdle()
    {
        Debug.Log("Idle");
    }

    public override void PerformRun()
    {
        Debug.Log("Run");
    }

    public override void PerformWalk()
    {
        Debug.Log("Walk");
    }

}
