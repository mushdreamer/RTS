// BuySlot.cs - �������汾
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

    // --- �޸ĵ� 3����ȫ��д�˷��� ---
    private void HandleResourcesChanged()
    {
        ObjectData objectData = DatabaseManager.Instance.databaseSO.objectsData[databaseItemID];

        bool requirementsMet = true;

        // 1. ������õ㣨Credits���Ƿ��㹻
        if (ResourceManager.Instance.GetCredits() < objectData.creditCost)
        {
            requirementsMet = false;
        }
        else
        {
            // 2. ������õ��㹻������һ���ÿ�����ʣ�Materials��
            foreach (BuildRequirement req in objectData.materialRequirements)
            {
                // ʹ�������µĲֿ��ѯ������
                if (ResourceManager.Instance.GetWarehouseStock(req.item) < req.amount)
                {
                    requirementsMet = false;
                    break; // һ�����κ�һ�����ʲ��㣬������ֹͣ���
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