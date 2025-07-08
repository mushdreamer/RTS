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

    // 在 Update 中根据电力状态执行逻辑
    void Update()
    {
        if (IsPowered)
        {
            // 在这里执行工厂的生产逻辑...
            // 例如：播放动画、创建资源、发出正常运转的声音
            Debug.Log(gameObject.name + " 正在生产... 电力充足！");
        }
        else
        {
            // 在这里执行工厂停止工作的逻辑...
            // 例如：停止动画、显示“电力不足”图标、切换到待机声音
            Debug.LogWarning(gameObject.name + " 电力不足，已停止工作！");
        }
    }
}