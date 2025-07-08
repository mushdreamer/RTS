// BuildingInfoPanelUI.cs - 侦探版
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfoPanelUI : MonoBehaviour
{
    [Header("UI引用")]
    public GameObject panel;
    public Button upgradeButton;

    private House _selectedHouse;

    void Start()
    {
        if (UnitSelectionManager.Instance != null)
        {
            UnitSelectionManager.Instance.OnSelectionChanged += HandleSelectionChanged;
        }
        else
        {
            Debug.LogError("BuildingInfoPanelUI can't find UnitSelectionManager's Instance");
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

    /// <summary>
    /// 当选择改变时，此方法被调用。我们在这里添加日志。
    /// </summary>
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

    /// <summary>
    /// 当UI升级按钮被点击时，此方法被调用。我们在这里添加日志。
    /// </summary>
    public void OnUpgradeButtonClicked()
    {
        // <<< 侦探日志 3 >>>
        Debug.Log("<b>[BuildingInfoPanelUI]</b> 'upgrade' button is pressed");

        if (_selectedHouse != null)
        {
            _selectedHouse.TryToUpgrade();
        }
    }
}