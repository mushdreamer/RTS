// ResourceManager.cs - 最终完整版
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    // --- UI 控制 ---
    public TextMeshProUGUI creditsUI;

    // --- 内部变量 ---
    private int credits = 60; // 旧的信用点系统，后台保留
    private Dictionary<ItemData, float> _itemStock = new Dictionary<ItemData, float>();
    private int activeBankCount = 0; // 用于追踪当前银行的数量

    // --- 事件 ---
    public event Action OnResourceChanged;
    public event Action OnBuildingsChanged;
    public event Action<float> OnMoneyChanged;

    // --- 游戏逻辑引用 ---
    public List<BuildingType> allExistingBuildings;
    public PlacementSystem placementSystem;

    [Header("经济系统 (银行与资金)")]
    [SerializeField] private float money = 1000f;
    public float Money => money;
    [SerializeField] private bool bankExists = false;
    public bool BankExists => bankExists;

    private void Start()
    {
        // 游戏开始时，直接禁用文本组件，让它不显示
        if (creditsUI != null)
        {
            creditsUI.enabled = false;
        }
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
            creditsUI.text = $"资金: {newAmount:F0}";
        }
    }

    public void RegisterBank()
    {
        activeBankCount++;
        bankExists = true;

        // 只要有银行存在，就启用文本组件
        if (creditsUI != null && !creditsUI.enabled)
        {
            creditsUI.enabled = true;
            // 首次显示时，立即更新一次文本内容
            UpdateMoneyUI(this.money);
        }
    }

    public void UnregisterBank()
    {
        activeBankCount--;
        // 如果最后一个银行也被摧毁了
        if (activeBankCount <= 0)
        {
            activeBankCount = 0; // 防止负数
            bankExists = false;
            // 禁用文本组件，让它再次消失
            if (creditsUI != null)
            {
                creditsUI.enabled = false;
            }
        }
    }

    internal void DecreaseResourcesBasedOnRequirement(ObjectData objectData)
    {
        TrySpendMoney(objectData.creditCost);
        foreach (BuildRequirement req in objectData.materialRequirements)
        {
            TryConsumeWarehouseItem(req.item, req.amount);
        }
    }

    public void UpdateBuildingChanged(BuildingType buildingType, bool isNew, Vector3 position)
    {
        if (isNew)
        {
            allExistingBuildings.Add(buildingType);
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
        var sellingPrice = 0;
        foreach (ObjectData obj in DatabaseManager.Instance.databaseSO.objectsData)
        {
            if (obj.thisBuildingType == buildingType)
            {
                sellingPrice = obj.creditCost;
                break;
            }
        }
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

    public enum ResourcesType
    {
        Credits
    }
}