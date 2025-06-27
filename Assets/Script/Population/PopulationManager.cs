// In folder: Assets/Scripts/PopulationManager.cs
using UnityEngine;
using System.Collections.Generic;

public class PopulationManager : MonoBehaviour
{
    // --- 单例模式的实现 ---
    public static PopulationManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    // -------------------------

    // 使用字典来存储每个阶层(PopulationTier)的总人口数(int)
    private Dictionary<PopulationTier, int> _populationByTier = new Dictionary<PopulationTier, int>();

    /// <summary>
    /// 更新指定阶层的人口数量（可增可减）
    /// </summary>
    /// <param name="tier">要更新的阶层</param>
    /// <param name="changeAmount">变化的人数（正数为增加，负数为减少）</param>
    public void UpdatePopulation(PopulationTier tier, int changeAmount)
    {
        if (_populationByTier.ContainsKey(tier))
        {
            _populationByTier[tier] += changeAmount;
        }
        else
        {
            _populationByTier[tier] = changeAmount;
        }

        // 确保人口不会变为负数
        if (_populationByTier[tier] < 0)
        {
            _populationByTier[tier] = 0;
        }

        Debug.Log($"Population Tier '{tier.tierName}' changed by {changeAmount}. New total: {_populationByTier[tier]}");
    }

    /// <summary>
    /// 获取指定阶层的总人口数
    /// </summary>
    public int GetPopulation(PopulationTier tier)
    {
        if (_populationByTier.ContainsKey(tier))
        {
            return _populationByTier[tier];
        }
        return 0;
    }
}