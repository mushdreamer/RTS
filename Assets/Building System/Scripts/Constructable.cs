// Constructable.cs - 修正版
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Constructable : MonoBehaviour, IDamageable
{
    private float constHealth;
    public float constMaxHealth;

    public HealthTracker healthTracker;

    public bool isEnemy = false;

    NavMeshObstacle obstacle;

    public BuildingType buildingType;

    public Vector3 buildPosition;

    public bool inPreviewMode;

    [Header("逻辑开关")]
    [Tooltip("如果此对象应作为导航障碍物，则勾选此项")]
    public bool actsAsObstacle = true;
    [Tooltip("如果此对象拥有生命值和血条，则勾选此项")]
    public bool hasHealthSystem = true;

    [Header("Effects")]
    public GameObject destructionEffectPrefab;

    private void Start()
    {
        if (hasHealthSystem)
        {
            constHealth = constMaxHealth;
            UpdateHealthUI();
        }
    }

    private void UpdateHealthUI()
    {
        if (hasHealthSystem && healthTracker != null)
        {
            healthTracker.UpdateSliderValue(constHealth, constMaxHealth);
        }

        if (constHealth <= 0)
        {
            //ResourceManager.Instance.UpdateBuildingChanged(buildingType, false, buildPosition);
            //SoundManager.Instance.PlayBuildingDestructionSound();

            if (destructionEffectPrefab != null)
            {
                Instantiate(destructionEffectPrefab, transform.position, transform.rotation);
            }

            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (inPreviewMode == false)
        {
            if (constHealth > 0 && buildPosition != Vector3.zero)
            {
                //ResourceManager.Instance.SellBuilding(buildingType);
            }
        }

    }

    public void TakeDamage(int damage)
    {
        if (!hasHealthSystem) return;

        constHealth -= damage;
        UpdateHealthUI();
    }

    public void ConstructableWasPlaced(Vector3 position, Vector3Int gridPosition)
    {
        buildPosition = position;
        inPreviewMode = false;

        if (hasHealthSystem && healthTracker != null)
        {
            healthTracker.gameObject.SetActive(true);
        }

        if (actsAsObstacle)
        {
            ActivateObstacle();
        }

        // 激活House组件（如果存在），并传入网格坐标
        GetComponent<House>()?.ActivateHouse(gridPosition);

        // ▼▼▼ 【核心修改点在这里】 ▼▼▼
        // 激活ProductionBuilding组件（如果存在），并传入网格坐标
        GetComponent<ProductionBuilding>()?.ActivateBuilding(gridPosition);

        if (isEnemy)
        {
            gameObject.tag = "Enemy";
        }

        // 查找和启用能源组件的逻辑...
        var producer = GetComponent<IEnergyProducer>();
        if (producer != null && producer is MonoBehaviour producerComponent)
        {
            producerComponent.enabled = true;
        }

        var consumer = GetComponent<IEnergyConsumer>();
        if (consumer != null && consumer is MonoBehaviour consumerComponent)
        {
            consumerComponent.enabled = true;
        }
    }

    private void ActivateObstacle()
    {
        if (isEnemy)
        {
            gameObject.tag = "Enemy";
        }

        obstacle = GetComponentInChildren<NavMeshObstacle>();
        if (obstacle != null)
        {
            obstacle.enabled = true;
        }
    }
}