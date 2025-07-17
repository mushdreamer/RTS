// House.cs - 修正版本
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class House : MonoBehaviour
{
    public PopulationTier currentTier;

    [Header("实时状态")]
    public List<HouseNeedState> trackedNeeds;
    public int currentResidents;
    public int maxResidents;
    public int currentHappiness;

    [Header("成长参数")]
    [Tooltip("基础居民数量")]
    public int baseResidents = 5;
    [Tooltip("每满足一个需求增加的居民")]
    public int residentsPerNeedMet = 5;

    private bool _isActivated = false;

    // --- 为了方便测试，我们添加一个简单的交互方式 ---
    private void OnMouseDown()
    {
        // 检查是否可以升级，如果可以就升级
        if (CanUpgrade())
        {
            TryToUpgrade();
        }
    }

    public void ActivateHouse()
    {
        if (_isActivated) return;

        PopulationManager.Instance.RegisterHouse(this);
        InitializeNeeds();
        RecalculateState();
        _isActivated = true;
    }

    void OnDestroy()
    {
        if (_isActivated && PopulationManager.Instance != null)
        {
            PopulationManager.Instance.UnregisterHouse(this);
        }
    }

    private void InitializeNeeds()
    {
        trackedNeeds = new List<HouseNeedState>();
        if (currentTier != null && currentTier.needs != null)
        {
            foreach (var need in currentTier.needs)
            {
                trackedNeeds.Add(new HouseNeedState(need));
            }
        }
    }

    public void RecalculateState()
    {
        if (currentTier == null) return;

        int needsMetCount = trackedNeeds.Count(n => n.isMet);

        maxResidents = baseResidents + (currentTier.needs.Count * residentsPerNeedMet);
        currentResidents = baseResidents + (needsMetCount * residentsPerNeedMet);
        currentResidents = Mathf.Min(currentResidents, maxResidents);

        currentHappiness = 10 + (needsMetCount * 2) - ((currentTier.needs.Count - needsMetCount) * 1);
        currentHappiness = Mathf.Clamp(currentHappiness, 0, 20);

        // 更新总人口，这里我们直接在PopulationManager中处理，确保数据同步
        PopulationManager.Instance.UpdatePopulationForHouse(this, currentResidents);
    }

    public bool CanUpgrade()
    {
        if (currentTier.nextTier == null) return false;
        if (currentResidents < maxResidents) return false;

        foreach (var material in currentTier.upgradeMaterials)
        {
            if (ResourceManager.Instance.GetWarehouseStock(material.item) < material.amount) return false;
        }
        return true;
    }

    /// <summary>
    /// 【新增】这就是UI脚本找不到的升级方法
    /// </summary>
    public void TryToUpgrade()
    {
        if (!CanUpgrade())
        {
            Debug.Log("升级条件未满足！");
            return;
        }

        // 消耗升级材料
        foreach (var material in currentTier.upgradeMaterials)
        {
            ResourceManager.Instance.TryConsumeWarehouseItem(material.item, material.amount);
        }

        // 从旧阶层注销并更新人口
        PopulationManager.Instance.UnregisterHouse(this);

        // 升级到新阶层
        currentTier = currentTier.nextTier;

        // 在新阶层重新注册并初始化
        PopulationManager.Instance.RegisterHouse(this);
        InitializeNeeds(); // 使用新阶层的需求重新初始化
        RecalculateState(); // 重新计算状态

        Debug.Log($"<color=cyan>房屋升级成功！现在是 {currentTier.tierName}！</color>");
    }
}