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

    private void UpdateAllTierNeeds()
    {
        if (_housesByTier.Keys.Count == 0) return;

        // 【修改点1】不再获取PlacementSystem，而是获取我们新的BuildingConnector
        BuildingConnector connector = BuildingConnector.Instance;
        if (connector == null) return; // 如果找不到，则不执行

        foreach (PopulationTier tier in _housesByTier.Keys.ToList())
        {
            if (_housesByTier[tier].Count == 0) continue;

            // 在处理需求之前，先更新所有房屋的连接状态
            foreach (House house in _housesByTier[tier])
            {
                // 【修改点2】调用新的、通用的连接检查方法
                house.isConnectedToWarehouse = connector.CheckConnection(house.GetGridPosition(), BuildingType.Warehouse);
            }

            // 后续的逻辑完全保持不变
            List<House> connectedHouses = _housesByTier[tier].Where(h => h.isConnectedToWarehouse).ToList();
            int connectedHouseCount = connectedHouses.Count;

            if (connectedHouseCount == 0) continue;

            foreach (Need need in tier.needs)
            {
                float consumptionRate = (need.consumptionPerMinute / 60f) * needsUpdateInterval;
                float totalDemand = consumptionRate * connectedHouseCount;
                bool canMeetDemand = ResourceManager.Instance.GetWarehouseStock(need.item) >= totalDemand;

                foreach (House house in _housesByTier[tier])
                {
                    bool isMet = house.isConnectedToWarehouse && canMeetDemand;

                    HouseNeedState state = house.trackedNeeds.FirstOrDefault(n => n.associatedNeed == need);
                    if (state != null)
                    {
                        state.isMet = isMet;
                    }
                }

                if (canMeetDemand)
                {
                    ResourceManager.Instance.TryConsumeWarehouseItem(need.item, totalDemand);
                }
            }
        }

        foreach (List<House> houseList in _housesByTier.Values)
        {
            foreach (House house in houseList.ToList())
            {
                house.RecalculateState();
            }
        }
    }

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
        // ▼▼▼ 唯一的修改点在这里 ▼▼▼
        return (float)_housesByTier[tier].Average(h => h.currentHappiness);
    }
}