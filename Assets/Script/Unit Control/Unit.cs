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

    //这个可以考虑加到别的里面，现在加在这里导致unitEnemy可以被选中并移动，但是把这个文件从unitEnemy上面去掉就会导致unitEnemy无法被攻击
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
