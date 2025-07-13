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
    public bool actsAsObstacle = true; // 默认设为true，这样您现有的建筑就不会受影响
                                       // <<< 在这里添加新的开关 >>>
    [Tooltip("如果此对象拥有生命值和血条，则勾选此项")]
    public bool hasHealthSystem = true; // 默认设为true，以兼容现有建筑

    // <<< 1. 在这里添加一个公共变量，用于链接破坏特效的Prefab >>>
    [Header("Effects")]
    public GameObject destructionEffectPrefab;

    private void Start()
    {
        // <<< 修改这里 >>>
        // 只有在启用生命值系统时，才初始化生命值并更新UI
        if (hasHealthSystem)
        {
            constHealth = constMaxHealth;
            UpdateHealthUI();
        }
    }

    private void UpdateHealthUI()
    {
        // <<< 修改这里 >>>
        // 只有在启用生命值系统且healthTracker已分配时，才更新滑块
        if (hasHealthSystem && healthTracker != null)
        {
            healthTracker.UpdateSliderValue(constHealth, constMaxHealth);
        }

        // 销毁逻辑保持不变，因为一个没有血条的物体也可能被摧毁
        if (constHealth <= 0)
        {
            ResourceManager.Instance.UpdateBuildingChanged(buildingType, false, buildPosition);
            SoundManager.Instance.PlayBuildingDestructionSound();

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
                ResourceManager.Instance.SellBuilding(buildingType);
            }
        }
        
    }

    public void TakeDamage(int damage)
    {
        // <<< 在方法开头添加一个“守护语句” >>>
        // 如果此对象没有生命值系统，则直接返回，不执行任何操作
        if (!hasHealthSystem) return;

        constHealth -= damage;
        UpdateHealthUI();
    }

    public void ConstructableWasPlaced(Vector3 position)
    {
        buildPosition = position;

        inPreviewMode = false;

        // <<< 修改这里 >>>
        // 仅在启用生命值系统且healthTracker已分配时，才激活UI
        if (hasHealthSystem && healthTracker != null)
        {
            healthTracker.gameObject.SetActive(true);
        }

        // <<< 我们在这里添加一个判断条件 >>>
        // 只有当 actsAsObstacle 为 true 时，才去激活障碍物
        if (actsAsObstacle)
        {
            ActivateObstacle();
        }
        // <<< 修改结束 >>>

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
