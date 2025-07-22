// PopulationManager.cs - 最终修正版
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance { get; private set; }

    private Dictionary<PopulationTier, int> _totalPopulationByTier = new Dictionary<PopulationTier, int>();
    private Dictionary<PopulationTier, List<House>> _housesByTier = new Dictionary<PopulationTier, List<House>>();
    private Dictionary<House, int> _populationPerHouse = new Dictionary<House, int>();

    [Header("更新设置")]
    [Tooltip("每隔多少秒进行一次总需求计算")]
    public float needsUpdateInterval = 2.0f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    void Start()
    {
        InvokeRepeating(nameof(UpdateAllTierNeeds), needsUpdateInterval, needsUpdateInterval);
    }

    /// <summary>
    /// 【核心逻辑重写】
    /// </summary>
    private void UpdateAllTierNeeds()
    {
        if (_housesByTier.Keys.Count == 0 || BuildingConnector.Instance == null) return;

        // 1. 先更新所有房屋的连接状态
        foreach (var houseList in _housesByTier.Values)
        {
            foreach (var house in houseList)
            {
                house.isConnectedToWarehouse = BuildingConnector.Instance.CheckConnection(house.GetGridPosition(), BuildingType.Warehouse);
            }
        }

        // 2. 遍历每一个人口阶层
        foreach (PopulationTier tier in _housesByTier.Keys.ToList())
        {
            if (_housesByTier[tier].Count == 0) continue;

            // 3. 对该阶层的每一个需求（如鱼、衣服）进行独立计算
            foreach (Need need in tier.needs)
            {
                // 筛选出所有已连接到仓库的房屋
                var connectedHouses = _housesByTier[tier].Where(h => h.isConnectedToWarehouse).ToList();
                int connectedHouseCount = connectedHouses.Count;

                // 如果没有任何房屋连接，则该需求对所有房屋都不满足
                if (connectedHouseCount == 0)
                {
                    foreach (var house in _housesByTier[tier])
                    {
                        var state = house.trackedNeeds.FirstOrDefault(n => n.associatedNeed == need);
                        if (state != null) state.isMet = false;
                    }
                    continue; // 继续检查下一个需求
                }

                // 计算已连接房屋的总需求量
                float consumptionRate = (need.consumptionPerMinute / 60f) * needsUpdateInterval;
                float totalDemand = consumptionRate * connectedHouseCount;

                // 检查仓库是否有足够的物资来满足这些已连接的房屋
                bool canMeetDemand = ResourceManager.Instance.GetWarehouseStock(need.item) >= totalDemand;

                // 如果可以满足，则消耗资源
                if (canMeetDemand)
                {
                    ResourceManager.Instance.TryConsumeWarehouseItem(need.item, totalDemand);
                }

                // 4. 最后，根据连接状态和物资满足情况，更新每一个房屋的需求状态
                foreach (var house in _housesByTier[tier])
                {
                    var state = house.trackedNeeds.FirstOrDefault(n => n.associatedNeed == need);
                    if (state != null)
                    {
                        // 需求被满足的条件是：房屋已连接 并且 物资充足
                        state.isMet = house.isConnectedToWarehouse && canMeetDemand;
                    }
                }
            }
        }

        // 5. 在所有计算结束后，命令所有房屋根据最新的需求满足状态，更新自己的人口和幸福度
        foreach (var houseList in _housesByTier.Values)
        {
            foreach (var house in houseList)
            {
                house.RecalculateState();
            }
        }
    }

    // --- 后续方法保持不变 ---

    public void RegisterHouse(House house)
    {
        PopulationTier tier = house.currentTier;
        if (!_housesByTier.ContainsKey(tier))
        {
            _housesByTier[tier] = new List<House>();
            _totalPopulationByTier[tier] = 0;
        }
        _housesByTier[tier].Add(house);
        _populationPerHouse[house] = 0;
    }

    public void UnregisterHouse(House house)
    {
        PopulationTier tier = house.currentTier;
        if (_housesByTier.ContainsKey(tier))
        {
            _housesByTier[tier].Remove(house);
            if (_populationPerHouse.ContainsKey(house))
            {
                _totalPopulationByTier[tier] -= _populationPerHouse[house];
                _populationPerHouse.Remove(house);
            }
        }
    }

    public void UpdatePopulationForHouse(House house, int newAmount)
    {
        int oldAmount = _populationPerHouse.ContainsKey(house) ? _populationPerHouse[house] : 0;
        _populationPerHouse[house] = newAmount;

        int difference = newAmount - oldAmount;
        if (house.currentTier != null && _totalPopulationByTier.ContainsKey(house.currentTier))
        {
            _totalPopulationByTier[house.currentTier] += difference;
        }
    }

    public int GetPopulation(PopulationTier tier)
    {
        if (tier != null && _totalPopulationByTier.ContainsKey(tier))
        {
            return _totalPopulationByTier[tier];
        }
        return 0;
    }

    public float GetAverageHappiness(PopulationTier tier)
    {
        if (tier == null || !_housesByTier.ContainsKey(tier) || _housesByTier[tier].Count == 0)
        {
            return 0;
        }
        return (float)_housesByTier[tier].Average(h => h.currentHappiness);
    }

    public int GetGrandTotalPopulation()
    {
        return _totalPopulationByTier.Values.Sum();
    }
}