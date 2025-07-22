// ProductionBuilding.cs - 添加了最终调试日志的版本
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

    // --- 公开状态，供UI读取 ---
    public float CurrentInternalStock { get; private set; }
    public float CurrentEfficiency { get; private set; }
    public int AssignedWorkforce { get; private set; }
    public bool IsConnected { get; private set; }

    // --- 内部变量 ---
    private float productionInterval;
    private float timer;
    private Vector3Int myGridPosition;
    private bool isActivated = false;

    public void ActivateBuilding(Vector3Int gridPosition)
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

        InvokeRepeating(nameof(UpdateBuildingState), 1f, 2f);
    }

    void OnDestroy()
    {
        if (WorkforceManager.Instance != null && AssignedWorkforce > 0)
        {
            WorkforceManager.Instance.ReleaseWorkforce(requiredWorkforceTier, AssignedWorkforce);
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
                WorkforceManager.Instance.ReleaseWorkforce(requiredWorkforceTier, AssignedWorkforce);
                AssignedWorkforce = 0;
            }
            CurrentEfficiency = 0;
            return;
        }

        if (AssignedWorkforce < requiredWorkforceAmount)
        {
            int needed = requiredWorkforceAmount - AssignedWorkforce;
            int newlyAssigned = WorkforceManager.Instance.RequestWorkforce(requiredWorkforceTier, needed);
            AssignedWorkforce += newlyAssigned;
        }

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
        // ▼▼▼【核心侦探日志】▼▼▼
        // 每隔60帧打印一次生产条件的状态，避免刷屏
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"[生产检查] 名称: {gameObject.name} | 已激活: {isActivated} | 已连接: {IsConnected} | 库存未满: {CurrentInternalStock < maxInternalStock} | 效率>0: {CurrentEfficiency > 0}");
        }
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

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

        // （库存转移逻辑可以放在这里，或者在UpdateBuildingState里）
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