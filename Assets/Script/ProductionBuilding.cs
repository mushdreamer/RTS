// ProductionBuilding.cs
using UnityEngine;

public class ProductionBuilding : MonoBehaviour
{
    [Header("生产设置")]
    [Tooltip("这个建筑生产的物品")]
    public ItemData outputItem;

    [Tooltip("每分钟生产的数量")]
    public float productionPerMinute = 2f;

    private float _productionIntervalInSeconds;
    private bool _isActivated = false;

    void Start()
    {
        // 我们同样将激活逻辑放在一个可被调用的方法里，以配合Constructable脚本
        // 计算出每次生产需要间隔多少秒
        if (productionPerMinute > 0)
        {
            _productionIntervalInSeconds = 60f / productionPerMinute;
        }
    }

    // 这个方法将由 Constructable 脚本在建筑被正式放置后调用
    public void ActivateBuilding()
    {
        if (_isActivated) return;
        if (outputItem == null)
        {
            Debug.LogError("生产建筑 " + name + " 没有设置产出物(Output Item)!", this.gameObject);
            return;
        }

        _isActivated = true;

        // 开始周期性地生产
        InvokeRepeating(nameof(Produce), _productionIntervalInSeconds, _productionIntervalInSeconds);
        Debug.Log($"生产建筑 {name} 已激活，开始每 {_productionIntervalInSeconds:F1} 秒生产一个 {outputItem.itemName}。");
    }

    /// <summary>
    /// 生产资源的核心逻辑
    /// </summary>
    private void Produce()
    {
        if (ResourceManager.Instance != null)
        {
            // 每次生产1个单位
            ResourceManager.Instance.AddWarehouseItem(outputItem, 1);
            // Debug.Log($"{name} 生产了 1 个 {outputItem.itemName}");
        }
    }
}