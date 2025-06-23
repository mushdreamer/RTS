using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour, IDamageable
{
    private float unitHealth;
    public float unitMaxHealth;

    public HealthTracker healthTracker;

    Animator animator;
    NavMeshAgent navMeshAgent;

    /*UnitType thisUnit;

    enum UnitType
    {
        Infantry,
        Tank
    }*/
    void Start()
    {
        UnitSelectionManager.Instance.allUnitsList.Add(gameObject);

        unitHealth = unitMaxHealth;
        UpdateHealthUI();

        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void UpdateHealthUI()
    {
        healthTracker.UpdateSliderValue(unitHealth, unitMaxHealth);

        if (unitHealth <= 0)
        {
            //dying logic

            //Destruction or dying animation

            //dying sound effect
            Destroy(gameObject);
            Debug.Log("destroy");
        }
    }

    private void OnDestroy()
    {
        UnitSelectionManager.Instance.allUnitsList.Remove(gameObject);
    }

    //������Կ��Ǽӵ�������棬���ڼ������ﵼ��unitEnemy���Ա�ѡ�в��ƶ������ǰ�����ļ���unitEnemy����ȥ���ͻᵼ��unitEnemy�޷�������
    public void TakeDamage(int damageToInflict)
    {
        unitHealth -= damageToInflict;
        UpdateHealthUI();
    }

    private void Update()
    {
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }
}
