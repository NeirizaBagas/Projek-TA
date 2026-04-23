using UnityEngine;

public class HewanDarat : AnimalBase
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public override void PerformIdle()
    {
        animator.SetFloat("Vert", 0);
    }

    public override void PerformWalk()
    {
        animator.SetFloat("Vert", 1);
    }

}
