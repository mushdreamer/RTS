using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitIdleState : StateMachineBehaviour
{
    AttackController attackController;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.transform.GetComponent<AttackController>();
        attackController.SetIdleMaterial();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //check if there is an available target
        if (attackController.targetToAttack != null)
        {
            //transition to follow state
            animator.SetBool("isFollowing", true);
        }
    }
}
