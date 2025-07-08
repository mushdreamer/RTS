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

    // <<< 1. 在这里添加一个公共变量，用于链接破坏特效的Prefab >>>
    [Header("Effects")]
    public GameObject destructionEffectPrefab;

    private void Start()
    {
        constHealth = constMaxHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        healthTracker.UpdateSliderValue(constHealth, constMaxHealth);

        if (constHealth <= 0)
        {
            ResourceManager.Instance.UpdateBuildingChanged(buildingType, false, buildPosition);

            SoundManager.Instance.PlayBuildingDestructionSound();//future when we have more sounds //SoundManager.Instance.PlayBuildingDestructionSound(buildingType);

            // <<< 2. 在销毁对象前，实例化破坏特效 >>>
            if (destructionEffectPrefab != null)
            {
                Instantiate(destructionEffectPrefab, transform.position, transform.rotation);
            }
            // <<< ----------------------------------- >>>

            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (inPreviewMode == false)
        {
            if (constHealth > 0 && buildPosition != Vector3.zero)
            {
                ResourceManager.Instance.SellBuilding(buildingType);
            }
        }
        
    }

    public void TakeDamage(int damage)
    {
        constHealth -= damage;
        // --- Add this English diagnostic line! ---
        //Debug.Log($"[DIAGNOSTIC] {name} took {damage} damage. Health remaining: {constHealth} / {constMaxHealth}");
        UpdateHealthUI();
    }

    public void ConstructableWasPlaced(Vector3 position)
    {
        buildPosition = position;

        inPreviewMode = false;

        healthTracker.gameObject.SetActive(true);
        ActivateObstacle();

        GetComponent<House>()?.ActivateHouse();
        GetComponent<ProductionBuilding>()?.ActivateBuilding();

        if (isEnemy)
        {
            gameObject.tag = "Enemy";
        }

        // =====================================================================
        // <<< 在这里添加以下新代码 >>>

        // 查找这个游戏对象上是否存在能源生产者组件 (IEnergyProducer)
        var producer = GetComponent<IEnergyProducer>();
        // 如果找到了，就将该组件本身（作为MonoBehaviour）启用
        if (producer != null && producer is MonoBehaviour producerComponent)
        {
            producerComponent.enabled = true;
        }

        // 同样，查找是否存在能源消费者组件 (IEnergyConsumer)
        var consumer = GetComponent<IEnergyConsumer>();
        // 如果找到了，就启用它
        if (consumer != null && consumer is MonoBehaviour consumerComponent)
        {
            consumerComponent.enabled = true;
        }

        // =====================================================================
    }

    private void ActivateObstacle()
    {
        if (isEnemy)
        {
            gameObject.tag = "Enemy";
        }

        obstacle = GetComponentInChildren<NavMeshObstacle>();
        obstacle.enabled = true;
    }
}
