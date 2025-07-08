using UnityEngine;

public class SolarPanel : MonoBehaviour, IEnergyProducer
{
    [Tooltip("����������µ���󷢵���")]
    public float maxProduction = 10f; // ��Ӧ��֮ǰ PowerUser �е� producingPower

    // ���ǿ������һ���򵥵���ҹģ��
    [Tooltip("��ǰ����ǿ�� (0=ҹ��, 1=����)")]
    [Range(0, 1)]
    public float sunIntensity = 1f;

    // ʵ�ֽӿ�
    public float CurrentProduction
    {
        get { return maxProduction * sunIntensity; }
    }

    void OnEnable()
    {
        // ����������ʱ���Զ��������ע���Լ�
        EnergyGridManager.Instance?.RegisterProducer(this);
    }

    void OnDisable()
    {
        // �����屻���û�ݻ�ʱ���Զ��ӹ�����ע��
        // ���������֮ǰ�� OnDestroy ��д���߼�
        EnergyGridManager.Instance?.UnregisterProducer(this);
    }
}