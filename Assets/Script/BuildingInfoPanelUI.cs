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
            Debug.LogError("BuildingInfoPanelUI 无法找到 UnitSelectionManager 的实例！");
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
        // <<< 侦探日志 1 >>>
        Debug.Log("<b>[BuildingInfoPanelUI]</b> 接到通知：选择对象已改变。新对象是: "
                  + (newSelection != null ? newSelection.name : "None"));

        if (newSelection != null && newSelection.TryGetComponent<House>(out House house))
        {
            _selectedHouse = house;
            panel.SetActive(true);

            // <<< 侦探日志 2 >>>
            Debug.Log("<b>[BuildingInfoPanelUI]</b> 确认选中的是房屋，已记录 _selectedHouse 并显示面板。");
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
        Debug.Log("<b>[BuildingInfoPanelUI]</b> ‘升级’按钮被点击！");

        if (_selectedHouse != null)
        {
            // <<< 侦探日志 4 >>>
            Debug.Log("<b>[BuildingInfoPanelUI]</b> 确认 _selectedHouse 存在，准备命令它升级。即将调用 TryToUpgrade()。");
            _selectedHouse.TryToUpgrade();
        }
        else
        {
            // <<< 侦探日志 5 (错误情况) >>>
            Debug.LogError("<b>[BuildingInfoPanelUI]</b> 错误：点击了升级按钮，但 _selectedHouse 是空的！");
        }
    }
}