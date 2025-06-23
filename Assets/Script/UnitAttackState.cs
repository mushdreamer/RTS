using UnityEngine;
using UnityEngine.AI;

public class UnitAttackState : StateMachineBehaviour
{
    private NavMeshAgent agent;
    private AttackController attackController;

    public float stopAttackingDistance = 1.2f;
    public float attackRate = 1.0f;
    private float attackTimer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        attackController = animator.GetComponent<AttackController>();

        attackController.SetAttackMaterial();
        if (attackController.muzzleEffect != null)
        {
            attackController.muzzleEffect.gameObject.SetActive(true);
        }

        if (agent.isActiveAndEnabled)
        {
            agent.isStopped = true;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackController.targetToAttack == null || animator.transform.GetComponent<UnitMovement>().isCommandToMove)
        {
            animator.SetBool("isAttacking", false);
            return;
        }

        LookAtTarget(animator.transform);

        float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);
        if (distanceFromTarget > stopAttackingDistance)
        {
            animator.SetBool("isAttacking", false);
        }
        else
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                Attack();
                attackTimer = 1f / attackRate;
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackController.muzzleEffect != null)
        {
            attackController.muzzleEffect.gameObject.SetActive(false);
        }

        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = false;
        }

        attackTimer = 0f;
    }

    private void LookAtTarget(Transform unitTransform)
    {
        if (attackController.targetToAttack == null) return;

        Vector3 direction = attackController.targetToAttack.position - unitTransform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            unitTransform.rotation = Quaternion.Slerp(unitTransform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void Attack()
    {
        if (attackController.targetToAttack == null) return;

        SoundManager.Instance.PlayInfantryAttackSound();

        var damageToInflict = attackController.unitDamage;

        // ---【本次测试的核心代码】---
        // 每次攻击时，在控制台打印出伤害值和时间
        Debug.Log("--- 攻击！造成伤害: " + damageToInflict + " | 时间: " + Time.time);

        var damageable = attackController.targetToAttack.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damageToInflict);
        }

        /*var damageToInflict = attackController.unitDamage;

        SoundManager.Instance.PlayInfantryAttackSound();

        attackController.targetToAttack.GetComponent<Unit>().TakeDamage(damageToInflict);*/
    }
}