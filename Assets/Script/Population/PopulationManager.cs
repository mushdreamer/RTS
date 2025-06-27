// In folder: Assets/Scripts/PopulationManager.cs
using UnityEngine;
using System.Collections.Generic;

public class PopulationManager : MonoBehaviour
{
    // --- ����ģʽ��ʵ�� ---
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

    // ʹ���ֵ����洢ÿ���ײ�(PopulationTier)�����˿���(int)
    private Dictionary<PopulationTier, int> _populationByTier = new Dictionary<PopulationTier, int>();

    /// <summary>
    /// ����ָ���ײ���˿������������ɼ���
    /// </summary>
    /// <param name="tier">Ҫ���µĽײ�</param>
    /// <param name="changeAmount">�仯������������Ϊ���ӣ�����Ϊ���٣�</param>
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

        // ȷ���˿ڲ����Ϊ����
        if (_populationByTier[tier] < 0)
        {
            _populationByTier[tier] = 0;
        }

        Debug.Log($"Population Tier '{tier.tierName}' changed by {changeAmount}. New total: {_populationByTier[tier]}");
    }

    /// <summary>
    /// ��ȡָ���ײ�����˿���
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