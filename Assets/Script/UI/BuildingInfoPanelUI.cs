// BuildingInfoPanelUI.cs - 最终完整版
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BuildingInfoPanelUI : MonoBehaviour
{
    [Header("UI引用")]
    public GameObject panel;
    public Button upgradeButton;
    public TextMeshProUGUI populationText;

    [Header("需求列表配置")]
    public Transform needsContainer;
    public GameObject needDisplayPrefab;

    private House _selectedHouse;
    private List<GameObject> _instantiatedNeedItems = new List<GameObject>();

    void Start()
    {
        if (UnitSelectionManager.Instance != null)
        {
            UnitSelectionManager.Instance.OnSelectionChanged += HandleSelectionChanged;
        }
        else
        {
            Debug.LogError("BuildingInfoPanelUI 找不到 UnitSelectionManager 的实例！");
        }

        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        }

        panel.SetActive(false);
    }

    void OnDestroy()
    {
        if (UnitSelectionManager.Instance != null)
        {
            UnitSelectionManager.Instance.OnSelectionChanged -= HandleSelectionChanged;
        }
    }

    void Update()
    {
        if (_selectedHouse != null && panel.activeSelf)
        {
            UpdatePanelContents(_selectedHouse);
        }
    }

    private void HandleSelectionChanged(GameObject newSelection)
    {
        if (newSelection != null && newSelection.TryGetComponent<House>(out House house))
        {
            _selectedHouse = house;
            panel.SetActive(true);
        }
        else
        {
            _selectedHouse = null;
            panel.SetActive(false);
        }
    }

    private void UpdatePanelContents(House house)
    {
        if (house == null || populationText == null || upgradeButton == null || needsContainer == null || needDisplayPrefab == null)
        {
            return;
        }

        populationText.text = $"Population: {house.currentResidents} / {house.maxResidents}";
        upgradeButton.interactable = house.CanUpgrade();

        foreach (GameObject item in _instantiatedNeedItems)
        {
            Destroy(item);
        }
        _instantiatedNeedItems.Clear();

        if (house.trackedNeeds != null)
        {
            foreach (HouseNeedState needState in house.trackedNeeds)
            {
                GameObject newNeedItem = Instantiate(needDisplayPrefab, needsContainer);
                var displayItem = newNeedItem.GetComponent<NeedDisplayItem>();
                if (displayItem != null)
                {
                    displayItem.Setup(needState);
                }
                _instantiatedNeedItems.Add(newNeedItem);
            }
        }
    }

    public void OnUpgradeButtonClicked()
    {
        if (_selectedHouse != null)
        {
            _selectedHouse.TryToUpgrade();
        }
    }
}