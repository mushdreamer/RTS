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

    // �� Update �и��ݵ���״ִ̬���߼�
    void Update()
    {
        if (IsPowered)
        {
            // ������ִ�й����������߼�...
            // ���磺���Ŷ�����������Դ������������ת������
            Debug.Log(gameObject.name + " ��������... �������㣡");
        }
        else
        {
            // ������ִ�й���ֹͣ�������߼�...
            // ���磺ֹͣ��������ʾ���������㡱ͼ�ꡢ�л�����������
            Debug.LogWarning(gameObject.name + " �������㣬��ֹͣ������");
        }
    }
}