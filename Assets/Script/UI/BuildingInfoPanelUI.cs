// BuildingInfoPanelUI.cs
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
        // 只有当选中的是House时，才显示此面板
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
        // ... （此方法内容保持不变，为简洁省略） ...
    }

    public void OnUpgradeButtonClicked()
    {
        if (_selectedHouse != null)
        {
            _selectedHouse.TryToUpgrade();
        }
    }
}