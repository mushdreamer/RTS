using UnityEngine;

public class Factory : MonoBehaviour, IEnergyConsumer
{
    [Tooltip("������ת��Ҫ�ĵ���")]
    public float powerRequirement = 15f; // ��Ӧ��֮ǰ PowerUser �е� consumingPower

    // ʵ�ֽӿ�
    public float RequestedPower { get { return powerRequirement; } }
    public bool IsPowered { get; private set; }

    public void SupplyPower(float suppliedAmount)
    {
        // �����õĵ������ڵ���������Ҫ�ĵ��������豸��������
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