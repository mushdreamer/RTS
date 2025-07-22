// WorkforceManager.cs - 完整修正版
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WorkforceManager : MonoBehaviour
{
    public static WorkforceManager Instance { get; private set; }

    private Dictionary<PopulationTier, int> _assignedWorkforce = new Dictionary<PopulationTier, int>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    public int RequestWorkforce(PopulationTier tier, int amountRequested, Vector3Int buildingPosition)
    {
        if (tier == null || amountRequested <= 0 || CityNetworkManager.Instance == null) return 0;

        int networkId = CityNetworkManager.Instance.GetNetworkIdAt(buildingPosition);
        if (networkId == -1) return 0;

        int totalPopulationOnNetwork = CityNetworkManager.Instance.GetAvailablePopulationOnNetwork(networkId, tier);
        int currentlyAssigned = GetAssignedWorkforce(tier); // 使用下面的方法获取已分配劳动力
        int availableWorkforce = totalPopulationOnNetwork - currentlyAssigned;

        int amountToAssign = Mathf.Min(amountRequested, availableWorkforce);

        if (amountToAssign > 0)
        {
            if (!_assignedWorkforce.ContainsKey(tier)) _assignedWorkforce[tier] = 0;
            _assignedWorkforce[tier] += amountToAssign;
        }

        return amountToAssign;
    }

    public void ReleaseWorkforce(PopulationTier tier, int amountToRelease)
    {
        if (tier == null || amountToRelease <= 0) return;

        if (_assignedWorkforce.ContainsKey(tier))
        {
            _assignedWorkforce[tier] -= amountToRelease;
            if (_assignedWorkforce[tier] < 0) _assignedWorkforce[tier] = 0;
        }
    }

    // === 【修正】确保这个方法存在，供 GlobalStatusUI 调用 ===
    public int GetAssignedWorkforce(PopulationTier tier)
    {
        if (tier != null && _assignedWorkforce.ContainsKey(tier))
        {
            return _assignedWorkforce[tier];
        }
        return 0;
    }

    // === 【修正】确保这个方法有返回值，供 GlobalStatusUI 调用 ===
    public int GetTotalAssignedWorkforce()
    {
        return _assignedWorkforce.Values.Sum();
    }
}