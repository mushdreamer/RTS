// House.cs - �������Ҹ����߼�
using UnityEngine;

public class House : MonoBehaviour
{
    public PopulationTier currentTier;

    [Header("״̬")]
    [Range(0, 20)] // ���Ҹ���������0��20�ķ�Χ��
    public int currentHappiness = 10; // ����Ļ����Ҹ���

    private int _residentCount;
    private float _consumptionIntervalInSeconds = 60f;
    private bool _isActivated = false;

    void Start() { }

    public void ActivateHouse()
    {
        if (_isActivated) return;
        if (currentTier == null)
        {
            Debug.LogError("�޷�����ݣ���Ϊ currentTier Ϊ��!", this.gameObject);
            return;
        }

        _isActivated = true;
        _residentCount = currentTier.residentsPerHouse;

        // <<< ������ע�ᵽ�������� >>>
        PopulationManager.Instance.RegisterHouse(this);

        PopulationManager.Instance.UpdatePopulation(currentTier, _residentCount);
        InvokeRepeating(nameof(ConsumeNeeds), _consumptionIntervalInSeconds, _consumptionIntervalInSeconds);
        Debug.Log($"���� {this.gameObject.name} �ѱ�����˿� +{_residentCount}");
    }

    void OnDestroy()
    {
        if (_isActivated && PopulationManager.Instance != null)
        {
            // <<< �ӹ�������ע������ >>>
            PopulationManager.Instance.UnregisterHouse(this);
            PopulationManager.Instance.UpdatePopulation(currentTier, -_residentCount);
            Debug.Log($"�Ѽ���ķ��� {this.gameObject.name} �ѱ��ݻ٣��˿� -{_residentCount}");
        }
    }

    private void ConsumeNeeds()
    {
        if (currentTier == null) return;

        foreach (var need in currentTier.needs)
        {
            bool consumedSuccessfully = ResourceManager.Instance.TryConsumeWarehouseItem(need.item, need.consumptionPerMinute);

            if (consumedSuccessfully)
            {
                // <<< �������㣬�����Ҹ��� >>>
                // ������ʱ�趨ÿ����������+1�Ҹ���
                currentHappiness++;
            }
            else
            {
                // <<< ����δ���㣬�����Ҹ��� >>>
                // ÿ��δ��������-2�Ҹ��ȣ��ͷ�����һЩ
                currentHappiness -= 2;
            }
        }

        // <<< ʹ�� Mathf.Clamp ȷ���Ҹ��Ȳ��ᳬ����Χ >>>
        currentHappiness = Mathf.Clamp(currentHappiness, 0, 20);
    }
}