// BuySlot.cs - 修正版本
using System;
using UnityEngine;
using UnityEngine.UI;

public class BuySlot : MonoBehaviour
{
    public Sprite availableSprite;
    public Sprite unAvailableSprite;

    public bool isAvailable;

    public BuySystem buySystem;

    public int databaseItemID;

    private void Start()
    {
        ResourceManager.Instance.OnResourceChanged += HandleResourcesChanged;
        // 【新增】订阅新的 OnMoneyChanged 事件
        ResourceManager.Instance.OnMoneyChanged += HandleMoneyChanged;

        HandleResourcesChanged();

        ResourceManager.Instance.OnBuildingsChanged += HandleBuildingsChanged;
        HandleBuildingsChanged();
    }

    public void ClickOnSlot()
    {
        if (isAvailable)
        {
            buySystem.placementSystem.StartPlacement(databaseItemID);
        }
    }

    private void OnDestroy()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged -= HandleResourcesChanged;
            // 【新增】取消订阅
            ResourceManager.Instance.OnMoneyChanged -= HandleMoneyChanged;
            ResourceManager.Instance.OnBuildingsChanged -= HandleBuildingsChanged;
        }
    }

    private void UpdateAvailabilityUI()
    {
        if (isAvailable)
        {
            GetComponent<Image>().sprite = availableSprite;
            GetComponent<Button>().interactable = true;
        }
        else
        {
            GetComponent<Image>().sprite = unAvailableSprite;
            GetComponent<Button>().interactable = false;
        }
    }

    // 【新增】专门处理资金变化的轻量方法
    private void HandleMoneyChanged(float newAmount)
    {
        // 资金变化时，重新检查一次可用性
        HandleResourcesChanged();
    }

    // --- 【核心修改点】---
    private void HandleResourcesChanged()
    {
        ObjectData objectData = DatabaseManager.Instance.databaseSO.GetObjectByID(databaseItemID);

        bool requirementsMet = true;

        // 1. 【修改】检查资金 (Money) 是否足够
        if (ResourceManager.Instance.Money < objectData.creditCost)
        {
            requirementsMet = false;
        }
        else
        {
            // 2. 如果资金足够，再逐一检查每种物资
            foreach (BuildRequirement req in objectData.materialRequirements)
            {
                if (ResourceManager.Instance.GetWarehouseStock(req.item) < req.amount)
                {
                    requirementsMet = false;
                    break;
                }
            }
        }

        isAvailable = requirementsMet;
        UpdateAvailabilityUI();
    }

    private void HandleBuildingsChanged()
    {
        ObjectData objectData = DatabaseManager.Instance.databaseSO.GetObjectByID(databaseItemID);

        foreach (BuildingType dependency in objectData.buildDependency)
        {
            if (dependency == BuildingType.None)
            {
                gameObject.SetActive(true);
                return;
            }

            if (ResourceManager.Instance.allExistingBuildings != null && !ResourceManager.Instance.allExistingBuildings.Contains(dependency))
            {
                gameObject.SetActive(false);
                return;
            }
        }
        gameObject.SetActive(true);
    }
}