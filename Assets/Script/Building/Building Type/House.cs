// House.cs - ����Ϊ��Ƶ���İ汾
using UnityEngine;

public class House : MonoBehaviour
{
    public PopulationTier currentTier;

    [Header("״̬")]
    [Range(0, 20)]
    public int currentHappiness = 10;

    // <<< �޸ĵ� 1�������ļ��������Inspector�����ã���Ĭ�ϸ�Ϊ1�� >>>
    [Header("��������")]
    [Tooltip("ÿ�����������һ���������ĺ��Ҹ��ȸ���")]
    public float consumptionInterval = 1f;

    private int _residentCount;
    private bool _isActivated = false;

    // --- Ϊ�˷�����ԣ��������һ���򵥵Ľ�����ʽ ---
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

        // ʹ��������Inspector�����õ� consumptionInterval
        InvokeRepeating(nameof(ConsumeNeeds), consumptionInterval, consumptionInterval);
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
            // <<< �޸ĵ� 2�����㱾�Σ�ÿ�룩Ӧ�����ĵ����� >>>
            // (ÿ���������� / 60��) * �������ĵļ��ʱ��
            float amountToConsume = (need.consumptionPerMinute / 60f) * consumptionInterval;

            if (ResourceManager.Instance.TryConsumeWarehouseItem(need.item, amountToConsume))
            {
                // Ϊ�����Ҹ��ȱ仯����ô���ң����ǿ�������������ʱ��������
                // ����ÿ10��ż�һ���Ҹ��ȣ�����������ʱ����ԭ������Ч��������
                currentHappiness++;
            }
            else
            {
                currentHappiness -= 2;
            }
        }
        currentHappiness = Mathf.Clamp(currentHappiness, 0, 20);
    }

    public bool CanUpgrade()
    {
        if (currentTier.nextTier == null) { return false; }
        if (currentHappiness < currentTier.HappinessToUpgrade) { return false; }

        foreach (var material in currentTier.upgradeMaterials)
        {
            if (ResourceManager.Instance.GetWarehouseStock(material.item) < material.amount) { return false; }
        }
        return true;
    }

    public void TryToUpgrade()
    {
        if (!CanUpgrade())
        {
            // �����������һ����Ч���Ӿ���ʾ
            return;
        }

        foreach (var material in currentTier.upgradeMaterials)
        {
            ResourceManager.Instance.TryConsumeWarehouseItem(material.item, material.amount);
        }

        PopulationManager.Instance.UpdatePopulation(currentTier, -_residentCount);
        currentTier = currentTier.nextTier;
        _residentCount = currentTier.residentsPerHouse;
        PopulationManager.Instance.UpdatePopulation(currentTier, _residentCount);
        currentHappiness = 10;
        Debug.Log($"<color=cyan>���������ɹ��������� {currentTier.tierName}��</color>");
    }
}