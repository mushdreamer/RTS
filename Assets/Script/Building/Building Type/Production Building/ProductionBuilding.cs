// ProductionBuilding.cs - 【联动修复最终版】
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

        // 【关键】将建筑自身的劳动力更新逻辑设置为定时重复执行
        InvokeRepeating(nameof(UpdateBuildingState), 1f, 2f);
    }

    // 【修正】当建筑被摧毁时，调用新的单参数ReleaseWorkforce方法
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

        // 如果断开连接，就释放所有工人并停止计算
        if (!IsConnected)
        {
            if (AssignedWorkforce > 0)
            {
                // 【修正】调用新的单参数ReleaseWorkforce方法
                WorkforceManager.Instance.ReleaseWorkforce(this.gameObject);
                AssignedWorkforce = 0; // 立即更新本地状态
            }
            CurrentEfficiency = 0;
            return;
        }

        // 【修正】持续请求（或“续约”）劳动力，并用返回的实际数量更新本地状态
        // 这确保了如果劳动力被WorkforceManager回收，这里能立刻知道
        AssignedWorkforce = WorkforceManager.Instance.RequestWorkforce(requiredWorkforceTier, requiredWorkforceAmount, this.gameObject);

        // 基于实际拥有的工人数计算效率
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
        // 生产逻辑保持不变...
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