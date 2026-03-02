using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    public void HandlePlayerAnimator(bool isMoving, bool isSprinting)
    {
        if (!isMoving)
        {
            animator.SetBool("IsIdling", true);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
        }
        else
        {
            animator.SetBool("IsIdling", false);
            if (isSprinting)
            {
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsRunning", true);
            }
            else
            {
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsRunning", false);
            }
        }
    }

    public void HandleJumping(bool isJumping)
    {
        animator.SetBool("Jump", isJumping);
    }
}
