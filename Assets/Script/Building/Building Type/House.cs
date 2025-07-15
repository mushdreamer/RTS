// House.cs - 升级为高频消耗版本
using UnityEngine;

public class House : MonoBehaviour
{
    public PopulationTier currentTier;

    [Header("状态")]
    [Range(0, 20)]
    public int currentHappiness = 10;

    // <<< 修改点 1：让消耗间隔可以在Inspector里设置，并默认改为1秒 >>>
    [Header("消耗设置")]
    [Tooltip("每隔多少秒进行一次物资消耗和幸福度更新")]
    public float consumptionInterval = 1f;

    private int _residentCount;
    private bool _isActivated = false;

    // <<< 新增的变量 >>>
    private PlacementSystem placementSystem;
    private Constructable constructable;
    private bool isConnectedToRoad = false; // 用于缓存连接状态

    // --- 为了方便测试，我们添加一个简单的交互方式 ---
    private void OnMouseDown()
    {
        TryToUpgrade();
    }

    public void ActivateHouse()
    {
        if (_isActivated) return;
        if (currentTier == null) { /* ... 错误处理 ... */ return; }

        _isActivated = true;

        // <<< 在这里添加初始化代码 >>>
        placementSystem = FindObjectOfType<PlacementSystem>(); // 找到场景中的PlacementSystem
        constructable = GetComponent<Constructable>(); // 获取挂在同一对象上的Constructable组件

        _residentCount = currentTier.residentsPerHouse;
        PopulationManager.Instance.RegisterHouse(this);
        PopulationManager.Instance.UpdatePopulation(currentTier, _residentCount);

        // 使用我们在Inspector里设置的 consumptionInterval
        InvokeRepeating(nameof(ConsumeNeeds), consumptionInterval, consumptionInterval);
    }

    void OnDestroy()
    {
        if (_isActivated && PopulationManager.Instance != null)
        {
            PopulationManager.Instance.UnregisterHouse(this);
            PopulationManager.Instance.UpdatePopulation(currentTier, -_residentCount);
        }
    }

    private void ConsumeNeeds()
    {
        if (currentTier == null || placementSystem == null || constructable == null) return;

        // 1. 调用 PlacementSystem 的功能来检查连接状态
        isConnectedToRoad = placementSystem.IsBuildingConnectedToRoad(constructable.buildPosition);

        // 2. 如果没有连接到道路，则直接惩罚幸福度，并且不消耗任何物资
        if (!isConnectedToRoad)
        {
            currentHappiness -= 2; // 或者一个更大的惩罚值
            currentHappiness = Mathf.Clamp(currentHappiness, 0, 20);
            // Debug.Log(gameObject.name + " 未连接到道路，无法获取物资！");
            return; // 直接结束，不执行后续的消耗逻辑
        }

        // 3. 如果已连接，才执行正常的物资消耗逻辑
        foreach (var need in currentTier.needs)
        {
            float amountToConsume = (need.consumptionPerMinute / 60f) * consumptionInterval;

            if (ResourceManager.Instance.TryConsumeWarehouseItem(need.item, amountToConsume))
            {
                // 物资充足，增加幸福度
                currentHappiness++;
            }
            else
            {
                // 物资不足，减少幸福度
                currentHappiness -= 2;
            }
        }
        currentHappiness = Mathf.Clamp(currentHappiness, 0, 20);
    }

    public bool CanUpgrade()
    {
        if (currentTier.nextTier == null) { return false; }
        if (currentHappiness < currentTier.HappinessToUpgrade) { return false; }

        foreach (var material in currentTier.upgradeMaterials)
        {
            if (ResourceManager.Instance.GetWarehouseStock(material.item) < material.amount) { return false; }
        }
        return true;
    }

    public void TryToUpgrade()
    {
        if (!CanUpgrade())
        {
            // 可以在这里加一个音效或视觉提示
            return;
        }

        foreach (var material in currentTier.upgradeMaterials)
        {
            ResourceManager.Instance.TryConsumeWarehouseItem(material.item, material.amount);
        }

        PopulationManager.Instance.UpdatePopulation(currentTier, -_residentCount);
        currentTier = currentTier.nextTier;
        _residentCount = currentTier.residentsPerHouse;
        PopulationManager.Instance.UpdatePopulation(currentTier, _residentCount);
        currentHappiness = 10;
        Debug.Log($"<color=cyan>房屋升级成功！现在是 {currentTier.tierName}！</color>");
    }
}