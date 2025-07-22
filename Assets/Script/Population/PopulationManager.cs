// PopulationManager.cs
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
        // ... （此方法内容保持不变，为简洁省略） ...
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
        return (float)_housesByTier[tier].Average(h => h.currentHappiness);
    }

    public int GetGrandTotalPopulation()
    {
        return _totalPopulationByTier.Values.Sum();
    }
}