// PopulationManager.cs - �����˷����б���Ҹ��ȼ���
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance { get; private set; }

    private Dictionary<PopulationTier, int> _populationByTier = new Dictionary<PopulationTier, int>();

    // <<< ������һ���ֵ䣬�������ײ�洢���з��ݵ��б� >>>
    private Dictionary<PopulationTier, List<House>> _housesByTier = new Dictionary<PopulationTier, List<House>>();

    void Awake()
    {
        Debug.Log("<b><color=green>PopulationManager.Awake() has been called!</color></b> �����־Ӧ������Ϸ��ʼʱ���̳��֡�");
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    // <<< ������ע�᷿�ݵķ��� >>>
    public void RegisterHouse(House house)
    {
        PopulationTier tier = house.currentTier;
        if (!_housesByTier.ContainsKey(tier))
        {
            _housesByTier[tier] = new List<House>();
        }
        _housesByTier[tier].Add(house);
    }

    // <<< ������ע�����ݵķ��� >>>
    public void UnregisterHouse(House house)
    {
        PopulationTier tier = house.currentTier;
        if (_housesByTier.ContainsKey(tier))
        {
            _housesByTier[tier].Remove(house);
        }
    }

    // <<< ����������ƽ���Ҹ��ȵķ��� >>>
    public float GetAverageHappiness(PopulationTier tier)
    {
        if (tier == null || !_housesByTier.ContainsKey(tier) || _housesByTier[tier].Count == 0)
        {
            return 0; // ���û�иýײ�ķ��ݣ��򷵻�0
        }

        // ʹ��LINQ�����ɼ���ƽ��ֵ
        return (float)_housesByTier[tier].Average(house => house.currentHappiness);
    }

    // --- ������ԭ�з��������ֲ��� ---
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