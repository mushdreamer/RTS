// ResourceManager.cs - 已修正版本
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
    private int credits = 60;
    public TextMeshProUGUI creditsUI;
    public enum ResourcesType
    {
        Credits
    }

    // --- Anno风格的物品库存 ---
    private Dictionary<ItemData, float> _itemStock = new Dictionary<ItemData, float>();

    // --- 事件和系统引用 ---
    public event Action OnResourceChanged;
    public event Action OnBuildingsChanged;
    public List<BuildingType> allExistingBuildings;
    public PlacementSystem placementSystem;

    private void Start()
    {
        UpdateUI();
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
                // --- 修改点 1 ---
                // 不再需要遍历resourceRequirements来找价格，直接读取creditCost
                sellingPrice = obj.creditCost;
                break; // 找到对应的建筑后就可以跳出循环
            }
        }
        int amountToReturn = (int)(sellingPrice * 0.50f);
        IncreaseResource(ResourcesType.Credits, amountToReturn);

        // (可选) 未来你也可以在这里添加返还部分建造物资的逻辑
    }

    // 这个方法在建筑被成功放置后调用，用来扣除资源
    internal void DecreaseResourcesBasedOnRequirement(ObjectData objectData)
    {
        // --- 修改点 2 ---
        // 分别扣除信用点和物资

        // 1. 扣除信用点
        DecreaseResource(ResourcesType.Credits, objectData.creditCost);

        // 2. 扣除物资
        foreach (BuildRequirement req in objectData.materialRequirements)
        {
            // 这里我们用TryConsume，因为它更安全。
            // 理论上此时资源一定是足够的，因为BuySlot已经检查过了。
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

    [Header("调试用的物品")]
    public ItemData debugFishData;
    public ItemData debugWoodData;
    void Update()
    {
        // 按下 F 键，增加 10 单位的鱼
        if (Input.GetKeyDown(KeyCode.F))
        {
            // 我们不再使用Resources.Load，直接使用在Inspector里设置好的公共变量
            if (debugFishData != null)
            {
                AddWarehouseItem(debugFishData, 10);
                Debug.Log($"调试：手动增加了10单位的【{debugFishData.itemName}】!");
            }
            else
            {
                Debug.LogWarning("调试失败：请在ResourceManager的Inspector面板中设置'Debug Fish Data'!");
            }
        }

        // 按下 W 键 (Wood)，增加 10 单位的木材
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (debugWoodData != null)
            {
                AddWarehouseItem(debugWoodData, 10);
                Debug.Log($"调试：手动增加了10单位的【{debugWoodData.itemName}】!");
            }
            else
            {
                Debug.LogWarning("调试失败：请在ResourceManager的Inspector面板中设置'Debug Wood Data'!");
            }
        }
    }
}