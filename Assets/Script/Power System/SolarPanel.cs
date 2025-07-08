using UnityEngine;

public class SolarPanel : MonoBehaviour, IEnergyProducer
{
    [Tooltip("在理想光照下的最大发电量")]
    public float maxProduction = 10f; // 对应你之前 PowerUser 中的 producingPower

    // 我们可以添加一个简单的日夜模拟
    [Tooltip("当前光照强度 (0=夜晚, 1=正午)")]
    [Range(0, 1)]
    public float sunIntensity = 1f;

    // 实现接口
    public float CurrentProduction
    {
        get { return maxProduction * sunIntensity; }
    }

    void OnEnable()
    {
        // 当物体启用时，自动向管理器注册自己
        EnergyGridManager.Instance?.RegisterProducer(this);
    }

    void OnDisable()
    {
        // 当物体被禁用或摧毁时，自动从管理器注销
        // 这替代了你之前在 OnDestroy 中写的逻辑
        EnergyGridManager.Instance?.UnregisterProducer(this);
    }
}