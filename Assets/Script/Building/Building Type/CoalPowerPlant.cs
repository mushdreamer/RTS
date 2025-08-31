using UnityEngine;

// 实现 IEnergyProducer 接口
public class CoalPowerPlant : MonoBehaviour, IEnergyProducer
{
    [Header("电网属性")]
    [Tooltip("该发电厂的最大发电量")]
    public float maxProduction = 100f;

    [Header("范围与容量")]
    [Tooltip("能够为输电塔供电的范围半径")]
    public float powerRadius = 30f;
    [Tooltip("能够支持的最大输电塔数量")]
    public int maxTowerConnections = 5;

    [Header("环境影响")]
    [Tooltip("发电时每秒产生的二氧化碳量")]
    public float co2EmissionPerSecond = 2f;

    // --- IEnergyProducer 接口实现 ---
    // 外部系统通过这个属性获取当前发电量
    public float CurrentProduction => maxProduction;

    void OnEnable()
    {
        // 自动向电网管理器注册自己为生产者
        EnergyGridManager.Instance?.RegisterProducer(this);
    }

    void OnDisable()
    {
        // 自动从电网管理器注销
        EnergyGridManager.Instance?.UnregisterProducer(this);
    }
    // --- 接口实现结束 ---

    void Update()
    {
        // 只有在实际发电时才排放污染
        if (CurrentProduction > 0)
        {
            AirQualityManager.Instance?.AddCO2(co2EmissionPerSecond * Time.deltaTime);
        }
    }

    // 在编辑器场景中绘制出范围，方便调试
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, powerRadius);
    }
}