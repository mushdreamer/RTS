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
        // 条件1：检查是否存在下一级
        if (currentTier.nextTier == null)
        {
            Debug.Log("Upgrade Failed! Population tier is" + currentTier.tierName + "There is nothing to upgrade");
            return false;
        }

        // 条件2：检查幸福度是否达标
        if (currentHappiness < currentTier.HappinessToUpgrade)
        {
            // 使用富文本让关键数字更显眼
            Debug.Log($"Upgrade Failed! You don't have enough happiness. You need:<b>{currentTier.HappinessToUpgrade}</b>, Now you have: <b>{currentHappiness}</b>");
            return false;
        }

        // 条件3：检查升级材料是否足够
        foreach (var material in currentTier.upgradeMaterials)
        {
            // 确保 material.item 不为空，避免潜在的错误
            if (material.item == null)
            {
                continue;
            }

            float stock = ResourceManager.Instance.GetWarehouseStock(material.item);
            if (stock < material.amount)
            {
                Debug.LogWarning($"Upgrade Failed! You don't have enough materials <b>{material.item.itemName}</b>。You need: {material.amount}, Now you have: {stock:F0}");
                return false;
            }
        }

        // 如果所有检查都通过了
        Debug.Log("<color=green>Ready to be upgraded</color>");
        return true;
    }

    // <<< 新增：尝试执行升级的方法 >>>
    public void TryToUpgrade()
    {
        // 现在这个方法只负责调用检查，如果CanUpgrade返回false，它什么也不做。
        // 所有的失败日志都由CanUpgrade自己来打印。
        if (!CanUpgrade())
        {
            return;
        }

        // 1. 消耗升级材料
        foreach (var material in currentTier.upgradeMaterials)
        {
            ResourceManager.Instance.TryConsumeWarehouseItem(material.item, material.amount);
        }

        // 2. 更新人口
        PopulationManager.Instance.UpdatePopulation(currentTier, -_residentCount);

        // 3. 升级！
        currentTier = currentTier.nextTier;
        _residentCount = currentTier.residentsPerHouse;
        PopulationManager.Instance.UpdatePopulation(currentTier, _residentCount);

        // 4. 重置幸福度
        currentHappiness = 10;

        Debug.Log($"<color=cyan>You upgrade your house! Now your population tier is {currentTier.tierName}，The population is {_residentCount}！</color>");

        // 5. (未来) 更换模型...
    }
}