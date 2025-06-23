using System;
using System.Collections;
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

    public event Action OnResourceChanged;
    public event Action OnBuildingsChanged;

    public TextMeshProUGUI creditsUI;

    public List<BuildingType> allExistingBuildings;

    public PlacementSystem placementSystem;
    public enum ResourcesType
    {
        Credits
    }
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

    public void IncreaseResource(ResourcesType resource, int amountToIncrease)
    {
        switch (resource)
        {
            case ResourcesType.Credits:
                credits += amountToIncrease;
                break;
            default:
                break;
        }

        OnResourceChanged?.Invoke();
    }
    public void DecreaseResource(ResourcesType resource, int amountToDecrease)
    {
        switch (resource)
        {
            case ResourcesType.Credits:
                credits -= amountToDecrease;
                break;
            default:
                break;
        }

        OnResourceChanged?.Invoke();
    }

    public void SellBuilding(BuildingType buildingType)
    {
        SoundManager.Instance.PlayBuildingSellingSound();

        var sellingPrice = 0;
        foreach (ObjectData obj in DatabaseManager.Instance.databseSO.objectsData)
        {
            if (obj.thisBuildingType == buildingType)
            {
                foreach (BuildRequirement req in obj.resourceRequirements)
                {
                    if (req.resource == ResourcesType.Credits)
                    {
                        sellingPrice = req.amount;
                    }
                }
            }
        }

        int amountToReturn = (int)(sellingPrice * 0.50f);

        IncreaseResource(ResourcesType.Credits, amountToReturn);
    }

    private void UpdateUI()
    {
        creditsUI.text = $"{credits}";
    }

    public int GetCredits()
    {
        return credits;
    }

    internal int GetResourceAmount(ResourcesType resource)
    {
        switch (resource)
        {
            case ResourcesType.Credits:
                return credits;
            default:
                break;
        }

        return 0;
    }

    internal void DecreaseResourcesBasedOnRequirement(ObjectData objectData)
    {
        foreach (BuildRequirement req in objectData.resourceRequirements)
        {
            DecreaseResource(req.resource, req.amount);
        }
    }

    private void OnEnable()
    {
        OnResourceChanged += UpdateUI;
    }
    private void OnDisable()
    {
        OnResourceChanged -= UpdateUI;
    }
}
