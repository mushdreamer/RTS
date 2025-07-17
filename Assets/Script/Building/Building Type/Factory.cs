using UnityEngine;

public class Factory : MonoBehaviour, IEnergyConsumer
{
    [Tooltip("工厂运转需要的电量")]
    public float powerRequirement = 15f; // 对应你之前 PowerUser 中的 consumingPower

    // 实现接口
    public float RequestedPower { get { return powerRequirement; } }
    public bool IsPowered { get; private set; }

    public void SupplyPower(float suppliedAmount)
    {
        // 如果获得的电力大于等于我们需要的电力，则设备正常工作
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

}