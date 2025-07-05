// House.cs - �����������߼�
using UnityEngine;

public class House : MonoBehaviour
{
    public PopulationTier currentTier;

    [Header("״̬")]
    [Range(0, 20)]
    public int currentHappiness = 10;

    private int _residentCount;
    private float _consumptionIntervalInSeconds = 60f;
    private bool _isActivated = false;

    // --- Ϊ�˷�����ԣ��������һ���򵥵Ľ�����ʽ ---
    // ��������������ʱ�����Խ�������
    private void OnMouseDown()
    {
        TryToUpgrade();
    }

    public void ActivateHouse()
    {
        if (_isActivated) return;
        if (currentTier == null) { /* ... ������ ... */ return; }

        _isActivated = true;
        _residentCount = currentTier.residentsPerHouse;
        PopulationManager.Instance.RegisterHouse(this);
        PopulationManager.Instance.UpdatePopulation(currentTier, _residentCount);
        InvokeRepeating(nameof(ConsumeNeeds), _consumptionIntervalInSeconds, _consumptionIntervalInSeconds);
    }

    void OnDestroy()
    {
        if (_isActivated && PopulationManager.Instance != null)
        {
            PopulationManager.Instance.UnregisterHouse(this);
            PopulationManager.Instance.UpdatePopulation(currentTier, -_residentCount);
        }
    }

    private void ConsumeNeeds()
    {
        if (currentTier == null) return;
        foreach (var need in currentTier.needs)
        {
            if (ResourceManager.Instance.TryConsumeWarehouseItem(need.item, need.consumptionPerMinute))
            {
                currentHappiness++;
            }
            else
            {
                currentHappiness -= 2;
            }
        }
        currentHappiness = Mathf.Clamp(currentHappiness, 0, 20);
    }

    // <<< ����������Ƿ��������������ķ��� >>>
    public bool CanUpgrade()
    {
        if (currentTier.nextTier == null) return false; // û����һ������������
        if (currentHappiness < currentTier.HappinessToUpgrade) return false; // �Ҹ��Ȳ���

        // �������Ƿ��㹻
        foreach (var material in currentTier.upgradeMaterials)
        {
            if (ResourceManager.Instance.GetWarehouseStock(material.item) < material.amount)
            {
                //Debug.Log($"����ʧ�ܣ�ȱ�ٲ��� {material.item.itemName}");
                return false; // ���ϲ���
            }
        }
        return true;
    }

    // <<< ����������ִ�������ķ��� >>>
    public void TryToUpgrade()
    {
        if (!CanUpgrade())
        {
            Debug.Log("��������δ���㣡");
            return;
        }

        // 1. ������������
        foreach (var material in currentTier.upgradeMaterials)
        {
            ResourceManager.Instance.TryConsumeWarehouseItem(material.item, material.amount);
        }

        // 2. �����˿ڣ���ע���ɽײ㣬��ע���½ײ�
        PopulationManager.Instance.UpdatePopulation(currentTier, -_residentCount);

        // 3. �������л�����һ���ײ�
        currentTier = currentTier.nextTier;
        _residentCount = currentTier.residentsPerHouse;
        PopulationManager.Instance.UpdatePopulation(currentTier, _residentCount);

        // 4. �����Ҹ��ȵ�����ֵ
        currentHappiness = 10;

        Debug.Log($"<color=cyan>���������ɹ��������� {currentTier.tierName}���˿ڱ�Ϊ {_residentCount}��</color>");

        // 5. (δ��) ��������Լ��ϸ�������ģ�͡�������Ч����Ч�Ĵ���
    }
}