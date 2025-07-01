// PopulationManager.cs - ��������־�İ汾
using UnityEngine;
using System.Collections.Generic;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance { get; private set; }

    void Awake()
    {
        // --- �����ĵ�����־ ---
        Debug.Log("<b><color=green>PopulationManager.Awake() has been called!</color></b> �����־Ӧ������Ϸ��ʼʱ���̳��֡�");

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // ... �������뱣�ֲ��� ...
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