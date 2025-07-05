// House.cs - 增加了升级逻辑
using UnityEngine;

public class House : MonoBehaviour
{
    public PopulationTier currentTier;

    [Header("状态")]
    [Range(0, 20)]
    public int currentHappiness = 10;

    private int _residentCount;
    private float _consumptionIntervalInSeconds = 60f;
    private bool _isActivated = false;

    // --- 为了方便测试，我们添加一个简单的交互方式 ---
    // 当鼠标点击这个房屋时，尝试进行升级
    private void OnMouseDown()
    {
        TryToUpgrade();
    }

    public void ActivateHouse()
    {
        if (_isActivated) return;
        if (currentTier == null) { /* ... 错误处理 ... */ return; }

        _isActivated = true;
        _residentCount = currentTier.residentsPerHouse;
        PopulationManager.Instance.RegisterHouse(this);
        PopulationManager.Instance.UpdatePopulation(currentTier, _residentCount);
        InvokeRepeating(nameof(ConsumeNeeds), _consumptionIntervalInSeconds, _consumptionIntervalInSeconds);
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
        if (currentTier == null) return;
        foreach (var need in currentTier.needs)
        {
            if (ResourceManager.Instance.TryConsumeWarehouseItem(need.item, need.consumptionPerMinute))
            {
                currentHappiness++;
            }
            else
            {
                currentHappiness -= 2;
            }
        }
        currentHappiness = Mathf.Clamp(currentHappiness, 0, 20);
    }

    // <<< 新增：检查是否满足升级条件的方法 >>>
    public bool CanUpgrade()
    {
        if (currentTier.nextTier == null) return false; // 没有下一级，不能升级
        if (currentHappiness < currentTier.HappinessToUpgrade) return false; // 幸福度不够

        // 检查材料是否足够
        foreach (var material in currentTier.upgradeMaterials)
        {
            if (ResourceManager.Instance.GetWarehouseStock(material.item) < material.amount)
            {
                //Debug.Log($"升级失败：缺少材料 {material.item.itemName}");
                return false; // 材料不足
            }
        }
        return true;
    }

    // <<< 新增：尝试执行升级的方法 >>>
    public void TryToUpgrade()
    {
        if (!CanUpgrade())
        {
            Debug.Log("升级条件未满足！");
            return;
        }

        // 1. 消耗升级材料
        foreach (var material in currentTier.upgradeMaterials)
        {
            ResourceManager.Instance.TryConsumeWarehouseItem(material.item, material.amount);
        }

        // 2. 更新人口：先注销旧阶层，再注册新阶层
        PopulationManager.Instance.UpdatePopulation(currentTier, -_residentCount);

        // 3. 升级！切换到下一个阶层
        currentTier = currentTier.nextTier;
        _residentCount = currentTier.residentsPerHouse;
        PopulationManager.Instance.UpdatePopulation(currentTier, _residentCount);

        // 4. 重置幸福度到基础值
        currentHappiness = 10;

        Debug.Log($"<color=cyan>房屋升级成功！现在是 {currentTier.tierName}，人口变为 {_residentCount}！</color>");

        // 5. (未来) 在这里可以加上更换房屋模型、播放特效和音效的代码
    }
}