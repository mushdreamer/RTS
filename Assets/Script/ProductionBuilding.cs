// ProductionBuilding.cs
using UnityEngine;

public class ProductionBuilding : MonoBehaviour
{
    [Header("��������")]
    [Tooltip("���������������Ʒ")]
    public ItemData outputItem;

    [Tooltip("ÿ��������������")]
    public float productionPerMinute = 2f;

    private float _productionIntervalInSeconds;
    private bool _isActivated = false;

    void Start()
    {
        // ����ͬ���������߼�����һ���ɱ����õķ���������Constructable�ű�
        // �����ÿ��������Ҫ���������
        if (productionPerMinute > 0)
        {
            _productionIntervalInSeconds = 60f / productionPerMinute;
        }
    }

    // ����������� Constructable �ű��ڽ�������ʽ���ú����
    public void ActivateBuilding()
    {
        if (_isActivated) return;
        if (outputItem == null)
        {
            Debug.LogError("�������� " + name + " û�����ò�����(Output Item)!", this.gameObject);
            return;
        }

        _isActivated = true;

        // ��ʼ�����Ե�����
        InvokeRepeating(nameof(Produce), _productionIntervalInSeconds, _productionIntervalInSeconds);
        Debug.Log($"�������� {name} �Ѽ����ʼÿ {_productionIntervalInSeconds:F1} ������һ�� {outputItem.itemName}��");
    }

    /// <summary>
    /// ������Դ�ĺ����߼�
    /// </summary>
    private void Produce()
    {
        if (ResourceManager.Instance != null)
        {
            // ÿ������1����λ
            ResourceManager.Instance.AddWarehouseItem(outputItem, 1);
            // Debug.Log($"{name} ������ 1 �� {outputItem.itemName}");
        }
    }
}