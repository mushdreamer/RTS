public interface IEnergyConsumer
{
    // �����������Ҫ��������
    float RequestedPower { get; }

    // ������ͨ���˷�����֪��������ʵ�ʻ���˶�������
    void SupplyPower(float suppliedAmount);

    // ���������ű���ѯ���豸�Ƿ��ڹ���
    bool IsPowered { get; }
}