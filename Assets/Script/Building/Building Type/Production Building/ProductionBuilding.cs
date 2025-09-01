// ProductionBuilding.cs - Final Version with Interface
using UnityEngine;

public class ProductionBuilding : MonoBehaviour, IActivatableBuilding // Implements the interface
{
    [Header("生产设置")]
    public ItemData outputItem;
    public float productionPerMinute = 60f;
    public int outputAmount = 1;

    [Header("库存设置")]
    public int maxInternalStock = 10;

    [Header("劳动力需求")]
    public PopulationTier requiredWorkforceTier;
    public int requiredWorkforceAmount = 5;

    [Header("连接需求")]
    public BuildingType requiredBuilding = BuildingType.Warehouse;

    public float CurrentInternalStock { get; private set; }
    public float CurrentEfficiency { get; private set; }
    public int AssignedWorkforce { get; private set; }
    public bool IsConnected { get; private set; }

    private float productionInterval;
    private float timer;
    private Vector3Int myGridPosition;
    private bool isActivated = false;

    // This is the required method from the IActivatableBuilding interface
    public void Activate(Vector3Int gridPosition)
    {
        if (isActivated) return;
        isActivated = true;

        myGridPosition = gridPosition;
        if (productionPerMinute > 0)
        {
            productionInterval = 60f / productionPerMinute;
        }
        else
        {
            this.enabled = false;
            return;
        }

        // Enable the component to start its Update loops
        this.enabled = true;
        InvokeRepeating(nameof(UpdateBuildingState), 1f, 2f);
    }

    void OnDestroy()
    {
        if (WorkforceManager.Instance != null)
        {
            WorkforceManager.Instance.ReleaseWorkforce(this.gameObject);
        }
    }

    private void UpdateBuildingState()
    {
        if (BuildingConnector.Instance == null || WorkforceManager.Instance == null) return;

        IsConnected = BuildingConnector.Instance.CheckConnection(myGridPosition, requiredBuilding);

        if (!IsConnected)
        {
            if (AssignedWorkforce > 0)
            {
                WorkforceManager.Instance.ReleaseWorkforce(this.gameObject);
                AssignedWorkforce = 0;
            }
            CurrentEfficiency = 0;
            return;
        }

        AssignedWorkforce = WorkforceManager.Instance.RequestWorkforce(requiredWorkforceTier, requiredWorkforceAmount, this.gameObject);

        if (requiredWorkforceAmount > 0)
        {
            CurrentEfficiency = (float)AssignedWorkforce / requiredWorkforceAmount;
        }
        else
        {
            CurrentEfficiency = 1f;
        }
    }

    void Update()
    {
        if (isActivated && IsConnected && CurrentInternalStock < maxInternalStock && CurrentEfficiency > 0)
        {
            timer += Time.deltaTime * CurrentEfficiency;
            if (timer >= productionInterval)
            {
                CurrentInternalStock += outputAmount;
                timer -= productionInterval;
            }
        }
        else
        {
            timer = 0f;
        }
        TryTransferStockToWarehouse();
    }

    private void TryTransferStockToWarehouse()
    {
        if (CurrentInternalStock > 0 && IsConnected)
        {
            int amountToTransfer = Mathf.FloorToInt(CurrentInternalStock);
            if (amountToTransfer > 0)
            {
                ResourceManager.Instance.AddWarehouseItem(outputItem, amountToTransfer);
                CurrentInternalStock -= amountToTransfer;
            }
        }
    }
}