using UnityEngine;
using System.Linq;

// 实现 IEnergyConsumer 接口
public class TransmissionTower : MonoBehaviour, IEnergyConsumer
{
    [Header("电网属性")]
    [Tooltip("维持自身运作所需的电量")]
    public float powerRequirement = 1f;

    [Header("范围与容量")]
    [Tooltip("为房屋供电的信号覆盖范围半径")]
    public float coverageRadius = 20f;
    [Tooltip("能够供应的最大房屋数量")]
    public int maxHouseConnections = 10;

    // --- 内部状态 ---
    private bool isConnectedToPlant = false; // 是否在电厂范围内

    // --- IEnergyConsumer 接口实现 ---
    // 如果在电厂范围内，就向电网请求运作所需电力；否则不请求
    public float RequestedPower => isConnectedToPlant ? powerRequirement : 0;

    // 公开的状态，供 PopulationManager 查询
    public bool IsPowered { get; private set; }

    // 电网通过这个方法告诉我们实际获得了多少电
    public void SupplyPower(float suppliedAmount)
    {
        // 如果获得的电力满足我们的需求，则我们正常通电
        IsPowered = (suppliedAmount >= powerRequirement);
    }

    void OnEnable()
    {
        EnergyGridManager.Instance?.RegisterConsumer(this);
    }

    void OnDisable()
    {
        EnergyGridManager.Instance?.UnregisterConsumer(this);
    }
    // --- 接口实现结束 ---

    void Start()
    {
        // 启动时检查一次连接状态
        CheckConnectionToPowerPlant();
    }

    // FixedUpdate 用于周期性物理或状态检查，比 Update 性能更好
    void FixedUpdate()
    {
        CheckConnectionToPowerPlant();
    }

    private void CheckConnectionToPowerPlant()
    {
        // 使用 OverlapSphere 来高效地查找范围内的所有碰撞体
        var colliders = Physics.OverlapSphere(transform.position, 1f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore);

        // 查找最近的、且能容纳更多塔的电厂
        CoalPowerPlant closestValidPlant = colliders
            .Select(col => col.GetComponentInParent<CoalPowerPlant>())
            .Where(plant => plant != null && Vector3.Distance(transform.position, plant.transform.position) <= plant.powerRadius)
            .OrderBy(plant => Vector3.Distance(transform.position, plant.transform.position))
            .FirstOrDefault(); // 这里可以加入检查 plant 连接数的逻辑

        // 如果找到了符合条件的电厂，我们就认为已连接
        isConnectedToPlant = (closestValidPlant != null);
    }

    // 在编辑器中绘制范围
    private void OnDrawGizmosSelected()
    {
        // 连接状态决定了范围Gizmo的颜色
        Gizmos.color = IsPowered ? Color.cyan : (isConnectedToPlant ? Color.blue : Color.gray);
        Gizmos.DrawWireSphere(transform.position, coverageRadius);
    }
}