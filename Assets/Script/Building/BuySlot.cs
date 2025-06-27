// BuySlot.cs - 已修正版本
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

    // --- 修改点 3：完全重写此方法 ---
    private void HandleResourcesChanged()
    {
        ObjectData objectData = DatabaseManager.Instance.databaseSO.objectsData[databaseItemID];

        bool requirementsMet = true;

        // 1. 检查信用点（Credits）是否足够
        if (ResourceManager.Instance.GetCredits() < objectData.creditCost)
        {
            requirementsMet = false;
        }
        else
        {
            // 2. 如果信用点足够，再逐一检查每种物资（Materials）
            foreach (BuildRequirement req in objectData.materialRequirements)
            {
                // 使用我们新的仓库查询方法！
                if (ResourceManager.Instance.GetWarehouseStock(req.item) < req.amount)
                {
                    requirementsMet = false;
                    break; // 一旦有任何一种物资不足，就立刻停止检查
                }
            }
        }

        isAvailable = requirementsMet;

        UpdateAvailabilityUI();
    }

    private void HandleBuildingsChanged()
    {
        ObjectData objectData = DatabaseManager.Instance.databaseSO.objectsData[databaseItemID];

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