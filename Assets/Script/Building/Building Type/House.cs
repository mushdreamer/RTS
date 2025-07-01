// House.cs - �����汾
using UnityEngine;

public class House : MonoBehaviour
{
    public PopulationTier currentTier;

    private int _residentCount;
    private float _consumptionIntervalInSeconds = 60f;
    private bool _isActivated = false; // <<< ������һ����־�������жϷ����Ƿ��ѱ���ʽ����

    // Start �������ڼ����ǿյģ���Ϊ�߼���������
    void Start()
    {
        // ���ǰ������߼����Ƶ� ActivateHouse() ������
    }

    /// <summary>
    /// ����ݣ�ע���˿ڲ���ʼ���ġ��������ֻӦ�ڷ��ݱ���ʽ���ú����һ�Ρ�
    /// </summary>
    public void ActivateHouse()
    {
        // ��ֹ����������ظ�����
        if (_isActivated) return;

        if (currentTier != null)
        {
            _residentCount = currentTier.residentsPerHouse;
            PopulationManager.Instance.UpdatePopulation(currentTier, _residentCount);
            InvokeRepeating(nameof(ConsumeNeeds), _consumptionIntervalInSeconds, _consumptionIntervalInSeconds);
            _isActivated = true; // <<< �����ݱ��Ϊ���Ѽ��
            Debug.Log($"���� {this.gameObject.name} �ѱ�����˿� +{_residentCount}");
        }
        else
        {
            Debug.LogError("�޷�����ݣ���Ϊ currentTier Ϊ��!", this.gameObject);
        }
    }

    void OnDestroy()
    {
        // <<< �޸ģ�ֻ���Ѽ���ķ����ڱ��ݻ�ʱ������Ҫע���˿�
        if (_isActivated && PopulationManager.Instance != null && currentTier != null)
        {
            PopulationManager.Instance.UpdatePopulation(currentTier, -_residentCount);
            Debug.Log($"�Ѽ���ķ��� {this.gameObject.name} �ѱ��ݻ٣��˿� -{_residentCount}");
        }
    }

    private void ConsumeNeeds()
    {
        if (currentTier == null) return;

        Debug.Log($"���� {this.name} ����Ϊ��{currentTier.tierName}��������������...");
        foreach (var need in currentTier.needs)
        {
            float amountToConsume = need.consumptionPerMinute;
            bool consumedSuccessfully = ResourceManager.Instance.TryConsumeWarehouseItem(need.item, amountToConsume);

            if (consumedSuccessfully)
            {
                Debug.Log($"�ɹ����� {amountToConsume} ��λ�ġ�{need.item.itemName}����");
            }
            else
            {
                Debug.LogWarning($"���ʡ�{need.item.itemName}����治�㣬�޷���������");
            }
        }
    }
}