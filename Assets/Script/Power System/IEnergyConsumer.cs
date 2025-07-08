public interface IEnergyConsumer
{
    // 这个消费者需要多少能量
    float RequestedPower { get; }

    // 管理器通过此方法告知消费者它实际获得了多少能量
    void SupplyPower(float suppliedAmount);

    // 方便其他脚本查询此设备是否在工作
    bool IsPowered { get; }
}