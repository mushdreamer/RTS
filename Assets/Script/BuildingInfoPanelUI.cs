// BuildingInfoPanelUI.cs - ��̽��
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfoPanelUI : MonoBehaviour
{
    [Header("UI����")]
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
    /// ��ѡ��ı�ʱ���˷��������á����������������־��
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
    /// ��UI������ť�����ʱ���˷��������á����������������־��
    /// </summary>
    public void OnUpgradeButtonClicked()
    {
        // <<< ��̽��־ 3 >>>
        Debug.Log("<b>[BuildingInfoPanelUI]</b> 'upgrade' button is pressed");

        if (_selectedHouse != null)
        {
            _selectedHouse.TryToUpgrade();
        }
    }
}