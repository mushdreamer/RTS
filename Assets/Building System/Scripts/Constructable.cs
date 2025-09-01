// Constructable.cs - 重构为使用接口的最终版
using UnityEngine;
using UnityEngine.AI;

public class Constructable : MonoBehaviour, IDamageable
{
    // --- 您的所有变量都保持不变 ---
    public float constHealth;
    public float constMaxHealth;
    public HealthTracker healthTracker;
    public bool isEnemy = false;
    NavMeshObstacle obstacle;
    public BuildingType buildingType;
    public Vector3 buildPosition;
    private PlacementSystem placementSystem;
    public bool inPreviewMode;
    [Header("逻辑开关")]
    public bool actsAsObstacle = true;
    public bool hasHealthSystem = true;
    [Header("Effects")]
    public GameObject destructionEffectPrefab;

    private void Awake()
    {
        placementSystem = FindObjectOfType<PlacementSystem>();
        if (hasHealthSystem)
        {
            constHealth = constMaxHealth;
            healthTracker?.UpdateSliderValue(constHealth, constMaxHealth);
        }
    }

    // ▼▼▼【核心修改点】▼▼▼
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

        // 【“一劳永逸”的激活逻辑】
        // 1. 找到这个游戏对象上所有“符合激活标准”的组件。
        IActivatableBuilding[] activatableComponents = GetComponents<IActivatableBuilding>();

        // 2. 告诉它们每一个去执行激活操作。
        foreach (var component in activatableComponents)
        {
            component.Activate(gridPosition);
        }
        // 无论你未来添加多少种新建筑，这段代码永远不需要再修改。

        if (isEnemy)
        {
            gameObject.tag = "Enemy";
        }
    }
    // ▲▲▲【修改结束】▲▲▲

    // --- 您的其他方法全部保持不变 ---
    public void TakeDamage(int damage)
    {
        if (!hasHealthSystem) return;
        constHealth -= damage;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthTracker != null && hasHealthSystem)
        {
            healthTracker.UpdateSliderValue(constHealth, constMaxHealth);
        }

        if (constHealth <= 0)
        {
            if (placementSystem != null)
            {
                placementSystem.ClearBuildingDataAt(buildPosition);
            }
            else
            {
                Debug.LogError("Constructable 无法找到 PlacementSystem! 网格数据可能没有被正确清除。");
            }

            if (destructionEffectPrefab != null)
            {
                Instantiate(destructionEffectPrefab, transform.position, transform.rotation);
            }
            Destroy(gameObject);
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