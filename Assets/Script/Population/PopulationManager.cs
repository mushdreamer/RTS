// PopulationManager.cs - Final Corrected Version
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance { get; private set; }

    private Dictionary<PopulationTier, int> _totalPopulationByTier = new Dictionary<PopulationTier, int>();
    private Dictionary<PopulationTier, List<House>> _housesByTier = new Dictionary<PopulationTier, List<House>>();
    private Dictionary<House, int> _populationPerHouse = new Dictionary<House, int>();

    [Header("Update Settings")]
    public float needsUpdateInterval = 2.0f;

    private TransmissionTower[] allTowers;
    private Dictionary<TransmissionTower, int> towerConnectionCounts = new Dictionary<TransmissionTower, int>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    void Start()
    {
        InvokeRepeating(nameof(UpdateAllTierNeeds), needsUpdateInterval, needsUpdateInterval);
    }

    private void FindAllTowers()
    {
        allTowers = FindObjectsOfType<TransmissionTower>();
    }

    private void UpdateAllTierNeeds()
    {
        if (_housesByTier.Keys.Count == 0) return;

        FindAllTowers();
        towerConnectionCounts.Clear();
        foreach (var tower in allTowers)
        {
            if (tower.IsPowered)
            {
                towerConnectionCounts[tower] = 0;
            }
        }

        foreach (var houseList in _housesByTier.Values)
        {
            foreach (var house in houseList)
            {
                if (BuildingConnector.Instance != null)
                {
                    house.isConnectedToWarehouse = BuildingConnector.Instance.CheckConnection(house.GetGridPosition(), BuildingType.Warehouse);
                }
            }
        }

        foreach (PopulationTier tier in _housesByTier.Keys.ToList())
        {
            if (_housesByTier[tier].Count == 0) continue;
            foreach (Need need in tier.needs)
            {
                switch (need.item.itemName)
                {
                    case "Fish":
                        ProcessStandardNeed(tier, need);
                        break;
                    case "Electricity":
                        ProcessElectricityNeed(tier, need);
                        break;
                    default:
                        ProcessStandardNeed(tier, need);
                        break;
                }
            }
        }

        foreach (var houseList in _housesByTier.Values)
        {
            foreach (var house in houseList)
            {
                house.RecalculateState();
            }
        }
    }

    private void ProcessStandardNeed(PopulationTier tier, Need need)
    {
        var connectedHouses = _housesByTier[tier].Where(h => h.isConnectedToWarehouse).ToList();
        if (connectedHouses.Count == 0)
        {
            SetNeedStatusForAllHouses(tier, need, false);
            return;
        }
        float consumptionRate = (need.consumptionPerMinute / 60f) * needsUpdateInterval;
        float totalDemand = consumptionRate * connectedHouses.Count;
        bool canMeetDemand = ResourceManager.Instance.GetWarehouseStock(need.item) >= totalDemand;
        if (canMeetDemand)
        {
            ResourceManager.Instance.TryConsumeWarehouseItem(need.item, totalDemand);
            if (need.item.itemName == "Fish" && ResourceManager.Instance.BankExists)
            {
                float profitPerUnit = 2.5f;
                float totalProfit = totalDemand * profitPerUnit;
                ResourceManager.Instance.AddMoney(totalProfit);
            }
        }
        foreach (var house in _housesByTier[tier])
        {
            var state = house.trackedNeeds.FirstOrDefault(n => n.associatedNeed == need);
            if (state != null)
            {
                state.isMet = house.isConnectedToWarehouse && canMeetDemand;
            }
        }
    }

    private void ProcessElectricityNeed(PopulationTier tier, Need need)
    {
        foreach (var house in _housesByTier[tier])
        {
            bool isCoveredAndHasCapacity = TryFindAvailableTowerForHouse(house);

            var state = house.trackedNeeds.FirstOrDefault(n => n.associatedNeed == need);
            if (state != null)
            {
                state.isMet = isCoveredAndHasCapacity;
            }
        }
    }

    private bool TryFindAvailableTowerForHouse(House house)
    {
        foreach (var tower in towerConnectionCounts.Keys)
        {
            if (towerConnectionCounts[tower] >= tower.maxHouseConnections)
            {
                continue;
            }

            float distance = Vector3.Distance(house.transform.position, tower.transform.position);
            if (distance <= tower.coverageRadius)
            {
                towerConnectionCounts[tower]++;
                return true;
            }
        }
        return false;
    }

    private void SetNeedStatusForAllHouses(PopulationTier tier, Need need, bool isMet)
    {
        foreach (var house in _housesByTier[tier])
        {
            var state = house.trackedNeeds.FirstOrDefault(n => n.associatedNeed == need);
            if (state != null) state.isMet = isMet;
        }
    }

    #region House Registration
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
    #endregion

    // ¨‹¨‹¨‹¡¾RE-ADDED PUBLIC METHODS¡¿¨‹¨‹¨‹
    #region Public Getters for UI and other Managers

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

    #endregion
    // ¡ø¡ø¡ø¡¾RE-ADDED PUBLIC METHODS END¡¿¡ø¡ø¡ø
}