// ResourceManager.cs - 修正版本
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

    // --- 信用点（Credits）管理 ---
    private int credits = 60; // 用于建造的信用点
    public TextMeshProUGUI creditsUI;
    public enum ResourcesType
    {
        Credits
    }

    // --- Anno风格的物品库存 ---
    private Dictionary<ItemData, float> _itemStock = new Dictionary<ItemData, float>();

    // --- 事件和系统引用 ---
    public event Action OnResourceChanged; // 用于信用点和物品库存变化
    public event Action OnBuildingsChanged;
    public List<BuildingType> allExistingBuildings;
    public PlacementSystem placementSystem;

    // ▼▼▼【修正部分】▼▼▼
    // --- 经济系统 (银行与资金) ---
    [Header("经济系统 (银行与资金)")]
    [SerializeField] private float money = 1000f; // 初始资金，用于科研等
    public float Money => money; // 公开的只读属性，用于访问资金

    [SerializeField] private bool bankExists = false;
    public bool BankExists => bankExists; // 公开的只读属性

    // 资金变化事件
    public event Action<float> OnMoneyChanged;
    // ▲▲▲【修正部分结束】▲▲▲


    private void Start()
    {
        UpdateUI();
        // 首次启动时，手动触发一次资金UI更新
        OnMoneyChanged?.Invoke(money);
    }

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
        int amountToReturn = (int)(sellingPrice * 0.50f);
        IncreaseResource(ResourcesType.Credits, amountToReturn);
    }

    internal void DecreaseResourcesBasedOnRequirement(ObjectData objectData)
    {
        DecreaseResource(ResourcesType.Credits, objectData.creditCost);

        foreach (BuildRequirement req in objectData.materialRequirements)
        {
            TryConsumeWarehouseItem(req.item, req.amount);
        }
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

    private void UpdateUI()
    {
        creditsUI.text = $"{credits}";
    }

    public int GetCredits()
    {
        return credits;
    }

    private void OnEnable()
    {
        OnResourceChanged += UpdateUI;
    }
    private void OnDisable()
    {
        OnResourceChanged -= UpdateUI;
    }

    #region Bank and Money System

    public void RegisterBank()
    {
        bankExists = true; // 【修正】现在修改私有字段
        Debug.Log("Bank has been built. Profits will now be collected.");
    }

    public void UnregisterBank()
    {
        bankExists = false; // 【修正】现在修改私有字段
        Debug.Log("Bank has been destroyed. Profits will no longer be collected.");
    }

    public void AddMoney(float amount)
    {
        if (amount <= 0) return;
        money += amount; // 【修正】现在修改私有字段
        OnMoneyChanged?.Invoke(money);
    }

    public bool TrySpendMoney(float amount)
    {
        if (money >= amount)
        {
            money -= amount; // 【修正】现在修改私有字段
            OnMoneyChanged?.Invoke(money);
            return true;
        }
        return false;
    }

    #endregion

    void Update()
    {

    }
}