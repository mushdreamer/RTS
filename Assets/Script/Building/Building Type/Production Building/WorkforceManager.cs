// WorkforceManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WorkforceManager : MonoBehaviour
{
    public static WorkforceManager Instance { get; private set; }

    private Dictionary<PopulationTier, int> _requiredWorkforce = new Dictionary<PopulationTier, int>();
    private Dictionary<PopulationTier, float> _tierEfficiency = new Dictionary<PopulationTier, float>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    void Start()
    {
        InvokeRepeating(nameof(UpdateAllEfficiencies), 1f, 1f);
    }

    private void UpdateAllEfficiencies()
    {
        if (PopulationManager.Instance == null) return;

        foreach (var tier in _requiredWorkforce.Keys.ToList())
        {
            int required = _requiredWorkforce[tier];
            int available = PopulationManager.Instance.GetPopulation(tier);

            if (required <= 0)
            {
                _tierEfficiency[tier] = 1f;
            }
            else
            {
                _tierEfficiency[tier] = Mathf.Clamp01((float)available / required);
            }
        }
    }

    public void RegisterWorkforce(PopulationTier tier, int amount)
    {
        if (tier == null) return;
        if (!_requiredWorkforce.ContainsKey(tier))
        {
            _requiredWorkforce[tier] = 0;
            _tierEfficiency[tier] = 1f;
        }
        _requiredWorkforce[tier] += amount;
        UpdateAllEfficiencies();
    }

    public void UnregisterWorkforce(PopulationTier tier, int amount)
    {
        if (tier == null) return;
        if (_requiredWorkforce.ContainsKey(tier))
        {
            _requiredWorkforce[tier] -= amount;
        }
        UpdateAllEfficiencies();
    }

    public float GetEfficiency(PopulationTier tier)
    {
        if (tier != null && _tierEfficiency.ContainsKey(tier))
        {
            return _tierEfficiency[tier];
        }
        return 1f;
    }

    public int GetOccupiedWorkforce(PopulationTier tier)
    {
        if (tier != null && _requiredWorkforce.ContainsKey(tier))
        {
            return _requiredWorkforce[tier];
        }
        return 0;
    }

    public int GetTotalOccupiedWorkforce()
    {
        return _requiredWorkforce.Values.Sum();
    }
}