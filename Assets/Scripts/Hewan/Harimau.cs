using UnityEngine;

public class Harimau : AnimalBase
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public override void PerformIdle()
    {
        animator.SetFloat("Vert", 0);
        animator.SetFloat("State", 0);
    }

    public override void PerformRun()
    {
        animator.SetFloat("Vert", 1);
        animator.SetFloat("State", 1);
    }

    public override void PerformWalk()
    {
        animator.SetFloat("Vert", 1);
        animator.SetFloat("State", 0);
    }

}
