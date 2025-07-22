// ProductionPanelUI.cs
using UnityEngine;
using TMPro;

public class ProductionPanelUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI buildingNameText;
    public TextMeshProUGUI efficiencyText;
    public TextMeshProUGUI workforceText;
    public TextMeshProUGUI inventoryText;

    private ProductionBuilding _selectedBuilding;

    void Start()
    {
        if (UnitSelectionManager.Instance != null)
        {
            UnitSelectionManager.Instance.OnSelectionChanged += HandleSelectionChanged;
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
        if (panel.activeSelf && _selectedBuilding != null)
        {
            UpdatePanelContents();
        }
    }

    private void HandleSelectionChanged(GameObject newSelection)
    {
        if (newSelection != null && newSelection.TryGetComponent<ProductionBuilding>(out var pb))
        {
            _selectedBuilding = pb;
            panel.SetActive(true);
        }
        else
        {
            _selectedBuilding = null;
            panel.SetActive(false);
        }
    }

    private void UpdatePanelContents()
    {
        if (_selectedBuilding == null) return;

        buildingNameText.text = _selectedBuilding.gameObject.name.Replace("(Clone)", "").Trim();
        efficiencyText.text = $"效率: {_selectedBuilding.CurrentEfficiency * 100:F0}%";

        int availableWorkforce = PopulationManager.Instance.GetPopulation(_selectedBuilding.requiredWorkforceTier);
        int currentlyUsing = Mathf.RoundToInt(_selectedBuilding.requiredWorkforceAmount * _selectedBuilding.CurrentEfficiency);

        workforceText.text = $"劳动力: {currentlyUsing} / {_selectedBuilding.requiredWorkforceAmount}";
        inventoryText.text = $"库存: {_selectedBuilding.CurrentInternalStock:F0} / {_selectedBuilding.maxInternalStock}";
    }
}