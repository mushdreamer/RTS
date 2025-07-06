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
            Debug.LogError("BuildingInfoPanelUI �޷��ҵ� UnitSelectionManager ��ʵ����");
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
        // <<< ��̽��־ 1 >>>
        Debug.Log("<b>[BuildingInfoPanelUI]</b> �ӵ�֪ͨ��ѡ������Ѹı䡣�¶�����: "
                  + (newSelection != null ? newSelection.name : "None"));

        if (newSelection != null && newSelection.TryGetComponent<House>(out House house))
        {
            _selectedHouse = house;
            panel.SetActive(true);

            // <<< ��̽��־ 2 >>>
            Debug.Log("<b>[BuildingInfoPanelUI]</b> ȷ��ѡ�е��Ƿ��ݣ��Ѽ�¼ _selectedHouse ����ʾ��塣");
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
        Debug.Log("<b>[BuildingInfoPanelUI]</b> ����������ť�������");

        if (_selectedHouse != null)
        {
            // <<< ��̽��־ 4 >>>
            Debug.Log("<b>[BuildingInfoPanelUI]</b> ȷ�� _selectedHouse ���ڣ�׼���������������������� TryToUpgrade()��");
            _selectedHouse.TryToUpgrade();
        }
        else
        {
            // <<< ��̽��־ 5 (�������) >>>
            Debug.LogError("<b>[BuildingInfoPanelUI]</b> ���󣺵����������ť���� _selectedHouse �ǿյģ�");
        }
    }
}