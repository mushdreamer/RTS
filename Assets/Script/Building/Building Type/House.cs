// House.cs - 修正版本
using UnityEngine;

public class House : MonoBehaviour
{
    public PopulationTier currentTier;

    private int _residentCount;
    private float _consumptionIntervalInSeconds = 60f;
    private bool _isActivated = false; // <<< 新增：一个标志，用于判断房屋是否已被正式放置

    // Start 方法现在几乎是空的，因为逻辑被移走了
    void Start()
    {
        // 我们把所有逻辑都移到 ActivateHouse() 方法中
    }

    /// <summary>
    /// 激活房屋，注册人口并开始消耗。这个方法只应在房屋被正式放置后调用一次。
    /// </summary>
    public void ActivateHouse()
    {
        // 防止这个方法被重复调用
        if (_isActivated) return;

        if (currentTier != null)
        {
            _residentCount = currentTier.residentsPerHouse;
            PopulationManager.Instance.UpdatePopulation(currentTier, _residentCount);
            InvokeRepeating(nameof(ConsumeNeeds), _consumptionIntervalInSeconds, _consumptionIntervalInSeconds);
            _isActivated = true; // <<< 将房屋标记为“已激活”
            Debug.Log($"房屋 {this.gameObject.name} 已被激活，人口 +{_residentCount}");
        }
        else
        {
            Debug.LogError("无法激活房屋，因为 currentTier 为空!", this.gameObject);
        }
    }

    void OnDestroy()
    {
        // <<< 修改：只有已激活的房屋在被摧毁时，才需要注销人口
        if (_isActivated && PopulationManager.Instance != null && currentTier != null)
        {
            PopulationManager.Instance.UpdatePopulation(currentTier, -_residentCount);
            Debug.Log($"已激活的房屋 {this.gameObject.name} 已被摧毁，人口 -{_residentCount}");
        }
    }

    private void ConsumeNeeds()
    {
        if (currentTier == null) return;

        Debug.Log($"房屋 {this.name} 正在为【{currentTier.tierName}】居民消耗物资...");
        foreach (var need in currentTier.needs)
        {
            float amountToConsume = need.consumptionPerMinute;
            bool consumedSuccessfully = ResourceManager.Instance.TryConsumeWarehouseItem(need.item, amountToConsume);

            if (consumedSuccessfully)
            {
                Debug.Log($"成功消耗 {amountToConsume} 单位的【{need.item.itemName}】。");
            }
            else
            {
                Debug.LogWarning($"物资【{need.item.itemName}】库存不足，无法满足需求！");
            }
        }
    }
}