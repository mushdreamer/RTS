// BuildingInfoPanelUI.cs - 升级版
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BuildingInfoPanelUI : MonoBehaviour
{
    [Header("UI引用")]
    public GameObject panel;
    public Button upgradeButton;
    public TextMeshProUGUI populationText; // 新增：人口文本的引用

    [Header("需求列表配置")]
    public Transform needsContainer; // 新增：需求列表的容器
    public GameObject needDisplayPrefab; // 新增：单行需求UI的预制件

    private House _selectedHouse;
    private List<GameObject> _instantiatedNeedItems = new List<GameObject>();

    void Start()
    {
        if (UnitSelectionManager.Instance != null)
        {
            UnitSelectionManager.Instance.OnSelectionChanged += HandleSelectionChanged;
        }
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
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
            UpdatePanelContents(house);
        }
        else
        {
            _selectedHouse = null;
            panel.SetActive(false);
        }
    }

    private void UpdatePanelContents(House house)
    {
        if (house == null) return;

        populationText.text = $"Population: {house.currentResidents} / {house.maxResidents}";
        upgradeButton.interactable = house.CanUpgrade();

        foreach (GameObject item in _instantiatedNeedItems)
        {
            Destroy(item);
        }
        _instantiatedNeedItems.Clear();

        foreach (HouseNeedState needState in house.trackedNeeds)
        {
            GameObject newNeedItem = Instantiate(needDisplayPrefab, needsContainer);
            newNeedItem.GetComponent<NeedDisplayItem>().Setup(needState);
            _instantiatedNeedItems.Add(newNeedItem);
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