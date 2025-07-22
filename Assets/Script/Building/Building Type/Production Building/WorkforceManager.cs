// WorkforceManager.cs - 升级为劳动力调度中心
using UnityEngine;
using System.Collections.Generic;

public class WorkforceManager : MonoBehaviour
{
    public static WorkforceManager Instance { get; private set; }

    // 记录每个阶层【已分配】或【正在工作】的劳动力数量
    private Dictionary<PopulationTier, int> _assignedWorkforce = new Dictionary<PopulationTier, int>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    /// <summary>
    /// 处理一个建筑的劳动力申请
    /// </summary>
    /// <returns>实际分配到的工人数</returns>
    public int RequestWorkforce(PopulationTier tier, int amountRequested)
    {
        if (tier == null || amountRequested <= 0) return 0;

        // 1. 计算可用的闲置劳动力
        int totalPopulation = PopulationManager.Instance.GetPopulation(tier);
        int currentlyAssigned = GetAssignedWorkforce(tier);
        int availableWorkforce = totalPopulation - currentlyAssigned;

        // 2. 确定能分配多少人
        int amountToAssign = Mathf.Min(amountRequested, availableWorkforce);

        // 3. 如果能分配到工人，则更新记录
        if (amountToAssign > 0)
        {
            if (!_assignedWorkforce.ContainsKey(tier))
            {
                _assignedWorkforce[tier] = 0;
            }
            _assignedWorkforce[tier] += amountToAssign;
        }

        return amountToAssign;
    }

    /// <summary>
    /// 释放一个建筑占用的劳动力
    /// </summary>
    public void ReleaseWorkforce(PopulationTier tier, int amountToRelease)
    {
        if (tier == null || amountToRelease <= 0) return;

        if (_assignedWorkforce.ContainsKey(tier))
        {
            _assignedWorkforce[tier] -= amountToRelease;
            if (_assignedWorkforce[tier] < 0)
            {
                _assignedWorkforce[tier] = 0;
            }
        }
    }

    /// <summary>
    /// 获取某个阶层【已分配】的总劳动力
    /// </summary>
    public int GetAssignedWorkforce(PopulationTier tier)
    {
        if (tier != null && _assignedWorkforce.ContainsKey(tier))
        {
            return _assignedWorkforce[tier];
        }
        return 0;
    }

    /// <summary>
    /// 获取所有【已分配】的总劳动力
    /// </summary>
    public int GetTotalAssignedWorkforce()
    {
        int total = 0;
        foreach (var amount in _assignedWorkforce.Values)
        {
            total += amount;
        }
        return total;
    }
}