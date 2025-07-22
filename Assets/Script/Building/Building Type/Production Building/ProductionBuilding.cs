// ProductionBuilding.cs
using UnityEngine;

public class ProductionBuilding : MonoBehaviour
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

    // --- 公开属性，供UI读取 ---
    public float CurrentInternalStock { get; private set; } = 0;
    public float CurrentEfficiency { get; private set; } = 1f;
    public bool IsConnected { get; private set; }

    // --- 私有变量 ---
    private float productionInterval;
    private float timer;
    private Vector3Int myGridPosition;

    public void ActivateBuilding(Vector3Int gridPosition)
    {
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

        if (requiredWorkforceTier != null && requiredWorkforceAmount > 0)
        {
            WorkforceManager.Instance.RegisterWorkforce(requiredWorkforceTier, requiredWorkforceAmount);
        }

        InvokeRepeating(nameof(CheckConnection), 1f, 5f);
        InvokeRepeating(nameof(TryTransferStockToWarehouse), 2f, 2f);
        InvokeRepeating(nameof(UpdateMyEfficiency), 1f, 1f);
    }

    void OnDestroy()
    {
        if (WorkforceManager.Instance != null && requiredWorkforceTier != null && requiredWorkforceAmount > 0)
        {
            WorkforceManager.Instance.UnregisterWorkforce(requiredWorkforceTier, requiredWorkforceAmount);
        }
    }

    void Update()
    {
        if (CurrentInternalStock < maxInternalStock && CurrentEfficiency > 0)
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
    }

    private void CheckConnection()
    {
        if (BuildingConnector.Instance != null)
        {
            IsConnected = BuildingConnector.Instance.CheckConnection(myGridPosition, requiredBuilding);
        }
    }

    private void UpdateMyEfficiency()
    {
        if (WorkforceManager.Instance != null)
        {
            CurrentEfficiency = WorkforceManager.Instance.GetEfficiency(requiredWorkforceTier);
        }
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