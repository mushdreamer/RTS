using System;
using System.Collections;
using System.Collections.Generic;
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
        //Subscribe to event / Listening to event
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
        ResourceManager.Instance.OnResourceChanged -= HandleResourcesChanged;
        ResourceManager.Instance.OnBuildingsChanged -= HandleBuildingsChanged;
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

    private void HandleResourcesChanged()
    {
        ObjectData objectData = DatabaseManager.Instance.databseSO.objectsData[databaseItemID];

        bool requirementMet = true;

        foreach (BuildRequirement req in objectData.resourceRequirements)
        {
            if (ResourceManager.Instance.GetResourceAmount(req.resource) < req.amount)
            {
                requirementMet = false;
                break;
            }
        }

        isAvailable = requirementMet;

        UpdateAvailabilityUI();
    }

    private void HandleBuildingsChanged()
    {
        ObjectData objectData = DatabaseManager.Instance.databseSO.objectsData[databaseItemID];

        foreach (BuildingType dependency in objectData.buildDependency)
        {
            // if the building has not dependencies
            if (dependency == BuildingType.None)
            {
                gameObject.SetActive(true);
                return;
            }

            // check if dependency exists
            if (!ResourceManager.Instance.allExistingBuildings.Contains(dependency))
            {
                gameObject.SetActive(false);
                return;
            }
        }

        // if all requirements are met
        gameObject.SetActive(true);
    }
}
