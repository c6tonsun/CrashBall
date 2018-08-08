using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    private Animator p_Animator;

    void Start()
    {
        p_Animator = GetComponent<Animator>();
    }

    public void AnimateMovement(float input)
    {
        p_Animator.SetFloat("MovementInput", input);
    }

    public void AnimateCheer(bool winner)
    {
        p_Animator.SetBool("Winner", winner);
    }
}
