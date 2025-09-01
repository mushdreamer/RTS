// House.cs - Final Version with Interface
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class House : MonoBehaviour, IActivatableBuilding // Implements the interface
{
    public PopulationTier currentTier;

    [Header("实时状态")]
    public List<HouseNeedState> trackedNeeds;
    public int currentResidents;
    public int maxResidents;
    public int currentHappiness;
    public bool isConnectedToWarehouse = false;

    [Header("成长参数")]
    public int baseResidents = 5;
    public int residentsPerNeedMet = 5;

    private bool _isActivated = false;
    private Constructable constructable;
    private Vector3Int _myGridPosition;

    // This is the required method from the IActivatableBuilding interface
    public void Activate(Vector3Int gridPosition)
    {
        if (_isActivated) return;

        _myGridPosition = gridPosition;
        constructable = GetComponent<Constructable>();
        if (constructable == null) return;

        PopulationManager.Instance.RegisterHouse(this);
        InitializeNeeds();
        RecalculateState();
        _isActivated = true;
    }

    void OnDestroy()
    {
        if (_isActivated && PopulationManager.Instance != null)
        {
            PopulationManager.Instance.UnregisterHouse(this);
        }
    }

    private void InitializeNeeds()
    {
        trackedNeeds = new List<HouseNeedState>();
        if (currentTier != null && currentTier.needs != null)
        {
            foreach (var need in currentTier.needs)
            {
                trackedNeeds.Add(new HouseNeedState(need));
            }
        }
    }

    public void RecalculateState()
    {
        if (currentTier == null) return;

        if (isConnectedToWarehouse)
        {
            int needsMetCount = trackedNeeds.Count(n => n.isMet);
            maxResidents = baseResidents + (currentTier.needs.Count * residentsPerNeedMet);
            currentResidents = baseResidents + (needsMetCount * residentsPerNeedMet);
            currentResidents = Mathf.Min(currentResidents, maxResidents);
            currentHappiness = 10 + (needsMetCount * 2) - ((currentTier.needs.Count - needsMetCount) * 1);
            currentHappiness = Mathf.Clamp(currentHappiness, 0, 20);
        }
        else
        {
            foreach (var need in trackedNeeds)
            {
                need.isMet = false;
            }
            maxResidents = baseResidents + (currentTier.needs.Count * residentsPerNeedMet);
            currentResidents = 0;
            currentHappiness = 0;
        }

        PopulationManager.Instance.UpdatePopulationForHouse(this, currentResidents);
    }

    public Vector3Int GetGridPosition()
    {
        return _myGridPosition;
    }

    public bool CanUpgrade()
    {
        if (currentTier.nextTier == null) return false;
        if (currentResidents < maxResidents) return false;

        foreach (var material in currentTier.upgradeMaterials)
        {
            if (ResourceManager.Instance.GetWarehouseStock(material.item) < material.amount) return false;
        }
        return true;
    }

    public void TryToUpgrade()
    {
        if (!CanUpgrade())
        {
            Debug.Log("Upgrade Condition doesn't Fulfill！");
            return;
        }

        foreach (var material in currentTier.upgradeMaterials)
        {
            ResourceManager.Instance.TryConsumeWarehouseItem(material.item, material.amount);
        }

        PopulationManager.Instance.UnregisterHouse(this);
        currentTier = currentTier.nextTier;
        PopulationManager.Instance.RegisterHouse(this);
        InitializeNeeds();
        RecalculateState();

        Debug.Log($"<color=cyan>House upgrade succeed！</color>");
    }
}