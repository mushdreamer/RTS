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

    void Update()
    {
        // 按下 F 键用于测试
        if (Input.GetKeyDown(KeyCode.F))
        {
            // 1. 从 Resources 文件夹中，根据路径加载 "Item_Fish" 资产
            // 注意：路径是相对于 Resources 文件夹的，不需要写 "Assets/Resources/"
            // 也不需要写文件后缀 ".asset"
            ItemData fishData = Resources.Load<ItemData>("GameData/Items/Item_Fish");

            // 2. 添加一个健壮性检查，确保我们成功加载到了东西
            if (fishData == null)
            {
                Debug.LogError("加载 'Item_Fish' 失败! " +
                               "请确认：\n1. 文件路径是否为 'Assets/Resources/GameData/Items/Item_Fish.asset' " +
                               "\n2. 路径字符串 'GameData/Items/Item_Fish' 是否拼写正确。");
                return; // 如果加载失败，直接返回，不执行后续代码
            }

            // 3. 如果加载成功，才执行添加操作
            AddWarehouseItem(fishData, 10);

            Debug.Log($"调试：通过代码加载并成功增加了10单位的【{fishData.itemName}】!");
        }
    }
}