// ResourceManager.cs - 修正扣款逻辑版
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; set; }

    private void Awake()
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

    private int credits = 60;
    public TextMeshProUGUI creditsUI;
    public enum ResourcesType
    {
        Credits
    }

    private Dictionary<ItemData, float> _itemStock = new Dictionary<ItemData, float>();
    public event Action OnResourceChanged;
    public event Action OnBuildingsChanged;
    public List<BuildingType> allExistingBuildings;
    public PlacementSystem placementSystem;

    [Header("Econ Sys(Bank and Funding)")]
    [SerializeField] private float money = 1000f;
    public float Money => money;

    [SerializeField] private bool bankExists = false;
    public bool BankExists => bankExists;

    public event Action<float> OnMoneyChanged;

    private void Start()
    {
        OnMoneyChanged?.Invoke(money);
    }

    private void OnEnable()
    {
        OnMoneyChanged += UpdateMoneyUI;
    }

    private void OnDisable()
    {
        OnMoneyChanged -= UpdateMoneyUI;
    }

    private void UpdateMoneyUI(float newAmount)
    {
        if (creditsUI != null)
        {
            creditsUI.text = $"{newAmount:F0}";
        }
    }

    // --- 【核心修改点】---
    internal void DecreaseResourcesBasedOnRequirement(ObjectData objectData)
    {
        // 1. 【修改】不再减少旧的 credits，而是调用 TrySpendMoney 减少 Money
        TrySpendMoney(objectData.creditCost);

        // 2. 扣除材料的部分保持不变
        foreach (BuildRequirement req in objectData.materialRequirements)
        {
            TryConsumeWarehouseItem(req.item, req.amount);
        }
    }

    // --- 其他方法保持不变 ---

    public void UpdateBuildingChanged(BuildingType buildingType, bool isNew, Vector3 position)
    {
        if (isNew)
        {
            allExistingBuildings.Add(buildingType);
            SoundManager.Instance.PlayBuildingConstructionSound();
        }
        else
        {
            placementSystem.RemovePlacementData(position);
            allExistingBuildings.Remove(buildingType);
        }
        OnBuildingsChanged?.Invoke();
    }

    public void SellBuilding(BuildingType buildingType)
    {
        SoundManager.Instance.PlayBuildingSellingSound();
        var sellingPrice = 0;
        foreach (ObjectData obj in DatabaseManager.Instance.databaseSO.objectsData)
        {
            if (obj.thisBuildingType == buildingType)
            {
                sellingPrice = obj.creditCost;
                break;
            }
        }
        // 出售建筑时，我们返还 Money 而不是 credits
        AddMoney(sellingPrice * 0.5f);
    }

    public void IncreaseResource(ResourcesType resource, int amountToIncrease)
    {
        if (resource == ResourcesType.Credits)
        {
            credits += amountToIncrease;
            OnResourceChanged?.Invoke();
        }
    }
    public void DecreaseResource(ResourcesType resource, int amountToDecrease)
    {
        if (resource == ResourcesType.Credits)
        {
            credits -= amountToDecrease;
            OnResourceChanged?.Invoke();
        }
    }

    public void AddWarehouseItem(ItemData item, float amount)
    {
        if (_itemStock.ContainsKey(item))
        {
            _itemStock[item] += amount;
        }
        else
        {
            _itemStock.Add(item, amount);
        }
        OnResourceChanged?.Invoke();
    }

    public bool TryConsumeWarehouseItem(ItemData item, float amount)
    {
        if (_itemStock.ContainsKey(item) && _itemStock[item] >= amount)
        {
            _itemStock[item] -= amount;
            OnResourceChanged?.Invoke();
            return true;
        }
        return false;
    }

    public float GetWarehouseStock(ItemData item)
    {
        return _itemStock.ContainsKey(item) ? _itemStock[item] : 0;
    }

    public int GetCredits()
    {
        return credits;
    }

    #region Bank and Money System

    public void RegisterBank()
    {
        bankExists = true;
        Debug.Log("Bank has been built. Profits will now be collected.");
    }

    public void UnregisterBank()
    {
        bankExists = false;
        Debug.Log("Bank has been destroyed. Profits will no longer be collected.");
    }

    public void AddMoney(float amount)
    {
        if (amount <= 0) return;
        money += amount;
        OnMoneyChanged?.Invoke(money);
    }

    public bool TrySpendMoney(float amount)
    {
        if (money >= amount)
        {
            money -= amount;
            OnMoneyChanged?.Invoke(money);
            return true;
        }
        return false;
    }
    #endregion
}