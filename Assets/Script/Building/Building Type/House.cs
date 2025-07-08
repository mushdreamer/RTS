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
        // ����1������Ƿ������һ��
        if (currentTier.nextTier == null)
        {
            Debug.Log("Upgrade Failed! Population tier is" + currentTier.tierName + "There is nothing to upgrade");
            return false;
        }

        // ����2������Ҹ����Ƿ���
        if (currentHappiness < currentTier.HappinessToUpgrade)
        {
            // ʹ�ø��ı��ùؼ����ָ�����
            Debug.Log($"Upgrade Failed! You don't have enough happiness. You need:<b>{currentTier.HappinessToUpgrade}</b>, Now you have: <b>{currentHappiness}</b>");
            return false;
        }

        // ����3��������������Ƿ��㹻
        foreach (var material in currentTier.upgradeMaterials)
        {
            // ȷ�� material.item ��Ϊ�գ�����Ǳ�ڵĴ���
            if (material.item == null)
            {
                continue;
            }

            float stock = ResourceManager.Instance.GetWarehouseStock(material.item);
            if (stock < material.amount)
            {
                Debug.LogWarning($"Upgrade Failed! You don't have enough materials <b>{material.item.itemName}</b>��You need: {material.amount}, Now you have: {stock:F0}");
                return false;
            }
        }

        // ������м�鶼ͨ����
        Debug.Log("<color=green>Ready to be upgraded</color>");
        return true;
    }

    // <<< ����������ִ�������ķ��� >>>
    public void TryToUpgrade()
    {
        // �����������ֻ������ü�飬���CanUpgrade����false����ʲôҲ������
        // ���е�ʧ����־����CanUpgrade�Լ�����ӡ��
        if (!CanUpgrade())
        {
            return;
        }

        // 1. ������������
        foreach (var material in currentTier.upgradeMaterials)
        {
            ResourceManager.Instance.TryConsumeWarehouseItem(material.item, material.amount);
        }

        // 2. �����˿�
        PopulationManager.Instance.UpdatePopulation(currentTier, -_residentCount);

        // 3. ������
        currentTier = currentTier.nextTier;
        _residentCount = currentTier.residentsPerHouse;
        PopulationManager.Instance.UpdatePopulation(currentTier, _residentCount);

        // 4. �����Ҹ���
        currentHappiness = 10;

        Debug.Log($"<color=cyan>You upgrade your house! Now your population tier is {currentTier.tierName}��The population is {_residentCount}��</color>");

        // 5. (δ��) ����ģ��...
    }
}