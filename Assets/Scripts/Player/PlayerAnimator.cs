using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayAttack()
    {
        anim?.SetTrigger("attack");
    }

    public void PlayDie()
    {
        anim?.SetTrigger("die");
    }

    public void PlaySkill()
    {
        anim?.SetTrigger("skill");
    }
}