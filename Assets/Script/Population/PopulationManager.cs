// PopulationManager.cs - 增加了房屋列表和幸福度计算
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance { get; private set; }

    private Dictionary<PopulationTier, int> _populationByTier = new Dictionary<PopulationTier, int>();

    // <<< 新增：一个字典，用来按阶层存储所有房屋的列表 >>>
    private Dictionary<PopulationTier, List<House>> _housesByTier = new Dictionary<PopulationTier, List<House>>();

    void Awake()
    {
        Debug.Log("<b><color=green>PopulationManager.Awake() has been called!</color></b> 这个日志应该在游戏开始时立刻出现。");
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    // <<< 新增：注册房屋的方法 >>>
    public void RegisterHouse(House house)
    {
        PopulationTier tier = house.currentTier;
        if (!_housesByTier.ContainsKey(tier))
        {
            _housesByTier[tier] = new List<House>();
        }
        _housesByTier[tier].Add(house);
    }

    // <<< 新增：注销房屋的方法 >>>
    public void UnregisterHouse(House house)
    {
        PopulationTier tier = house.currentTier;
        if (_housesByTier.ContainsKey(tier))
        {
            _housesByTier[tier].Remove(house);
        }
    }

    // <<< 新增：计算平均幸福度的方法 >>>
    public float GetAverageHappiness(PopulationTier tier)
    {
        if (tier == null || !_housesByTier.ContainsKey(tier) || _housesByTier[tier].Count == 0)
        {
            return 0; // 如果没有该阶层的房屋，则返回0
        }

        // 使用LINQ来轻松计算平均值
        return (float)_housesByTier[tier].Average(house => house.currentHappiness);
    }

    // --- 以下是原有方法，保持不变 ---
    public void UpdatePopulation(PopulationTier tier, int changeAmount)
    {
        if (!_populationByTier.ContainsKey(tier))
        {
            _populationByTier[tier] = 0;
        }
        _populationByTier[tier] += changeAmount;
        if (_populationByTier[tier] < 0)
        {
            _populationByTier[tier] = 0;
        }
    }

    public int GetPopulation(PopulationTier tier)
    {
        if (_populationByTier.ContainsKey(tier))
        {
            return _populationByTier[tier];
        }
        return 0;
    }
}