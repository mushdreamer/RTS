// PopulationManager.cs - 带调试日志的版本
using UnityEngine;
using System.Collections.Generic;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance { get; private set; }

    void Awake()
    {
        // --- 新增的调试日志 ---
        Debug.Log("<b><color=green>PopulationManager.Awake() has been called!</color></b> 这个日志应该在游戏开始时立刻出现。");

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // ... 其他代码保持不变 ...
    private Dictionary<PopulationTier, int> _populationByTier = new Dictionary<PopulationTier, int>();

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
        Debug.Log($"Population Tier '{tier.tierName}' changed by {changeAmount}. New total: {_populationByTier[tier]}");
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