// ResourceManager.cs - �������汾
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

    // --- ���õ㣨Credits������ ---
    private int credits = 60;
    public TextMeshProUGUI creditsUI;
    public enum ResourcesType
    {
        Credits
    }

    // --- Anno������Ʒ��� ---
    private Dictionary<ItemData, float> _itemStock = new Dictionary<ItemData, float>();

    // --- �¼���ϵͳ���� ---
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
                // --- �޸ĵ� 1 ---
                // ������Ҫ����resourceRequirements���Ҽ۸�ֱ�Ӷ�ȡcreditCost
                sellingPrice = obj.creditCost;
                break; // �ҵ���Ӧ�Ľ�����Ϳ�������ѭ��
            }
        }
        int amountToReturn = (int)(sellingPrice * 0.50f);
        IncreaseResource(ResourcesType.Credits, amountToReturn);

        // (��ѡ) δ����Ҳ������������ӷ������ֽ������ʵ��߼�
    }

    // ��������ڽ������ɹ����ú���ã������۳���Դ
    internal void DecreaseResourcesBasedOnRequirement(ObjectData objectData)
    {
        // --- �޸ĵ� 2 ---
        // �ֱ�۳����õ������

        // 1. �۳����õ�
        DecreaseResource(ResourcesType.Credits, objectData.creditCost);

        // 2. �۳�����
        foreach (BuildRequirement req in objectData.materialRequirements)
        {
            // ����������TryConsume����Ϊ������ȫ��
            // �����ϴ�ʱ��Դһ�����㹻�ģ���ΪBuySlot�Ѿ������ˡ�
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

    [Header("�����õ���Ʒ")]
    public ItemData debugFishData;
    public ItemData debugWoodData;
    void Update()
    {
        // ���� F �������� 10 ��λ����
        if (Input.GetKeyDown(KeyCode.F))
        {
            // ���ǲ���ʹ��Resources.Load��ֱ��ʹ����Inspector�����úõĹ�������
            if (debugFishData != null)
            {
                AddWarehouseItem(debugFishData, 10);
                Debug.Log($"���ԣ��ֶ�������10��λ�ġ�{debugFishData.itemName}��!");
            }
            else
            {
                Debug.LogWarning("����ʧ�ܣ�����ResourceManager��Inspector���������'Debug Fish Data'!");
            }
        }

        // ���� W �� (Wood)������ 10 ��λ��ľ��
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (debugWoodData != null)
            {
                AddWarehouseItem(debugWoodData, 10);
                Debug.Log($"���ԣ��ֶ�������10��λ�ġ�{debugWoodData.itemName}��!");
            }
            else
            {
                Debug.LogWarning("����ʧ�ܣ�����ResourceManager��Inspector���������'Debug Wood Data'!");
            }
        }
    }
}