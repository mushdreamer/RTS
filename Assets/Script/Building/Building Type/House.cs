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
        if (currentTier == null) return;

        foreach (var need in currentTier.needs)
        {
            // <<< 修改点 2：计算本次（每秒）应该消耗的数量 >>>
            // (每分钟消耗量 / 60秒) * 本次消耗的间隔时间
            float amountToConsume = (need.consumptionPerMinute / 60f) * consumptionInterval;

            if (ResourceManager.Instance.TryConsumeWarehouseItem(need.item, amountToConsume))
            {
                // 为了让幸福度变化不那么剧烈，我们可以让它在满足时缓慢增加
                // 比如每10秒才加一点幸福度，这里我们暂时保持原样，让效果更明显
                currentHappiness++;
            }
            else
            {
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