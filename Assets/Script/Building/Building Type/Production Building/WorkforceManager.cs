// WorkforceManager.cs - 【联动修复最终版】
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// 新增一个辅助类，用来存储每次劳动力分配的详细信息
public class WorkforceAllocation
{
    public PopulationTier Tier { get; set; }
    public int Amount { get; set; }
    public GameObject Building { get; set; } // 引用请求劳动力的建筑
}

public class WorkforceManager : MonoBehaviour
{
    public static WorkforceManager Instance { get; private set; }

    // 【核心数据结构】记录每个建筑的分配情况 (Key: 建筑的位置, Value: 分配详情)
    private Dictionary<Vector3Int, WorkforceAllocation> _buildingAllocations = new Dictionary<Vector3Int, WorkforceAllocation>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    void Start()
    {
        // 每隔2.5秒进行一次状态同步检查
        InvokeRepeating(nameof(ReconcileWorkforceState), 2.5f, 2.5f);
    }

    // 状态同步方法，当人口不足时，会找到对应建筑并“裁员”
    private void ReconcileWorkforceState()
    {
        if (PopulationManager.Instance == null) return;

        var totalAssignedByTier = GetTotalAssignedWorkforceByTier();

        foreach (var tier in totalAssignedByTier.Keys)
        {
            int totalInTier = PopulationManager.Instance.GetPopulation(tier);
            int assignedInTier = totalAssignedByTier[tier];

            if (assignedInTier > totalInTier)
            {
                Debug.LogWarning($"[WorkforceManager] 同步检查: '{tier.tierName}'阶层分配数({assignedInTier}) > 总人口({totalInTier})。开始修正...");

                var buildingsToCorrect = _buildingAllocations.Where(pair => pair.Value.Tier == tier).ToList();

                // 【联动核心】直接移除这些建筑的劳动力记录，强制它们停工
                foreach (var pair in buildingsToCorrect)
                {
                    Debug.Log($"正在撤销建筑 {pair.Value.Building.name} (位于 {pair.Key}) 的劳动力。");
                    _buildingAllocations.Remove(pair.Key);
                }
            }
        }
    }

    // 【重构】请求劳动力，需要传入建筑的GameObject
    public int RequestWorkforce(PopulationTier tier, int amountRequested, GameObject building)
    {
        Vector3Int buildingPosition = new Vector3Int(Mathf.RoundToInt(building.transform.position.x), Mathf.RoundToInt(building.transform.position.y), Mathf.RoundToInt(building.transform.position.z));

        // 如果该建筑已有工人，先释放旧的（这可以简化建筑端的逻辑）
        if (_buildingAllocations.ContainsKey(buildingPosition))
        {
            ReleaseWorkforce(building);
        }

        int totalPopulationOfTier = PopulationManager.Instance.GetPopulation(tier);
        int totalAssignedOfTier = GetAssignedWorkforce(tier);
        int availableWorkforce = totalPopulationOfTier - totalAssignedOfTier;
        int amountToAssign = Mathf.Min(amountRequested, availableWorkforce);

        if (amountToAssign > 0)
        {
            var allocation = new WorkforceAllocation { Tier = tier, Amount = amountToAssign, Building = building };
            _buildingAllocations[buildingPosition] = allocation;
            return amountToAssign;
        }
        return 0;
    }

    // 【重构】释放劳动力，通过建筑的GameObject来操作
    public void ReleaseWorkforce(GameObject building)
    {
        Vector3Int buildingPosition = new Vector3Int(Mathf.RoundToInt(building.transform.position.x), Mathf.RoundToInt(building.transform.position.y), Mathf.RoundToInt(building.transform.position.z));
        if (_buildingAllocations.ContainsKey(buildingPosition))
        {
            _buildingAllocations.Remove(buildingPosition);
        }
    }

    // --- 查询方法 ---
    public int GetAssignedWorkforce(PopulationTier tier)
    {
        if (tier == null) return 0;
        return _buildingAllocations.Values.Where(alloc => alloc.Tier == tier).Sum(alloc => alloc.Amount);
    }

    public int GetTotalAssignedWorkforce()
    {
        return _buildingAllocations.Values.Sum(alloc => alloc.Amount);
    }

    private Dictionary<PopulationTier, int> GetTotalAssignedWorkforceByTier()
    {
        var dictionary = new Dictionary<PopulationTier, int>();
        foreach (var alloc in _buildingAllocations.Values)
        {
            if (!dictionary.ContainsKey(alloc.Tier))
            {
                dictionary[alloc.Tier] = 0;
            }
            dictionary[alloc.Tier] += alloc.Amount;
        }
        return dictionary;
    }
}